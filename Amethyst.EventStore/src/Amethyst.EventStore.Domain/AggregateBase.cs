using System;
using System.Collections.Generic;
using System.Linq;
using Amethyst.EventStore.Domain.Abstractions;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Domain
{
    public abstract class AggregateBase<TId> : IAggregate<TId>
    {
        private readonly List<IDomainEvent> _uncommittedEvents = new List<IDomainEvent>();
        
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
        
        public IReadOnlyCollection<IDomainEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();

        public TId Id { get; }

        public Maybe<long> Version { get; private set; }

        public void ClearUncommittedEvents(long newVersion)
        {
            _uncommittedEvents.Clear();
            Version = newVersion;
        }

        public bool HasChanges() => UncommittedEvents.Any();

        protected void ApplyEvent(IDomainEvent @event)
        {
            OnApplyEvent(@event);

            _uncommittedEvents.Add(@event);
        }

        protected abstract void OnApplyEvent(IDomainEvent @event);

        protected void ApplyCommittedEvents(IReadOnlyCollection<IDomainEvent> events)
        {
            foreach (var @event in events)
            {
                OnApplyEvent(@event);
            }
        }

        protected void When(IDomainEvent @event)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            throw new InvalidOperationException(
                $"Aggregate {GetType()} doesn't have a When method for event {@event.GetType()}.");
        }
    }
}