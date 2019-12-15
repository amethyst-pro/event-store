using System;

namespace Amethyst.EventStore.Postgres
{
    public sealed class DbConnections
    {
        public DbConnections(string @default, string readOnly)
        {
            if (string.IsNullOrWhiteSpace(@default))
                throw new ArgumentException("Default connection sting not specified.", nameof(@default));
            
            if (string.IsNullOrWhiteSpace(readOnly))
                throw new ArgumentException("ReadOnly connection sting not specified.", nameof(readOnly));
            
            Default = @default;
            ReadOnly = readOnly;
        }

        public string Default{ get;}
        
        public string ReadOnly{ get; }
    }
}