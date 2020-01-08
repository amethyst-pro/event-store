using System.Collections.Generic;

namespace Amethyst.Domain.EventStore
{
    public interface IAggregateFactory<out TAggregate, in TId>
        where TAggregate : IAggregate<TId>
    {
        TAggregate Create(TId id, long version, IReadOnlyCollection<object> events);
    }
}