using UnityEngine;
using ValueSystem;

namespace Projectiles.ProjectileData
{
    public abstract class ProjectileSO : ScriptableObject
    {
        public FloatValue speed;
        public float ttl = 10f;
    }
}