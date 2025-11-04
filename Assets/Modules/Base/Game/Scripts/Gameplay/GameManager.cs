using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Services.Input;
using Modules.Base.Game.Scripts.Gameplay.Player;
using Modules.Base.Game.Scripts.Gameplay.Player.Factory;
using Modules.Base.Game.Scripts.Gameplay.Enemy;
using UnityEngine;
using VContainer;

namespace Modules.Base.GameModule.Scripts.Gameplay.Systems
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game World Settings")]
        [SerializeField] private Transform gameWorldTransform;
        
        [Header("2D Player Settings")]
        [SerializeField] private Vector3 playerStartPosition = Vector3.zero;
        
        [Header("Enemy Settings")]
        [SerializeField] private EnemyManager enemyManager;
        
        private InputSystemService _inputSystemService;
        private IPlayerFactory _playerFactory;
        private GameObject _activePlayer;
        
        public Transform GameWorldTransform => gameWorldTransform;
        public GameObject ActivePlayer => _activePlayer;
        public bool HasActivePlayer => _activePlayer != null;
        
        public event Action OnReturnToMenu;
        
        [Inject]
        private void Construct(InputSystemService inputSystemService, IPlayerFactory playerFactory)
        {
            _inputSystemService = inputSystemService;
            _playerFactory = playerFactory;
        }
        
        private void Update()
        {
            if (_inputSystemService != null && _inputSystemService.InputActions.UI.Cancel.triggered)
            {
                ReturnToMenu();
            }
        }
        
        public void StartGame()
        {
            if (!ValidateConfiguration())
                return;

            InitializeGameInput();
            SpawnPlayer2D();
            
            if (enemyManager != null && _activePlayer != null)
            {
                enemyManager.Initialize(_activePlayer.transform);
            }
            
            Debug.Log("2D Game started successfully!");
        }

        public void EndGame()
        {
            DestroyPlayer();
            
            if (enemyManager != null)
            {
                enemyManager.ClearEnemies();
            }
            
            Debug.Log("Game ended!");
        }
        
        private void ReturnToMenu()
        {
            Debug.Log("Returning to menu...");
            OnReturnToMenu?.Invoke();
        }

        /// <summary>
        /// Spawns 2D player at start position
        /// </summary>
        private void SpawnPlayer2D()
        {
            if (_playerFactory == null)
            {
                Debug.LogError("PlayerFactory is not injected!");
                return;
            }

            // Create player as child of GameWorld transform
            Transform parentTransform = gameWorldTransform != null ? gameWorldTransform : transform;
            _activePlayer = _playerFactory.Create(playerStartPosition, Quaternion.identity, parentTransform);
            
            if (_activePlayer != null)
            {
                _activePlayer.name = "Player";
                
                // Set up player in game world
                var playerComponent = _activePlayer.GetComponent<Player>();
                if (playerComponent != null)
                {
                    playerComponent.SetGameWorldTransform(gameWorldTransform);
                }
                
                Debug.Log($"2D Player spawned successfully in parent: {parentTransform.name}");
            }
        }

        /// <summary>
        /// Destroys the active player
        /// </summary>
        private void DestroyPlayer()
        {
            if (_activePlayer != null)
            {
                Destroy(_activePlayer);
                _activePlayer = null;
                Debug.Log("Player destroyed");
            }
        }

        private bool ValidateConfiguration()
        {
            if (_playerFactory == null)
            {
                Debug.LogError("PlayerFactory is not injected in GameManager!");
                return false;
            }

            if (enemyManager == null)
            {
                Debug.LogError("EnemyManager is not assigned in GameManager!");
                return false;
            }

            return true;
        }

        private void InitializeGameInput()
        {
            _inputSystemService?.SwitchToPlayerHumanoid();
        }

        private void OnDestroy()
        {
            DestroyPlayer();
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(playerStartPosition, 0.3f);
            UnityEditor.Handles.Label(playerStartPosition + Vector3.up * 0.5f, "Player Start");
        }
        #endif
    }
}