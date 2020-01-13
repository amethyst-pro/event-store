using System.Threading.Tasks;

namespace Amethyst.EventStore.Abstractions
{
    public interface IEventsReader<T>
    {
        Task<SliceReadResult<T>> ReadEventsForward(StreamId stream, long start, int count);

        Task<bool> Exists(StreamId stream);
    }
}