using System.Collections.Generic;
using Modules.Base.Game.Scripts.Gameplay.Enemy.Factory;
using UnityEngine;
using VContainer;

namespace Modules.Base.Game.Scripts.Gameplay.Enemy
{
    /// <summary>
    /// Manages spawning and updating of all enemies using Factory pattern
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private int enemyCount = 1000;
        [SerializeField] private int maxEnemiesUpdatedPerFrame = 250;
        [SerializeField] private float colliderActivationRadius = 1.2f; // enable triggers near player
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnRadius = 10f;
        [SerializeField] private Transform spawnCenter;
        
        private readonly List<Enemy> _activeEnemies = new();
        private Transform _playerTransform;
        private Transform _spawnParent;
        private IEnemyFactory _enemyFactory;
        private Camera _mainCamera;
        private Vector2 _screenBounds;
        private int _updateCursor;

        [Inject]
        public void Construct(IEnemyFactory enemyFactory, Camera mainCamera)
        {
            _enemyFactory = enemyFactory;
            _mainCamera = mainCamera;
        }

        public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;

        private void Start()
        {
            _mainCamera = Camera.main;
            CalculateScreenBounds();
        }

        public void Initialize(Transform playerTransform, Transform spawnParent)
        {
            _playerTransform = playerTransform;
            _spawnParent = spawnParent;
            
            // Ensure camera and bounds are initialized before spawning
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
            
            CalculateScreenBounds();
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            if (_enemyFactory == null)
            {
                return;
            }

            if (_playerTransform == null)
            {
                return;
            }

            for (int i = 0; i < enemyCount; i++)
            {
                Vector2 randomPosition2D = GetRandomPositionInScreen();
                // Set Z to 0.1 to be slightly in front of background but behind UI
                Vector3 spawnPosition = new Vector3(randomPosition2D.x, randomPosition2D.y, 0.1f);
                
                // Create enemy using factory (with DI)
                GameObject enemyObj = _enemyFactory.Create(spawnPosition, _spawnParent);
                enemyObj.name = $"Enemy_{i}";

                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Ensure enemy is manager-driven for performance
                    // (Enemy has a managedByManager flag; we drive via Tick regardless)
                    enemy.Initialize(_playerTransform);
                    _activeEnemies.Add(enemy);
                }
            }
        }

        private Vector2 GetRandomPositionInScreen()
        {
            if (_mainCamera == null)
            {
                return Random.insideUnitCircle * spawnRadius;
            }

            float x = Random.Range(-_screenBounds.x * 0.9f, _screenBounds.x * 0.9f);
            float y = Random.Range(-_screenBounds.y * 0.9f, _screenBounds.y * 0.9f);
            
            return new Vector2(x, y);
        }

        private void CalculateScreenBounds()
        {
            if (_mainCamera == null) return;
            
            // Use camera's orthographic size to calculate bounds correctly
            float height = _mainCamera.orthographicSize;
            float width = height * _mainCamera.aspect;
            _screenBounds = new Vector2(width, height);
        }

        public void ClearEnemies()
        {
            foreach (var enemy in _activeEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
            _activeEnemies.Clear();
            // Enemies cleared
        }

        private void Update()
        {
            if (_playerTransform == null || _activeEnemies.Count == 0)
                return;

            // Tick a subset each frame to reduce CPU spikes
            int count = _activeEnemies.Count;
            int toUpdate = Mathf.Min(maxEnemiesUpdatedPerFrame, count);
            Vector2 playerPos = _playerTransform.position;
            float dt = Time.deltaTime;

            for (int i = 0; i < toUpdate; i++)
            {
                int index = (_updateCursor + i) % count;
                Enemy enemy = _activeEnemies[index];
                if (enemy != null)
                {
                    enemy.Tick(playerPos, dt, _screenBounds);
                }
            }

            _updateCursor = (_updateCursor + toUpdate) % count;

            // Manage trigger activation for collisions near player
            float activationRadiusSqr = colliderActivationRadius * colliderActivationRadius;
            for (int i = 0; i < count; i++)
            {
                Enemy enemy = _activeEnemies[i];
                if (enemy == null) continue;
                Vector2 pos = enemy.transform.position;
                bool shouldEnable = (pos - playerPos).sqrMagnitude <= activationRadiusSqr;
                enemy.SetTriggerActive(shouldEnable);
            }
        }

        private void OnDestroy()
        {
            ClearEnemies();
        }

        private void OnDrawGizmosSelected()
        {
            if (spawnCenter == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnCenter.position, spawnRadius);
        }
    }
}

