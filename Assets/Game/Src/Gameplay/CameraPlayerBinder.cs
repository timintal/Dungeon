using UnityEngine;

namespace Game.Src.Gameplay
{
    public class CameraPlayerBinder
    {
        private readonly Camera _gameplayCamera;
        private readonly EcsSystem _ecsSystem;

        public CameraPlayerBinder(Camera gameCamera, EcsSystem ecs)
        {
            _ecsSystem = ecs;
            _gameplayCamera = gameCamera;
        }
    }
}
