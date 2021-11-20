using System;
using Unity.Mathematics;

namespace Astar
{
    public struct GridNode : IEquatable<GridNode>
    {
        public int PathCost;

        public int gCost;
        public int hCost;

        public int fCost;

        public int2 GridIndex;

        public int2 PreviousNodeIndex;

        public bool IsWalkable => PathCost < Int32.MaxValue;

        public void UpdateFCost()
        {
            fCost = gCost + hCost;
        }

        public int HeapIndex { get; set; }

        public bool Equals(GridNode other)
        {
            return PathCost == other.PathCost &&
                   gCost == other.gCost &&
                   hCost == other.hCost &&
                   fCost == other.fCost &&
                   GridIndex.x == other.GridIndex.x &&
                   GridIndex.y == other.GridIndex.y &&
                   PreviousNodeIndex.x == other.PreviousNodeIndex.x &&
                   PreviousNodeIndex.y == other.PreviousNodeIndex.y &&
                   HeapIndex == other.HeapIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is GridNode other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PathCost, gCost, hCost, fCost, GridIndex, PreviousNodeIndex, HeapIndex);
        }
    }
}