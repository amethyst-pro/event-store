namespace Amethyst.EventStore.Streams.Abstractions.Metadata
{
    public readonly struct EventMetadata
    {
        public EventMetadata(string user, string correlationId, bool isCompressed)
        {
            User = user;
            CorrelationId = correlationId;
            IsCompressed = isCompressed;
        }
        
        public string User { get; }
        
        public string CorrelationId { get; }
        
        public bool IsCompressed { get; }
    }
}