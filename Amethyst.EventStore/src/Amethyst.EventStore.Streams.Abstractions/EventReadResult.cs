namespace Amethyst.EventStore.Streams.Abstractions
{
    public readonly struct EventReadResult
    {
        public EventReadResult(ReadStatus status, StreamId streamId, RecordedEvent @event)
        {
            Status = status;
            StreamId = streamId;
            Event = @event;
        }
        
        public ReadStatus Status { get; }
        
        public StreamId StreamId { get; }
        
        public RecordedEvent Event { get; }
    }
}