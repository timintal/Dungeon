using MessagePipe;
using UnityEngine;
using VContainer;

namespace Game.Src.Gameplay.Player
{
    public struct PlayerSpawnerMessages 
    {
        public struct OnSpawn
        {
            public GameObject Player;
        }

        public struct OnDestroy
        {
            public GameObject Player;
        }

        public static void RegisterMessages(IContainerBuilder builder, MessagePipeOptions options)
        {
            builder.RegisterMessageBroker<OnSpawn>(options);
            builder.RegisterMessageBroker<OnDestroy>(options);
        }
    }
}
