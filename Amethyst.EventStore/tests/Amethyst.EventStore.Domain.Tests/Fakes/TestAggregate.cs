using System;
using System.Collections.Generic;
using Amethyst.EventStore.Domain.Abstractions;

namespace Amethyst.EventStore.Domain.Tests.Fakes
{
    public sealed class TestAggregate : AggregateBase<Guid>
    {
        public TestAggregate(Guid id, long version, IReadOnlyCollection<IDomainEvent> events)
            : base(id, version, events)
        {
        }

        public int ExecutedCount { get; private set; }

        public TestAggregate(Guid id) : base(id)
        {
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