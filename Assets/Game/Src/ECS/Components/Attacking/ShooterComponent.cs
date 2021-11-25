using System;
using Game.Src.ECS.Components.Shooting;
using UnityEngine;

namespace Game.Src.ECS.Components.Attacking
{
    [Serializable]
    public struct ShooterComponent
    {
        public float MaxAmmo;
        public float CurrentAmmo;
        public float Cooldown;
        public float ProjectileSpeed;
        public float Lifetime;
        public Mesh ProjectileMesh;
        public Material ProjectileMaterial;
        public Transform SpawnTransform;
    }

    public struct ShootCooldownTimerFlag{}
}