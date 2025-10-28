using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ValueSystem.Modifiers;

namespace ValueSystem
{
    public abstract class SharedValue<T> : ScriptableObject
    {
        [SerializeField] private string displayValueName;
        [SerializeField] private T value;
        [SerializeField] private List<ValueModifier<T>> modifiers = new();
        
        public void SetBase(T newValue) => this.value = newValue;
        
        public T GetBase() => value;
        public T Get()
        {
            return modifiers.Count <= 0 ? GetBase() :
                modifiers.OrderBy(x => x.GetRank()).Aggregate(GetBase(), (res, next) =>
                    next.ApplyModifier(res)
                );
        }

        public override string ToString()
        {
            return displayValueName;
        }
    }
}