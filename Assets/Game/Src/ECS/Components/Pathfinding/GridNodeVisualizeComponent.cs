using System;
using UnityEngine;

namespace Game.Src.ECS.Components.Pathfinding
{
    [Serializable]
    public struct GridNodeVisualizeComponent
    {
        public Material WalkableMaterial;
        public Material ObstacleMaterial;

        public Mesh VisualizeMesh;
    }
}