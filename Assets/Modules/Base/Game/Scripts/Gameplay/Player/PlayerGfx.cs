using UnityEngine;
using VContainer;

namespace Modules.Base.Game.Scripts.Gameplay.Player
{
    /// <summary>
    /// Manages player graphics and visual effects for 2D gameplay
    /// Simplified version for 2D sprite-based player (no complex animations needed)
    /// </summary>
    public class PlayerGfx : MonoBehaviour
    {
        private PlayerMoveController _moveController;
        private Player _player;
        private SpriteRenderer _spriteRenderer;
        
        [Header("Visual Settings")]
        [SerializeField] private bool enableVisualEffects = false;

        private void Awake()
        {
            _moveController = GetComponent<PlayerMoveController>();
            _player = GetComponent<Player>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (!_moveController) 
                Debug.LogWarning("PlayerMoveController not found on the same GameObject!");
            
            if (!_player) 
                Debug.LogWarning("Player not found on the same GameObject!");
        }

        private void Update()
        {
            if (enableVisualEffects)
            {
                UpdateVisualEffects();
            }
        }

        /// <summary>
        /// Updates visual effects based on player state
        /// Can be extended for sprite animations, particles, etc.
        /// </summary>
        private void UpdateVisualEffects()
        {
            if (!_moveController || !_spriteRenderer) return;

            // Example: Could add sprite flipping based on movement direction
            // Example: Could add color tinting based on speed
            // Example: Could add particle effects
            
            // For now, this is a placeholder for future visual enhancements
        }
    }
}