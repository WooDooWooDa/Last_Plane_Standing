using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ValueSystem.Modifiers;

namespace ValueSystem
{
    [CreateAssetMenu(fileName = "newFloatValue", menuName = "SO/Values/FloatValue", order = 0)]
    public class FloatValue : ScriptableObject
    {
        [SerializeField] private string displayValueName;
        [SerializeField] private float value;
        [SerializeField] private List<ValueModifier<float>> modifiers = new();
        
        public float GetBase() => value;
        public float Get()
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