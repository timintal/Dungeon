using System;
using System.Collections.Generic;
using Tests.ProceduralGeneration;
using UnityEngine;
using Random = System.Random;

[Serializable]
public struct CAClassicGeneratorSettings
{
	[HideInInspector]
	public int MaxX;
	[HideInInspector]
	public int MaxY;
	public int Iterations;
	public float WallPercent;
}

namespace Generation
{
	public class CAClassicGenerator : IMapGenerator
	{
		private Random rand;

		public int[,] Map;

		public int MapWidth { get; }
		public int MapHeight { get;  }
		public float PercentAreWalls { get; }
		
		public int Iterations { get; }

		public CAClassicGenerator(CAClassicGeneratorSettings settings) :
			this(settings.MaxX, settings.MaxY, settings.Iterations, settings.WallPercent)
		{
		}
		
		public CAClassicGenerator(int mapWidth, int mapHeight, int iterations, float percentWalls = 40)
		{
			MapWidth = mapWidth;
			MapHeight = mapHeight;
			PercentAreWalls = percentWalls;
			Iterations = iterations;
		}
		
		public void MakeCaverns()
		{
			// By initilizing column in the outter loop, its only created ONCE
			for (int column = 0, row = 0; row <= MapHeight - 1; row++)
			{
				for (column = 0; column <= MapWidth - 1; column++)
				{
					Map[column, row] = PlaceWallLogic(column, row);
				}
			}
		}

		public int PlaceWallLogic(int x, int y)
		{
			int numWalls = GetAdjacentWalls(x, y, 1, 1);


			if (Map[x, y] == 1)
			{
				if (numWalls >= 4)
				{
					return 0;
				}

				if (numWalls < 2)
				{
					return 1;
				}

			}
			else
			{
				if (numWalls >= 5)
				{
					return 0;
				}
			}

			return 1;
		}

		public int GetAdjacentWalls(int x, int y, int scopeX, int scopeY)
		{
			int startX = x - scopeX;
			int startY = y - scopeY;
			int endX = x + scopeX;
			int endY = y + scopeY;

			int iX = startX;
			int iY = startY;

			int wallCounter = 0;

			for (iY = startY; iY <= endY; iY++)
			{
				for (iX = startX; iX <= endX; iX++)
				{
					if (!(iX == x && iY == y))
					{
						if (IsWall(iX, iY))
						{
							wallCounter += 1;
						}
					}
				}
			}

			return wallCounter;
		}

		public bool IsWall(int x, int y)
		{
			// Consider out-of-bound a wall
			if (IsOutOfBounds(x, y))
			{
				return true;
			}

			return Map[x, y] == 0;
		}

		bool IsOutOfBounds(int x, int y)
		{
			if (x < 0 || y < 0)
			{
				return true;
			}
			else if (x > MapWidth - 1 || y > MapHeight - 1)
			{
				return true;
			}

			return false;
		}

		public void RandomFillMap()
		{
			// New, empty map
			Map = new int[MapWidth, MapHeight];

			int mapMiddle = MapHeight / 2;
			
			for (int column = 0, row = 0; row < MapHeight; row++)
			{
				for (column = 0; column < MapWidth; column++)
				{
					// If coordinants lie on the the edge of the map (creates a border)
					if (column == 0)
					{
						Map[column, row] = 0;
					}
					else if (row == 0)
					{
						Map[column, row] = 0;
					}
					else if (column == MapWidth - 1)
					{
						Map[column, row] = 0;
					}
					else if (row == MapHeight - 1)
					{
						Map[column, row] = 0;
					}
					else
					{

						if (row == mapMiddle)
						{
							Map[column, row] = 1;
						}
						else
						{
							Map[column, row] = rand.Next(0, 100) <= PercentAreWalls ? 0 : 1;
						}
					}
				}
			}
		}

		public int[,] Generate(int seed)
		{
			rand = new Random(seed);
			RandomFillMap();
			for (int i = 0; i < Iterations; i++)
			{
				MakeCaverns();
			}
			
			return Map;
		}
	}
}