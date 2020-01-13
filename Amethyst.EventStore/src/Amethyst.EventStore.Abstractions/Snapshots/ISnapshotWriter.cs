using System.Threading.Tasks;

namespace Amethyst.EventStore.Abstractions.Snapshots
{
    public interface ISnapshotWriter
    {
        Task Save(SnapshotData snapshot);
    }
}