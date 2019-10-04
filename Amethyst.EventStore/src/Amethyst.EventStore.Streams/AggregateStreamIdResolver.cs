using System;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Domain.Abstractions;

namespace Amethyst.EventStore.Streams
{
    public sealed class AggregateStreamIdResolver : IAggregateStreamIdResolver
    {
        public StreamId Resolve<TAggregate, TId>(Guid id)
            where TAggregate : IAggregate<TId>
            => new StreamId(typeof(TAggregate).Name, id);
    }
}