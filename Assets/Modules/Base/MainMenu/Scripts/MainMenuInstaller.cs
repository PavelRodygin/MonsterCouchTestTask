using CodeBase.Services.SceneInstallerService;
using Modules.Base.MainMenu.Scripts.MainMenuState;
using Modules.Base.MainMenu.Scripts.SettingsState;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.MainMenu.Scripts
{
    public class MainMenuInstaller : BaseModuleSceneInstaller
    {
        [SerializeField] private MainMenuStateView mainMenuStateView;
        [SerializeField] private SettingsStateView settingsStateView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            base.RegisterSceneDependencies(builder);

            // Register Views for their respective Presenters
            builder.RegisterComponent(mainMenuStateView).As<MainMenuStateView>();
            builder.RegisterComponent(settingsStateView).As<SettingsStateView>();
            
            builder.Register<MainMenuModuleModel>(Lifetime.Singleton);
            
            // Register Presenters (they will get their respective Views injected)
            builder.Register<MainMenuStatePresenter>(Lifetime.Singleton);
            builder.Register<SettingsStatePresenter>(Lifetime.Singleton);
            
            // Register Controller (it only gets Presenters, not Views)
            builder.Register<MainMenuModuleController>(Lifetime.Singleton);
        }
    }
}