using Game.Src.ECS.Components.Physics;
using Game.Src.ECS.Components.Shooting;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Movement
{
    public class RotateToTargetSystem : IEcsRunSystem
    {
        private EcsWorld _world;
        
        [EcsFilter(typeof(RigidbodyComponent), typeof(AttackerComponent))]
        private EcsFilter _attackerFilter;

        [EcsPool] private EcsPool<AttackerComponent> _attackerPool;
        [EcsPool] private EcsPool<RigidbodyComponent> _rigidbodyPool;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _attackerFilter)
            {
                var attacker = _attackerPool.Get(entity);

                if (attacker.Target.Unpack(_world, out int targetEntity))
                {
                    var targetRigidbody = _rigidbodyPool.Get(targetEntity);
                    ref var myRigidbody = ref _rigidbodyPool.Get(entity); 

                    Quaternion desiredRotation = myRigidbody.CalculatedRotation;
                    desiredRotation.SetLookRotation(targetRigidbody.CalculatedPosition - myRigidbody.CalculatedPosition);
                    
                    myRigidbody.CalculatedRotation = Quaternion.Lerp(myRigidbody.CalculatedRotation, desiredRotation, 5 * Time.fixedDeltaTime);
                } 
            }
        }
    }
}