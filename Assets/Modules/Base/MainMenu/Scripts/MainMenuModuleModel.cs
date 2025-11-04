using System;
using CodeBase.Core.Patterns.Architecture.MVP;
using Cysharp.Threading.Tasks;
using Stateless;
using UnityEngine;

namespace Modules.Base.MainMenu.Scripts
{
    public enum MainMenuStates { MainMenu, Settings }
    public enum MainMenuTriggers { OpenSettings, BackToMainMenu }

    public class MainMenuModuleModel : IModel
    {
        // Throttle delays for anti-spam protection
        public int CommandThrottleDelay => 300;
        public int ModuleTransitionThrottleDelay => 500;

        private readonly StateMachine<MainMenuStates, MainMenuTriggers> _stateMachine;

        public StateMachine<MainMenuStates, MainMenuTriggers> StateMachine => _stateMachine;

        public MainMenuModuleModel()
        {
            _stateMachine = new StateMachine<MainMenuStates, MainMenuTriggers>(MainMenuStates.MainMenu);
        }

        /// <summary>
        /// Configure FSM with state transition callbacks
        /// </summary>
        public void ConfigureStateMachine(
            Func<UniTask> onEnterMainMenu, Func<UniTask> onExitMainMenu,
            Func<UniTask> onEnterSettings, Func<UniTask> onExitSettings)
        {
            _stateMachine.Configure(MainMenuStates.MainMenu)
                .OnEntryAsync(async () => await onEnterMainMenu())
                .OnExitAsync(async () => await onExitMainMenu())
                .PermitReentry(MainMenuTriggers.BackToMainMenu) // Allow re-entering MainMenu for initialization
                .Permit(MainMenuTriggers.OpenSettings, MainMenuStates.Settings);

            _stateMachine.Configure(MainMenuStates.Settings)
                .OnEntryAsync(async () => await onEnterSettings())
                .OnExitAsync(async () => await onExitSettings())
                .Permit(MainMenuTriggers.BackToMainMenu, MainMenuStates.MainMenu);
        }

        /// <summary>
        /// Change menu state with validation
        /// </summary>
        public async UniTask ChangeState(MainMenuTriggers trigger)
        {
            if (!_stateMachine.CanFire(trigger))
            {
                Debug.LogWarning($"Cannot fire trigger {trigger} from state {_stateMachine.State}");
                return;
            }

            await _stateMachine.FireAsync(trigger);
        }

        public void Dispose() { }
    }
}