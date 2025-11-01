using System;
using UnityEngine;
using ValueSystem;

namespace Plane
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlaneMovement : MonoBehaviour
    {
        public float CurrentSpeed => _currentSpeed;
        public float SpeedPercentage => _currentSpeedPercentage;

        [SerializeField] private RangeSharedValueWithFallback _speedRange;
        [SerializeField] private RangeSharedValueWithFallback _steerRange;
        
        private Rigidbody2D _rb;
        
        private float _currentSpeed;
        private float _currentSpeedPercentage;
        private Vector3 _targetPos;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _currentSpeed = _speedRange.Get().x;
        }

        public void SetTarget(Vector3 targetPosition)
        {
            _targetPos =  targetPosition;
        }
        
        public void SetSpeedPercentage(float speedPercentage)
        {
            var speedRange = _speedRange.Get();
            _currentSpeed = Mathf.Lerp(speedRange.x, speedRange.y, speedPercentage);
            _currentSpeedPercentage = speedPercentage;
        }

        private void FixedUpdate()
        {
            var directionToTarget = (_targetPos - transform.position).normalized;
            var rotationSteer = Vector3.Cross(transform.up, directionToTarget).z;

            var steerRange = _steerRange.Get();
            _rb.angularVelocity = rotationSteer * Mathf.Lerp(steerRange.y, steerRange.x, SpeedPercentage) * 10f;
            _rb.linearVelocity = transform.up * (_currentSpeed / 10);
        }
    }
}
       