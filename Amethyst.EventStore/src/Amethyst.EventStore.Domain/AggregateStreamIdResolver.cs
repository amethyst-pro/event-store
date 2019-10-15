using Amethyst.Domain;

namespace Amethyst.EventStore.Domain
{
    public sealed class AggregateStreamIdResolver : IAggregateStreamIdResolver
    {
        public StreamId Resolve<TAggregate, TId>(TId id)
            where TAggregate : IAggregate<TId>
            where TId : IGuidId 
            => new StreamId(typeof(TAggregate).Name, id.Value);
    }
}