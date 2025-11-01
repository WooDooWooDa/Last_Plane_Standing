using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace ValueSystem
{
    //[CreateAssetMenu(fileName = "newChanceValue", menuName = "SO/Values/ChanceValue", order = 4)]
    [Obsolete("Use SharedValue.GetChance instead")]
    public class ChanceSharedValue : SharedValue<float>
    {
        public bool GetChanceBase() => GetBase() >= Random.value;

        public bool GetChance()
        {
            return modifiers.Count <= 0 ? GetChanceBase() :
                modifiers.OrderBy(x => x.GetRank()).Aggregate(GetBase(), (res, next) =>
                    next.ApplyModifier(res)
                ) >= Random.value;
        }
    }
}