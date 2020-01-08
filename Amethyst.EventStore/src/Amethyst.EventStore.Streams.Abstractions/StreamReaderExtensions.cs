using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Streams.Abstractions
{
    public static class StreamReaderExtensions
    {
        public static async Task<long> GetLastEventNumber(this IStreamReader reader, StreamId id)
        {
            return (await ReadLastEvent(reader, id))
                .Bind(e => e.EventNumber)
                .OrElse(-1);
        }

        public static Task<SliceReadResult<StoredEvent>> ReadEventsForward(
            this IStreamReader reader, 
            StreamId id,
            Maybe<long> startPosition = default)
        {
            return reader.ReadEventsForward(
                id,
                startPosition.OrElse(StreamPosition.Start));
        }

        public static Task<Maybe<StoredEvent>> ReadFirstEvent(this IStreamReader reader, StreamId id)
            => ReadEvent(reader, id, StreamPosition.Start);

        public static Task<Maybe<StoredEvent>> ReadLastEvent(this IStreamReader reader, StreamId id)
            => ReadEvent(reader, id, StreamPosition.End);


        public static async Task<bool> HasEvents(this IStreamReader reader, StreamId id)
        {
            var result = await reader.ReadEvent(id, StreamPosition.Start);
            return result.Status == ReadStatus.Success && result.Event != null;
        }

        private static async Task<Maybe<StoredEvent>> ReadEvent(IStreamReader reader, StreamId id, long streamPosition)
        {
            var result = await reader.ReadEvent(id, streamPosition);
            return GetEventFrom(id, result);
        }

        private static Maybe<StoredEvent> GetEventFrom(StreamId id, ReadResult<StoredEvent> result)
        {
            switch (result.Status)
            {
                case ReadStatus.Success:
                    Debug.Assert(result.Event != null, "result.Event != null, stream = " + result.Stream);

                    return result.Event;
                case ReadStatus.NotFound:
                case ReadStatus.NoStream:
                    return new Maybe<StoredEvent>();

                case ReadStatus.StreamDeleted:
                    throw new InvalidOperationException($"Stream {id} has been deleted.");
                default:
                    throw new NotSupportedException(result.Status.ToString());
            }
        }
    }
}