namespace Amethyst.Domain.EventStore
{
    public interface IAggregateOptions<TAggregate>
    {
        public long SnapshotThreshold { get; }
    }
}