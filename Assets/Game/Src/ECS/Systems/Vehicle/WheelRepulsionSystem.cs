using Game.Src.ECS.Components.Vehicle;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Vehicle
{
    public class WheelRepulsionSystem : IEcsRunSystem, IEcsInitSystem
    {
        [EcsFilter(typeof(WheelComponent))] 
        private EcsFilter _wheelsFilter;

        [EcsPool] EcsPool<WheelComponent> _wheelPool;
        private int _layerMask;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _wheelsFilter)
            {
                ref var wheel = ref _wheelPool.Get(entity);
                float previousCompression = wheel.CurrentSuspensionCompression;

                Vector3 up = wheel.Transform.up;
                Vector3 position = wheel.Transform.position;

                RaycastHit hit;
                if (Physics.Raycast(position, -up, out hit, wheel.FallSlowdownDistance, _layerMask))
                {
                    Vector3 diff = hit.point - position;
                    float dist = diff.magnitude;

                    if (dist < wheel.SuspensionLength)
                    {
                        float compressionFactor = (wheel.SuspensionLength - dist) / wheel.SuspensionLength;

                        float repulsion = wheel.SuspensionRepulsionCurve.Evaluate(compressionFactor) * wheel.SuspensionRepulsionAmplitude;
                        
                        wheel.ConnectedRigidbody.AddForceAtPosition(repulsion * up, hit.point, ForceMode.Acceleration);
                        wheel.CurrentSuspensionCompression = compressionFactor;
                        
                        Vector3 wheelVelocity = wheel.ConnectedRigidbody.GetPointVelocity(position);

                        float suspensionAxisVelocity = Vector3.Dot(up, wheelVelocity);
                        Vector3 dampingForce = -suspensionAxisVelocity * wheel.SuspensionDamping * up;
                        wheel.ConnectedRigidbody.AddForceAtPosition(dampingForce, position, ForceMode.Acceleration);
                    }
                }
            }
            
        }

        public void UpdateWheel(WheelComponent wheel, RaycastHit hit)
        {
            
        }

        public void Init(EcsSystems systems)
        {
            _layerMask = ~0;
        }
    }
}
