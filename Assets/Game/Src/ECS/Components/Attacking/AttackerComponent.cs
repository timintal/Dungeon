using System;
using Leopotam.EcsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Src.ECS.Components.Shooting
{
    [Serializable]
    public struct AttackerComponent
    {
        public TargetType AcceptedTargets;
        public LayerMask TargetPhysicsMask;
        public EcsPackedEntity Target;
        public float DetectionRadius;
        public Vector3 lastVisibleTargetPos;
    }
}