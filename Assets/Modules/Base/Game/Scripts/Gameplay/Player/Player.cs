using CodeBase.Services.Input;
using UnityEngine;
using VContainer;

namespace Modules.Base.Game.Scripts.Gameplay.Player
{
    [RequireComponent(typeof(PlayerMoveController))]
    public class Player : MonoBehaviour
    {
        private InputSystemService _inputSystemService;
        private Camera _mainCamera;
        private Transform _gameWorldTransform;
        
        public bool IsInVehicle { get; private set; }

        // Public properties for component access
        [Header("Controllers")]
        [field: SerializeField] public PlayerMoveController MoveController { get; private set; }
        [field: SerializeField] public PlayerGfx GfxManager { get; private set; }
        [field: SerializeField] public PlayerSfx SfxManager { get; private set; }

        [Inject]
        private void Construct(InputSystemService inputSystemService, Camera mainCamera)
        {
            _inputSystemService = inputSystemService;
            _mainCamera = mainCamera;
        }

        // Method to set GameWorldTransform from external source (e.g., GameManager)
        public void SetGameWorldTransform(Transform gameWorldTransform)
        {
            _gameWorldTransform = gameWorldTransform;
        }

        public void Initialize(bool isPlayerInVehicle)
        {
            MoveController.Initialize(_mainCamera);
        }

        private void Awake()
        {
            if (!MoveController)
                MoveController = GetComponent<PlayerMoveController>();

            if (!GfxManager)
                GfxManager = GetComponent<PlayerGfx>();

            if (!SfxManager)
                SfxManager = GetComponent<PlayerSfx>();

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

        private void Update()
        {
            if (IsInVehicle) return;
            
            MoveController.UpdateController();
        }

        private void LateUpdate()
        {
            if (!IsInVehicle) 
            {
                MoveController.LateUpdateController();
            }
        }

        private void EnterVehicle()
        {
            // Mock vehicle entry - not needed for basic movement
            
        }

        private void ExitVehicle()
        {
            // Mock vehicle exit - not needed for basic movement  
            
        }

        private void OnDestroy()
        {
            // Cleanup if needed
        }
    }
}