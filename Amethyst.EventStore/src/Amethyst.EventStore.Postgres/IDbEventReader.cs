using System.Data.Common;
using System.Threading.Tasks;

namespace Amethyst.EventStore.Postgres
{
    public interface IDbEventReader<T>
    {
        Task<SliceReadResult<T>> Read(StreamId stream, DbDataReader reader);
    }
}