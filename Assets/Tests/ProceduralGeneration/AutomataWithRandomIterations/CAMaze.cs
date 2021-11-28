using System;
using Tests.ProceduralGeneration;
using UnityEngine;
using Random = System.Random;


namespace Generation
{
    [Serializable]
    public struct CAMazeSettings
    {
        [HideInInspector]
        public int MaxX;
        [HideInInspector]
        public int MaxY;
        public bool OpenWhenOvercrowded;
        public int Iterations;
        public int NeighboursThreshold;
        public float WallPercent;
    }
    
    public class CAMaze : IMapGenerator
    {
        private Random random;

        public int NeighboursThreshold {get ; }
        public int Iterations {get ; }
        public int MapX {get ; }
        public int MapY {get ; }
        public float WallPercent {get ; }
        public bool OpenWhenOvercrowded {get ; }

        public int [,] Map;
        
        public CAMaze(CAMazeSettings settings) : 
            this(settings.MaxX, settings.MaxY, settings.NeighboursThreshold, settings.WallPercent, settings.Iterations, settings.OpenWhenOvercrowded)
        {
        }
        
        public CAMaze(int x, int y,  int neighboursThreshold, float wallPercent, int iterations, bool openWhenOvercrowded)
        {
            NeighboursThreshold = neighboursThreshold;
            Iterations = iterations;
            OpenWhenOvercrowded = openWhenOvercrowded;
            MapX = x;
            MapY = y;
            WallPercent = wallPercent;
        }

        /// <summary>
        /// Build a Map
        /// </summary>
        /// <param name="closeCellProb">Probability of closing a cell</param>
        /// <param name="neighbours">The number of cells required to trigger</param>
        /// <param name="iterations">Number of iterations</param>
        /// <param name="Map">Map array to opearate on</param>
        /// <param name="reset">Clear the Map before operation</param>
        /// <param name="probExceeded">probability exceeded</param>
        /// <param name="invert"></param>
        /// <returns></returns>
        public int[,]  Generate(int seed)
        {
            random = new Random(seed);
            
            Map = new int[MapX, MapY];

            //go through each cell and use the specified probability to determine if it's open
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    if (random.Next(0, 100) < WallPercent)
                    {
                        Map[x, y] = 0;
                    }
                    else
                    {
                        Map[x, y] = 1;
                    }
                }
            }

            //pick some cells at random
            for (int x = 0; x <= Iterations; x++)
            {
                int rX = random.Next(0, Map.GetLength(0));
                int rY = random.Next(0, Map.GetLength(1));

                if (OpenWhenOvercrowded)
                {
                    if (GetNeighboursCount(rX, rY) > NeighboursThreshold)
                    {
                        Map[rX, rY] = 1;
                    }
                    else
                    {
                        Map[rX, rY] = 0;
                    }
                }
                else
                {
                    if (GetNeighboursCount(rX, rY) > NeighboursThreshold)
                    {
                        Map[rX, rY] = 0;
                    }
                    else
                    {
                        Map[rX, rY] = 1;
                    }
                }
            }

            return Map;
        }

        /// <summary>
        /// Count all the closed cells around the specified cell and return that number
        /// </summary>
        /// <param name="xVal">cell X value</param>
        /// <param name="yVal">cell Y value</param>
        /// <returns>Number of surrounding cells</returns>
        private int GetNeighboursCount(int xVal, int yVal)
        {
            int count = 0;

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    
                    if (IsOpen(xVal + x, yVal + y))
                    {
                        count += 1;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Check the examined cell is legal and closed
        /// </summary>
        /// <param name="x">cell X value</param>
        /// <param name="y">cell Y value</param>
        /// <returns>Cell state - true if closed, false if open or illegal</returns>
        private Boolean IsOpen(int x, int y)
        {
            if (x >= 0 && x < Map.GetLength(0) &&
                y >= 0 && y < Map.GetLength(1))
            {
                if (Map[x, y] > 0)
                {
                    return true;
                }
            }
            
            return false;
            
        }
    }


}


