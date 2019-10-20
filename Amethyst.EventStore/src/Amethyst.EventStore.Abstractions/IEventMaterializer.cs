namespace Amethyst.EventStore.Abstractions
{
    public interface IEventMaterializer<out T>
    {
        T Create(EventHeader header, IEventDataReader reader);
    }
}