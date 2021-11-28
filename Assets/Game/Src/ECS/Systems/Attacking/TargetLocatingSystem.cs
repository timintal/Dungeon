using Game.Src.ECS.Components.Attacking;
using Game.Src.ECS.Components.Physics;
using Game.Src.ECS.Components.Shooting;
using Game.Src.ECS.Components.Timers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Attacking
{
    public class TargetLocatingSystem : IEcsRunSystem, IEcsInitSystem
    {
        [EcsWorld] private EcsWorld _world;

        [EcsFilter(typeof(AttackerComponent), typeof(RigidbodyComponent))]
        [EcsFilterExclude(typeof(TimerComponent<TargetCheckTimerFlag>))]
        private EcsFilter _attackersFilter;

        [EcsFilter(typeof(TargetComponent), typeof(RigidbodyComponent))]
        private EcsFilter _targetsFilter;


        [EcsPool] private EcsPool<AttackerComponent> _attackersPool;

        [EcsPool] private EcsPool<RigidbodyComponent> _rigidbodyPool;

        [EcsPool] private EcsPool<TargetComponent> _targetsPool;

        [EcsPool] private EcsPool<TimerComponent<TargetCheckTimerFlag>> _targetCheckTimerPool;

        public void Run(EcsSystems systems)
        {
            foreach (var attackerEntity in _attackersFilter)
            {
                ref var attacker = ref _attackersPool.Get(attackerEntity);
                var attackerRigidbody = _rigidbodyPool.Get(attackerEntity);

                FindNewTarget(ref attacker, attackerRigidbody, attackerEntity);
            }
        }

        
        private int _enviromnemtMask;

        private void FindNewTarget(ref AttackerComponent attacker, RigidbodyComponent rigidbody, int attackerEntity)
        {
            float currDist = float.MaxValue;
            attacker.Target = new EcsPackedEntity();

            bool found = false;
            
            foreach (var enemyEntity in _targetsFilter)
            {
                var target = _targetsPool.Get(enemyEntity);
                if (((int)attacker.AcceptedTargets & (int)target.Type) > 0)
                {
                    var targetRigidbody = _rigidbodyPool.Get(enemyEntity);
                    Vector3 diff = targetRigidbody.CalculatedPosition - rigidbody.CalculatedPosition;

                    float dist = diff.magnitude;

                    if (dist < currDist)
                    {
                        Ray ray = new Ray(rigidbody.CalculatedPosition + Vector3.up, diff);

                        if (!Physics.Raycast(ray, dist, _enviromnemtMask))
                        {
                            attacker.Target = _world.PackEntity(enemyEntity);
                            currDist = dist;
                            found = true;
                        }
                    }
                }
            }

            if (found)
            {
                ref var timer = ref _targetCheckTimerPool.Add(attackerEntity);
                timer.TimeLeft = Random.Range(attacker.CurrentTargetHoldTime * 0.6f, attacker.CurrentTargetHoldTime * 1.4f);
            }
        }

        public void Init(EcsSystems systems)
        {
            _enviromnemtMask = LayerMask.GetMask("Environment");
        }
    }
}
