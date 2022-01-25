using Game.Src.ECS.Systems.Vehicle;
using Leopotam.EcsLite;

namespace Game.Src.ECS.Helpers
{
    public static class VehicleSystems
    {
        public static EcsSystems AddVehicleSystems(this EcsSystems systems)
        {
            return systems
                .Add(new WheelRepulsionSystem());
        }
    }
}