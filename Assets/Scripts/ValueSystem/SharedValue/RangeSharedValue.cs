using System.Linq;
using UnityEngine;

namespace ValueSystem
{
    [CreateAssetMenu(fileName = "RangeValue", menuName = "SO/Values/RangeValue", order = 2)]
    public class RangeSharedValue : SharedValue<float>
    {
        [SerializeField] protected Vector2 range;
        
        public void SetBase(float low, float high) => range = new Vector2(low, high);
        public void SetBase(Vector2 newRange) => range = newRange;

        public new Vector2 GetBase() => range;
        public new Vector2 Get()
        {
            return modifiers.Count <= 0 ? GetBase() :
                modifiers.OrderBy(x => x.GetRank()).Aggregate(GetBase(), (res, next) =>
                    new Vector2(next.ApplyModifier(res.x),  next.ApplyModifier(res.y))
                );
        }
    }
}