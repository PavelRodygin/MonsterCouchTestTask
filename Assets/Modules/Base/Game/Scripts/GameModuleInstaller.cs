using CodeBase.Services.SceneInstallerService;
using Modules.Base.GameModule.Scripts.Gameplay.Systems;
using Modules.Base.Game.Scripts.Gameplay.Player.Factory;
using Modules.Base.Game.Scripts.Gameplay.Enemy;
using Modules.Base.Game.Scripts.Gameplay.Enemy.Factory;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.Game.Scripts
{
    public class GameModuleInstaller : BaseModuleSceneInstaller
    {
        [SerializeField] private GameView gameView;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject enemyPrefab;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            base.RegisterSceneDependencies(builder);
            
            builder.Register<GameModuleController>(Lifetime.Singleton);
            
            builder.Register<GameModuleModel>(Lifetime.Singleton);
            builder.Register<GamePresenter>(Lifetime.Singleton);
            builder.RegisterComponent(gameView).As<GameView>();
            
            builder.RegisterComponent(gameManager);
            
            // Register EnemyManager component from scene
            builder.RegisterComponent(enemyManager);
            
            // Camera is already registered in BaseModuleSceneInstaller
            
            // Register player factory
            builder
                .Register<PlayerFactory>(Lifetime.Singleton)
                .WithParameter("playerPrefab", playerPrefab)
                .AsSelf()
                .AsImplementedInterfaces();

            // Register enemy factory
            builder
                .Register<EnemyFactory>(Lifetime.Singleton)
                .WithParameter("enemyPrefab", enemyPrefab)
                .AsSelf()
                .AsImplementedInterfaces();
        }
    }
}
