using Game.Src.ECS.Helpers;
using UnityEngine;

namespace Game.Src.ECS.Components.Pathfinding.UnityRepresentation
{
    public class ObstacleCircleComponentRepresentation : EcsComponentRepresentation<ObstacleCircleComponent>
    {
        private void OnDrawGizmosSelected()
        {
            Color color = Color.yellow;
            color.a = 0.5f;
            Gizmos.color = color;
            
            Gizmos.DrawSphere(transform.position + _component.offset, _component.radius);
            Gizmos.color = Color.white;
        }
    }
}