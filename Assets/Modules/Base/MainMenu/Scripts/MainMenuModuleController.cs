using System;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Infrastructure.Modules;
using Cysharp.Threading.Tasks;
using Modules.Base.MainMenu.Scripts.MainMenuState;
using Modules.Base.MainMenu.Scripts.SettingsState;
using R3;
using Stateless;
using UnityEngine;

namespace Modules.Base.MainMenu.Scripts
{
    public class MainMenuModuleController : IModuleController
    {
        private readonly UniTaskCompletionSource _moduleCompletionSource;
        private readonly MainMenuModuleModel _mainMenuModuleModel;
        private readonly MainMenuStatePresenter _mainMenuStatePresenter;
        private readonly SettingsStatePresenter _settingsStatePresenter;
        private readonly IScreenStateMachine _screenStateMachine;
        
        private readonly ReactiveCommand<ModulesMap> _openNewModuleCommand = new();
        
        private readonly CompositeDisposable _disposables = new();
        
        public MainMenuModuleController(IScreenStateMachine screenStateMachine, 
            MainMenuModuleModel mainMenuModuleModel, 
            MainMenuStatePresenter mainMenuStatePresenter,
            SettingsStatePresenter settingsStatePresenter)
        {
            _screenStateMachine = screenStateMachine ?? throw new ArgumentNullException(nameof(screenStateMachine));
            _mainMenuModuleModel = mainMenuModuleModel ?? throw new ArgumentNullException(nameof(mainMenuModuleModel));
            _mainMenuStatePresenter = mainMenuStatePresenter ?? throw new ArgumentNullException(nameof(mainMenuStatePresenter));
            _settingsStatePresenter = settingsStatePresenter ?? throw new ArgumentNullException(nameof(settingsStatePresenter));
            
            _moduleCompletionSource = new UniTaskCompletionSource();
        }

        public async UniTask Enter(object param)
        {
            SubscribeToModuleUpdates();

            // Configure FSM in model with separate state presenters
            _mainMenuModuleModel.ConfigureStateMachine(
                onEnterMainMenu: async () => await _mainMenuStatePresenter.Enter(_openNewModuleCommand),
                onExitMainMenu: () => _mainMenuStatePresenter.Exit(),
                onEnterSettings: async () => await _settingsStatePresenter.Enter(null),
                onExitSettings: () => _settingsStatePresenter.Exit()
            );

            _mainMenuModuleModel.StateMachine.OnTransitionCompleted(OnChangeState);

            _mainMenuStatePresenter.HideStateInstantly();
            _settingsStatePresenter.HideStateInstantly();
            
            // Start with main menu state
            await _mainMenuModuleModel.StateMachine.FireAsync(MainMenuTriggers.BackToMainMenu);
        }

        public async UniTask Execute() => await _moduleCompletionSource.Task;

        public async UniTask Exit()
        {
            _disposables.Dispose();
            await UniTask.WhenAll(_mainMenuStatePresenter.Exit(), _settingsStatePresenter.Exit());
        }

        public void Dispose()
        {
            _mainMenuModuleModel.Dispose();
            _mainMenuStatePresenter.Dispose();
            _settingsStatePresenter.Dispose();
            _disposables.Dispose();
        }

        private void OnChangeState(StateMachine<MainMenuStates, MainMenuTriggers>.Transition transition)
        {
            Debug.Log($"MainMenu OnChangeState: {transition.Trigger} -> {transition.Destination}");
        }

        private void SubscribeToModuleUpdates()
        {
            _openNewModuleCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_mainMenuModuleModel.ModuleTransitionThrottleDelay))
                .Subscribe(RunNewModule)
                .AddTo(_disposables);
        }

        private void RunNewModule(ModulesMap screen)
        {
            _moduleCompletionSource.TrySetResult();
            _screenStateMachine.RunModule(screen);
        }
    }
}