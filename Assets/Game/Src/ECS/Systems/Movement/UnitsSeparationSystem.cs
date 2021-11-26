using Game.Src.ECS.Components.Bots;
using Game.Src.ECS.Components.Physics;
using Game.Src.ECS.Components.Player;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Movement
{
    public class UnitsSeparationSystem : IEcsRunSystem, IEcsInitSystem
    {
        private const float MAX_SEPARATION_DIST = 0.05f;
        
        [EcsFilter(typeof(RigidbodyComponent), typeof(CharacterColliderComponent))]
        private EcsFilter _botsFilter;

        [EcsPool] private EcsPool<RigidbodyComponent> _rigidbodyPool;
        [EcsPool] private EcsPool<CharacterColliderComponent> _characterColliderPool;

        private Collider[] _collidersCache = new Collider[10];
        private int _mask;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _botsFilter)
            {
                ref var rigidbody = ref _rigidbodyPool.Get(entity);
                var character = _characterColliderPool.Get(entity);

                Vector3 position = rigidbody.CalculatedPosition;

                int count = Physics.OverlapCapsuleNonAlloc(
                    position + character.sphere1Offset, 
                    position + character.sphere2Offset,
                    character.Radius, 
                    _collidersCache,
                    _mask);
                
                Vector3 offset = Vector3.zero;
                bool updated = false;

                for (int i = 0; i < count && i < _collidersCache.Length; i++)
                {
                    if (ReferenceEquals(_collidersCache[i], character.Collider))
                    {
                        continue; // ignore self
                    }

                    updated = true;
                    
                    Vector3 diff =  position - _collidersCache[i].transform.position;
                    float dist = diff.magnitude;
                    if (dist < float.Epsilon)
                    {
                        dist = 0.001f;
                    }
                    float desiredDist = character.Radius + Mathf.Max(_collidersCache[i].bounds.size.x, _collidersCache[i].bounds.size.z);
                    if (dist < desiredDist)
                    {

                        offset += diff / dist * Mathf.Min(desiredDist - dist, MAX_SEPARATION_DIST);
                    }
                }

                offset.y = 0;
                
                if (updated)
                {
                    rigidbody.CalculatedPosition = (position + offset);
                    rigidbody.isStateDirty = true;
                }
            }
        }

        public void Init(EcsSystems systems)
        {
            _mask = LayerMask.GetMask("Enemy", "Player");
        }
    }
}