using System;
using UnityEngine;

namespace Game.Src.ECS.Components.Shooting
{
    [Flags]
    public enum TargetType
    {
        Player = 1 << 0,
        Bot = 1 << 1
    }
    
    [Serializable]
    public struct TargetComponent
    {
        public TargetType Type;
        public Collider Collider;
    }
}