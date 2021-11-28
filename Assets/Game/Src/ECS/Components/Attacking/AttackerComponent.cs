using System;
using Game.Src.ECS.Components.Attacking;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Src.ECS.Components.Shooting
{
    [Serializable]
    public struct AttackerComponent
    {
        public TargetType AcceptedTargets;
        public LayerMask TargetPhysicsMask;
        public float DetectionRadius;
        public EcsPackedEntity Target;
        public float CheckForNewTargetCooldown;
    }
    
    public struct TargetCheckTimerFlag{}
}