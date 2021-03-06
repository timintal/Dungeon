using Game.Src.ECS.Components.Bots;
using Game.Src.ECS.Components.Movement;
using Game.Src.ECS.Components.Shooting;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Bots
{
    public class BotInputSystem : IEcsRunSystem
    {
        [EcsWorld]
        private EcsWorld _world;
        
        [EcsFilter(typeof(BotComponent), typeof(InputComponent), typeof(AttackerComponent), typeof(TransformComponent))]
        private EcsFilter _bots;

        [EcsPool] private EcsPool<InputComponent> _inputPool;
        [EcsPool] private EcsPool<BotComponent> _botPool;
        [EcsPool] private EcsPool<AttackerComponent> _attackerPool;
        [EcsPool] private EcsPool<TransformComponent> _transformPool;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _bots)
            {
                ref var botInput = ref _inputPool.Get(entity);
                var attacker = _attackerPool.Get(entity);
                int targetEntity;

                if (attacker.Target.Unpack(_world, out targetEntity) &&
                    _transformPool.Has(targetEntity))
                {
                    botInput = CalculateInputWithTarget(entity, targetEntity);
                }
                else
                {
                    botInput.Direction = Vector2.zero;
                }

            }
        }

        private InputComponent CalculateInputWithTarget(int entity, int targetEntity)
        {
            InputComponent botInput;
            var bot = _botPool.Get(entity);
            var botTransform = _transformPool.Get(entity);
            var targetTransform = _transformPool.Get(targetEntity);

            Vector3 diff = TransformComponent.DiffVector(targetTransform, botTransform);
            diff.y = 0;
            float diffMagnitude = diff.magnitude;
            diff = diff / diffMagnitude;

            if (diffMagnitude < bot.PreferredDistance.x)
            {
                botInput.Direction = new Vector2(-diff.x, -diff.z);
            }
            else if (diffMagnitude < bot.PreferredDistance.y)
            {
                botInput.Direction = Vector2.zero;
            }
            else
            {
                botInput.Direction = new Vector2(diff.x, diff.z);
            }

            return botInput;
        }
    }
}