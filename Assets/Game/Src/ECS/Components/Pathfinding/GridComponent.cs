using System;
using Astar;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Src.ECS.Components.Pathfinding
{
    [Serializable]
    public struct GridComponent
    {
        public Vector3 GridStartPoint;
        public Vector3 GridSize;
        
        public NativeHashMap<int2, GridNode> Grid;

        public int XMax;
        public int YMax;

        public float NodeSize;
        #if UNITY_EDITOR
        public bool showDebugInfo;
        #endif
    }
}
