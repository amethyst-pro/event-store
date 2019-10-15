using System.Collections.Generic;

namespace Amethyst.EventStore.Streams
{
    public readonly struct StreamReadResult
    {
        public IReadOnlyCollection<StoredEvent> Events { get; }
        public long NextExpectedVersion { get; }

        public StreamReadResult(IReadOnlyCollection<StoredEvent> events, long nextExpectedVersion)
        {
            Events = events;
            NextExpectedVersion = nextExpectedVersion;
        }
    }
}