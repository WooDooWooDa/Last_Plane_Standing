using System;
using BlackCatPool;
using Projectiles;
using Projectiles.ProjectileData;
using UnityEngine;

namespace Plane
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField] private Bullet _baseBulletProjectile;
        [SerializeField] private BulletDataSO _bulletData;
        [SerializeField] private Transform _fireTransform;
        [SerializeField] private float _fireRate;

        private ObjectPool _bulletPool;
        private BulletDataSO _currentBullet;
        private Plane _plane;
        
        private void Start()
        {
            _plane = GetComponentInParent<Plane>();
            _bulletPool = ObjectPoolManager.Instance.GetPool(_baseBulletProjectile.gameObject);
            _currentBullet = _bulletData;
        }

        public void TryShoot()
        {
            if (_bulletPool == null)
            {
                _bulletPool = ObjectPoolManager.Instance.GetPool(_baseBulletProjectile.gameObject);
            }
            var bullet = _bulletPool.Get<Bullet>();
            bullet.transform.position = _fireTransform.position;
            bullet.transform.rotation = _fireTransform.rotation;
            bullet.SetBulletData(_currentBullet);
            bullet.SetOwner(_plane);
            bullet.gameObject.SetActive(true);
        }
    }
}