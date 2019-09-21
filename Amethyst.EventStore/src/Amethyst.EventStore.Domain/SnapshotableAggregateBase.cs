using System;
using System.Collections.Generic;
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

        protected SnapshotableAggregate(
            TId id,
            long version,
            IReadOnlyCollection<IDomainEvent> events)
            : base(id, version, events)
        {
        }

        protected SnapshotableAggregate(
            TId id,
            long version,
            IAggregateSnapshot snapshot,
            IReadOnlyCollection<IDomainEvent> events)
            : base(id, version)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));

            ApplySnapshot(snapshot);
            ApplyCommittedEvents(events);
        }

        private void ApplySnapshot(IAggregateSnapshot snapshot)
        {
            StoredSnapshotVersion = snapshot.Version;
            OnApplyEvent(snapshot);
        }

        public long StoredSnapshotVersion { get; private set; } = -1;

        public abstract IAggregateSnapshot GetSnapshot();
    }
}