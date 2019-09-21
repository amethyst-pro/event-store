using Amethyst.EventStore.Domain.Abstractions;

namespace Amethyst.EventStore.Domain
{
    public abstract class SnapshotableAggregate<TId> :
        AggregateBase<TId>,
        ISnapshotableAggregate<TId>
    {
        protected SnapshotableAggregate(TId id)
            : base(id)
        {
        }

        protected void ApplySnapshot(IAggregateSnapshot snapshot)
        {
            StoredSnapshotVersion = snapshot.Version;
            OnApplyEvent(snapshot);
        }

        public long StoredSnapshotVersion { get; private set; } = -1;

        public abstract IAggregateSnapshot GetSnapshot();
    }
}