using System;
using UnityEngine;

namespace Game.Src.ECS.Components.Player
{
    [Serializable]
    public struct PlayerComponent
    {
        
        public KeyCode Up;
        public KeyCode Down;
        public KeyCode Left;
        public KeyCode Right;
    }
}