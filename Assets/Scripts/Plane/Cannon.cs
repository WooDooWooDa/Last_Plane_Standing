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
        [SerializeField] private BulletDataSO _baseBulletData;
        [SerializeField] private Transform _fireTransform;
        [SerializeField] private float _fireRate;

        private ObjectPool _bulletPool;
        private BulletDataSO _currentBullet;
        
        private void Start()
        {
            _bulletPool = ObjectPoolManager.Instance.GetPool(_baseBulletProjectile.gameObject);
            _currentBullet = _baseBulletProjectile.BulletData;
        }

        public void TryShoot()
        {
            var bullet = _bulletPool.Get<Bullet>();
            bullet.transform.position = _fireTransform.position;
            bullet.transform.rotation = _fireTransform.rotation;
            bullet.SetBulletData(_currentBullet);
            bullet.gameObject.SetActive(true);
        }
    }
}