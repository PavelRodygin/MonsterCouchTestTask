using CodeBase.Core.UI.Views;
using CodeBase.Services.Input;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Modules.Base.MainMenu.Scripts.SettingsState
{
    public readonly struct SettingsCommands
    {
        public readonly ReactiveCommand<Unit> BackCommand;
        public readonly ReactiveCommand<bool> SoundToggleCommand;

        public SettingsCommands(ReactiveCommand<Unit> backCommand, ReactiveCommand<bool> soundToggleCommand)
        {
            BackCommand = backCommand;
            SoundToggleCommand = soundToggleCommand;
        }
    }
    
    public class SettingsStateView : BaseView
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Toggle musicToggle;

        private InputSystemService _inputSystemService;
        
        [Inject]
        private void Construct(InputSystemService inputSystemService)
        {
            _inputSystemService = inputSystemService;   
        }
        
        protected override void Awake()
        {
            base.Awake();
            
            #if UNITY_EDITOR
            ValidateUIElements();
            #endif
        }

        public void SetupEventListeners(SettingsCommands commands)
        {
            backButton.OnClickAsObservable()
                .Where(_ => IsActive)
                .Subscribe(_ => commands.BackCommand.Execute(default))
                .AddTo(this);

            musicToggle.OnValueChangedAsObservable()
                .Where(_ => IsActive)
                .Subscribe(_ => commands.SoundToggleCommand.Execute(musicToggle.isOn))
                .AddTo(this);
        }

        public override async UniTask Show()
        {
            _inputSystemService.SetFirstSelectedObject(backButton);
            await base.Show();
        }

        public void InitializeSoundToggle(bool isMusicOn) => musicToggle.SetIsOnWithoutNotify(isMusicOn);

        private void ValidateUIElements()
        {
            if (backButton == null) Debug.LogError($"{nameof(backButton)} is not assigned in {nameof(SettingsStateView)}");
            if (musicToggle == null) Debug.LogError($"{nameof(musicToggle)} is not assigned in {nameof(SettingsStateView)}");
        }
    }
}