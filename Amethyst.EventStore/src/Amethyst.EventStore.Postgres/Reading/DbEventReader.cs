using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;

namespace Amethyst.EventStore.Postgres.Reading
{
    public sealed class DbEventReader<T> : IDbEventReader<T>
    {
        private readonly IEventMaterializer<T> _materializer;
        private readonly IEventTypeRegistry _typeRegistry;

        public DbEventReader(IEventMaterializer<T> materializer, IEventTypeRegistry typeRegistry)
        {
            _materializer = materializer ?? throw new ArgumentNullException(nameof(materializer));
            _typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
        }

        public async Task<SliceReadResult<T>> Read(StreamId stream, DbDataReader reader)
        {
            var events = new List<T>();
            var lastEventNumber = 0L;
            var dataReader = new EventDataReader(reader);

            while (await reader.ReadAsync())
            {
                var id = reader.GetGuid(EventFieldMap.IdIndex);
                var number = reader.GetInt64(EventFieldMap.NumberIndex);
                var type = _typeRegistry.GetTypeName(reader.GetInt16(EventFieldMap.TypeIndex), stream);
                var created = reader.GetDateTime(EventFieldMap.CreatedIndex);

                var @event = _materializer.Create(
                    stream,
                    new EventHeader(id, number, type, created),
                    dataReader);

                events.Add(@event);
                lastEventNumber = number;
            }

            return new SliceReadResult<T>(
                ReadStatus.Success,
                stream,
                events,
                lastEventNumber,
                true);
        }

        private sealed class EventDataReader : IEventDataReader
        {
            private readonly DbDataReader _reader;

            public EventDataReader(DbDataReader reader)
            {
                _reader = reader;
            }

            public Stream GetData() => _reader.GetStream(EventFieldMap.DataIndex);

            public Stream GetMetadata() => _reader.GetStream(EventFieldMap.MetadataIndex);

            public byte[] GetRawData() => _reader.GetFieldValue<byte[]>(EventFieldMap.DataIndex);

            public byte[] GetRawMetadata() => _reader.GetFieldValue<byte[]>(EventFieldMap.MetadataIndex);
        }
    }
}