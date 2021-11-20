using JetBrains.Annotations;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using OnDestroy = Game.Src.Gameplay.Player.PlayerSpawnerMessages.OnDestroy;
using OnSpawn = Game.Src.Gameplay.Player.PlayerSpawnerMessages.OnSpawn;

namespace Game.Src.Gameplay
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;

        private IObjectResolver _container;
        private GameObject _player;
        private IPublisher<OnDestroy> _onDestroy;
        private IPublisher<OnSpawn> _onSpawn;

        [Inject][UsedImplicitly]
        void Construct(IObjectResolver container, 
            IPublisher<OnDestroy> onDestroy,
            IPublisher<OnSpawn> onSpawn)
        {
            _container = container;
            _onDestroy = onDestroy;
            _onSpawn = onSpawn;
        }

        private void Start()
        {
            SpawnPlayer(transform.position);
        }

        public void SpawnPlayer(Vector3 pos)
        {
            _player = _container.Instantiate(_playerPrefab, pos, Quaternion.identity);
            _onSpawn.Publish(new OnSpawn{Player = _player});
        }

        public void DestroyPlayer()
        {
            if (_player != null)
            {
                Destroy(_player);
                _player = null;
                _onDestroy.Publish(new OnDestroy{Player = _player});
            }
        }
    
    
    }
}
