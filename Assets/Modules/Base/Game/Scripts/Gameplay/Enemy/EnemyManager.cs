using System.Collections.Generic;
using UnityEngine;

namespace Modules.Base.Game.Scripts.Gameplay.Enemy
{
    /// <summary>
    /// Manages spawning and updating of all enemies
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int enemyCount = 1000;
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnRadius = 10f;
        [SerializeField] private Transform spawnCenter;
        
        private readonly List<Enemy> _activeEnemies = new();
        private Transform _playerTransform;
        private Camera _mainCamera;
        private Vector2 _screenBounds;

        public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;

        private void Start()
        {
            _mainCamera = Camera.main;
            CalculateScreenBounds();
        }

        public void Initialize(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            if (enemyPrefab == null)
            {
                Debug.LogError("Enemy prefab is not assigned in EnemyManager!");
                return;
            }

            Vector3 center = spawnCenter != null ? spawnCenter.position : Vector3.zero;

            for (int i = 0; i < enemyCount; i++)
            {
                Vector2 randomPosition = GetRandomPositionInScreen();
                GameObject enemyObj = Instantiate(enemyPrefab, randomPosition, Quaternion.identity, transform);
                enemyObj.name = $"Enemy_{i}";

                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Initialize(_playerTransform);
                    _activeEnemies.Add(enemy);
                }
            }

            Debug.Log($"Spawned {_activeEnemies.Count} enemies");
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
            Debug.Log("All enemies cleared");
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

