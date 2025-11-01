using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ValueSystem
{
    [Serializable]
    public class FloatSharedValueWithFallback
    {
        [SerializeField] private float _fallbackValue;
        [SerializeField] private FloatSharedValue _value;
        
        public FloatSharedValueWithFallback(float fallback) => _fallbackValue = fallback;
        public FloatSharedValueWithFallback(FloatSharedValue value) => _value = value;
        
        [CanBeNull] public FloatSharedValue value => _value;
        
        public float GetBase()
        {
            return _value?.GetBase() ?? _fallbackValue;
        }
        
        public float Get()
        {
            return _value?.Get() ?? _fallbackValue;
        }
        
        // Allows for implicit cast like : "FloatSharedValueWithFallback sharedValue = 10f;"
        public static implicit operator FloatSharedValueWithFallback(float fallback) => new(fallback);
    }
}