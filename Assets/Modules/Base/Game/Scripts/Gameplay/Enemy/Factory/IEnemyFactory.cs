using UnityEngine;

namespace Modules.Base.Game.Scripts.Gameplay.Enemy.Factory
{
    /// <summary>
    /// Factory interface for creating enemies with dependency injection
    /// </summary>
    public interface IEnemyFactory
    {
        /// <summary>
        /// Creates a new enemy instance at specified position
        /// </summary>
        /// <param name="position">Spawn position</param>
        /// <param name="parent">Parent transform</param>
        /// <returns>Enemy GameObject with injected dependencies</returns>
        GameObject Create(Vector3 position, Transform parent);
    }
}

