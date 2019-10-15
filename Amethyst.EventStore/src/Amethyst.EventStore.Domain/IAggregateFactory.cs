using System.Collections.Generic;
using Amethyst.Domain;

namespace Amethyst.EventStore.Domain.Abstractions
{
    public interface IAggregateFactory<out TAggregate, in TId>
        where TAggregate : IAggregate<TId>
    {
        TAggregate Create(TId id, long version, IReadOnlyCollection<object> events);
    }
}