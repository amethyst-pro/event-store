using System.Threading.Tasks;

namespace Amethyst.EventStore.Abstractions.Snapshots
{
    public interface ITailWriter
    {
        Task MoveFromStream(StreamId id, long maxEventNumber);
    }
}