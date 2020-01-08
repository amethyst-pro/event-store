using System;
using System.Collections.Generic;

namespace Amethyst.Domain.EventStore.Snapshots
{
    public sealed class SnapshotableAggregateFactory<TAggregate, TId> : ISnapshotableAggregateFactory<TAggregate, TId>
        where TAggregate : IAggregate<TId>
    {
        private readonly Func<TId, long, IReadOnlyCollection<object>, TAggregate> _factory;
        private readonly Func<TId, long, IAggregateSnapshot, IReadOnlyCollection<object>, TAggregate> _factorySnapshot;

        public SnapshotableAggregateFactory(
            Func<TId, long, IReadOnlyCollection<object>, TAggregate> factory,
            Func<TId, long, IAggregateSnapshot, IReadOnlyCollection<object>, TAggregate> factorySnapshot)
        {
            _factory = factory;
            _factorySnapshot = factorySnapshot;
        }

        public TAggregate Create(TId id, long version, IReadOnlyCollection<object> events) 
            => _factory(id, version, events);
                                                       
        public TAggregate Create(TId id, long version, IAggregateSnapshot snapshot, IReadOnlyCollection<object> events) 
            => _factorySnapshot(id, version, snapshot, events);
    }
}