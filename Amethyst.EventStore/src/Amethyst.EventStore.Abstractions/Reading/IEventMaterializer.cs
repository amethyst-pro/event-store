namespace Amethyst.EventStore.Abstractions.Reading
{
    public interface IEventMaterializer<out T>
    {
        T Create(EventHeader header, IEventDataReader reader);
    }
}