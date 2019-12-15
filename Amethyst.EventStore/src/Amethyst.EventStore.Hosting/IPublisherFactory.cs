using Amethyst.EventStore.Abstractions.Publishing;
using Amethyst.EventStore.Kafka;

namespace Amethyst.EventStore.Hosting
{
    public interface IPublisherFactory
    {
        ProducerSettings Settings { get; }

        IEventPublisher Create(string topic);
    }
}