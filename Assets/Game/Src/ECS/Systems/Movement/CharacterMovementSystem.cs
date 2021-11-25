using Game.Src.ECS.Components.Movement;
using Game.Src.ECS.Components.Physics;
using Game.Src.ECS.Components.Player;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Movement
{
    public class CharacterMovementSystem : IEcsRunSystem, IEcsInitSystem
    {
        const float COLLISION_OFFSET = 0.02f;
        
        [EcsFilter(typeof(RigidbodyComponent), typeof(SpeedComponent), typeof(CharacterColliderComponent))]
        private EcsFilter _playerFilter;

        [EcsPool] private EcsPool<RigidbodyComponent> _rigidbodyPool;
        [EcsPool] private EcsPool<SpeedComponent> _speedPool;
        [EcsPool] private EcsPool<CharacterColliderComponent> _colliderPool;
        private int _mask;

        private readonly RaycastHit[] _cachedHits = new RaycastHit[100];
        public void Run(EcsSystems systems)
        {
            foreach (var playerEntity in _playerFilter)
            {
                var speed = _speedPool.Get(playerEntity);
                ref var rigidbody = ref _rigidbodyPool.Get(playerEntity);

                if (speed.Magnitude < float.Epsilon)
                {
                    continue;
                }
                
                var collider = _colliderPool.Get(playerEntity);

                Vector3 currentPos = rigidbody.CalculatedPosition;

                Vector3 diff = speed.Speed * Time.fixedDeltaTime;
                Vector3 desiredPosition = currentPos + diff;
                float speedMagnitude = speed.Magnitude * Time.fixedDeltaTime;

                int count = Physics.CapsuleCastNonAlloc(
                    currentPos + collider.sphere1Offset,
                    currentPos + collider.sphere2Offset, 
                    collider.Radius, 
                    diff / speedMagnitude,
                    _cachedHits, 
                    speedMagnitude,
                    _mask);

                for (int i = 0; i < count; i++)
                {
                    if (Physics.ComputePenetration(
                        collider.Collider, 
                        desiredPosition, 
                        rigidbody.CalculatedRotation, 
                        _cachedHits[i].collider, 
                        _cachedHits[i].collider.transform.position,
                        _cachedHits[i].collider.transform.rotation,
                        out Vector3 direction, out float distance))
                    {
                        desiredPosition += direction * distance;
                    }
                    else
                    {
                        desiredPosition += _cachedHits[i].normal * COLLISION_OFFSET;
                    } 
                }

                if (speedMagnitude > float.Epsilon || count > 0)
                {
                    rigidbody.CalculatedPosition = desiredPosition;
                    rigidbody.isStateDirty = true;
                }

            }
        }

        public void Init(EcsSystems systems)
        {
            _mask = LayerMask.GetMask("Environment");
        }
    }
}