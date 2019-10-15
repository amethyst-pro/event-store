using System;
using System.Collections.Generic;

namespace Amethyst.EventStore.Domain
{
    public class TriggerThresholdResolver : ITriggerThresholdResolver
    {
        private readonly IDictionary<Type, int> _thresholds = new Dictionary<Type, int>();
        
        public int ResolveByAggregate<T>()
        {
            var exist = _thresholds.TryGetValue(typeof(T), out var triggerthreshold);
            if (exist)
                return triggerthreshold;

            throw new InvalidOperationException($"Can't resolve type {typeof(T)}.");
        }

        public void AddTriggerThreshold<T>(int triggerThreshold)
        {
            if (triggerThreshold == default)
                throw new ArgumentException("TriggerThreshold not specified", nameof(triggerThreshold));
            _thresholds.Add(typeof(T), triggerThreshold);
        }
    }
}