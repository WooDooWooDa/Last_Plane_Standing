using System;
using System.Linq;
using UnityEngine;

namespace ValueSystem
{
    [CreateAssetMenu(fileName = "RangeValue", menuName = "SO/Values/RangeValue", order = 2)]
    public class RangeSharedValue : SharedValue<float>
    {
        [SerializeField] protected Vector2 range;

        [NonSerialized] private new Vector2 _cachedValue; 
        
        public new Vector2 GetBase() => range;
        public new Vector2 Get()
        {
            if (_isValueDirty)
            {
                _cachedValue = modifiers.Count <= 0 ? GetBase() :
                    modifiers.OrderBy(x => x.GetRank()).Aggregate(GetBase(), (res, next) =>
                        new Vector2(next.ApplyModifier(res.x),  next.ApplyModifier(res.y))
                    );
                _isValueDirty = false;
            }

            return _cachedValue;
        }
    }
}