using Amethyst.Domain;

namespace Amethyst.EventStore.Domain
{
    public interface IAggregateStreamIdResolver
    {
        StreamId Resolve<TAggregate, TId>(TId id)
            where TAggregate : IAggregate<TId>
            where TId : IGuidId;
    }
}