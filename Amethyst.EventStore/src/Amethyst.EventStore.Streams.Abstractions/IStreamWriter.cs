using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amethyst.EventStore.Streams.Abstractions
{
    public interface IStreamWriter
    {
        Task<WriteResult> Append(
            StreamId stream,
            long expectedVersion,
            IEnumerable<StreamEvent> events);

        Task SaveSnapshot<T>(StreamId stream, T snapshot);
    }
}