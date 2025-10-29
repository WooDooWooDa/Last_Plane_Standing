using UnityEngine;

namespace Damage
{
    public interface IDamageable
    {
        /// <summary>
        /// Cause the Damageable to take damage from a source.
        /// </summary>
        /// <param name="amount">The amount of damage caused.</param>
        /// <param name="source">"The WHO", The source responsible for the damage (Ex: an enemy or the player).</param>
        /// <param name="causer">"The WHAT", The physical object that directly caused the damage (Ex: a projectile).
        /// Causer may be the same as the source in case of contact damage.</param>
        /// <param name="damageType">The type of damage received.</param>
        public void TakeDamage(float amount, IDamageSource source = null, 
            IDamageSource causer = null);
        HealthComponent HealthComponent { get; set; }
        Collider2D HurtBox { get; set; }
    }
}