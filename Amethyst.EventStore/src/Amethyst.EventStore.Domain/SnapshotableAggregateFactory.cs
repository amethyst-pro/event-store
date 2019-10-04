using System;
using System.Collections.Generic;
using Amethyst.EventStore.Domain.Abstractions;

namespace Amethyst.EventStore.Domain
{
    public sealed class SnapshotableAggregateFactory<TAggregate, TId> : ISnapshotableAggregateFactory<TAggregate, TId>
        where TAggregate : IAggregate<TId>
    {
        private readonly Func<TId, long, IReadOnlyCollection<IDomainEvent>, TAggregate> _factory;

        public SnapshotableAggregateFactory(Func<TId, long, IReadOnlyCollection<IDomainEvent>, TAggregate> factory)
            => _factory = factory;

        public TAggregate Create(TId id, long version, IReadOnlyCollection<IDomainEvent> events) 
            => _factory(id, version, events);
                                                       
        public TAggregate Create(TId id, long version, IAggregateSnapshot snapshot, IReadOnlyCollection<IDomainEvent> events) 
            => _factory(id, version, events);
    }
}