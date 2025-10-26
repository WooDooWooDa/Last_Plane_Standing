using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Plane
{
    public class PlayerPlaneController : MonoBehaviour
    {
        [SerializeField] private Plane _playerPlane;
        [SerializeField] private float _minSpeedRange = 0.5f;
        [SerializeField] private float _maxSpeedRange;
        [SerializeField] private Transform _followCameraAnchor;
        [SerializeField] private bool _controllerActive = true;
        [SerializeField] private bool _debug = true;
        
        private void Update()
        {
            if (_controllerActive)
            {
                UpdatePlaneTarget();
                UpdatePlaneSpeed();
            }
        }

        private void FixedUpdate()
        {
            _followCameraAnchor.localPosition = Vector3.Lerp(Vector3.up * _minSpeedRange, Vector3.up * _maxSpeedRange,
                _playerPlane.planeMovement.SpeedPercentage);
        }

        private void UpdatePlaneTarget()
        {
            var mousePos = GetClampedMousePosInsideSpeedRange();

            _playerPlane.planeMovement.SetTarget(mousePos);

            if (_debug)
            {
                //Draw target line
                Debug.DrawLine(_playerPlane.transform.position, mousePos, Color.green);
                //Draw actual mouse pos
                var actualMousePos = GetActualMousePosition();
                actualMousePos.z = 0;
                Debug.DrawLine(_playerPlane.transform.position, actualMousePos, Color.yellow);
            }
        }

        private void UpdatePlaneSpeed()
        {
            var mousePos = GetClampedMousePosInsideSpeedRange();
            var distance = (mousePos - _playerPlane.transform.position).magnitude;

            // Compute speed percentage relative to outer circle
            var speedPercentage = Mathf.Clamp((distance - _minSpeedRange) / (_maxSpeedRange - _minSpeedRange), 0f, 1f);

            _playerPlane.planeMovement.SetSpeedPercentage(speedPercentage);
            
            if (_debug)
            {
                //Draw speed line
                Debug.DrawLine(_playerPlane.transform.position, _followCameraAnchor.position, Color.red);
            }
        }

        private Vector3 GetClampedMousePosInsideSpeedRange()
        {
            var mousePos = GetActualMousePosition();
            mousePos.z = 0;
            
            var planePos = _playerPlane.transform.position;

            var dir = mousePos - planePos;
            var distance = dir.magnitude;

            if (distance > 0f)
            {
                var clampedDistance = Mathf.Clamp(distance, _minSpeedRange, _maxSpeedRange);
                dir.Normalize();

                mousePos = planePos + dir * clampedDistance;
            }
            return mousePos;
        }

        public Vector3 GetActualMousePosition()
        {
            var plane = new UnityEngine.Plane(Vector3.back, Vector3.zero);
            //var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var mousePos = Vector3.zero;
            if (plane.Raycast(ray, out var enter)) {
                mousePos = ray.GetPoint(enter);
            }
            return mousePos;
        }

        private void OnDrawGizmos()
        {
            if (_debug)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_playerPlane.transform.position, _minSpeedRange);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_playerPlane.transform.position, _maxSpeedRange);
            }
        }
    }
}