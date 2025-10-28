using System;
using UnityEngine;

namespace ValueSystem.Modifiers
{
    public abstract class ValueModifier<T> : ScriptableObject
    {
        public string symbol => modifierSymbol;
        public T GetModifier() => modifier;
        
        [SerializeField] private bool negativeModifier;
        protected T modifier;
        protected string modifierSymbol;     // +,x,etc

        public void SetModifierValue(T newValue) => modifier = newValue;
        
        public abstract T ApplyModifier(T valueToModify);

        public int GetRank() => modifierSymbol switch
        {
            "+" => 4,
            "x" => 3,
            _ => -1
        };

        protected abstract void Reset();
    }
}