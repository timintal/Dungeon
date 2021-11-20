using System;
using UnityEngine;

namespace Game.Src.ECS.Components.Camera
{
    [Serializable]
    public struct CameraFollowComponent
    {
        public Transform CameraTransform;
        public Vector3 Offset;
        public float followDamping;
    }
}