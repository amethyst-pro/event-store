namespace Amethyst.EventStore
{
    public readonly struct ReadResult<T>
    {
        public readonly ReadStatus Status;
        public readonly StreamId Stream;
        public readonly T Event;

        public ReadResult(ReadStatus status, StreamId stream, T @event)
        {
            Status = status;
            Stream = stream;
            Event = @event;
        }
    }
}