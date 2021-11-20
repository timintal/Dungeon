using System;
using System.Collections.Generic;
using Game.Src.ECS.Components.Shooting;
using Game.Src.ECS.Systems.Bots;
using Game.Src.ECS.Systems.Camera;
using Game.Src.ECS.Systems.Input;
using Game.Src.ECS.Systems.Movement;
using Game.Src.ECS.Systems.Pathfinding;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using VContainer.Unity;

public class EcsSystem : IStartable, ITickable, ILateTickable, IFixedTickable, IDisposable
{
    public EcsWorld GetWorld(string name)
    {
        if (_worlds.ContainsKey(name))
        {
            return _worlds[name];
        }

        return _world;
    }
    
    EcsWorld _world;
    EcsSystems _updateSystems;
    EcsSystems _fixedSystems;
    EcsSystems _lateUpdateSystems;

    private Dictionary<string, EcsWorld> _worlds = new Dictionary<string, EcsWorld>();

    public EcsSystem()
    {
        
    }
    
    public void Start () 
    {
        _world = new EcsWorld ();
        _fixedSystems = new EcsSystems(_world);
        
        _fixedSystems
            .Add (new NavMeshMovementSystem())
            .Add (new CameraSystem())
            .Inject()
            .Init ();
        
        _updateSystems = new EcsSystems(_world);
        
        _updateSystems
            .Add (new PlayerInputSystem())
            .Add (new BotInputSystem())
            .Add (new AccelerationSystem())
            .Add(new EnemyLocatingSystem())
            .Add(new TimerSystem<ReloadTag>())
            .Add(new NavGridVisualizeSystem())
#if UNITY_EDITOR
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
#endif
            .Inject()
            .Init ();
        
        _lateUpdateSystems = new EcsSystems(_world);
        
        _lateUpdateSystems
            .Inject()
            .Init ();
    }
    
    public void Tick () 
    {
        _updateSystems?.Run ();
    }
    
    public void FixedTick () 
    {
        _fixedSystems?.Run ();
    }

    public void LateTick()
    {
        _lateUpdateSystems?.Run();
    }
    
    public void Dispose()
    {
        if (_updateSystems != null)
        {
            _updateSystems.Destroy ();
            _updateSystems = null;
        }
        
        if (_fixedSystems != null)
        {
            _fixedSystems.Destroy ();
            _fixedSystems = null;
        }
        
        if (_lateUpdateSystems != null)
        {
            _lateUpdateSystems.Destroy ();
            _lateUpdateSystems = null;
        }
        
        // destroy world.
        if (_world != null) 
        {
            _world.Destroy ();
            _world = null;
        }
    }

}