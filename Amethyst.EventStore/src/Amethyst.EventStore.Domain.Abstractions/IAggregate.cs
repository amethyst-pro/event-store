using System.Collections.Generic;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Domain.Abstractions
{
    public interface IAggregate<out TId>
    {
        TId Id { get; }
        
        Maybe<long> Version { get; }
        
        IReadOnlyCollection<IDomainEvent> UncommittedEvents { get; }
        
        void ClearUncommittedEvents(long newVersion);
        
        bool HasChanges();
    }
}