using System;
using Game.Src.ECS.Helpers;
using UnityEngine;

namespace Game.Src.ECS.Components.Player.UnityRepresentation
{
    public class CharacterColliderRepresentation : EcsComponentRepresentation<CharacterColliderComponent>
    {
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1,1, 0,0.3f);
            Gizmos.DrawSphere(transform.position + _component.sphere1Offset, _component.Radius);
            Gizmos.DrawSphere(transform.position + _component.sphere2Offset, _component.Radius);
            Gizmos.color = Color.white;
        }
    }
}