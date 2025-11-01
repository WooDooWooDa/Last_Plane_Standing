using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ValueSystem
{
    [Serializable]
    [Obsolete("Use RangeSharedValue.GetRandomInRange() instead")]
    public class RandomRangeSharedValueWithFallback : RangeSharedValueWithFallback
    {
        public RandomRangeSharedValueWithFallback() {}
        public RandomRangeSharedValueWithFallback(Vector2 fallback) => _fallbackValue = fallback;
        public RandomRangeSharedValueWithFallback(RandomRangeSharedValue value) => _value = value;
        
        public float GetRandom()
        {
            return ((RandomRangeSharedValue)_value)?.GetRandom() ?? Random.Range(_fallbackValue.x, _fallbackValue.y);
        }
        
        // Allows for implicit cast like : "RandomRangeSharedValue sharedValue = new Vector2(10f, 20f);"
        public static implicit operator RandomRangeSharedValueWithFallback(Vector2 fallbackRange) => new(fallbackRange);
    }
}