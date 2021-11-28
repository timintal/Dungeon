using Game.Src.ECS.Components.General;
using Game.Src.ECS.Components.Health;
using Game.Src.ECS.Components.Timers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Health
{
    public class DestroyObjectSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(RootGOComponent), typeof(DestroyComponent))]
        [EcsFilterExclude(typeof(TimerComponent<DestroyComponent>))]
        private EcsFilter _deadObjectsFilter;

        [EcsPool] private EcsPool<RootGOComponent> _objectsPool;
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _deadObjectsFilter)
            {
                var gameObject = _objectsPool.Get(entity);

                if (gameObject.GameObject != null)
                {
                    Object.Destroy(gameObject.GameObject);
                }

                EcsWorld world = systems.GetWorld();
                world.DelEntity(entity);

            }
        }
    }
}