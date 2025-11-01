using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace ValueSystem
{
    //[CreateAssetMenu(fileName = "newRandomRangeValue", menuName = "SO/Values/RandomRangeValue", order = 3)]
    [Obsolete("Use RangeSharedValue.GetRandomInRange() instead")]
    public class RandomRangeSharedValue : RangeSharedValue
    {
        public float GetRandomBase() => Random.Range(range.x, range.y);
        public float GetRandom()
        {
            return modifiers.Count <= 0 ? GetRandomBase() :
                modifiers.OrderBy(x => x.GetRank()).Aggregate(GetRandomBase(), (res, next) =>
                    next.ApplyModifier(res)
                );
        }
    }
}