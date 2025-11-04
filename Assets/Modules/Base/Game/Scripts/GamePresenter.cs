using System;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.PopupHub;
using Cysharp.Threading.Tasks;
using R3;
using Unit = R3.Unit;

namespace Modules.Base.Game.Scripts
{
    /// <summary>
    /// Presenter for Game module that handles business logic and coordinates between Model and View
    /// 
    /// IMPORTANT: This is a game file for ModuleCreator system.
    /// When creating a new module, this file will be copied and modified.
    /// 
    /// Key points for customization:
    /// 1. Change class name from GamePresenter to YourModuleNamePresenter
    /// 2. Update namespace Modules.Base.GameModule.Scripts match your module location
    /// 3. Add your specific business logic and commands
    /// 4. Customize module navigation logic
    /// 5. Implement your specific UI event handling
    /// 6. Add any additional services or systems your module needs
    /// 
    /// NOTE: Navigation to MainMenuModule is already implemented via exit button
    /// </summary>
    public class GamePresenter : IDisposable
    {
        private readonly GameModuleModel _gameModuleModel;
        private readonly GameView _gameView;
        private readonly AudioSystem _audioSystem;
        private readonly IPopupHub _popupHub;
        
        private readonly CompositeDisposable _disposables = new();
        
        private ReactiveCommand<ModulesMap> _openNewModuleCommand;
        private readonly ReactiveCommand<Unit> _openMainMenuCommand = new();
        private readonly ReactiveCommand<Unit> _settingsPopupCommand = new();
        private readonly ReactiveCommand<bool> _toggleSoundCommand = new();

        public GamePresenter(
            GameModuleModel gameModuleModel,
            GameView gameView,
            AudioSystem audioSystem,
            IPopupHub popupHub)
        {
            _gameModuleModel = gameModuleModel ?? throw new ArgumentNullException(nameof(gameModuleModel));
            _gameView = gameView ?? throw new ArgumentNullException(nameof(gameView));
            _audioSystem = audioSystem ?? throw new ArgumentNullException(nameof(audioSystem));
            _popupHub = popupHub ?? throw new ArgumentNullException(nameof(popupHub));
        }

        public async UniTask Enter(ReactiveCommand<ModulesMap> runModuleCommand)
        {
            _openNewModuleCommand = runModuleCommand ?? throw new ArgumentNullException(nameof(runModuleCommand));
            
            _gameView.HideInstantly();

            var commands = new GameCommands(
                _openMainMenuCommand,
                _settingsPopupCommand,
                _toggleSoundCommand
            );

            _gameView.SetupEventListeners(commands);
            SubscribeToUIUpdates();

            // _gameView.InitializeSoundToggle(isMusicOn: _audioSystem.MusicVolume != 0);
            await _gameView.Show();
            
            _audioSystem.PlayMainMenuMelody();
        }

        public async UniTask Exit()
        {
            await _gameView.Hide();
        }
        
        public void HideInstantly() => _gameView.HideInstantly();

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void SubscribeToUIUpdates()
        {
            _openMainMenuCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_gameModuleModel.CommandThrottleDelay))
                .Subscribe(_ => OnMainMenuButtonClicked())
                .AddTo(_disposables);

            _settingsPopupCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_gameModuleModel.CommandThrottleDelay))
                .Subscribe(_ => OnSettingsPopupButtonClicked())
                .AddTo(_disposables);

            _toggleSoundCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_gameModuleModel.CommandThrottleDelay))
                .Subscribe(OnSoundToggled)
                .AddTo(_disposables);
        }

        private void OnMainMenuButtonClicked()
        {
            _openNewModuleCommand.Execute(ModulesMap.MainMenu);
        }

        private void OnSettingsPopupButtonClicked()
        {
            _popupHub.OpenSettingsPopup();
        }

        private void OnSoundToggled(bool isOn)
        {
            _audioSystem.SetMusicVolume(isOn ? 1f : 0f);
        }
    }
}
