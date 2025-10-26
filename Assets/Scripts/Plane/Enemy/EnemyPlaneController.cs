using System;
using UnityEngine;

namespace Plane.Enemy
{
    public class EnemyPlaneController : MonoBehaviour
    {
        [SerializeField] private Plane _plane;

        private PlayerPlane _playerPlane;
        
        private void Start()
        {
            _playerPlane = FindFirstObjectByType<PlayerPlane>();
        }

        private void Update()
        {
            UpdatePlaneTarget();
            
            _plane.planeMovement.SetSpeedPercentage(1f);
        }
        
        private void UpdatePlaneTarget()
        {
            _plane.planeMovement.SetTarget(_playerPlane.transform.position);
        }
    }
}