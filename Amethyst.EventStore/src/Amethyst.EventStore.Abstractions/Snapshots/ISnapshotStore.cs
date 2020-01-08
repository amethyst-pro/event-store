using System.Threading.Tasks;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Abstractions.Snapshots
{
    public interface ISnapshotStore<T>
    {
        Task<Maybe<ReadSnapshotResult<T>>> GetAsync(StreamId stream);

        Task SaveAsync(SnapshotData snapshot);
    }
}