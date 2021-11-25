using Game.Src.ECS.Components.Attacking;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Movement
{
    public class ProjectileMovementSystem : IEcsRunSystem
    {
        private EcsWorld _world;
        
        [EcsFilter(typeof(ProjectileComponent))]
        private EcsFilter _projectileFiler;


        [EcsPool] private EcsPool<ProjectileComponent> _projectilesPool;

        public void Run(EcsSystems systems)
        {
            float dt = Time.deltaTime;

            foreach (var entity in _projectileFiler)
            {
                ref var projectile = ref _projectilesPool.Get(entity);

                projectile.Lifetime -= dt;
                if (projectile.Lifetime < 0)
                {
                    _world.DelEntity(entity);
                }
                else
                {
                    if (MoveBullet(ref projectile))
                    {
                        _world.DelEntity(entity);
                    }
                }
            }
        }


        bool MoveBullet(ref ProjectileComponent projectile)
        {
            Matrix4x4 transformMatrix = projectile.TransformationMatrix;
            
            Vector3 currPos = transformMatrix.GetPosition();
            Quaternion rotation = transformMatrix.rotation;
            Vector3 direction = rotation * Vector3.forward;

            float distance = projectile.Speed * Time.fixedDeltaTime;
            
            if (Physics.Raycast(currPos, direction, out RaycastHit hit, distance, projectile.PhysicsLayers))
            {
                return true;
            }

            Vector3 newPos = currPos + direction * distance;
            transformMatrix[0, 3] = newPos.x;
            transformMatrix[1, 3] = newPos.y;
            transformMatrix[2, 3] = newPos.z;

            projectile.TransformationMatrix = transformMatrix;

            return false;
        }
    }
}
