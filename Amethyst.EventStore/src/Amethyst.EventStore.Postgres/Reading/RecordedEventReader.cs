using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions.Reading;

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

        public async Task<ReadResult<T>> Read(StreamId stream, DbDataReader reader)
        {
            var events = new List<T>();
            var lastEventNumber = 0L;

            while (await reader.ReadAsync())
            {
                var id = reader.GetGuid(EventFieldMap.IdIndex);
                var number = reader.GetInt64(EventFieldMap.NumberIndex);
                var type = _typeRegistry.GetTypeName(reader.GetInt16(EventFieldMap.TypeIndex), stream);
                var created = reader.GetDateTime(EventFieldMap.CreatedIndex);

                var @event = _materializer.Create(
                    new EventHeader(
                        stream,
                        id,
                        number,
                        type,
                        created),
                    new DbReader(reader)
                );

                events.Add(@event);
                lastEventNumber = number;
            }

            return new ReadResult<T>(
                events,
                events.Count > 0 ? lastEventNumber : 0L);
        }

        private sealed class DbReader : IEventDataReader
        {
            private readonly DbDataReader _reader;

            public DbReader(DbDataReader reader)
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