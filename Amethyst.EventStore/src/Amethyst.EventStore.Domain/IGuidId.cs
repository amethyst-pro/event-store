using System;

namespace Amethyst.EventStore.Domain
{
    public interface IGuidId
    {
        Guid Value { get; }
    }
}