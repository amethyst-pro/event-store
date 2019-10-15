namespace Amethyst.EventStore
{
    public enum ReadStatus
    {
        Success,
        NotFound,
        NoStream,
        StreamDeleted
    }
}