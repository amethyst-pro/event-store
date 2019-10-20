using System;
using System.Collections.Generic;
using Amethyst.Domain;

namespace Amethyst.EventStore.Domain
{
    public sealed class AggregateFactory<TAggregate, TId> : IAggregateFactory<TAggregate, TId>
        where TAggregate : IAggregate<TId>
    {
        private readonly Func<TId, long, IReadOnlyCollection<object>, TAggregate> _factory;

        public AggregateFactory(Func<TId, long, IReadOnlyCollection<object>, TAggregate> factory)
            => _factory = factory;

        public TAggregate Create(TId id, long version, IReadOnlyCollection<object> events) 
            => _factory(id, version, events);
    }
}