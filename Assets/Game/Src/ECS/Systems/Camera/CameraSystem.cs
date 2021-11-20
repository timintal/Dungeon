using Game.Src.ECS.Components.Camera;
using Game.Src.ECS.Components.Movement;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Camera
{
    public class CameraSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(CameraFollowComponent))]
        private readonly EcsFilter _cameraFollowers;
        
        [EcsFilter(typeof(CameraTargetComponent), typeof(TransformComponent))]
        private readonly EcsFilter _cameraTargets;

        [EcsPool] 
        private readonly EcsPool<CameraFollowComponent> _cameraFollowPool;

        [EcsPool] private readonly EcsPool<TransformComponent> _transformsPool;
        public void Run(EcsSystems systems)
        {
            Vector3 centerPosition = Vector3.zero;
            int count = 0;

            foreach (var entity in _cameraTargets)
            {
                count += 1;
                centerPosition += _transformsPool.Get(entity).Transform.position;
            }

            if (count > 0)
            {
                centerPosition /= count;
            }
            
            foreach (var entity in _cameraFollowers)
            {
                var cameraFollow = _cameraFollowPool.Get(entity);
                
                UpdateCameraPosition(cameraFollow, centerPosition);
            }
        }

        void UpdateCameraPosition(CameraFollowComponent cameraFollow, Vector3 targetPos)
        {
            Vector3 desiredPos = targetPos + cameraFollow.Offset;
            
            cameraFollow.CameraTransform.position = 
                Vector3.Lerp(cameraFollow.CameraTransform.position, desiredPos, cameraFollow.followDamping * Time.deltaTime);
            
            // cameraFollow.CameraTransform.LookAt(targetPos, Vector3.up);
        }
    }
}
