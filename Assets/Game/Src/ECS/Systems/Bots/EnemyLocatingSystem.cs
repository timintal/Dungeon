using System.Linq;
using Game.Src.ECS.Components.Bots;
using Game.Src.ECS.Components.Movement;
using Game.Src.ECS.Components.Shooting;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class EnemyLocatingSystem : IEcsRunSystem
{
    [EcsWorld]
    private EcsWorld _world;
    
    [EcsFilter(typeof(BotTag), typeof(AttackerComponent), typeof(TransformComponent))]
    private EcsFilter _bots;
    
    [EcsFilter(typeof(TargetComponent), typeof(TransformComponent))]
    private EcsFilter _enemies;
    

    [EcsPool] 
    private EcsPool<AttackerComponent> _attackersPool;

    [EcsPool] 
    private EcsPool<TransformComponent> _transformsPool;
    
    [EcsPool] 
    private EcsPool<TargetComponent> _targetsPool;

    public void Run(EcsSystems systems)
    {
        foreach (var botEntity in _bots)
        {
            ref var botAttacker = ref _attackersPool.Get(botEntity);

            var botTransform = _transformsPool.Get(botEntity);
            
            FindNewTarget(ref botAttacker, botTransform);
        }
    }

    private void FindNewTarget(ref AttackerComponent botAttacker, TransformComponent transform)
    {
        float currDist = float.MaxValue;
        botAttacker.Target = new EcsPackedEntity();
        
        foreach (var enemyEntity in _enemies)
        {
            var target = _targetsPool.Get(enemyEntity);
            if (((int)botAttacker.AcceptedTargets & (int)target.Type) > 0)
            {
                var targetTransform = _transformsPool.Get(enemyEntity);
                Vector3 diff = TransformComponent.DiffVector(targetTransform, transform);

                float dist = diff.magnitude;

                if (dist < currDist)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(new Ray(transform.Transform.position + Vector3.up, diff), out hit, dist * 2))
                    {
                        if (hit.collider == target.Collider)
                        {
                            botAttacker.Target = _world.PackEntity(enemyEntity); 
                            currDist = dist;
                        }
                    }
                }
            }
        }
    }
}
