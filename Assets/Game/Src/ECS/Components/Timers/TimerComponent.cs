namespace Game.Src.ECS.Components.Timers
{
    public struct TimerComponent<ITimerFlag> where ITimerFlag: struct
    {
        public float TimeLeft;
    }
}