using System;

namespace Game.Src.ECS.Components.Movement
{
    [Serializable]
    public struct AccelerationComponent
    {
        public float MaxSpeed;
        public float Acceleration;
        public float NoInputDeacceleration;
        public float RotationSpeed;
    }
}