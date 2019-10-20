using System;
using System.Collections.Generic;

namespace Amethyst.EventStore.Domain.Tests.Fakes
{
    public sealed class TestAggregate : AggregateBase<Guid>
    {
        public int ExecutedCount { get; private set; }

        public TestAggregate(Guid id) : base(id)
        {
        }
        
        public TestAggregate(Guid id, long version, IReadOnlyCollection<IDomainEvent> events)
            : base(id, version)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));

            ApplyCommittedEvents(events);
        }

        public void Count()
        {
            ApplyEvent(new TestCounted());
        }

        protected override void OnApplyEvent(IDomainEvent @event)
        {
            When((dynamic)@event);
        }
        

        private void When(TestCounted @event)
        {
            ExecutedCount++;
        }
    }
}