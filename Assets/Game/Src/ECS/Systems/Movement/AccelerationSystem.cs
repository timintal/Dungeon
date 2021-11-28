using Game.Src.ECS.Components.Input;
using Game.Src.ECS.Components.Movement;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Movement
{
    public sealed class AccelerationSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(InputComponent),
            typeof(SpeedComponent), 
            typeof(AccelerationComponent))]
        private readonly EcsFilter _controllableEntities;

        [EcsPool] 
        private readonly EcsPool<InputComponent> _inputPool;
        [EcsPool] 
        private readonly EcsPool<SpeedComponent> _speedPool;
        [EcsPool]
        private readonly EcsPool<AccelerationComponent> _accelerationPool;
        

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _controllableEntities)
            {
                var input = _inputPool.Get(entity);
                ref var speed = ref _speedPool.Get(entity);
                var acceleration = _accelerationPool.Get(entity);

                float x = 0;
                float z = 0;
                
                if (Mathf.Abs(input.Direction.x) < float.Epsilon &&
                    Mathf.Abs(input.Direction.y) < float.Epsilon)
                {
                    if (speed.Magnitude > float.Epsilon)
                    {
                        x = Deaccelearte(speed.Speed.x, acceleration.NoInputDeacceleration);
                        z = Deaccelearte(speed.Speed.z, acceleration.NoInputDeacceleration);
                        speed.Speed = new Vector3(x, speed.Speed.y, z);
                        speed.Magnitude = speed.Speed.magnitude;
                    }
                }
                else
                {
                    x = speed.Speed.x + input.Direction.x * acceleration.Acceleration * Time.deltaTime;
                    z = speed.Speed.z + input.Direction.y * acceleration.Acceleration * Time.deltaTime;

                    speed.Speed = new Vector3(x, 0, z);

                    float speedMagn = speed.Speed.magnitude;
                    if (speedMagn > acceleration.MaxSpeed)
                    {
                        speed.Speed = speed.Speed / speedMagn * acceleration.MaxSpeed;
                        speedMagn = acceleration.MaxSpeed;
                    }

                    speed.Magnitude = speedMagn;
                }
                
                
            }
        }

        float Deaccelearte(float current, float amount)
        {
            float sign = Mathf.Sign(current);

            float result = Mathf.Abs(current) - amount * Time.deltaTime;

            result = Mathf.Max(0, result) * sign;
            
            return result;
        }
    }
}