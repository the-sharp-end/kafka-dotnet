namespace Kafka.Common
{
    public class KafkaOptions
    {
        public string BootstrapServers { get; set; }
        public string ClientId { get; set; }
        public string GroupId { get; set; }
        public string Topic { get; set; }
    }
}
