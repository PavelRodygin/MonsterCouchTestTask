using UnityEngine;
using UnityEditor;
using Modules.Base.Game.Scripts.Gameplay.Player;
using Modules.Base.Game.Scripts.Gameplay.Enemy;

namespace Modules.Base.Game.Scripts.Editor
{
    /// <summary>
    /// Editor utility to create 2D game prefabs (Player and Enemy with Circle sprites)
    /// </summary>
    public class GamePrefabCreator : EditorWindow
    {
        private const string PREFABS_PATH = "Assets/Modules/Base/Game/Prefabs/";
        private const string PLAYER_PREFAB_NAME = "Player2D.prefab";
        private const string ENEMY_PREFAB_NAME = "Enemy2D.prefab";
        
        [MenuItem("Tools/Game Module/Create 2D Prefabs")]
        public static void ShowWindow()
        {
            GetWindow<GamePrefabCreator>("Create 2D Prefabs");
        }

        private void OnGUI()
        {
            GUILayout.Label("2D Game Prefab Creator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "This will create Player2D and Enemy2D prefabs with:\n" +
                "- Circle sprite\n" +
                "- Scale (0.2, 0.2, 0.2)\n" +
                "- Proper 2D components\n" +
                "- Required scripts",
                MessageType.Info);

            GUILayout.Space(10);

            if (GUILayout.Button("Create Player Prefab", GUILayout.Height(30)))
            {
                CreatePlayerPrefab();
            }

            if (GUILayout.Button("Create Enemy Prefab", GUILayout.Height(30)))
            {
                CreateEnemyPrefab();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Create Both Prefabs", GUILayout.Height(40)))
            {
                CreatePlayerPrefab();
                CreateEnemyPrefab();
            }
        }

        private static void CreatePlayerPrefab()
        {
            GameObject player = new GameObject("Player2D");
            
            // Add sprite renderer
            SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            spriteRenderer.color = Color.blue;
            
            // Set scale
            player.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            
            // Add 2D collider
            CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            
            // Add rigidbody2D for physics
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            
            // Add Player component (main controller) and its sub-controllers
            player.AddComponent<Player>();
            player.AddComponent<PlayerMoveController>();
            player.AddComponent<PlayerGfx>();
            player.AddComponent<PlayerSfx>();
            
            // Set tag
            player.tag = "Player";
            
            // Save as prefab
            string prefabPath = PREFABS_PATH + PLAYER_PREFAB_NAME;
            EnsureDirectoryExists(PREFABS_PATH);
            
            PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
            DestroyImmediate(player);
            
            Debug.Log($"Player prefab created at: {prefabPath}");
            EditorUtility.DisplayDialog("Success", "Player2D prefab created successfully!", "OK");
        }

        private static void CreateEnemyPrefab()
        {
            GameObject enemy = new GameObject("Enemy2D");
            
            // Add sprite renderer
            SpriteRenderer spriteRenderer = enemy.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            spriteRenderer.color = Color.white;
            
            // Set scale
            enemy.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            
            // Add 2D collider
            CircleCollider2D collider = enemy.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = true;
            
            // Add rigidbody2D
            Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            
            // Add enemy script
            enemy.AddComponent<Enemy>();
            
            // Set tag
            enemy.tag = "Enemy";
            
            // Save as prefab
            string prefabPath = PREFABS_PATH + ENEMY_PREFAB_NAME;
            EnsureDirectoryExists(PREFABS_PATH);
            
            PrefabUtility.SaveAsPrefabAsset(enemy, prefabPath);
            DestroyImmediate(enemy);
            
            Debug.Log($"Enemy prefab created at: {prefabPath}");
            EditorUtility.DisplayDialog("Success", "Enemy2D prefab created successfully!", "OK");
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path.TrimEnd('/')))
            {
                string parentPath = System.IO.Path.GetDirectoryName(path.TrimEnd('/')).Replace('\\', '/');
                string folderName = System.IO.Path.GetFileName(path.TrimEnd('/'));
                
                if (!string.IsNullOrEmpty(parentPath) && !string.IsNullOrEmpty(folderName))
                {
                    if (!AssetDatabase.IsValidFolder(parentPath))
                    {
                        EnsureDirectoryExists(parentPath);
                    }
                    AssetDatabase.CreateFolder(parentPath, folderName);
                }
            }
        }
    }
}

