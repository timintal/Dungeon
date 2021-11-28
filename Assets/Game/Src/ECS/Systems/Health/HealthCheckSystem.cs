using Game.Src.ECS.Components.Health;
using Game.Src.ECS.Components.Timers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Src.ECS.Systems.Health
{
    public class HealthCheckSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(HealthComponent))] 
        private EcsFilter _healthFilter;

        [EcsPool] private EcsPool<HealthComponent> _healthPool;
        [EcsPool] EcsPool<DestroyComponent> _deadComponentsPool;
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _healthFilter)
            {
                var health = _healthPool.Get(entity);
                if (health.CurrentHealth <= 0)
                {
                    _deadComponentsPool.Add(entity);
                    _healthPool.Del(entity);
                }
            }
        }
    }
}