﻿using System;
using System.Threading.Tasks;

namespace Amethyst.EventStore.Streams
{
    public sealed class StreamReader : IStreamReader
    {
        private readonly EventsReader<StoredEvent> _eventsReader;

        public StreamReader(PgsqlConnections settings, IEventStoreContext context, IDbEventReader<StoredEvent> reader)
        {
            _eventsReader = new EventsReader<StoredEvent>(settings, context, reader);
        }

        public Task<SliceReadResult<StoredEvent>> ReadEventsForward(StreamId stream, long start,
            int count = int.MaxValue)
        {
            return _eventsReader.ReadEventsForwardAsync(stream, start, count);
        }

        public async Task<ReadResult<StoredEvent>> ReadEvent(StreamId stream, long eventNumber)
        {
            var eventsReadResult = await _eventsReader.ReadEventsForwardAsync(stream, eventNumber, count: 1);

            switch (eventsReadResult.Status)
            {
                case ReadStatus.Success:
                    return new ReadResult<StoredEvent>(ReadStatus.Success, stream,
                        eventsReadResult.Events.Single());
                case ReadStatus.NotFound:
                    return new ReadResult<StoredEvent>(ReadStatus.NotFound, stream, default);
                case ReadStatus.NoStream:
                    return new ReadResult<StoredEvent>(ReadStatus.NoStream, stream, default);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}