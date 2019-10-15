using System;

namespace Amethyst.EventStore.Postgres
{
    public sealed class PgsqlConnections
    {
        public PgsqlConnections(string @default, string readOnly, string readOnlyAsync = default)
        {
            if (string.IsNullOrWhiteSpace(@default))
                throw new ArgumentException($"Default connection sting not specified.", nameof(@default));

            if (string.IsNullOrWhiteSpace(readOnly))
                throw new ArgumentException($"ReadOnly connection sting not specified.", nameof(readOnly));

            Default = @default;
            ReadOnly = readOnly;
            ReadOnlyAsync = readOnlyAsync ?? readOnly;
        }

        public string Default { get; }

        public string ReadOnly { get; }

        public string ReadOnlyAsync { get; }
    }
}