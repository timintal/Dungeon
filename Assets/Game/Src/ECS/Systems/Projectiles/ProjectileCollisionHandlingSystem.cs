using Game.Src.ECS.Components.Attacking;
using Game.Src.ECS.Components.Health;
using Game.Src.ECS.Components.Physics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Projectiles
{
    public class ProjectileCollisionHandlingSystem : IEcsRunSystem
    {
        [EcsWorld]
        private EcsWorld _world;
        
        [EcsFilter(typeof(ProjectileComponent), typeof(ProjectileHitComponent))]
        private EcsFilter _projectileHitFilter;

        [EcsFilter(typeof(ColliderComponent), typeof(HealthComponent), typeof(TargetComponent))]
        private EcsFilter _colliderFilter;
        

        [EcsPool] private EcsPool<ProjectileComponent> _projectilePool;
        [EcsPool] private EcsPool<ProjectileHitComponent> _projectileHitPool;
        [EcsPool] private EcsPool<ColliderComponent> _collidersPool;
        [EcsPool] private EcsPool<HealthComponent> _healthPool;
        [EcsPool] private EcsPool<TargetComponent> _targetPool;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _projectileHitFilter)
            {
                bool needToDestroyProjectile = true;
                var hitInfo = _projectileHitPool.Get(entity);

                foreach (var colliderEntity in _colliderFilter)
                {
                    var collider = _collidersPool.Get(colliderEntity);
                    
                    if (ReferenceEquals(collider.Collider, hitInfo.Collider))
                    {
                        needToDestroyProjectile = ProcessProjectileCollision(entity, colliderEntity);
                        break;
                    }
                }

                if (needToDestroyProjectile)
                {
                    _world.DelEntity(entity);
                }
                else
                {
                    _projectileHitPool.Del(entity);
                }
            }
        }

        private bool ProcessProjectileCollision(int projectileEntity, int colliderEntity)
        {
            var projectile = _projectilePool.Get(projectileEntity);
            var target = _targetPool.Get(colliderEntity);

            if ((target.Type & projectile.Targets) > 0)
            {
                ref var health = ref _healthPool.Get(colliderEntity);
                health.CurrentHealth -= projectile.Damage;
                return true;
            }

            return false;
        }
    }
}