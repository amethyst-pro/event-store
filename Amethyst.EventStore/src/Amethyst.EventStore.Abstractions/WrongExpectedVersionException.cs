using System;

namespace Amethyst.EventStore.Abstractions
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