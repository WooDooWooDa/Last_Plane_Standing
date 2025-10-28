using UnityEngine;
using ValueSystem;

namespace Projectiles.ProjectileData
{
    public abstract class ProjectileSO : ScriptableObject
    {
        public FloatSharedValue speed;
        public float ttl = 10f;
    }
}