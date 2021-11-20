using Game.Src.ECS.Components.Movement;
using Game.Src.ECS.Components.Physics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Src.ECS.Systems.Movement
{
    public class RigidbodyMovementSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(RigidbodyComponent), typeof(SpeedComponent))]
        private EcsFilter _movableRigidbodyFilter;

        [EcsPool] private EcsPool<RigidbodyComponent> _rigidbodyPool;
        [EcsPool] private EcsPool<SpeedComponent> _speedPool;

        public void Run(EcsSystems systems)
        {
            
            
        }
    }
}