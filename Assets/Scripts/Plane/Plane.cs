using System;
using UnityEngine;

namespace Plane
{
    public class Plane : MonoBehaviour
    {
        public PlaneMovement planeMovement => _movement;
        public Cannon Cannon => _cannon;
        
        [SerializeField] protected PlaneMovement _movement;
        [SerializeField] protected Cannon _cannon;
    }
}