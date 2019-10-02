namespace Amethyst.EventStore.Abstractions
{
    public enum ReadStatus
    {
        Success,
        NotFound,
        NoStream,
        StreamDeleted
    }
}