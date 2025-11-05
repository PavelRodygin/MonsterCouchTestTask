using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

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
        
        [Header("Update Mode")]
        [SerializeField] private bool managedByManager = true;
        
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D triggerCollider;
        
        private Transform _playerTransform;
        private Camera _mainCamera;
        private Vector2 _screenBounds;
        private bool _isStopped;
        private bool _isInitialized;

        public bool IsStopped => _isStopped;

        [Inject]
        public void Construct(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        private void Awake()
        {
            // Fallback if DI didn't work
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            // Cache collider if not set
            if (triggerCollider == null)
                triggerCollider = GetComponent<Collider2D>();

            // Ensure Rigidbody2D simulation is enabled (silent)
            var rb2D = GetComponent<Rigidbody2D>();
            if (rb2D != null)
            {
                if (!rb2D.simulated)
                {
                    rb2D.simulated = true;
                    rb2D.gravityScale = 0f;
                    rb2D.bodyType = RigidbodyType2D.Kinematic;
                }
            }
        }

        private void Start()
        {
            CalculateScreenBounds();
            _isInitialized = true;
        }

        public void Initialize(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            
            // Force bounds calculation
            if (_mainCamera != null)
            {
                CalculateScreenBounds();
            }
        }

        private void Update()
        {
            if (managedByManager)
                return;

            if (!_isInitialized || _isStopped || _playerTransform == null)
                return;

            if (_screenBounds == Vector2.zero && _mainCamera != null)
                CalculateScreenBounds();

            if (_screenBounds == Vector2.zero)
                return;

            FleeFromPlayerInternal(_playerTransform.position, Time.deltaTime, _screenBounds);
        }

        public void Tick(Vector2 playerPosition, float deltaTime, Vector2 screenBounds)
        {
            if (!_isInitialized || _isStopped)
                return;

            FleeFromPlayerInternal(playerPosition, deltaTime, screenBounds);
        }

        private void FleeFromPlayerInternal(Vector2 playerPosition, float deltaTime, Vector2 screenBounds)
        {
            Vector2 fleeDirection = ((Vector2)transform.position - playerPosition).normalized;
            if (fleeDirection == Vector2.zero)
                fleeDirection = Random.insideUnitCircle.normalized;

            Vector3 movement = (Vector3)(fleeDirection * fleeSpeed * deltaTime);
            Vector3 newPosition = transform.position + movement;

            if (constrainToScreen)
                newPosition = ClampToScreen(newPosition, screenBounds);

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

        private Vector3 ClampToScreen(Vector3 position, Vector2 screenBounds)
        {
            float margin = 0.5f;
            float clampedX = Mathf.Clamp(position.x, -screenBounds.x + margin, screenBounds.x - margin);
            float clampedY = Mathf.Clamp(position.y, -screenBounds.y + margin, screenBounds.y - margin);
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
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.red;
            }
        }

        public void Resume()
        {
            _isStopped = false;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
        }

        public void SetTriggerActive(bool active)
        {
            if (triggerCollider != null)
            {
                triggerCollider.enabled = active;
            }
        }
    }
}

