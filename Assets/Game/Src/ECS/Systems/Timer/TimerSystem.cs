using Game.Src.ECS.Components.Timers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class TimerSystem<ITimerFlag> : IEcsInitSystem, IEcsRunSystem where ITimerFlag: struct
{
    [EcsWorld]
    private EcsWorld _world;
    
    private EcsFilter _timers;

    [EcsPool] private EcsPool<TimerComponent<ITimerFlag>> _timerPool;

    public void Init(EcsSystems systems)
    {
        _timers = _world.Filter<TimerComponent<ITimerFlag>>().End();
    }
    
    public void Run(EcsSystems systems)
    {
        float dt = Time.deltaTime;
        foreach (var entity in _timers)
        {
            ref var timer = ref _timerPool.Get(entity);
            timer.TimeLeft -= dt;

            if (timer.TimeLeft < 0)
            {
                _timerPool.Del(entity);
            }
        }
    }

}
