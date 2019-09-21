using System.Collections.Generic;

namespace Amethyst.EventStore.Domain.Abstractions
{
    public interface IAggregateFactory<out TAggregate, in TId>
        where TAggregate : IAggregate<TId>
    {
        TAggregate Create(TId id, long version, IReadOnlyCollection<IDomainEvent> events);
    }
}