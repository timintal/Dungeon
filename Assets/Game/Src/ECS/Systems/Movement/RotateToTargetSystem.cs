using Game.Src.ECS.Components.Movement;
using Game.Src.ECS.Components.Physics;
using Game.Src.ECS.Components.Shooting;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Movement
{
    public class RotateToTargetSystem : IEcsRunSystem
    {
        [EcsWorld]
        private EcsWorld _world;
        
        [EcsFilter(typeof(AttackerComponent), typeof(RigidbodyComponent), typeof(AccelerationComponent))]
        private EcsFilter _attackerFilter;

        [EcsPool] private EcsPool<AttackerComponent> _attackerPool;
        [EcsPool] private EcsPool<RigidbodyComponent> _rigidbodyPool;
        [EcsPool] private EcsPool<AccelerationComponent> _accelerationPool;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _attackerFilter)
            {
                var attacker = _attackerPool.Get(entity);

                if (attacker.Target.Unpack(_world, out int targetEntity))
                {
                    var targetRigidbody = _rigidbodyPool.Get(targetEntity);
                    ref var myRigidbody = ref _rigidbodyPool.Get(entity);
                    var acceleration = _accelerationPool.Get(entity);

                    Quaternion desiredRotation = myRigidbody.CalculatedRotation;
                    desiredRotation.SetLookRotation(targetRigidbody.CalculatedPosition - myRigidbody.CalculatedPosition);
                    
                    myRigidbody.CalculatedRotation = Quaternion.RotateTowards(myRigidbody.CalculatedRotation, desiredRotation,
                        acceleration.RotationSpeed * Time.fixedDeltaTime);
                } 
            }
        }
    }
}