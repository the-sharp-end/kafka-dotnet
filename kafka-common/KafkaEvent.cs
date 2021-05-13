using System;

namespace Kafka.Common
{
    public class KafkaEvent
    {
        public KafkaEvent(string message)
        {
            EventId = Guid.NewGuid();
            EventTimestamp = DateTimeOffset.UtcNow;
            Message = message;
        }

        protected KafkaEvent() { }

        public Guid EventId { get; set; }
        public DateTimeOffset EventTimestamp { get; set; }
        public string Message { get; set; }

        public static KafkaEvent HealthCheckEvent()
        {
            return new KafkaEvent("Health Check event");
        }
    }
}
