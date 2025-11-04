using System;
using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Core.Systems;
using Cysharp.Threading.Tasks;
using R3;

namespace Modules.Base.MainMenu.Scripts.SettingsState
{
    public class SettingsStatePresenter : IPresenter
    {
        private readonly MainMenuModuleModel _mainMenuModuleModel;
        private readonly SettingsStateView _settingsStateView;
        private readonly AudioSystem _audioSystem;
        private readonly CompositeDisposable _disposables = new();

        private readonly ReactiveCommand<Unit> _backCommand = new();
        private readonly ReactiveCommand<bool> _toggleSoundCommand = new();

        public SettingsStatePresenter(MainMenuModuleModel mainMenuModuleModel, SettingsStateView settingsStateView, 
            AudioSystem audioSystem)
        {
            _mainMenuModuleModel = mainMenuModuleModel ?? throw new ArgumentNullException(nameof(mainMenuModuleModel));
            _settingsStateView = settingsStateView ?? throw new ArgumentNullException(nameof(settingsStateView));
            _audioSystem = audioSystem ?? throw new ArgumentNullException(nameof(audioSystem));
            
            SubscribeToCommands();
        }

        public async UniTask Enter(object param)
        {
            var commands = new SettingsCommands(_backCommand, _toggleSoundCommand);
            _settingsStateView.SetupEventListeners(commands);
            
            _settingsStateView.InitializeSoundToggle(isMusicOn: _audioSystem.MusicVolume != 0);
            await _settingsStateView.Show();
        }

        public async UniTask Exit()
        {
            if (_settingsStateView.isActiveAndEnabled)
                await _settingsStateView.Hide();
        }

        public void HideStateInstantly() => _settingsStateView.HideInstantly();

        public void Dispose()
        {
            _settingsStateView?.Dispose();
            _disposables?.Dispose();
        }

        private void SubscribeToCommands()
        {
            _backCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_mainMenuModuleModel.CommandThrottleDelay))
                .Subscribe(_ => OnBackButtonClicked())
                .AddTo(_disposables);
            
            _toggleSoundCommand.Subscribe(OnToggleSoundCommand).AddTo(_disposables);
        }

        private async void OnBackButtonClicked()
        {
            await _mainMenuModuleModel.ChangeState(MainMenuTriggers.BackToMainMenu);
        }

        private void OnToggleSoundCommand(bool isOn)
        {
            _audioSystem.SetMusicVolume(isOn ? 1 : 0);
        }
    }
}