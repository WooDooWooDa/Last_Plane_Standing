using System;
using UnityEngine;

namespace ValueSystem
{
    [Serializable]
    public class FloatSharedValueWithFallback
    {
        [SerializeField] private float _fallbackValue;
        [SerializeField] private FloatSharedValue _value;

        public float Get()
        {
            return _value?.Get() ?? _fallbackValue;
        }
    }
}