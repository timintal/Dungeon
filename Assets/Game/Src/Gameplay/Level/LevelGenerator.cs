using System;
using System.Collections.Generic;
using Generation;
using Sirenix.OdinInspector;
using Tests.ProceduralGeneration;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Game.Src.Gameplay.Level
{
    public enum GeneratorType
    {
        CAClassic,
        CAWithRandom,
        Digger
    }
    
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private int maxX;
        [SerializeField] private int maxY;
        [SerializeField] private int seed;
        [SerializeField] private float cellSize;
        [SerializeField] private GameObject wallPrefab;

        [SerializeField] private GeneratorType _generatorType;
        
        [SerializeField] private CAClassicGeneratorSettings classicGeneratorSettings;
        [SerializeField] private CAMazeSettings caMazeSettings;
        [SerializeField] private DiggerMapCreatorSettings diggerMapCreatorSettings;

        private IMapGenerator _currentGenerator;
        private int[,] _map;
        private List<GameObject> _spawnedPrefabs = new List<GameObject>();

        private void Start()
        {
            GenerateLevel();
        }

        private void InitGenerator()
        {
            switch (_generatorType)
            {
                case GeneratorType.CAClassic:
                    classicGeneratorSettings.MaxX = maxX;
                    classicGeneratorSettings.MaxY = maxY;
                    _currentGenerator = new CAClassicGenerator(classicGeneratorSettings);
                    break;
                case GeneratorType.CAWithRandom:
                    caMazeSettings.MaxX = maxX;
                    caMazeSettings.MaxY = maxY;
                    _currentGenerator = new CAMaze(caMazeSettings);
                    break;
                case GeneratorType.Digger:
                    diggerMapCreatorSettings.MaxX = maxX;
                    diggerMapCreatorSettings.MaxY = maxY;
                    _currentGenerator = new DiggerMapCreator(diggerMapCreatorSettings);
                    break;
                default:
                    Debug.LogError("Unsupported Map Generator!");
                    break;
            }
        }

        void SpawnPrefabs()
        {
            Transform myTransform = transform;
            Vector3 initialPos = myTransform.position;
            
            for (int i = 0; i < maxX; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    if (_map[i,j] == 0)
                    {
                        Vector3 position = initialPos + new Vector3(i * cellSize, 0, j * cellSize);
                        _spawnedPrefabs.Add(Instantiate(wallPrefab, position, quaternion.identity, myTransform));
                    }
                }
            }
        }

        
        [Button]
        void GenerateLevel()
        {
            InitGenerator();
            
            ClearCurrentField();

            _map = _currentGenerator.Generate(seed > 0 ? seed : DateTimeOffset.Now.Second);
            SpawnPrefabs();
        }

        [Button]
        private void ClearCurrentField()
        {
            foreach (var prefab in _spawnedPrefabs)
            {
                if (Application.isPlaying)
                {
                    Destroy(prefab);
                }
                else
                {
                    DestroyImmediate(prefab);
                }
            }

            _spawnedPrefabs.Clear();
        }
    }
}