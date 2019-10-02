using System.Threading.Tasks;
using Amethyst.EventStore.Domain.Abstractions;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Abstractions
{
    public interface ISnapshotStore
    {
        Task<Maybe<IAggregateSnapshot>> ReadSnapshotAsync<T, TId>(StreamId streamId)
            where T: IAggregate<TId>;

        Task SaveSnapshotAsync<T, TId>(IAggregateSnapshot snapshot, StreamId streamId)
            where T: IAggregate<TId>;
    }
}