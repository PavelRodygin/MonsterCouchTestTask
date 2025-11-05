using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.Game.Scripts.Gameplay.Enemy.Factory
{
    /// <summary>
    /// VContainer factory for creating enemies with dependency injection
    /// </summary>
    public class EnemyFactory : IEnemyFactory
    {
        private readonly IObjectResolver _resolver;
        private readonly GameObject _enemyPrefab;
        private readonly Camera _mainCamera;

        public EnemyFactory(IObjectResolver resolver, GameObject enemyPrefab, Camera mainCamera)
        {
            _resolver = resolver;
            _enemyPrefab = enemyPrefab;
            _mainCamera = mainCamera;
        }

        public GameObject Create(Vector3 position, Transform parent)
        {
            // Use VContainer's Instantiate method which automatically injects dependencies
            var enemyInstance = _resolver.Instantiate(_enemyPrefab, position, Quaternion.identity, parent);
            
            // Initialize enemy after all dependencies are injected
            var enemy = enemyInstance.GetComponent<Enemy>();
            
            return enemyInstance;
        }
    }
}

