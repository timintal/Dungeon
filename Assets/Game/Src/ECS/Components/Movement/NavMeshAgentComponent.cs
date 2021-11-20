using System;
using UnityEngine.AI;

namespace Game.Src.ECS.Components.Physics
{
    [Serializable]
    public struct NavMeshAgentComponent
    {
        public NavMeshAgent Agent;
    }
}