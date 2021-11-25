using Game.Src.ECS.Components.Physics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Src.ECS.Systems.Movement
{
    public class RigidbodySetTransformSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(RigidbodyComponent))]
        private EcsFilter _rigidbodyFilter;

        [EcsPool] private EcsPool<RigidbodyComponent> _rigidbodyPool;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _rigidbodyFilter)
            {
                var rigidbody = _rigidbodyPool.Get(entity);
                if (rigidbody.isStateDirty)
                {
                    rigidbody.Rigidbody.MovePosition(rigidbody.CalculatedPosition);
                    rigidbody.Rigidbody.MoveRotation(rigidbody.CalculatedRotation);
                }
            }
        }
    }
}