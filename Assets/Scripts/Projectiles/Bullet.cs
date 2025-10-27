using System;
using BlackCatPool;
using Projectiles.ProjectileData;
using UnityEngine;

namespace Projectiles
{
    public class Bullet : MonoBehaviour, IPoolable
    {
        public BulletDataSO BulletData => _bulletData ?? baseBulletData;
        
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private BulletDataSO baseBulletData;
        
        private BulletDataSO _bulletData;
        private float _flightTimeLeft;

        private void Awake()
        {
            _bulletData = baseBulletData;
        }

        public void OnCreated() { }

        public void OnObtained() { }

        public void OnPooled()
        {
            _rb.linearVelocity = Vector2.zero;
        }

        public void OnDestroyed() { }

        public void SetBulletData(BulletDataSO bulletData)
        {
            _bulletData = bulletData;
            _flightTimeLeft = _bulletData.ttl;
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
            Debug.Log(_bulletData.speed.Get());
            _rb.linearVelocity = transform.up * (_bulletData.speed.Get() / 10);
        }
    }
}