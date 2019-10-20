using SharpJuice.Essentials;

namespace Amethyst.Domain
{
    public interface ISnapshotableAggregate<out TId, out TSnapshot> : IAggregate<TId>
        where TSnapshot : IAggregateSnapshot
    {
        Maybe<long> StoredSnapshotVersion { get; }

        TSnapshot GetSnapshot();
    }
}