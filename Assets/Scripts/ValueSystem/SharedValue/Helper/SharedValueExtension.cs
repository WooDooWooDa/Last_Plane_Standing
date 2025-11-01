using UnityEngine;

namespace ValueSystem
{
    public static class SharedValueExtension
    {
        // Trying something, maybe its not the place for it. Calculate when needed in logic script
        public static bool GetChanceBase(this SharedValue<float> value)
        {
            return value.GetBase() >= Random.value;
        }
        
        public static bool GetChance(this SharedValue<float> value)
        {
            return value.Get() >= Random.value;
        }
    }
}