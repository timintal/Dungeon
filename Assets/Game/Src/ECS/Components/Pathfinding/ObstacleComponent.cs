using System;
using UnityEngine;

namespace Game.Src.ECS.Components.Pathfinding
{
    [Serializable]
    public struct ObstacleRectComponent
    {
        public Vector3 size;
        public Vector3 offset;
    }

    [Serializable]
    public struct ObstacleCircleComponent
    {
        public float radius;
        public Vector3 offset;
    }
}
