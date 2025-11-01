using System;
using UnityEngine;

namespace ValueSystem
{
    [Serializable]
    public class RangeSharedValueWithFallback
    {
        [SerializeField] protected Vector2 _fallbackValue;
        [SerializeField] protected RangeSharedValue _value;

        public RangeSharedValueWithFallback(Vector2 fallback) => _fallbackValue = fallback;
        public RangeSharedValueWithFallback(RangeSharedValue value) => _value = value;
        public RangeSharedValueWithFallback() {}

        public Vector2 Get()
        {
            return _value?.Get() ?? _fallbackValue;
        }
        
        // Allows for implicit cast like : "RangeSharedValue sharedValue = new Vector2(10f, 20f);"
        public static implicit operator RangeSharedValueWithFallback(Vector2 fallbackRange) => new(fallbackRange);
    }
}