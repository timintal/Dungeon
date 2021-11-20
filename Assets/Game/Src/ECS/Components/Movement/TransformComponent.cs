using System;
using UnityEngine;

namespace Game.Src.ECS.Components.Movement {
    
    [Serializable]
    public struct TransformComponent
    {
        public Transform Transform;

        public static Vector3 DiffVector(TransformComponent t1, TransformComponent t2)
        {
            return t1.Transform.position - t2.Transform.position;
        }
    }
    
}