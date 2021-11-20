using System;
using Game.Src.ECS.Helpers;
using UnityEngine;

namespace Game.Src.ECS.Components.Pathfinding.UnityRepresentation
{
    public class GridComponentRepresentation : EcsComponentRepresentation<GridComponent>
    {
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            
            Gizmos.DrawWireCube(transform.position, _component.GridSize);
        }
    }
}