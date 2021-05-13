using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Common;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Kafka.Consumer
{
    public class KafkaConsumer : BackgroundService
    {
        private static readonly ILogger _logger = Log.ForContext<KafkaConsumer>();
        private readonly KafkaOptions _kafkaOptions;

        public KafkaConsumer(KafkaOptions kafkaOptions)
        {
            _kafkaOptions = kafkaOptions;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork(stoppingToken);
                _logger.Information("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
        public async Task DoWork(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaOptions.BootstrapServers,
                ClientId = $"{_kafkaOptions.ClientId} - {Dns.GetHostName()}",
                GroupId = _kafkaOptions.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _logger.Debug("Starting Kafka Consumer ...");
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(_kafkaOptions.Topic);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    ConsumeResult(consumeResult);
                }

                consumer.Close();
            }
            _logger.Debug("... Kafka Consumer Stopped");
        }


        public void ConsumeResult(ConsumeResult<Ignore, string> result)
        {
            _logger.Information("");
            _logger.Information($"Partition: {result.Partition}");
            _logger.Information($"Offset: {result.Offset}");
            _logger.Information($"TopicPartition: {result.TopicPartition}");
            _logger.Information($"TopicPartitionOffset: {result.TopicPartitionOffset}");
            _logger.Information($"Value: {result.Message.Value}");
            _logger.Information("");
        }
    }
}
