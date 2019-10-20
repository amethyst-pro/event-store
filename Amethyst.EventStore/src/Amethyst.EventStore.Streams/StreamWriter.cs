using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Streams.Abstractions;

namespace Amethyst.EventStore.Streams
{
    public sealed class StreamWriter : IStreamWriter
    {
        private readonly IEventsWriter _writer;
        private readonly IEventSerializer _serializer;

        public StreamWriter(IEventsWriter writer, IEventSerializer serializer)
        {
            _writer = writer;
            _serializer = serializer;
        }

        public Task<WriteResult> Append(
            StreamId stream, long expectedVersion, IEnumerable<StreamEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var data = events
                .Select(e => _serializer.Serialize(e.Event, Guid.NewGuid(), e.Metadata))
                .ToArray();

            return _writer.AppendToStreamAsync(stream, expectedVersion, data);
        }
    }
}