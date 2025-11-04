using CodeBase.Services.Input;
using UnityEngine;
using VContainer;

namespace Modules.Base.Game.Scripts.Gameplay.Player
{
    [RequireComponent(typeof(PlayerInteractionController))]
    [RequireComponent(typeof(PlayerMoveController))]
    [RequireComponent(typeof(PlayerGfx))]
    [RequireComponent(typeof(PlayerSfx))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform playerCameraTransform;
        [SerializeField] private CharacterController characterController;

        private InputSystemService _inputSystemService;
        private Transform _gameWorldTransform;
        private Vector3 _originalPosition;
        
        public bool IsInVehicle { get; private set; }

        // Public properties for component access
        [Header("Controllers")]
        [field: SerializeField] public PlayerMoveController MoveController { get; private set; }
        [field: SerializeField] public PlayerInteractionController InteractionController { get; private set; }
        [field: SerializeField] public PlayerGfx GfxManager { get; private set; }
        [field: SerializeField] public PlayerSfx SfxManager { get; private set; }

        [Inject]
        private void Construct(InputSystemService inputSystemService)
        {
            _inputSystemService = inputSystemService;
            
            // playerInteractionController.OnEnterVehicle += EnterVehicle;
            // playerInteractionController.OnExitVehicle += ExitVehicle;
        }

        // Method to set GameWorldTransform from external source (e.g., GameManager)
        public void SetGameWorldTransform(Transform gameWorldTransform)
        {
            _gameWorldTransform = gameWorldTransform;
        }

        public void Initialize(bool isPlayerInVehicle)
        {
            // playerInteractionController.Initialize(playerCameraTransform.transform, isPlayerInVehicle);
            MoveController.Initialize(playerCameraTransform, isPlayerInVehicle);
        }

        private void Awake()
        {
            if (!MoveController)
                MoveController = GetComponent<PlayerMoveController>();

            if (!InteractionController)
                InteractionController = GetComponent<PlayerInteractionController>();

            if (!GfxManager)
                GfxManager = GetComponent<PlayerGfx>();

            if (!SfxManager)
                SfxManager = GetComponent<PlayerSfx>();
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
            Debug.Log("[Mock] Player entered vehicle");
        }

        private void ExitVehicle()
        {
            // Mock vehicle exit - not needed for basic movement  
            Debug.Log("[Mock] Player exited vehicle");
        }

        private void OnDestroy()
        {
            // playerInteractionController.OnEnterVehicle -= EnterVehicle;
            // playerInteractionController.OnExitVehicle -= ExitVehicle;
        }
    }
}