using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public static ParallaxController Instance;
    
    [SerializeField] private List<Transform> _parallaxedTransforms;
    [SerializeField] private float _parallaxSpeed;
    [SerializeField] private CameraController _cameraController;

    private Vector3 _lastCameraPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _lastCameraPosition = _cameraController.GetCameraPosition();
    }

    private void LateUpdate()
    {
        var cameraPosition = _cameraController.GetCameraPosition();
        var delta = cameraPosition - _lastCameraPosition;

        foreach (var transform in _parallaxedTransforms)
        {
            transform.position += (delta * (_parallaxSpeed * transform.position.z));
        }

        _lastCameraPosition = cameraPosition;
    }

    public void AddToParallax(Transform newTransform)
    {
        _parallaxedTransforms.Add(newTransform);
    }

    public void RemoveFromParallax(Transform transformToRemove)
    {
        _parallaxedTransforms.Remove(transformToRemove);
    }
}