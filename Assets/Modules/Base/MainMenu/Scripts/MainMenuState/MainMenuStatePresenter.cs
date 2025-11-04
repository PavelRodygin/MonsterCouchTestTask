using System;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Core.Systems;
using Cysharp.Threading.Tasks;
using R3;
using Unit = R3.Unit;

namespace Modules.Base.MainMenu.Scripts.MainMenuState
{
    public class MainMenuStatePresenter : IPresenter
    {
        private readonly MainMenuModuleModel _mainMenuModuleModel;
        private readonly MainMenuStateView _mainMenuStateView;
        private readonly AudioSystem _audioSystem;
        
        private readonly ReactiveCommand<Unit> _openConverterCommand = new();
        private readonly ReactiveCommand<Unit> _openTicTacCommand = new();
        private readonly ReactiveCommand<Unit> _openGameCommand = new();
        private readonly ReactiveCommand<Unit> _openSettingsCommand = new();
        
        private ReactiveCommand<ModulesMap> _openNewModuleCommand = new();
        private readonly CompositeDisposable _disposables = new();
        
        public MainMenuStatePresenter(MainMenuModuleModel mainMenuModuleModel, 
            MainMenuStateView mainMenuStateView, AudioSystem audioSystem)
        {
            _mainMenuModuleModel = mainMenuModuleModel ?? throw new ArgumentNullException(nameof(mainMenuModuleModel));
            _mainMenuStateView = mainMenuStateView ?? throw new ArgumentNullException(nameof(mainMenuStateView));
            _audioSystem = audioSystem ?? throw new ArgumentNullException(nameof(audioSystem));

            SubscribeToUIUpdates();
        }

        private void SubscribeToUIUpdates()
        {
            _openConverterCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_mainMenuModuleModel.CommandThrottleDelay))
                .Subscribe(_ => OnConverterCommand())
                .AddTo(_disposables);
            _openTicTacCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_mainMenuModuleModel.CommandThrottleDelay))
                .Subscribe(_ => OnTicTacCommand())
                .AddTo(_disposables);
            _openGameCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_mainMenuModuleModel.CommandThrottleDelay))
                .Subscribe(_ => OnGameCommand())
                .AddTo(_disposables);
            _openSettingsCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_mainMenuModuleModel.CommandThrottleDelay))
                .Subscribe(_ => OnOpenSettingsCommand())
                .AddTo(_disposables);
        }

        public async UniTask Enter(object param)
        {
            _openNewModuleCommand = param as ReactiveCommand<ModulesMap> ?? throw new ArgumentException("Expected ReactiveCommand<ModulesMap>", nameof(param));
            
            _mainMenuStateView.HideInstantly();

            var commands = new MainMenuCommands(
                _openGameCommand,
                _openSettingsCommand
                );

            _mainMenuStateView.SetupEventListeners(commands);

            await _mainMenuStateView.Show();
            
            _audioSystem.PlayMainMenuMelody();
        }
        
        public async UniTask Exit()
        {
            if (_mainMenuStateView.isActiveAndEnabled)
                await _mainMenuStateView.Hide();
        }

        public void HideStateInstantly() => _mainMenuStateView.HideInstantly();

        public void Dispose()
        {
            _disposables?.Dispose();
            _mainMenuStateView?.Dispose();
        }

        private void OnConverterCommand() => _openNewModuleCommand.Execute(ModulesMap.Converter);
        private void OnTicTacCommand() => _openNewModuleCommand.Execute(ModulesMap.TicTac);
        private void OnGameCommand() => _openNewModuleCommand.Execute(ModulesMap.Game);
        private async void OnOpenSettingsCommand() => await _mainMenuModuleModel.ChangeState(MainMenuTriggers.OpenSettings);
    }
}

