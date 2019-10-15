using System;
using Confluent.Kafka;

namespace Amethyst.EventStore.Kafka
{
    public sealed class ProducerSettings
    {
        public ProducerSettings(string brokers)
        {
            if (string.IsNullOrEmpty(brokers))
                throw new ArgumentException("Brokers not specified.", nameof(brokers));

            Config = new ProducerConfig
            {
                BootstrapServers = brokers,
                Acks = Acks.All,
                EnableIdempotence = true,
                MaxInFlight = 1,
                MessageSendMaxRetries = 5,
                LingerMs = 50
            };
        }

        public ProducerConfig Config { get; }
    }
}