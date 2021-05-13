using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;
using Confluent.Kafka;

namespace Kafka.Producer.HealthChecks
{
    public class KafkaHealthCheck : ScopedHealthCheckBase
    {
        public KafkaHealthCheck(IServiceScopeFactory serviceScopeFactory)
            : base(serviceScopeFactory, "Kafka Connection Health Check") { }

        protected override async ValueTask<HealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await CanConnectKafka(cancellationToken))
                return HealthCheckResult.Healthy();
            return HealthCheckResult.Unhealthy();
        }

        public Task<bool> CanConnectKafka(CancellationToken cancellationToken = default(CancellationToken))
        {

            //var config = new ProducerConfig
            //{
            //    BootstrapServers = _kafkaOptions.BootstrapServers,
            //    ClientId = $"{_kafkaOptions.ClientId} - {Dns.GetHostName()}",
            //};

            //using (var producer = new ProducerBuilder<Null, string>(config).Build())
            //{
            //    try
            //    {
            //        //_logger.Debug("Sending Message ...");
            //        producer.Produce(model.Topic, new Message<Null, string> { Value = model.Message }, ProducerHandler);
            //        producer.Flush();
            //        _logger.Debug("... Message Produced");
            //    }
            //    catch (Exception ex)
            //    {
            //        return BadRequest(ex.Message);
            //    }
            //}
            return Task.FromResult(true);
        }
    }
}
