using System;
using Leopotam.EcsLite;
using Sirenix.OdinInspector;

namespace Game.Src.ECS.Components.Shooting
{
    [Serializable]
    public struct AttackerComponent
    {
        public TargetType AcceptedTargets;
        public EcsPackedEntity Target;
        public float cooldown;
    }
}