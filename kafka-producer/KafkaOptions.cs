namespace kafka_producer
{
    public class KafkaOptions
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public string ClientId { get; set; }
    }
}
