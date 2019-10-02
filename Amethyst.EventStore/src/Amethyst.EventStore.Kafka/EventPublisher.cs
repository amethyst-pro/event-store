using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Publishing;
using Amethyst.EventStore.Abstractions.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Amethyst.EventStore.Kafka
{
    public sealed class EventPublisher : IEventPublisher
    {
        private readonly IRecordedEventSerializer _serializer;
        private readonly IProducer _kafkaProducer;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(
            ProducerSettings settings,
            string topic,
            IRecordedEventSerializer serializer,
            ILoggerFactory loggerFactory)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentNullException(nameof(settings));

            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _kafkaProducer = new KafkaProducer(settings, topic, loggerFactory);
            _logger = loggerFactory.CreateLogger<EventPublisher>();
        } 

        public async Task PublishAsync(IReadOnlyCollection<RecordedEvent> events)
        {
            foreach (var @event in events)
            {
                try
                {
                    await _kafkaProducer.ProduceAsync(
                        _serializer.Serialize(@event),
                        @event.StreamId.Id.ToByteArray());
                }
                catch (KafkaException e)
                {
                    _logger.LogError(e, "Error produce event to Kafka.");
                    throw;
                }
            }
        }
    }
}