using CodeBase.Services.Input;
using UnityEngine;
using VContainer;

namespace Modules.Base.Game.Scripts.Gameplay.Player
{
    public class PlayerMoveController : MonoBehaviour
    {
        [Header("2D Movement Settings")]
        [Tooltip("Move speed of the player")]
        public float moveSpeed = 5f;
        
        [Header("Screen Bounds")]
        [Tooltip("Constrain player movement to screen boundaries")]
        public bool constrainToScreen = true;

        private InputSystemService _inputSystemService;
        private Player _player;
        private Camera _mainCamera;
        private Vector2 _screenBounds;
        private Vector2 _moveInput;
        private float _speed;

        // Public property for accessing Player reference
        public Player Player => _player;
        
        // Public properties for passing data to PlayerGfx
        public float CurrentSpeed => _speed;
        public float InputMagnitude => _moveInput.magnitude;

        [Inject]
        public void Construct(InputSystemService inputSystemService, Camera mainCamera)
        {
            _inputSystemService = inputSystemService;
            _mainCamera = mainCamera;
        }

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        private void Start()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
            CalculateScreenBounds();
        }

        public void Initialize(Camera camera)
        {
            if (camera != null)
            {
                _mainCamera = camera;
            }
            CalculateScreenBounds();
        }

        public void UpdateController()
        {
            HandleInput();
            Move();
        }

        public void LateUpdateController()
        {
            // No camera updates needed for 2D
        }

        private void HandleInput()
        {
            if (_inputSystemService != null)
            {
                _moveInput = _inputSystemService.InputActions.PlayerHumanoid.Move.ReadValue<Vector2>();
                _speed = _moveInput.magnitude * moveSpeed;
            }
        }

        private void Move()
        {
            if (_moveInput == Vector2.zero) return;

            Vector3 movement = new Vector3(_moveInput.x, _moveInput.y, 0f) * moveSpeed * Time.deltaTime;
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

        private void OnDrawGizmosSelected()
        {
            if (!constrainToScreen || _mainCamera == null || _screenBounds == Vector2.zero) return;

            Gizmos.color = Color.cyan;
            float margin = 0.5f;
            float maxX = _screenBounds.x - margin;
            float maxY = _screenBounds.y - margin;
            
            Vector3 topRight = new Vector3(maxX, maxY, 0);
            Vector3 topLeft = new Vector3(-maxX, maxY, 0);
            Vector3 bottomRight = new Vector3(maxX, -maxY, 0);
            Vector3 bottomLeft = new Vector3(-maxX, -maxY, 0);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}