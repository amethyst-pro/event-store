using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amethyst.EventStore.Abstractions
{
    public interface IEventsWriter
    {
        Task<WriteResult> AppendToStreamAsync(
            StreamId stream,
            long expectedVersion,
            IReadOnlyCollection<EventData> events);
    }
}