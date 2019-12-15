using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Domain.Abstractions;
using Amethyst.EventStore.Streams.Repositories;

namespace Amethyst.EventStore.Streams.Resolvers
{
    public sealed class AggregateStreamIdResolver : IAggregateStreamIdResolver
    {
        public StreamId Resolve<TAggregate, TId>(TId id)
            where TAggregate : IAggregate<TId>
            where TId : IGuidId
        {
            return new StreamId(typeof(TAggregate).Name, id.Value);
        }
    }
}