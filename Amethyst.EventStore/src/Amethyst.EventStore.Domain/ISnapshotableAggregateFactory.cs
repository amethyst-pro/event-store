using System.Collections.Generic;
using Amethyst.Domain;

namespace Amethyst.EventStore.Domain
{
    public interface ISnapshotableAggregateFactory<out TAggregate, in TId>
        : IAggregateFactory<TAggregate, TId>
        where TAggregate : IAggregate<TId>
    {
        TAggregate Create(TId id, long version, IAggregateSnapshot snapshot, IReadOnlyCollection<object> events);
    }
}