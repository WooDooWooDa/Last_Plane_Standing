using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ValueSystem.Modifiers;

namespace ValueSystem
{
    [CreateAssetMenu(fileName = "newFloatValue", menuName = "SO/Values/IntValue", order = 0)]
    public class IntValue : ScriptableObject
    {
        [SerializeField] private string displayValueName;
        [SerializeField] private int value;
        [SerializeField] private List<ValueModifier<int>> modifiers = new();
        
        public int GetBase() => value;
        public int Get()
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