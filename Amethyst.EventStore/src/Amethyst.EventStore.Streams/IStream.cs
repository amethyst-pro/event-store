using System.Collections.Generic;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Domain.Abstractions;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Streams
{
    public interface IStream
    {
        Task<WriteResult> Append(IEnumerable<IDomainEvent> events, Maybe<long> expectedVersion);
       
        Task<Maybe<StoredEvent>> ReadLastEvent();
      
        Task<Maybe<StoredEvent>> ReadFirstEvent();
      
        Task<StreamReadResult> ReadEventsForward(Maybe<long> startPosition = default);
      
        Task<long> GetLastEventNumber();
        
        Task<bool> HasEvents();
    }
}