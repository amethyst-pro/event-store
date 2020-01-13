using System.Threading.Tasks;

namespace Amethyst.EventStore.Streams.Abstractions
{
    public interface IStreamReader
    {
        Task<SliceReadResult<StoredEvent>> ReadEventsForward(StreamId stream, long start,
            int count = int.MaxValue);

        Task<ReadResult<StoredEvent>> ReadEvent(StreamId stream, long eventNumber);

        Task<SnapshotableReadResult<StoredEvent, TSnapshot>> ReadStream<TSnapshot>(StreamId stream);

        Task<bool> Exists(StreamId stream);
    }
}