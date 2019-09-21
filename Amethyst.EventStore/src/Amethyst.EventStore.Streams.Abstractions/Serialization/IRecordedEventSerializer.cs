namespace Amethyst.EventStore.Streams.Abstractions.Serialization
{
    public interface IRecordedEventSerializer
    {
        byte[] Serialize(RecordedEvent @event);
    }
}