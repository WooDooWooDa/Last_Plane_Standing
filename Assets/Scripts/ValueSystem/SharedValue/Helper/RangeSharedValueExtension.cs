using UnityEngine;

namespace ValueSystem
{
    public static class RangeSharedValueExtension
    {
        public static float GetRandomInBaseRange(this RangeSharedValue value)
        {
            var range = value.GetBase();
            return Random.Range(range.x, range.y);
        }
        
        public static float GetRandomInRange(this RangeSharedValue value)
        {
            var range = value.Get();
            return Random.Range(range.x, range.y);
        }
    }
}