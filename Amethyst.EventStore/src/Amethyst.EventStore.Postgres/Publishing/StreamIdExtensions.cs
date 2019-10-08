using System;
using Amethyst.EventStore.Abstractions;

namespace Amethyst.EventStore.Postgres.Publishing
{
    public static class StreamIdExtensions
    {
        public static long GetLockId(this StreamId stream)
        {
            Span<byte> streamId = stackalloc byte[16];
            
            if (!stream.Id.TryWriteBytes(streamId))
                throw new InvalidOperationException("Can't write bytes");

            return BitConverter.ToInt64(streamId.Slice(0, 8))
                   ^ BitConverter.ToInt64(streamId.Slice(8));
        }
    }
}