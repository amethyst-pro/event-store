namespace Amethyst.EventStore.Abstractions.Serialization
{
    public interface IRecordedEventSerializer
    {
        byte[] Serialize(RecordedEvent @event);
    }
}