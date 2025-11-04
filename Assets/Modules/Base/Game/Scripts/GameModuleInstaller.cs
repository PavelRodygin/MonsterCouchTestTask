using CodeBase.Services.SceneInstallerService;
using Modules.Base.GameModule.Scripts.Gameplay.Systems;
using Modules.Base.Game.Scripts.Gameplay.Player.Factory;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.Game.Scripts
{
    public class GameModuleInstaller : BaseModuleSceneInstaller
    {
        [SerializeField] private GameView gameView;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject playerPrefab;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            base.RegisterSceneDependencies(builder);
            
            builder.Register<GameModuleController>(Lifetime.Singleton);
            
            builder.Register<GameModuleModel>(Lifetime.Singleton);
            builder.Register<GamePresenter>(Lifetime.Singleton);
            builder.RegisterComponent(gameView).As<GameView>();
            
            builder.RegisterComponent(gameManager);
            
            // Register player factory
            builder
                .Register<PlayerFactory>(Lifetime.Singleton)
                .WithParameter("playerPrefab", playerPrefab)
                .AsSelf()
                .AsImplementedInterfaces();
        }
    }
}
