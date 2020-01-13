using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Amethyst.EventStore.Streams.Abstractions;
using Amethyst.EventStore.Streams.Abstractions.Metadata;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Streams
{
    public sealed class Stream : IStream
    {
        private readonly IStreamReader _reader;
        private readonly IStreamWriter _writer;
        private readonly StreamId _id;
        private readonly IMetadataContext _metadataContext;

        public Stream(
            StreamId id,
            IStreamReader reader,
            IStreamWriter writer,
            IMetadataContext metadataContext)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _id = id;
            _metadataContext = metadataContext ?? throw new ArgumentNullException(nameof(metadataContext));
        }

        public StreamId Id { get; }

        public async Task<long> GetLastEventNumber()
        {
            return (await ReadLastEvent())
                .Bind(e => e.EventNumber)
                .OrElse(-1);
        }

        public async Task<StreamReadResult> ReadEventsForward(Maybe<long> startPosition = default)
        {
            var storedEvents = await _reader.ReadEventsForward(
                _id,
                startPosition.OrElse(StreamPosition.Start));

            if (storedEvents.Status != ReadStatus.Success)
                return new StreamReadResult(Array.Empty<StoredEvent>(), ExpectedVersion.NoStream);

            return new StreamReadResult(storedEvents.Events, storedEvents.LastEventNumber);
        }

        public Task<Maybe<StoredEvent>> ReadFirstEvent()
            => ReadEvent(StreamPosition.Start);

        public Task<Maybe<StoredEvent>> ReadLastEvent()
            => ReadEvent(StreamPosition.End);

        public Task<WriteResult> Append(IEnumerable<object> events, Maybe<long> expectedVersion = default)
        {
            return _writer.AppendAsync(
                _id,
                expectedVersion.OrElse(ExpectedVersion.NoStream),
                events.Select(e => new StreamEvent(e, _metadataContext.GetMetadata())));
        }

        public async Task<bool> HasEvents()
        {
            var result = await _reader.ReadEvent(_id, StreamPosition.Start);
            return result.Status == ReadStatus.Success && result.Event != null;
        }

        private async Task<Maybe<StoredEvent>> ReadEvent(long streamPosition)
        {
            var result = await _reader.ReadEvent(_id, streamPosition);
            return GetEventFrom(result);
        }

        private Maybe<StoredEvent> GetEventFrom(ReadResult<StoredEvent> result)
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
                    throw new InvalidOperationException($"Stream {_id} has been deleted.");
                default:
                    throw new NotSupportedException(result.Status.ToString());
            }
        }
    }
}