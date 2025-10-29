using UnityEngine;
using UnityEngine.Serialization;
using ValueSystem;

namespace Projectiles.ProjectileData
{
    public abstract class ProjectileSO : ScriptableObject
    {
        public FloatSharedValue speed;
        public float timeToLive = 5f;
        public FloatSharedValue damage;
    }
}