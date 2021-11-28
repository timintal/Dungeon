using Game.Src.ECS.Components.Attacking;
using Game.Src.ECS.Components.Health;
using Game.Src.ECS.Components.Shooting;
using Leopotam.EcsLite;

namespace Game.Src.ECS.Helpers
{
    public static class TimerSystems
    {
        public static EcsSystems AddTimerSystems(this EcsSystems systems)
        {
            return systems
                .Add(new TimerSystem<ShootCooldownTimerFlag>())
                .Add(new TimerSystem<TargetCheckTimerFlag>())
                .Add(new TimerSystem<DestroyComponent>());
        }
    }
}