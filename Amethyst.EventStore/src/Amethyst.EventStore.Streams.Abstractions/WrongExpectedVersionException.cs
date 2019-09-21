using System;

namespace Amethyst.EventStore.Streams.Abstractions
{
    public class WrongExpectedVersionException : Exception
    {
        public WrongExpectedVersionException(string message)
            : base(message)
        {
        }

        public WrongExpectedVersionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}