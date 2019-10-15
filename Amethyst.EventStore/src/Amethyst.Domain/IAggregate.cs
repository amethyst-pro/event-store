using System.Collections.Generic;
using SharpJuice.Essentials;

namespace Amethyst.Domain
{
    public interface IAggregate<out TId>
    {
        TId Id { get; }
        
        Maybe<long> Version { get; }
        
        IReadOnlyCollection<object> UncommittedEvents { get; }
        
        void ClearUncommittedEvents(long newVersion);
        
        bool HasChanges();
    }
}