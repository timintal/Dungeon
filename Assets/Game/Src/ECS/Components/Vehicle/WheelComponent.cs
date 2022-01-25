using System;
using UnityEngine;

namespace Game.Src.ECS.Components.Vehicle
{
    [Serializable]
    public struct WheelComponent
    {
        public Rigidbody ConnectedRigidbody;
        public Transform Transform;
        public float SuspensionLength;
        public float SuspensionDamping;
        public float FallSlowdownDistance;
        public AnimationCurve SuspensionRepulsionCurve;
        public float SuspensionRepulsionAmplitude;
        public float CurrentSuspensionCompression;
    }

    public struct WheelContact
    {
        public float Compression;
    }
}