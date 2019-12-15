using System;
using Amethyst.EventStore.Abstractions.Publishing;
using Amethyst.EventStore.Abstractions.Serialization;
using Amethyst.EventStore.Kafka;
using Microsoft.Extensions.Logging;

namespace Amethyst.EventStore.Hosting
{
    public class PublisherFactory : IPublisherFactory
    {
        private readonly IRecordedEventSerializer _serializer;
        private readonly ILoggerFactory _loggerFactory;

        public PublisherFactory(
            IRecordedEventSerializer serializer,
            PublisherConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            
            Settings = new ProducerSettings(configuration.Brokers);
        }
        
        public ProducerSettings Settings { get; }
        
        public IEventPublisher Create(string topic)
            => new EventPublisher(Settings, topic, _serializer, _loggerFactory);
    }
}