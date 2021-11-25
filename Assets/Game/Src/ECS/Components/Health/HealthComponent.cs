using System;

namespace Game.Src.ECS.Components.Health
{
    [Serializable]
    public struct HealthComponent
    {
        public int MaxHealth;
        public int CurrentHealth;
    }
}