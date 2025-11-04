using CodeBase.Core.UI.Views;
using CodeBase.Services.Input;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;

namespace Modules.Base.MainMenu.Scripts.MainMenuState
{
    public readonly struct MainMenuCommands
    {
        public readonly ReactiveCommand<Unit> OpenGameCommand;
        public readonly ReactiveCommand<Unit> OpenSettingsCommand;

        public MainMenuCommands(
            ReactiveCommand<Unit> openGameCommand,
            ReactiveCommand<Unit> openSettingsCommand)
        {
            OpenGameCommand = openGameCommand;
            OpenSettingsCommand = openSettingsCommand;
        }
    }
    
    public class MainMenuStateView : BaseView
    {
        [SerializeField] private Button openGameButton;
        [SerializeField] private Button settingsButton;

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

        public void SetupEventListeners(MainMenuCommands commands)
        {
            openGameButton.OnClickAsObservable()
                .Where(_ => IsActive)
                .Subscribe(_ => commands.OpenGameCommand.Execute(default))
                .AddTo(this);

            settingsButton.OnClickAsObservable()
                .Where(_ => IsActive)
                .Subscribe(_ => commands.OpenSettingsCommand.Execute(default))
                .AddTo(this);
        }

        public override async UniTask Show()
        {
            OnScreenEnabled();
            await base.Show();
        }

        public void OnScreenEnabled()
        {
            _inputSystemService.SetFirstSelectedObject(openGameButton);
        }

        private void ValidateUIElements()
        {
            if (settingsButton == null) Debug.LogError($"{nameof(settingsButton)} is not assigned in {nameof(MainMenuStateView)}");
            if (openGameButton == null) Debug.LogError($"{nameof(openGameButton)} is not assigned in {nameof(MainMenuStateView)}");
        }
    }
}

