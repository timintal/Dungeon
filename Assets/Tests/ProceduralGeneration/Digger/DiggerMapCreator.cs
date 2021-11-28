using System;
using System.Collections.Generic;
using Tests.ProceduralGeneration;
using UnityEngine;
using Random = System.Random;

namespace Generation
{
    [Serializable]
    public struct DiggerMapCreatorSettings
    {
        [HideInInspector]
        public int MaxX;
        [HideInInspector]
        public int MaxY;
        public float DiggerSpawnProbability;
        public int MaxDiggers;
        [Range(0,100)]
        public int WallsPercent;
    }
    
    public class DiggerMapCreator : IMapGenerator
    {
        private Random random;

        private int maxX;
        private int maxY;

        private float diggerSpawnProbability;
        private int maxDiggers;
        private int WallsPercent;

        private int[,] map;

        private int spawnedDiggersCount;
        private List<Vector2Int> currentActiveDiggers = new List<Vector2Int>();
        private List<Vector2Int> discoveredAvailableCells = new List<Vector2Int>();

        public DiggerMapCreator(DiggerMapCreatorSettings settings)
        {
            maxX = settings.MaxX;
            maxY = settings.MaxY;

            WallsPercent = settings.WallsPercent;
            
            diggerSpawnProbability = settings.DiggerSpawnProbability;
            maxDiggers = settings.MaxDiggers;
        }

        public int[,] Generate(int seed)
        {
            map = new int[maxX, maxY];
            random = new Random(seed);
            currentActiveDiggers.Clear();
            discoveredAvailableCells.Clear();
            discoveredAvailableCells.Add(new Vector2Int(maxX/2, maxY / 2));

            spawnedDiggersCount = 0;
            diggedCellsCount = 0;
            
            RunDiggers();
            Cleanup();

            return map;
        }

        private void RunDiggers()
        {
            int minDiggedArea = maxX * maxY * (100 - WallsPercent) / 100;
            while (diggedCellsCount < minDiggedArea && spawnedDiggersCount <= maxDiggers)
            {
                if (currentActiveDiggers.Count == 0)
                {
                    SpawnRandomDigger();
                }

                for (int i = currentActiveDiggers.Count - 1; i >= 0; i--)
                {
                    Vector2Int offset = GetRandomOffset(currentActiveDiggers[i]);
                    if (offset != Vector2Int.zero)
                    {
                        Vector2Int newPos = currentActiveDiggers[i] + offset;
                        Dig(newPos);
                        diggedCellsCount++;
                        currentActiveDiggers[i] = newPos;
                        TrySpawnDigger(newPos);
                    }
                    else
                    {
                        currentActiveDiggers.RemoveAt(i);
                    }
                }
            }
        }

        void Dig(Vector2Int pos)
        {
            map[pos.x, pos.y] = 1;
            discoveredAvailableCells.Remove(pos);

            Vector2Int neighbour = pos + Vector2Int.up;
            if (CanDig(neighbour))
            {
                discoveredAvailableCells.Add(neighbour);
            }
            neighbour = pos + Vector2Int.down;
            if (CanDig(neighbour))
            {
                discoveredAvailableCells.Add(neighbour);
            }
            neighbour = pos + Vector2Int.left;
            if (CanDig(neighbour))
            {
                discoveredAvailableCells.Add(neighbour);
            }
            neighbour = pos + Vector2Int.right;
            if (CanDig(neighbour))
            {
                discoveredAvailableCells.Add(neighbour);
            }
        }

        void SpawnRandomDigger()
        {
            if (discoveredAvailableCells.Count > 0)
            {
                int index = random.Next(0, discoveredAvailableCells.Count);
                Vector2Int pos = discoveredAvailableCells[index];
                discoveredAvailableCells.RemoveAt(index);
                Dig(pos);
                currentActiveDiggers.Add(pos);
            }
        }
        
        
        private void TrySpawnDigger(Vector2Int newPos)
        {
            if (random.Next(0, 100) < diggerSpawnProbability && spawnedDiggersCount < maxDiggers)
            {
                Vector2Int newDiggerPos = GetRandomOffset(newPos);
                if (newDiggerPos != Vector2Int.zero)
                {
                    SpawnDigger(newPos + newDiggerPos);
                }
            }
        }

        private void SpawnDigger(Vector2Int newDiggerPos)
        {
            currentActiveDiggers.Add(newDiggerPos);
            map[newDiggerPos.x, newDiggerPos.y] = 1;
            spawnedDiggersCount++;
        }

        private Vector2Int[] possibleDirections = new Vector2Int[4];
        private int diggedCellsCount;

        Vector2Int GetRandomOffset(Vector2Int pos)
        {
            int possibleDirectionsCount = 0;

            if (CanDig(pos + Vector2Int.up))
            {
                possibleDirections[possibleDirectionsCount++] = Vector2Int.up;
            }

            if (CanDig(pos + Vector2Int.down))
            {
                possibleDirections[possibleDirectionsCount++] = Vector2Int.down;
            }

            if (CanDig(pos + Vector2Int.left))
            {
                possibleDirections[possibleDirectionsCount++] = Vector2Int.left;
            }

            if (CanDig(pos + Vector2Int.right))
            {
                possibleDirections[possibleDirectionsCount++] = Vector2Int.right;
            }

            if (possibleDirectionsCount > 0)
            {
                return possibleDirections[random.Next(0, possibleDirectionsCount)];
            }

            return Vector2Int.zero;
        }

        private bool CanDig(Vector2Int neighbour)
        {
            return IsInBoundsAndNotEdge(neighbour) && map[neighbour.x, neighbour.y] == 0;
        }
        
        bool IsInBoundsAndNotEdge(Vector2Int pos)
        {
            //Consider border as out of bounds to have closed area
            return pos.x > 1 && pos.y > 1 && pos.x < maxX - 1 && pos.y < maxY - 1;
        }

        void Cleanup()
        {
            for (int i = 0; i < maxX; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    if (NeedClear(i, j))
                    {
                        map[i, j] = 1;
                    }
                }
            }
        }

        bool NeedClear(int x, int y)
        {
            int neighboursCount = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y)
                    {
                        continue;
                    }

                    if (IsInBounds(i,j) && map[i, j] == 0)
                    {
                        neighboursCount++;
                    }
                }
            }

            return neighboursCount <= 1;
        }

        bool IsInBounds(int x, int y)
        {
            return x > 0 && y > 0 && x < maxX && y < maxY;
        }

        
    }
}