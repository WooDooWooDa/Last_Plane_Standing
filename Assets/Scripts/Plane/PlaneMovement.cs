
using System;
using UnityEngine;

namespace Plane
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlaneMovement : MonoBehaviour
    {
        public float CurrentSpeed => _currentSpeed;
        public float SpeedPercentage => _currentSpeedPercentage;  
        
        [SerializeField] private float _minSpeed;
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _minSteer;
        [SerializeField] private float _maxSteer;
        [SerializeField] private float tiltAmount = 60f;
        [SerializeField] private float tiltSmooth = 5f;
        
        private Rigidbody2D _rb;
        
        private float _currentSpeed;
        private float _currentSpeedPercentage;
        private float _currentTilt;
        private Vector3 _targetPos;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _currentSpeed = _minSpeed;
        }

        public void SetTarget(Vector3 targetPosition)
        {
            _targetPos =  targetPosition;
        }
        
        public void SetSpeedPercentage(float speedPercentage)
        {
            _currentSpeed = Mathf.Lerp(_minSpeed, _maxSpeed, speedPercentage);
        }

        private void Update()
        {
            _currentSpeedPercentage = (_currentSpeed - _minSpeed) / (_maxSpeed - _minSpeed);
        }

        private void FixedUpdate()
        {
            var directionToTarget = (_targetPos - transform.position).normalized;
            var rotationSteer = Vector3.Cross(transform.up, directionToTarget).z;

            _rb.angularVelocity = rotationSteer * Mathf.Lerp(_maxSteer, _minSteer, _currentSpeedPercentage) * 10f;
            _rb.linearVelocity = transform.up * (_currentSpeed / 10);
            
            //todo : move out Visual tilt of plane 
            var targetTilt = -rotationSteer * tiltAmount;
            _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, Time.deltaTime * tiltSmooth);

            transform.GetChild(0).localRotation = Quaternion.Euler(0f, _currentTilt, 0f);
        }
    }
}
       