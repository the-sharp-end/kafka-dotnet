using System;
using System.Net;
using Confluent.Kafka;
using Kafka.Common;
using Kafka.Producer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace Kafka.Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KafkaController : ControllerBase
    {
        private static readonly ILogger _logger = Log.ForContext<KafkaController>();
        private readonly KafkaOptions _kafkaOptions;

        public KafkaController(KafkaOptions kafkaOptions)
        {
            _kafkaOptions = kafkaOptions;
        }

        [HttpPost]
        public IActionResult Post(TopicMessageModel model)
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
                    var kafkaEvent = new KafkaEvent(model.Message);
                    _logger.Debug("Sending Message ...");
                    producer.Produce(model.Topic, new Message<Null, string> { Value = JsonConvert.SerializeObject(kafkaEvent) }, ProducerHandler);
                    producer.Flush();
                    _logger.Debug("... Message Produced");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Accepted();
        }

        public static void ProducerHandler(DeliveryReport<Null, string> report)
        {
            _logger.Information("");
            _logger.Information($"Status: {report.Status}");
            _logger.Information($"Partition: {report.Partition}");
            _logger.Information($"Offset: {report.Offset}");
            _logger.Information($"TopicPartition: {report.TopicPartition}");
            _logger.Information($"TopicPartitionOffset: {report.TopicPartitionOffset}");
            _logger.Information($"Value: {report.Value}");
            _logger.Information("");
        }
    }
}
