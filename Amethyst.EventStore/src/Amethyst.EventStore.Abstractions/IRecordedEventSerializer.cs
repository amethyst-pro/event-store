namespace Amethyst.EventStore.Abstractions
{
    public interface IRecordedEventSerializer
    {
        byte[] Serialize(RecordedEvent @event);
    }
}