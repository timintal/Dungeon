using Game.Src.ECS.Components.Movement;
using Game.Src.ECS.Components.Pathfinding;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Src.ECS.Systems.Pathfinding
{
    public struct NavGridCarveSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(GridComponent))] 
        private EcsFilter _gridFilter;

        [EcsFilter(typeof(ObstacleCircleComponent), typeof(TransformComponent))]
        private EcsFilter _obstacleCircleFilter;
        
        [EcsFilter(typeof(ObstacleRectComponent), typeof(TransformComponent))]
        private EcsFilter _obstacleRectFilter;
        

        [EcsPool] private EcsPool<GridComponent> _gridPool;
        [EcsPool] private EcsPool<GridNodeComponent> _gridNodePool;
        [EcsPool] private EcsPool<TransformComponent> _transformPool;

        public void Run(EcsSystems systems)
        {
            foreach (var gridEntity in _gridFilter)
            {
                ref var grid = ref _gridPool.Get(gridEntity);

                foreach (var keyValue in grid.Grid)
                {
                    var gridNode = grid.Grid[keyValue.Key];
                    gridNode.PathCost = 1;
                    grid.Grid[keyValue.Key] = gridNode;
                }
                
                foreach (var obstacleEntity in _obstacleRectFilter)
                {
                    
                }
            }
        }
    }
}
