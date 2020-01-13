namespace Amethyst.Metadata
{
    public readonly struct EventMetadata
    {
        public string User { get; }
        public string CorrelationId { get; }
        public bool IsCompressed { get; }

        public EventMetadata(string user, string correlationId, bool isCompressed)
        {
            User = user;
            CorrelationId = correlationId;
            IsCompressed = isCompressed;
        }
    }
}