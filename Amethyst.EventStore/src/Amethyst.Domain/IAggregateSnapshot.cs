namespace Amethyst.Domain
{
    public interface IAggregateSnapshot
    {
        long Version { get; }
    }
}