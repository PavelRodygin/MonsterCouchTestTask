using UnityEngine;

namespace Modules.Base.Game.Scripts.Gameplay.Enemy
{
    /// <summary>
    /// Enemy that flees from the player and stops when touched
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float fleeSpeed = 3f;
        
        [Header("Screen Bounds")]
        [SerializeField] private bool constrainToScreen = true;
        
        private Transform _playerTransform;
        private Camera _mainCamera;
        private Vector2 _screenBounds;
        private bool _isStopped = false;
        private SpriteRenderer _spriteRenderer;

        public bool IsStopped => _isStopped;

        private void Start()
        {
            _mainCamera = Camera.main;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            CalculateScreenBounds();
        }

        public void Initialize(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        private void Update()
        {
            if (_isStopped || _playerTransform == null) return;

            FleeFromPlayer();
        }

        private void FleeFromPlayer()
        {
            // Always flee from player, no distance check needed
            Vector2 fleeDirection = ((Vector2)transform.position - (Vector2)_playerTransform.position).normalized;
            Vector3 movement = fleeDirection * fleeSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + movement;

            if (constrainToScreen)
            {
                newPosition = ClampToScreen(newPosition);
            }

            transform.position = newPosition;
        }

        private void CalculateScreenBounds()
        {
            if (_mainCamera == null) return;
            
            // Use camera's orthographic size to calculate bounds correctly
            float height = _mainCamera.orthographicSize;
            float width = height * _mainCamera.aspect;
            _screenBounds = new Vector2(width, height);
        }

        private Vector3 ClampToScreen(Vector3 position)
        {
            // Add small margin (0.5 units) from screen edge
            float margin = 0.5f;
            float clampedX = Mathf.Clamp(position.x, -_screenBounds.x + margin, _screenBounds.x - margin);
            float clampedY = Mathf.Clamp(position.y, -_screenBounds.y + margin, _screenBounds.y - margin);
            return new Vector3(clampedX, clampedY, position.z);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Stop();
            }
        }

        public void Stop()
        {
            _isStopped = true;
            
            // Visual feedback - change color when stopped
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.red;
            }
        }

        public void Resume()
        {
            _isStopped = false;
            
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.white;
            }
        }
    }
}

