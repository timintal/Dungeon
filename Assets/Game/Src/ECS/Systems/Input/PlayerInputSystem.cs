using Game.Src.ECS.Components.Player;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Input {
    sealed class PlayerInputSystem : IEcsRunSystem
    {
        [EcsFilter(typeof(InputComponent), typeof(PlayerTag))]
        private readonly EcsFilter _controllableEntities;

        [EcsPool]
        private readonly EcsPool<InputComponent> _input;

        public void Run (EcsSystems systems) 
        {
            foreach (var entity in _controllableEntities)
            {
                ref InputComponent input = ref _input.Get(entity);
                float horizontal = UnityEngine.Input.GetKey(KeyCode.A) ? -1 : 0;
                horizontal += UnityEngine.Input.GetKey(KeyCode.D) ? 1 : 0;

                float vertical = UnityEngine.Input.GetKey(KeyCode.S) ? -1 : 0;
                vertical += UnityEngine.Input.GetKey(KeyCode.W) ? 1 : 0;

                input.Direction = new Vector2(horizontal, vertical);

            }
        }
    }
}