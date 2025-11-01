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
        [SerializeField, DisableInPlayMode] protected List<ValueModifier<T>> modifiers = new();
        
        public void SetBase(T newValue) => this.value = newValue;
        
        public T GetBase() => value;
        public T Get()
        {
            return modifiers.Count <= 0 ? GetBase() :
                modifiers.OrderBy(x => x.GetRank()).Aggregate(GetBase(), (res, next) =>
                    next.ApplyModifier(res)
                );
        }

        public void AddModifier(ValueModifier<T> modifier)
        {
            if (modifiers.Contains(modifier)) return;
            modifiers.Add(modifier);
        }
        
        public void RemoveModifier(ValueModifier<T> modifier)
        {
            modifiers.Remove(modifier);
        }

        public override string ToString()
        {
            return displayValueName;
        }
    }
}