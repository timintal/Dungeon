using System.Collections.Generic;
using Astar;
using Game.Src.ECS.Components.Pathfinding;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Pathfinding
{
    public class NavGridVisualizeSystem : IEcsRunSystem, IEcsInitSystem
    {
        [EcsFilter(typeof(GridComponent), typeof(GridNodeVisualizeComponent))]
        private EcsFilter _gridFilter;

        [EcsFilter(typeof(GridNodeComponent))] private EcsFilter _nodeFilter;

        [EcsPool] private EcsPool<GridNodeComponent> _gridNodePool;
        private EcsPool<GridNodeVisualizeComponent> _gridNodeVisualizePool;

        [EcsPool] private EcsPool<GridComponent> _gridPool;

        private List<Matrix4x4> _matricesToDrawNormal;
        private List<Matrix4x4> _matricesToDrawObstacles;

        public void Run(EcsSystems systems)
        {
            _matricesToDrawNormal.Clear();
            _matricesToDrawObstacles.Clear();

            foreach (var gridEntity in _gridFilter)
            {
                var grid = _gridPool.Get(gridEntity);
                if (!grid.showDebugInfo)
                {
                    break;
                }
                var nodeVisualize = _gridNodeVisualizePool.Get(gridEntity);

                foreach (var nodeEntity in _nodeFilter)
                {
                    var node = _gridNodePool.Get(nodeEntity);

                    if (grid.Grid.TryGetValue(node.Index, out GridNode gridNode))
                    {
                        if (gridNode.IsWalkable)
                        {
                            _matricesToDrawNormal.Add(node.WorldMatrix);
                        }
                        else
                        {
                            _matricesToDrawObstacles.Add(node.WorldMatrix);
                        }
                    }

                    if (_matricesToDrawNormal.Count == 1023)
                    {
                        Graphics.DrawMeshInstanced(nodeVisualize.VisualizeMesh, 0, nodeVisualize.WalkableMaterial,
                            _matricesToDrawNormal);
                        _matricesToDrawNormal.Clear();
                    }

                    if (_matricesToDrawObstacles.Count == 1023)
                    {
                        Graphics.DrawMeshInstanced(nodeVisualize.VisualizeMesh, 0, nodeVisualize.ObstacleMaterial,
                            _matricesToDrawObstacles);
                        _matricesToDrawObstacles.Clear();
                    }
                }

                if (_matricesToDrawNormal.Count > 0)
                {
                    Graphics.DrawMeshInstanced(nodeVisualize.VisualizeMesh, 0, nodeVisualize.WalkableMaterial,
                        _matricesToDrawNormal);
                }

                if (_matricesToDrawObstacles.Count > 0)
                {
                    Graphics.DrawMeshInstanced(nodeVisualize.VisualizeMesh, 0, nodeVisualize.ObstacleMaterial,
                        _matricesToDrawObstacles);
                }
            }
        }

        public void Init(EcsSystems systems)
        {
            _matricesToDrawNormal = new List<Matrix4x4>(20000);
            _matricesToDrawObstacles = new List<Matrix4x4>(20000);
        }
    }
}