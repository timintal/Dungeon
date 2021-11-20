using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Astar
{
    [BurstCompile(CompileSynchronously = true)]
    public struct AStarJob : IJob
    {
        private const int NORMAL_STEP_COST = 10;
        private const int DIAGONAL_STEP_COST = 14;

        [ReadOnly]
        public NativeHashMap<int2, GridNode> grid;

        [ReadOnly] public int2 start;
        [ReadOnly] public int2 end;
        [WriteOnly] public NativeList<int2> result;

        public void Execute()
        {
            NativeHashMap<int2, GridNode> currentField = new NativeHashMap<int2, GridNode>(grid.Count(), Allocator.Temp);

            FillCurrentField(ref currentField);

            NativeList<int2> openSet = new NativeList<int2>(Allocator.Temp);
            NativeHashSet<int2> closedSet = new NativeHashSet<int2>(grid.Count(), Allocator.Temp);

            NativeArray<int2> neighbours = new NativeArray<int2>(8, Allocator.Temp);
            neighbours[0] = new int2(-1, -1);
            neighbours[1] = new int2(0, -1);
            neighbours[2] = new int2(1, -1);
            neighbours[3] = new int2(-1, 0);
            neighbours[4] = new int2(1, 0);
            neighbours[5] = new int2(-1, 1);
            neighbours[6] = new int2(0, 1);
            neighbours[7] = new int2(1, 1);

            GridNode startNode = currentField[start];
            startNode.gCost = 0;
            startNode.UpdateFCost();
            currentField[startNode.GridIndex] = startNode;
            openSet.Add(startNode.GridIndex);

            while (!openSet.IsEmpty)
            {
                var currentNode = FindNextNode(currentField, ref openSet);

                if (end.Equals(currentNode.GridIndex))
                {
                    RestorePath(currentNode, currentField);
                    return;
                }

                closedSet.Add(currentNode.GridIndex);

                foreach (var neighbourOffset in neighbours)
                {
                    int2 neighbourIndex = currentNode.GridIndex + neighbourOffset;

                    if (currentField.TryGetValue(neighbourIndex, out GridNode neighbour))
                    {
                        if (closedSet.Contains(neighbourIndex) || !neighbour.IsWalkable)
                        {
                            continue;
                        }

                        bool isDiagonal = neighbourOffset.x != 0 && neighbourOffset.y != 0;
                        int currentGCost = currentNode.gCost + (isDiagonal ? DIAGONAL_STEP_COST : NORMAL_STEP_COST);

                        if (neighbour.gCost > currentGCost)
                        {
                            neighbour.gCost = currentGCost;
                            neighbour.PreviousNodeIndex = currentNode.GridIndex;
                            neighbour.UpdateFCost();
                            if (!openSet.Contains(neighbour.GridIndex))
                            {
                                openSet.Add(neighbour.GridIndex);
                            }

                            currentField[neighbourIndex] = neighbour;
                        }
                    }
                }
            }
        }

        private void RestorePath(GridNode currentNode, NativeHashMap<int2, GridNode> currentField)
        {
            while (currentNode.PreviousNodeIndex.x >= 0 &&
                   currentNode.PreviousNodeIndex.y >= 0)
            {
                result.Add(currentNode.GridIndex);
                currentNode = currentField[currentNode.PreviousNodeIndex];
            }
        }

        private GridNode FindNextNode(NativeHashMap<int2, GridNode> currentField, ref NativeList<int2> openSet)
        {
            int minFCost = int.MaxValue;
            GridNode currentNode = currentField[openSet[0]];
            int openSetIndex = 0;
            for (int i = 0; i < openSet.Length; i++)
            {
                int2 nodeIndex = openSet[i];
                if (currentField[nodeIndex].fCost < minFCost)
                {
                    currentNode = currentField[nodeIndex];
                    minFCost = currentNode.fCost;
                    openSetIndex = i;
                }
            }

            openSet.RemoveAt(openSetIndex);
            return currentNode;
        }

        private void FillCurrentField(ref NativeHashMap<int2, GridNode> currentField)
        {
            var keyValueArrays = grid.GetKeyValueArrays(Allocator.Temp);

            foreach (var key in keyValueArrays.Keys)
            {
                GridNode n = grid[key];
                n.HeapIndex = -1;
                n.gCost = DIAGONAL_STEP_COST * (grid.Count());
                n.hCost = MinimumCost(key, end);
                n.fCost = 0;
                n.GridIndex = key;
                n.PreviousNodeIndex = new int2(-1, -1);
                currentField[key] = n;
            }
        }
        
        int MinimumCost(int2 start, int2 end)
        {
            int2 diff = start - end;
            int distX = math.abs(diff.x);
            int distY = math.abs(diff.y);

            if (distX > distY)
            {
                return DIAGONAL_STEP_COST * distY + NORMAL_STEP_COST * (distX - distY);
            }

            return DIAGONAL_STEP_COST * distX + NORMAL_STEP_COST * (distY - distX);
        }
    }
}