using System.Threading.Tasks;
using Amethyst.Domain;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Domain
{
    public interface ISnapshotStore
    {
        Task<Maybe<IAggregateSnapshot>> ReadSnapshotAsync<T, TId>(StreamId stream)
            where T: IAggregate<TId>;

        Task SaveSnapshotAsync<T, TId>(IAggregateSnapshot snapshot, StreamId stream)
            where T: IAggregate<TId>;
    }
}