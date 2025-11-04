using UnityEngine;

namespace Modules.Base.Game.Scripts.Gameplay
{
    /// <summary>
    /// Sets up camera for 2D gameplay with green background
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraSetup : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Vector3 cameraPosition = new Vector3(0, 0, -10);
        [SerializeField] private Color backgroundColor = Color.green;
        [SerializeField] private bool orthographic = true;
        [SerializeField] private float orthographicSize = 5f;

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            SetupCamera();
        }

        private void SetupCamera()
        {
            if (_camera == null) return;

            transform.position = cameraPosition;
            _camera.backgroundColor = backgroundColor;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.orthographic = orthographic;
            
            if (orthographic)
            {
                _camera.orthographicSize = orthographicSize;
            }

            Debug.Log($"Camera setup complete: Position {cameraPosition}, Background {backgroundColor}");
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();

            SetupCamera();
        }
        #endif
    }
}

