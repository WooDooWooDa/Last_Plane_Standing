using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using UnityEngine;
using ValueSystem.Modifiers;

namespace ValueSystem
{
    public abstract class SharedValue<T> : ScriptableObject
    {
        [SerializeField] protected string displayValueName;
        [SerializeField] protected T value;
        [SerializeField] protected bool allowEditAnywhere = false;  // Used in custom value drawer
        [SerializeField, DisableInPlayMode, DisableInEditMode] protected List<ValueModifier<T>> modifiers = new();
        
        // Runtime caching
        [NonSerialized] private T _cachedValue;
        [NonSerialized] protected bool _isValueDirty = true;
        
        public T GetBase() => value;
        public T Get()
        {
            if (_isValueDirty)
            {
                _cachedValue = modifiers.Count <= 0 ? GetBase() :
                    modifiers.OrderBy(x => x.GetRank()).Aggregate(GetBase(), (res, next) =>
                        next.ApplyModifier(res)
                    );
                _isValueDirty = false;
            }
            return _cachedValue;
        }

        public void AddModifier(ValueModifier<T> modifier)
        {
            if (modifiers.Contains(modifier)) return;
            modifiers.Add(modifier);
            _isValueDirty = true;
        }
        
        public void RemoveModifier(ValueModifier<T> modifier)
        {
            modifiers.Remove(modifier);
            _isValueDirty = true;
        }

        public override string ToString()
        {
            return displayValueName;
        }
    }
}