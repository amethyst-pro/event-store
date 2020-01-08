using Amethyst.EventStore;

namespace Amethyst.Domain.EventStore
{
    public sealed class AggregateStreamIdResolver : IAggregateStreamIdResolver
    {
        public StreamId Resolve<TAggregate, TId>(TId id)
            where TAggregate : IAggregate<TId>
            where TId : IGuidId 
            => new StreamId(typeof(TAggregate).Name, id.Value);
    }
}