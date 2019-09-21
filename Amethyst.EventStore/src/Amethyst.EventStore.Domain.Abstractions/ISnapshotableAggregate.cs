namespace Amethyst.EventStore.Domain.Abstractions
{
    public interface ISnapshotableAggregate<out TId> : IAggregate<TId>
    {
        long StoredSnapshotVersion { get; }

        IAggregateSnapshot GetSnapshot();
    }
}