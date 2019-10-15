namespace Amethyst.EventStore.Domain
{
    public interface ITriggerThresholdResolver
    {
        int ResolveByAggregate<T>();
    }
}