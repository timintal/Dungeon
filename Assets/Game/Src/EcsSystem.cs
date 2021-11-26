using System;
using System.Collections.Generic;
using Game.Src.ECS.Components.Attacking;
using Game.Src.ECS.Components.Shooting;
using Game.Src.ECS.Systems.Attacking;
using Game.Src.ECS.Systems.Bots;
using Game.Src.ECS.Systems.Camera;
using Game.Src.ECS.Systems.Input;
using Game.Src.ECS.Systems.Movement;
using Game.Src.ECS.Systems.Pathfinding;
using Game.Src.ECS.Systems.Projectiles;
using Game.Src.ECS.Systems.Visual;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using VContainer.Unity;

public class EcsSystem : IStartable, ITickable, ILateTickable, IFixedTickable, IDisposable
{
    private const string EVENTS_WORLD = "events";
    public EcsWorld GetWorld(string name)
    {
        if (_worlds.ContainsKey(name))
        {
            return _worlds[name];
        }

        return _world;
    }
    
    EcsWorld _world;
    EcsWorld _eventsWorld;
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
        _eventsWorld = new EcsWorld();
        _worlds.Add(EVENTS_WORLD, _eventsWorld);
        
        _fixedSystems = new EcsSystems(_world);
        
        _fixedSystems
            .AddWorld(_eventsWorld, EVENTS_WORLD)
            .Add (new RigidbodyResetTransformSystem()) //sets initial rigidbody positions
            .Add (new UnitsSeparationSystem())
            .Add (new CharacterMovementSystem())
            .Add(new RotateToTargetSystem())
            .Add(new ProjectileMovementSystem())
            .Add (new RigidbodySetTransformSystem()) //sets resulting calculated position to rigidbody
            .Add (new CameraSystem())
            .Inject()
            .Init ();
        
        _updateSystems = new EcsSystems(_world);

        
        _updateSystems
            .AddWorld(_eventsWorld, EVENTS_WORLD)
            .Add (new PlayerInputSystem())
            .Add (new BotInputSystem())
            .Add (new AccelerationSystem())
            .Add(new TargetLocatingSystem())
            .Add(new ShootingSystem())
            .Add(new ProjectileDrawSystem())
            .Add(new TimerSystem<ShootCooldownTimerFlag>())
            .Add(new ProjectileDrawSystem())
#if UNITY_EDITOR && ENABLE_ECS_DEBUG
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
#endif
            .Inject()
            .Init ();
        
        _lateUpdateSystems = new EcsSystems(_world);
        
        _lateUpdateSystems
            .AddWorld(_eventsWorld, EVENTS_WORLD)
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
        
        // destroy world.
        if (_eventsWorld != null) 
        {
            _eventsWorld.Destroy ();
            _eventsWorld = null;
        }
    }

}