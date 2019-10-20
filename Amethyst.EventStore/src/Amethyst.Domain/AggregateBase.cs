using System;
using System.Collections.Generic;
using System.Linq;
using SharpJuice.Essentials;

namespace Amethyst.Domain
{
    public abstract class AggregateBase<TId> : IAggregate<TId>
    {
        private readonly List<object> _uncommittedEvents = new List<object>();

        protected AggregateBase(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            Id = id;
        }

        protected AggregateBase(TId id, long version)
            : this(id)
        {
            if (version < 0) throw new ArgumentException("Must be >= 0", nameof(version));

            Version = version;
        }

        public IReadOnlyCollection<object> UncommittedEvents => _uncommittedEvents;

        public TId Id { get; }

        public Maybe<long> Version { get; private set; }

        public void ClearUncommittedEvents(long newVersion)
        {
            _uncommittedEvents.Clear();
            Version = newVersion;
        }

        public bool HasChanges() => UncommittedEvents.Any();

        protected abstract void OnApplyEvent(object @event);

        protected void ApplyEvent(object @event)
        {
            OnApplyEvent(@event);

            _uncommittedEvents.Add(@event);
        }

        protected void ApplyCommittedEvents(IReadOnlyCollection<object> events)
        {
            foreach (var @event in events)
            {
                OnApplyEvent(@event);
            }
        }
    }
}