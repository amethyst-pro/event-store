using System.Collections.Generic;

namespace Amethyst.EventStore.Domain.Abstractions
{
    public interface ISnapshotableAggregateFactory<out TAggregate, in TId> 
        : IAggregateFactory<TAggregate, TId>
        where TAggregate : IAggregate<TId>
    {
        TAggregate Create(TId id, long version, IAggregateSnapshot snapshot, IReadOnlyCollection<IDomainEvent> events);
    }
}