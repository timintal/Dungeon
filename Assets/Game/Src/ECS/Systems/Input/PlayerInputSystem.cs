using Game.Src.ECS.Components.Player;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Input 
{
    sealed class PlayerInputSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(InputComponent), typeof(PlayerComponent))]
        private readonly EcsFilter _controllableEntities;

        [EcsPool]
        private readonly EcsPool<InputComponent> _input;

        [EcsPool] EcsPool<PlayerComponent> _playerPool;

        public void Run (EcsSystems systems) 
        {
            foreach (var entity in _controllableEntities)
            {
                ref InputComponent input = ref _input.Get(entity);
                var player = _playerPool.Get(entity);
                
                float horizontal = UnityEngine.Input.GetKey(player.Left) ? -1 : 0;
                horizontal += UnityEngine.Input.GetKey(player.Right) ? 1 : 0;

                float vertical = UnityEngine.Input.GetKey(player.Down) ? -1 : 0;
                vertical += UnityEngine.Input.GetKey(player.Up) ? 1 : 0;

                input.Direction = new Vector2(horizontal, vertical);

            }
        }
    }
}