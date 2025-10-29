using System;
using BlackCatPool;
using Damage;
using Plane;
using Projectiles.ProjectileData;
using UnityEngine;

namespace Projectiles
{
    //todo : extend from a projectile class common to missile
    public class Bullet : MonoBehaviour, IPoolable, IDamageSource
    {
        [SerializeField] private Rigidbody2D _rb;
        
        private BulletDataSO _bulletData;
        private float _currentSpeed;
        private float _flightTimeLeft;
        
        private IDamageSource _damageSource;
        public ETeam Team { get; set; }

        public void OnCreated() { }
        public void OnObtained() { }
        public void OnPooled()
        {
            _rb.linearVelocity = Vector2.zero;
        }
        public void OnDestroyed() { }
    
        public void SetOwner(IDamageSource owner)
        {
            _damageSource = owner;
            Team = owner.Team;
        }
        
        public void SetBulletData(BulletDataSO bulletData)
        {
            _bulletData = bulletData;
            _flightTimeLeft = _bulletData.timeToLive;
            _currentSpeed = _bulletData.speed.Get();
        }

        private void Update()
        {
            _flightTimeLeft -= Time.deltaTime;
            if (_flightTimeLeft <= 0)
            {
                gameObject.ReturnToPool();
            }
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = transform.up * (_currentSpeed / 10);
        }

        public float GetDamage()
        {
            return _bulletData.damage.Get();
        }

        public void ApplyDamage(IDamageable damageable)
        {
            damageable.TakeDamage(GetDamage(), _damageSource, this);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            //Touched source?
            if (other.gameObject.TryGetComponent<IDamageSource>(out var source) && source == _damageSource) return;
        
            if (other.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                ApplyDamage(damageable);
                
                //todo : play hit effect
                gameObject.ReturnToPool();
            }
            else
            {
                gameObject.ReturnToPool();
            }
        }
    }
}