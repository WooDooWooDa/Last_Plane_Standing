using System;
using UnityEngine;

namespace Plane
{
    public class Plane : MonoBehaviour
    {
        public PlaneMovement planeMovement => _movement;
        
        [SerializeField] protected PlaneMovement _movement;
    }
}