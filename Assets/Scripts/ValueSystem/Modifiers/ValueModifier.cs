using System;
using Alchemy.Inspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace ValueSystem.Modifiers
{
    public abstract class ValueModifier<T> : ScriptableObject
    {
        public T GetModifierValue() => currentModifier;

        //private SharedValue<T> _lastValueModified;  //Only use to unassign the modifier when the value changes
        //[SerializeReference, OnValueChanged(nameof(AssignModifierToValue))] private SharedValue<T> valueModified;
        [SerializeField] protected bool negativeModifier;
        [SerializeField] protected T baseModifier;
        [SerializeField, DisableInEditMode, DisableInPlayMode] protected T currentModifier;
        protected string modifierSymbol;     // +,x,%,etc

        public void SetModifierValue(T newValue) => currentModifier = newValue;
        
        public abstract T ApplyModifier(T valueToModify);

        public int GetRank() => modifierSymbol switch
        {
            "+" => 4,
            "x" => 3,
            "%" => 2,
            _ => -1
        };

       /* private void AssignModifierToValue(SharedValue<T> newValue)
        {
            _lastValueModified?.RemoveModifier(this);
            newValue?.AddModifier(this);
            _lastValueModified = newValue;
        }*/

        private void Reset()
        {
            ResetModifier();
        }

        public abstract void ResetModifier();
    }
}