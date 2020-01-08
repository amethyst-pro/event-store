using System;

namespace Amethyst.Domain.EventStore
{
    public interface IGuidId
    {
        Guid Value { get; }
    }
}