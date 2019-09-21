namespace Amethyst.EventStore.Streams.Abstractions
{
    public interface ITriggerThresholdResolver
    {
        int ResolveByAggregate<T>();
    }
}