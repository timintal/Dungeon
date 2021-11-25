using System;
using Game.Src.ECS.Components.Shooting;
using UnityEngine;

namespace Game.Src.ECS.Components.Attacking
{
    [Serializable]
    public struct ProjectileComponent
    {
        public TargetType Targets;
        public LayerMask PhysicsLayers;
        public float Lifetime;
        public float Speed;
        public Matrix4x4 TransformationMatrix;
        public Mesh Mesh;
        public Material Material;
    }

    public struct Damagable
    {
        public int Damage;
    }
}