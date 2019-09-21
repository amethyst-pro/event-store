using System;

namespace Amethyst.EventStore.Domain.Abstractions
{
    public interface IGuidId
    {
        Guid Value { get; }
    }
}