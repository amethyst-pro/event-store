using System;
using System.Collections.Generic;
using Amethyst.EventStore.Domain.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace Amethyst.EventStore.Domain.Tests
{
    public sealed class AggregateBaseTests
    {
        [Fact]
        public void ChangingAggregate_EventIssued()
        {
            var sut = new TestAggregate(Guid.NewGuid());

            sut.Count();

            sut.UncommittedEvents.Should().ContainSingle().Subject.Should().BeOfType<TestCounted>();
        }

        [Fact]
        public void ClearingEvents_UncommittedEventsIsEmpty()
        {
            var sut = new TestAggregate(Guid.NewGuid());

            sut.Count();

            sut.ClearUncommittedEvents(0);

            sut.UncommittedEvents.Should().BeEmpty();
        }

        [Fact]
        public void RehydratingAggregate_EventsApplied()
        {
            var sut = new TestAggregate(Guid.NewGuid(), 1, new[] {new TestCounted(), new TestCounted()});

            sut.ExecutedCount.Should().Be(2);
        }

        [Fact]
        public void CreatingWithVersion_VersionSet()
        {
            var sut = new TestAggregate(Guid.NewGuid(), 2, new List<IDomainEvent>());

            sut.Version.Single().Should().Be(2L);
        }

        [Fact]
        public void CreatingWithUnknownEvent_ThrowException()
        {
            Func<TestAggregate> action = () => new TestAggregate(Guid.NewGuid(), 2, 
                new[] {new UnknownEvent()});

            action.Should().Throw<InvalidOperationException>();
        }
    }
}