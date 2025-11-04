using System;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Infrastructure.Modules;
using Cysharp.Threading.Tasks;
using Modules.Base.GameModule.Scripts.Gameplay.Systems;
using Modules.Base.Game.Scripts.Gameplay.Player;
using Modules.Base.Game.Scripts.Gameplay.Player.Factory;
using R3;
using UnityEngine;
using VContainer;

namespace Modules.Base.Game.Scripts
{
    public class GameModuleController : IModuleController
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly GameModuleModel _gameModuleModel;
        private readonly GamePresenter _gamePresenter;
        
        private readonly GameManager _gameManager;
        
        private readonly ReactiveCommand<ModulesMap> _openNewModuleCommand = new();
        private readonly UniTaskCompletionSource _moduleCompletionSource;

        private readonly CompositeDisposable _disposables = new();
        
        public GameModuleController(IScreenStateMachine screenStateMachine, 
            GameModuleModel gameModuleModel, GamePresenter gamePresenter, GameManager gameManager)
        {
            _gameModuleModel = gameModuleModel ?? throw new ArgumentNullException(nameof(gameModuleModel));
            _gamePresenter = gamePresenter ?? throw new ArgumentNullException(nameof(gamePresenter));
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            _screenStateMachine = screenStateMachine ?? throw new ArgumentNullException(nameof(screenStateMachine));
            
            _moduleCompletionSource = new UniTaskCompletionSource();
        }

        public async UniTask Enter(object param)
        {
            SubscribeToModuleUpdates();

            _gamePresenter.HideInstantly();
            
            // Start game - GameManager will handle player spawning and game logic
            _gameManager.StartGame();
            
            await _gamePresenter.Enter(_openNewModuleCommand);
        }

        public async UniTask Execute() => await _moduleCompletionSource.Task;

        public async UniTask Exit()
        {
            // End game - GameManager will clean up players
            _gameManager.EndGame();
            
            await _gamePresenter.Exit();
        }

        public void Dispose()
        {
            _disposables.Dispose();
            
            _gamePresenter.Dispose();
            
            _gameModuleModel.Dispose();
        }

        private void SubscribeToModuleUpdates()
        {
            // Prevent rapid module switching
            _openNewModuleCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_gameModuleModel.ModuleTransitionThrottleDelay))
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
