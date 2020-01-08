using System.Collections.Generic;

namespace Amethyst.Domain.EventStore.Snapshots
{
    public interface ISnapshotableAggregateFactory<out TAggregate, in TId, in TSnapshot>
        : IAggregateFactory<TAggregate, TId>
        where TAggregate : IAggregate<TId>
        where TSnapshot : IAggregateSnapshot
    {
        TAggregate Create(TId id, long version, TSnapshot snapshot, IReadOnlyCollection<object> events);
    }
}