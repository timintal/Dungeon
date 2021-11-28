using System;
using UnityEngine;

namespace Game.Src.ECS.Components.Physics
{
    [Serializable]
    public struct ColliderComponent
    {
        public Collider Collider;
    }
}