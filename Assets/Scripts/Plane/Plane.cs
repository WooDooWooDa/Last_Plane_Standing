using System;
using Damage;
using UnityEngine;

namespace Plane
{
    public class Plane : MonoBehaviour, IDamageable, IDamageSource
    {
        [SerializeField] private ETeam _team;
        [SerializeField] protected PlaneMovement _movement;
        [SerializeField] protected Cannon _cannon;
        
        public PlaneMovement planeMovement => _movement;
        public Cannon Cannon => _cannon;
        public HealthComponent HealthComponent { get; set; }
        public Collider2D HurtBox { get; set; }
        public ETeam Team
        {
            get => _team; 
            set => _team = value;
        }

        private void Awake()
        {
            HealthComponent = GetComponent<HealthComponent>();
        }
        
        public void TakeDamage(float amount, IDamageSource source = null, IDamageSource causer = null)
        {
            HealthComponent.HandleDamage(amount, source, causer);
        }
        
        public float GetDamage()
        {
            // noop
            return 0f;
        }

        public void ApplyDamage(IDamageable damageable)
        {
            // noop
        }
    }
}