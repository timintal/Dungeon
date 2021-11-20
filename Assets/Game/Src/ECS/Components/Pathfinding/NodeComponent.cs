using Unity.Mathematics;
using UnityEngine;

namespace Game.Src.ECS.Components.Pathfinding
{
    public struct GridNodeComponent
    {
        public int2 Index;
        public Vector3 WorldPosition;
        public float NodeSize;
        public Matrix4x4 WorldMatrix;
    }
}