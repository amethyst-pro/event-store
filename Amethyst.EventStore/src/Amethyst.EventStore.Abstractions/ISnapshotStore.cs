using System.Threading.Tasks;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Abstractions
{
    public interface ISnapshotStore<T>
    {
        Task<Maybe<T>> GetAsync(StreamId stream);

        Task SaveAsync(StreamId stream, T snapshot);
    }
}