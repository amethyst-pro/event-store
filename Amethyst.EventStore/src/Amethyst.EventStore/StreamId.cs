using System;

namespace Amethyst.EventStore
{
    public readonly struct StreamId : IEquatable<StreamId>
    {
        public readonly string Category;
        public readonly Guid Id;

        public StreamId(string category, Guid id)
        {
            Category = category ?? throw new ArgumentNullException(nameof(category));
            Id = id;
        }

        public override string ToString() => $"{Category}-{Id}";

        public bool Equals(StreamId other)
        {
            return string.Equals(Category, other.Category) && Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is StreamId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Category.GetHashCode() * 397) ^ Id.GetHashCode();
            }
        }
    }
}