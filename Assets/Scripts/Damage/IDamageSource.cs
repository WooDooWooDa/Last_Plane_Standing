using Plane;

namespace Damage
{
    public interface IDamageSource
    {
        public ETeam Team { get; set; }
        public float GetDamage();
        public void ApplyDamage(IDamageable damageable);
    }
}