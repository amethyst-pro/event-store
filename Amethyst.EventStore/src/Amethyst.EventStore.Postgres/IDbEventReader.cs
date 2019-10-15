using System.Data.Common;
using System.Threading.Tasks;

namespace Amethyst.EventStore.Postgres
{
    public interface IDbEventReader<T>
    {
        Task<Reading.ReadResult<T>> Read(StreamId stream, DbDataReader reader);
    }
}