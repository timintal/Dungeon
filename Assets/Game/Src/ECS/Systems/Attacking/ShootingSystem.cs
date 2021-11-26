using Game.Src.ECS.Components.Attacking;
using Game.Src.ECS.Components.Shooting;
using Game.Src.ECS.Components.Timers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Attacking
{
    public class ShootingSystem : IEcsRunSystem
    {
        private EcsWorld _world;
        
        [EcsFilter(typeof(ShooterComponent), typeof(AttackerComponent))]
        [EcsFilterExclude(typeof(TimerComponent<ShootCooldownTimerFlag>))]
        private EcsFilter _shooterFilter;

        [EcsPool] private EcsPool<ShooterComponent> _shooterPool;
        [EcsPool] private EcsPool<TimerComponent<ShootCooldownTimerFlag>> _timerPool;
        [EcsPool] private EcsPool<ProjectileComponent> _projectilesPool;
        [EcsPool] private EcsPool<AttackerComponent> _attackersPool;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _shooterFilter)
            {
                var attakerComponent = _attackersPool.Get(entity);
                if (attakerComponent.Target.Unpack(_world, out _))
                {
                    var shooter = _shooterPool.Get(entity);

                    CreateProjectile(shooter, attakerComponent);
                
                    ref var timer = ref _timerPool.Add(entity);
                    timer.TimeLeft = shooter.Cooldown;
                }
            }
        }

        void CreateProjectile(ShooterComponent shooter, AttackerComponent attacker)
        {
            var entity = _world.NewEntity();
            ref var projectile = ref _projectilesPool.Add(entity);

            projectile.Damage = shooter.Damage;
            projectile.Lifetime = shooter.Lifetime;
            projectile.Material = shooter.ProjectileMaterial;
            projectile.Mesh = shooter.ProjectileMesh;
            projectile.Speed = shooter.ProjectileSpeed;
            projectile.Targets = attacker.AcceptedTargets;
            projectile.PhysicsLayers = attacker.TargetPhysicsMask;
            projectile.TransformationMatrix = Matrix4x4.TRS(shooter.SpawnTransform.position, shooter.SpawnTransform.rotation, Vector3.one);
        }
    }
}