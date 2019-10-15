using System.Threading.Tasks;

namespace Amethyst.EventStore.Abstractions
{
    public interface IEventsReader<T>
    {
        Task<SliceReadResult<T>> ReadEventsForwardAsync(StreamId stream, long start, int count);
    }
}