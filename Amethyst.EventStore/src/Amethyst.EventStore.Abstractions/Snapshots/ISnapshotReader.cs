using System.Threading.Tasks;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Abstractions.Snapshots
{
    public interface ISnapshotReader<T>
    {
        Task<Maybe<ReadSnapshotResult<T>>> Get(StreamId id);
    }
}