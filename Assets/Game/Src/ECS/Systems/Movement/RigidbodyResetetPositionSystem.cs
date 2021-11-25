using Game.Src.ECS.Components.Physics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Src.ECS.Systems.Movement
{
    public class RigidbodyResetTransformSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(RigidbodyComponent))]
        private EcsFilter _rigidbodyFilter;

        [EcsPool] private EcsPool<RigidbodyComponent> _rigidbodyPool;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _rigidbodyFilter)
            {
                ref var rigidbody = ref _rigidbodyPool.Get(entity);
                rigidbody.CalculatedPosition = rigidbody.Rigidbody.position;
                rigidbody.CalculatedRotation = rigidbody.Rigidbody.rotation;
                rigidbody.isStateDirty = false;
            }
        }
    }
}