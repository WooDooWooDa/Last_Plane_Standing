using System.Linq;
using UnityEngine;

namespace ValueSystem
{
    [CreateAssetMenu(fileName = "newChanceValue", menuName = "SO/Values/ChanceValue", order = 4)]
    public class ChanceSharedValue : SharedValue<float>
    {
        public bool GetBaseChance() => value >= Random.value;

        public bool GetChance()
        {
            return modifiers.Count <= 0 ? GetBaseChance() :
                modifiers.OrderBy(x => x.GetRank()).Aggregate(GetBase(), (res, next) =>
                    next.ApplyModifier(res)
                ) >= Random.value;
        }
    }
}