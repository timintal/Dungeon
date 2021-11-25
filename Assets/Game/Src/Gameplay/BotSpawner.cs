using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

public class BotSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _botPrefab;
    [SerializeField] private int count;
    [SerializeField] private Vector3 _spawnArea;

    private IObjectResolver _container;

    [Inject] [UsedImplicitly]
    void Construct(IObjectResolver container)
    {
        _container = container;
    }

    private void Start()
    {
        for (int i = 0; i < count; i++)
        {
            var position = transform.position;
            _container.Instantiate(_botPrefab, 
                new Vector3(position.x + Random.Range(-_spawnArea.x, _spawnArea.x), 
                    0,//position.y + Random.Range(-_spawnArea.y, _spawnArea.y),
                    position.z + Random.Range(-_spawnArea.z, _spawnArea.z)), 
                Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color prevColor = Gizmos.color;
    
        Gizmos.color = new Color(0.2f,1f, 0.2f, 0.5f);
        
        Gizmos.DrawCube(transform.position, _spawnArea);

        Gizmos.color = prevColor;
    }
}
