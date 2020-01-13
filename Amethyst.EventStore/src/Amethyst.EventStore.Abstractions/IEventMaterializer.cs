namespace Amethyst.EventStore.Abstractions
{
    public interface IEventMaterializer<out T>
    {
        T Create(in StreamId stream, in EventHeader header, IEventDataReader reader);
    }
}