using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;
using Confluent.Kafka;
using System.Net;
using System;
using Serilog;
using Kafka.Common;
using Newtonsoft.Json;

namespace Kafka.Producer.HealthChecks
{
    public class KafkaHealthCheck : ScopedHealthCheckBase
    {
        private static readonly ILogger _logger = Log.ForContext<KafkaHealthCheck>();
        private readonly KafkaOptions _kafkaOptions;

        public KafkaHealthCheck(KafkaOptions kafkaOptions, IServiceScopeFactory serviceScopeFactory)
            : base(serviceScopeFactory, "Kafka Connection Health Check")
        {
            _kafkaOptions = kafkaOptions;
        }

        protected override async ValueTask<HealthCheckResult> CheckAsync(CancellationToken cancellationToken = default)
        {
            if (await CanConnectKafka(cancellationToken))
                return HealthCheckResult.Healthy();
            return HealthCheckResult.Unhealthy();
        }

        public Task<bool> CanConnectKafka(CancellationToken cancellationToken = default)
        {

            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaOptions.BootstrapServers,
                ClientId = $"{_kafkaOptions.ClientId} - {Dns.GetHostName()}",
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var kafkaEvent = KafkaEvent.HealthCheckEvent();
                    _logger.Debug("Sending Message ...");
                    producer.Produce("HealthCheck.Sent", new Message<Null, string> { Value = JsonConvert.SerializeObject(kafkaEvent) }, ProducerHandler);
                    producer.Flush();
                    _logger.Debug("... Message Produced");
                }
                catch (Exception ex)
                {
                    Task.FromResult(false);
                }
            }
            return Task.FromResult(true);
        }

        private static void ProducerHandler(DeliveryReport<Null, string> report)
        {
            if (report.Status == PersistenceStatus.Persisted)
            {
                _logger.Debug("Status: {KafkaEventStatus}", report.Status);
                _logger.Debug("Partition: {KafkaEventPartition}", report.Partition);
                _logger.Debug("Offset: {KafkaEventOffset}", report.Offset);
                _logger.Debug("TopicPartition: {KafkaEventPartition}", report.TopicPartition);
                _logger.Debug("TopicPartitionOffset: {KafkaEventPartitionOffset}", report.TopicPartitionOffset);
                _logger.Debug("Value: {KafkaEventValue}", report.Value);
            }
            else
            {
                _logger.Error("Status: {KafkaEventStatus}", report.Status);
                _logger.Error("IsError: {IsError}", report.Error.IsError);
                _logger.Error("IsFatal: {IsFatal}", report.Error.IsFatal);
                _logger.Error("ErrorMessage: {KafkaEventPartition}", report.Error.Reason);
            }
        }
    }
}
