using System.Collections;
using Nexus.Core.ServiceLocation;
using Nexus.Pooling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Galacron.Actors
{
    public class Weapon : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private PoolReference<BulletBase> _bulletPool;
        [SerializeField] private float _weaponDelay = 0.1f;
        [SerializeField] private float _bulletSpeed = 15f;
        [SerializeField] private bool _automaticFire = true;
        [SerializeField] private bool _stopDelayOnRelease = true;


        [Header("Events")] 
        [SerializeField] private UnityEvent onShoot = null!;

        
        private bool _isShooting;
        private bool _isReloading;
        private string _bulletPoolId;
        public bool IsShooting => _isShooting;

        

        private void Update()
        {
            if (!_isShooting || _isReloading) return;

            onShoot?.Invoke();

            ShootBullet();
            FinishShooting();
        }

        public void Shoot()
        {
            _isShooting = true;
        }

        public void StopShooting()
        {
            _isShooting = false;
            
            if (!_stopDelayOnRelease) return;
            
            _isReloading = false;
            StopAllCoroutines();
        }

        private void ShootBullet()
        {
            var rotation = transform.rotation;
            
            var bulletGO = _bulletPool.Get(transform.position, rotation);
            if (bulletGO == null) return;
            var bullet = bulletGO.GetComponent<BulletBase>();
            bullet.Velocity = transform.up * _bulletSpeed;
        }

        private void FinishShooting()
        {
            StartCoroutine(DelayNextShootCoroutine());
            if (!_automaticFire)
            {
                _isShooting = false;
            }
        }

        private IEnumerator DelayNextShootCoroutine()
        {
            _isReloading = true;
            yield return new WaitForSeconds(_weaponDelay);
            _isReloading = false;
        }
    }
}