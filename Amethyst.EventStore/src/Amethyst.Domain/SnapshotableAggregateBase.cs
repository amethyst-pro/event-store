using SharpJuice.Essentials;

namespace Amethyst.Domain
{
    public abstract class SnapshotableAggregate<TId, TSnapshot> :
        AggregateBase<TId>,
        ISnapshotableAggregate<TId, TSnapshot>
        where TSnapshot : IAggregateSnapshot
    {
        protected SnapshotableAggregate(TId id)
            : base(id)
        {
        }

        protected SnapshotableAggregate(TId id, long version)
            : base(id, version)
        {
        }

        protected SnapshotableAggregate(TId id, long version, long storedSnapshotVersion)
            : base(id, version)
        {
            StoredSnapshotVersion = storedSnapshotVersion;
        }

        public Maybe<long> StoredSnapshotVersion { get; }

        public abstract TSnapshot GetSnapshot();
    }
}