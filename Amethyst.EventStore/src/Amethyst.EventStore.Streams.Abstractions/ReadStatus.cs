namespace Amethyst.EventStore.Streams.Abstractions
{
    public enum ReadStatus
    {
        Success,
        NotFound,
        NoStream,
        StreamDeleted
    }
}