
using Game.Src.ECS.Components.Movement;
using Game.Src.ECS.Components.Physics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Movement {
    sealed class NavMeshMovementSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(SpeedComponent), typeof(NavMeshAgentComponent))]
        private readonly EcsFilter _movableFilter;

        [EcsPool]
        private readonly EcsPool<SpeedComponent> _speedPool;
        
        [EcsPool]
        private readonly EcsPool<NavMeshAgentComponent> _transformPool;
        
        public void Run (EcsSystems systems) 
        {
            foreach (var entity in _movableFilter)
            {
                NavMeshAgentComponent agent = _transformPool.Get(entity);
                SpeedComponent speed = _speedPool.Get(entity);

                agent.Agent.nextPosition = agent.Agent.transform.position + speed.Speed * Time.fixedDeltaTime;
            }
        }
    }
}