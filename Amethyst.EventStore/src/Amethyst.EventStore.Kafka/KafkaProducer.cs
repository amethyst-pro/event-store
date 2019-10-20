using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Amethyst.EventStore.Kafka
{
    internal sealed class KafkaProducer 
    {
        private readonly IProducer<byte[], byte[]> _producer;
        private readonly string _topic;
        private readonly ILogger<KafkaProducer> _logger;
        
        public KafkaProducer(ProducerSettings settings, string topic, ILoggerFactory loggerFactory)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("Topic is not specified.", nameof(topic));

            if (string.IsNullOrWhiteSpace(settings.Config.BootstrapServers))
                throw new ArgumentException("Servers are not specified.");

            _topic = topic;
            _logger = loggerFactory.CreateLogger<KafkaProducer>();
            _producer = new ProducerBuilder<byte[], byte[]>(settings.Config).Build();
        }

        public async Task ProduceAsync(byte[] @event, byte[] key = null)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            _logger.LogTrace($"Producer {_producer.Name} producing on topic {_topic}.");

            var message = new Message<byte[], byte[]> {Key = key, Value = @event};

            var deliveryReport = await _producer.ProduceAsync(_topic, message);

            _logger.LogTrace($"Delivered to: {deliveryReport.TopicPartitionOffset}");
        }

        public void Dispose() => _producer.Dispose();
    }
}