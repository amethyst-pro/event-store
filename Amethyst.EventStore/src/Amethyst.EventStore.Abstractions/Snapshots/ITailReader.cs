using System.Threading.Tasks;

namespace Amethyst.EventStore.Abstractions.Snapshots
{
    public interface ITailReader<T>
    {
        Task<SliceReadResult<T>> Get(StreamId id);
    }
}