namespace Amethyst.EventStore.Abstractions
{
    public interface ITriggerThresholdResolver
    {
        int ResolveByAggregate<T>();
    }
}