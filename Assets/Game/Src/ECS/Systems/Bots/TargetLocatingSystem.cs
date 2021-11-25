using Game.Src.ECS.Components.Bots;
using Game.Src.ECS.Components.Movement;
using Game.Src.ECS.Components.Physics;
using Game.Src.ECS.Components.Shooting;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Collections;
using UnityEngine;

namespace Game.Src.ECS.Systems.Bots
{
    public class TargetLocatingSystem : IEcsRunSystem, IEcsInitSystem
    {
        [EcsWorld] private EcsWorld _world;

        [EcsFilter(typeof(AttackerComponent), typeof(RigidbodyComponent))]
        private EcsFilter _attackersFilter;

        [EcsFilter(typeof(TargetComponent), typeof(RigidbodyComponent))]
        private EcsFilter _targetsFilter;


        [EcsPool] private EcsPool<AttackerComponent> _attackersPool;

        [EcsPool] private EcsPool<RigidbodyComponent> _rigidbodyPool;

        [EcsPool] private EcsPool<TargetComponent> _targetsPool;

        public void Run(EcsSystems systems)
        {
            foreach (var botEntity in _attackersFilter)
            {
                ref var attacker = ref _attackersPool.Get(botEntity);

                var attackerRigidbody = _rigidbodyPool.Get(botEntity);

                var isCurrentTargetValid = IsCurrentTargetValid(attacker, attackerRigidbody);

                if (!isCurrentTargetValid)
                {
                    FindNewTarget(ref attacker, attackerRigidbody);
                }
                
                
            }
        }

        private bool IsCurrentTargetValid(AttackerComponent attacker, RigidbodyComponent attackerRigidbody)
        {
            bool isCurrentTargetValid = false;
            if (attacker.Target.Unpack(_world, out int targetEntity))
            {
                var targetRigidbody = _rigidbodyPool.Get(targetEntity);
                var diff = targetRigidbody.Rigidbody.position - attackerRigidbody.Rigidbody.position;
                float dist = diff.magnitude;
                if (dist < attacker.DetectionRadius)
                {
                    Ray ray = new Ray(attackerRigidbody.Rigidbody.position + Vector3.up, diff / dist);

                    if (!Physics.Raycast(ray, dist, _enviromnemtMask))
                    {
                        isCurrentTargetValid = true;
                        attacker.lastVisibleTargetPos = targetRigidbody.CalculatedPosition;
                    }
                }
            }

            return isCurrentTargetValid;
        }


        private Collider[] _cachedColliders = new Collider[200];
        private int _enviromnemtMask;

        private void FindNewTarget(ref AttackerComponent botAttacker, RigidbodyComponent rigidbody)
        {
            float currDist = float.MaxValue;
            botAttacker.Target = new EcsPackedEntity();

            int count = Physics.OverlapSphereNonAlloc(rigidbody.CalculatedPosition, botAttacker.DetectionRadius, _cachedColliders, botAttacker.TargetPhysicsMask);

            if (count > 0)
            {
                foreach (var enemyEntity in _targetsFilter)
                {
                    var target = _targetsPool.Get(enemyEntity);
                    if (((int)botAttacker.AcceptedTargets & (int)target.Type) > 0)
                    {
                        var targetRigidbody = _rigidbodyPool.Get(enemyEntity);
                        Vector3 diff = targetRigidbody.CalculatedPosition - rigidbody.CalculatedPosition;

                        float dist = diff.magnitude;

                        if (dist < currDist)
                        {
                            Ray ray = new Ray(rigidbody.CalculatedPosition + Vector3.up, diff);

                            if (!Physics.Raycast(ray, dist, _enviromnemtMask))
                            {
                                botAttacker.Target = _world.PackEntity(enemyEntity);
                                botAttacker.lastVisibleTargetPos = targetRigidbody.CalculatedPosition;
                                currDist = dist;
                            }
                        }
                    }
                }
            }
        }

        public void Init(EcsSystems systems)
        {
            _enviromnemtMask = LayerMask.GetMask("Environment");
        }
    }
}
