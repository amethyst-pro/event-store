using System.Threading.Tasks;

namespace Amethyst.EventStore.Abstractions.Snapshots
{
    public interface ISnapshotableEventsReader<TEvent, TSnapshot>
    {
        Task<SnapshotableReadResult<TEvent, TSnapshot>> ReadEvents(StreamId stream);
    }
}