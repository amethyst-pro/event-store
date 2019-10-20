using SharpJuice.Essentials;

namespace Amethyst.Domain
{
    public abstract class SnapshotableAggregate<TId, TSnapshot> :
        AggregateBase<TId>, ISnapshotableAggregate<TId, TSnapshot>
        where TSnapshot : IAggregateSnapshot
    {
        protected SnapshotableAggregate(TId id)
            : base(id)
        {
        }

        protected SnapshotableAggregate(TId id, TSnapshot snapshot)
            : base(id)
        {
            StoredSnapshotVersion = snapshot.Version;
        }

        public Maybe<long> StoredSnapshotVersion { get; }

        public abstract TSnapshot GetSnapshot();
    }
}