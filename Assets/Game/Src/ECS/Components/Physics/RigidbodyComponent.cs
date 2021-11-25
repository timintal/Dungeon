using System;
using UnityEngine;

namespace Game.Src.ECS.Components.Physics
{
    [Serializable]
    public struct RigidbodyComponent
    {
        public Rigidbody Rigidbody;
        public Vector3 CalculatedPosition;
        public Quaternion CalculatedRotation;
        public bool isStateDirty;
    }
}