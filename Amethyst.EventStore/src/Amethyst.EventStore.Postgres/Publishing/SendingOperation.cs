using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Publishing;
using Npgsql;

namespace Amethyst.EventStore.Postgres.Publishing
{
    public sealed class SendingOperation : ISendingOperation
    {
        private readonly StreamId _stream;
        private readonly long _streamLock;
        private readonly IReadOnlyCollection<RecordedEvent> _events;
        private readonly NpgsqlConnection _connection;
        private readonly IEventPublisher _publisher;

        public SendingOperation(
            StreamId stream,
            long streamLock,
            IReadOnlyCollection<RecordedEvent> events,
            NpgsqlConnection connection, 
            IEventPublisher publisher)
        {
            _stream = stream;
            _streamLock = streamLock;
            _events = events;
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task SendAsync()
        {
            try
            {
                await _publisher.PublishAsync(_events);
                var lastEvent = _events.Last().Number;

                await using var transaction = _connection.BeginTransaction();
                await using var complete = BuildCompleteCommand(lastEvent, transaction);
                
                await complete.PrepareAsync();
                await complete.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await using (var unlock = BuildUnlockCommand())
                {
                    await unlock.PrepareAsync();
                    await unlock.ExecuteNonQueryAsync();
                }

                throw;
            }
        }

        private NpgsqlCommand BuildCompleteCommand(
            long lastSentEventNumber,
            NpgsqlTransaction transaction)
        {
            const string complete = @"
                UPDATE streams SET last_sent_event_number = @lastSentEventNumber
                    WHERE id = @streamId;
                DELETE FROM outbox WHERE stream_id = @streamId;
                SELECT pg_advisory_unlock(@lock);";

            var command = new NpgsqlCommand(complete, _connection, transaction)
            {
                Parameters =
                {
                    new NpgsqlParameter<long>("lock", _streamLock),
                    new NpgsqlParameter<long>("lastSentEventNumber", lastSentEventNumber),
                    new NpgsqlParameter<Guid>("streamId", _stream.Id)
                }
            };

            return command;
        }

        private NpgsqlCommand BuildUnlockCommand()
        {
            const string unlock = "SELECT pg_advisory_unlock(@lock);";

            var command = new NpgsqlCommand(unlock, _connection)
            {
                Parameters =
                {
                    new NpgsqlParameter<long>("lock", _streamLock),
                }
            };

            return command;
        }
    }
}