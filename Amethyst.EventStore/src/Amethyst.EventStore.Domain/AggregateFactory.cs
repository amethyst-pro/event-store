using System;
using System.Collections.Generic;
using Amethyst.EventStore.Domain.Abstractions;

namespace Amethyst.EventStore.Domain
{
    public sealed class AggregateFactory<TAggregate, TId> : IAggregateFactory<TAggregate, TId>
        where TAggregate : IAggregate<TId>
    {
        private readonly Func<TId, long, IReadOnlyCollection<IDomainEvent>, TAggregate> _factory;

        public AggregateFactory(Func<TId, long, IReadOnlyCollection<IDomainEvent>, TAggregate> factory)
            => _factory = factory;

        public TAggregate Create(TId id, long version, IReadOnlyCollection<IDomainEvent> events) 
            => _factory(id, version, events);
    }
}