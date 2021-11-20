using Game.Src.Gameplay.Player;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Src.Gameplay
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private Camera _gameplayCamera;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EcsSystem>().AsSelf();
            builder.RegisterInstance(_gameplayCamera);
            
            RegisterMessagePipe(builder);
        }


        void RegisterMessagePipe(IContainerBuilder builder)
        {
            // RegisterMessagePipe returns options.
            MessagePipeOptions options = builder.RegisterMessagePipe();
            
            // Setup GlobalMessagePipe to enable diagnostics window and global function
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));
            
            PlayerSpawnerMessages.RegisterMessages(builder, options);
            
        }
    }
}
