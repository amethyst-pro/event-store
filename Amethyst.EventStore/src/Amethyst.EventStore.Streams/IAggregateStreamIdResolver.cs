using System;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Domain.Abstractions;

namespace Amethyst.EventStore.Streams
{
    public interface IAggregateStreamIdResolver
    {
        StreamId Resolve<TAggregate, TId>(Guid id)
            where TAggregate : IAggregate<TId>;
    }
}