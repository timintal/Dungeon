using System;
using UnityEngine;

namespace Game.Src.ECS.Components.Player
{
    [Serializable]
    public struct CharacterColliderComponent
    {
        public CapsuleCollider Collider;

        public Vector3 sphere1Offset;
        public Vector3 sphere2Offset;
        public float Radius;
    }
}