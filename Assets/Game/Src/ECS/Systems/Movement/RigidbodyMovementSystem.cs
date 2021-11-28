using Game.Src.ECS.Components.Movement;
using Game.Src.ECS.Components.Physics;
using Game.Src.ECS.Components.Player;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Movement
{
    public class RigidbodyMovementSystem : IEcsRunSystem, IEcsInitSystem
    {
        [EcsFilter(typeof(RigidbodyComponent), typeof(SpeedComponent))]
        [EcsFilterExclude(typeof(PlayerComponent))]
        private EcsFilter _movableRigidbodyFilter;

        [EcsPool] private EcsPool<RigidbodyComponent> _rigidbodyPool;
        [EcsPool] private EcsPool<SpeedComponent> _speedPool;
        private int _mask;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _movableRigidbodyFilter)
            {
                var rigidbody = _rigidbodyPool.Get(entity);
                var speed = _speedPool.Get(entity);

                if (Mathf.Abs(speed.Speed.x) > float.Epsilon ||
                    Mathf.Abs(speed.Speed.z) > float.Epsilon)
                {
                    Vector3 dist = speed.Speed * Time.fixedDeltaTime;
                    float magnitude = speed.Magnitude * Time.fixedDeltaTime;

                    Ray hitRay = new Ray(rigidbody.Rigidbody.position, dist / magnitude);
                    if (!Physics.SphereCast(hitRay, 0.5f, magnitude, _mask))
                    {
                        rigidbody.Rigidbody.MovePosition(rigidbody.Rigidbody.position + dist);
                    }
                }
            }
            
        }

        public void Init(EcsSystems systems)
        {
            _mask = LayerMask.GetMask("Environment", "Player", "Enemy");
        }
    }
}