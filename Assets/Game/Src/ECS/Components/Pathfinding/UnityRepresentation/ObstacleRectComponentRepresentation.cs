using Game.Src.ECS.Helpers;
using UnityEngine;

namespace Game.Src.ECS.Components.Pathfinding.UnityRepresentation
{
    public class ObstacleRectComponentRepresentation : EcsComponentRepresentation<ObstacleRectComponent>
    {
        private void OnDrawGizmosSelected()
        {
            Color color = Color.yellow;
            color.a = 0.5f;
            Gizmos.color = color;
            
            Gizmos.DrawCube(transform.position + _component.offset, _component.size);
            Gizmos.color = Color.white;
            
        }
    }
}