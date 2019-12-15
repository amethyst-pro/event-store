namespace Amethyst.EventStore.Abstractions.Storage
{
    public interface ITriggerThresholdResolver
    {
        int ResolveByAggregate<T>();
    }
}