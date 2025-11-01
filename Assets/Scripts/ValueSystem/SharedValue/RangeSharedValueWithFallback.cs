using System;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

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

        [CanBeNull] public RangeSharedValue value => _value;
        
        public Vector2 GetBase()
        {
            return _value?.GetBase() ?? _fallbackValue;
        }
        
        public Vector2 Get()
        {
            return _value?.Get() ?? _fallbackValue;
        }
        
        // Allows for implicit cast like : "RangeSharedValue sharedValue = new Vector2(10f, 20f);"
        public static implicit operator RangeSharedValueWithFallback(Vector2 fallbackRange) => new(fallbackRange);
    }
}