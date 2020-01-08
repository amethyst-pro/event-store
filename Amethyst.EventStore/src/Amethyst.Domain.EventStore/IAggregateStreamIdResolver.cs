using Amethyst.EventStore;

namespace Amethyst.Domain.EventStore
{
    public interface IAggregateStreamIdResolver
    {
        StreamId Resolve<TAggregate, TId>(TId id)
            where TAggregate : IAggregate<TId>
            where TId : IGuidId;
    }
}