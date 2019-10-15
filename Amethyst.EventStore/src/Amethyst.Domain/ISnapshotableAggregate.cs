namespace Amethyst.Domain
{
    public interface ISnapshotableAggregate<out TId> : IAggregate<TId>
    {
        long StoredSnapshotVersion { get; }

        IAggregateSnapshot GetSnapshot();
    }
}