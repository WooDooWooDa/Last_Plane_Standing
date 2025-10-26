using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _minFOV;
    [SerializeField] private float _maxFOV;
    [SerializeField] private Plane.Plane _playerPlane;
    [SerializeField] private Transform _targetPointToFollow;
    [SerializeField] private float _cameraFollowSpeed = 1f;

    private void FixedUpdate()
    {
        var nextCamPos = Vector3.MoveTowards(transform.position, _targetPointToFollow.position, _cameraFollowSpeed);
        transform.position = new Vector3(nextCamPos.x, nextCamPos.y, transform.position.z);

        var speed = _playerPlane.planeMovement.SpeedPercentage;
        //_camera.
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var camPos = transform.position;
        camPos.z = 0;
        Gizmos.DrawWireSphere(camPos, 0.05f);
    }
}