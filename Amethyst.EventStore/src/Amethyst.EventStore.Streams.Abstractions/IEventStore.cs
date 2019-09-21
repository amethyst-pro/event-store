using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amethyst.EventStore.Streams.Abstractions
{
    public interface IEventStore
    {
        Task<EventSliceReadResult> ReadStreamEventsForwardAsync(StreamId streamId, long start, int count = int.MaxValue);
        
        Task<EventReadResult> ReadEventAsync(StreamId streamId, long eventNumber);
       
        Task<WriteResult> AppendToStreamAsync(StreamId streamId, long expectedVersion, IEnumerable<EventData> events);
    }
}