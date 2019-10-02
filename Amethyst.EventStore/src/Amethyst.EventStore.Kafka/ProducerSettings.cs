using System;
using Confluent.Kafka;

namespace Amethyst.EventStore.Kafka
{
    public sealed class ProducerSettings
    {
        private const int DefaultMaxMessageBytes = 5 * 1024 * 1024;

        public ProducerSettings(string brokers)
        {
            if (string.IsNullOrEmpty(brokers))
                throw new ArgumentException("Brokers not specified.", nameof(brokers));

            Config = new ProducerConfig
            {
                BootstrapServers = brokers,
                MessageMaxBytes = DefaultMaxMessageBytes,
                Acks = Acks.All,
                EnableIdempotence = true,
                MaxInFlight = 1,
                MessageSendMaxRetries = 5
            };
        }

        public ProducerConfig Config { get; }
    }
}