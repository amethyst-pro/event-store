namespace Amethyst.EventStore.Domain.Abstractions
{
    public interface IAggregateSnapshot : IDomainEvent
    {
        long Version { get; }
    }
}