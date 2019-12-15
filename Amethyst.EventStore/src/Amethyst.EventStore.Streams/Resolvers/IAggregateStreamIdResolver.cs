using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Domain.Abstractions;

namespace Amethyst.EventStore.Streams.Resolvers
{
    public interface IAggregateStreamIdResolver
    {
        StreamId Resolve<TAggregate, TId>(TId id)
            where TAggregate : IAggregate<TId>
            where TId : IGuidId;
    }
}