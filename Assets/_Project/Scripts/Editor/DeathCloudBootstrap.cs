using UnityEngine;
using UnityEditor;
using DeathCloud.Core.Input;
using DeathCloud.Player;
using DeathCloud.Player.Core;
using System.IO;

namespace DeathCloud.Editor
{
    public class DeathCloudBootstrap : EditorWindow
    {
        private const string PROJECT_PATH = "Assets/_Project";
        private const string DATA_PATH = PROJECT_PATH + "/Data";
        private const string STATS_NAME = "PlayerStats.asset";
        private const string INPUT_NAME = "InputReader.asset";

        [MenuItem("DeathCloud Tools/Setup Project & Player")]
        public static void InitializeProject()
        {
            Debug.Log("<color=cyan>🌩️ Iniciando configuración de DeathCloud...</color>");

            // 1. Asegurar carpetas
            if (!AssetDatabase.IsValidFolder(DATA_PATH))
            {
                Directory.CreateDirectory(DATA_PATH);
                AssetDatabase.Refresh();
            }

            // 2. Crear o buscar ScriptableObjects
            PlayerStatsSO stats = SetupScriptableObject<PlayerStatsSO>(DATA_PATH + "/" + STATS_NAME);
            InputReader input = SetupScriptableObject<InputReader>(DATA_PATH + "/" + INPUT_NAME);

            // 3. Configurar al Jugador en la Escena
            SetupPlayer(stats, input);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("<color=green>✅ ¡Proyecto configurado! Pulsa Play para testear.</color>");
        }

        private static T SetupScriptableObject<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                Debug.Log($"Creado Asset: {path}");
            }
            return asset;
        }

        private static void SetupPlayer(PlayerStatsSO stats, InputReader input)
        {
            GameObject playerGo = GameObject.Find("Player");
            if (playerGo == null)
            {
                playerGo = new GameObject("Player");
                Undo.RegisterCreatedObjectUndo(playerGo, "Create Player");
            }

            // Rigidbody2D
            Rigidbody2D rb = GetOrAddComponent<Rigidbody2D>(playerGo);
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.freezeRotation = true;

            // CapsuleCollider2D
            CapsuleCollider2D col = GetOrAddComponent<CapsuleCollider2D>(playerGo);
            col.size = new Vector2(0.8f, 1.8f);

            // Grapple Components
            GetOrAddComponent<DistanceJoint2D>(playerGo).enabled = false;
            LineRenderer lr = GetOrAddComponent<LineRenderer>(playerGo);
            lr.enabled = false;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.material = new Material(Shader.Find("Sprites/Default"));

            // State Machine
            PlayerStateMachine sm = GetOrAddComponent<PlayerStateMachine>(playerGo);
            
            // Usar SerializedObject para asignar campos privados serializados
            SerializedObject so = new SerializedObject(sm);
            so.FindProperty("_stats").objectReferenceValue = stats;
            so.FindProperty("_input").objectReferenceValue = input;
            so.ApplyModifiedProperties();

            Selection.activeGameObject = playerGo;
        }

        private static T GetOrAddComponent<T>(GameObject target) where T : Component
        {
            T component = target.GetComponent<T>();
            if (component == null) component = target.AddComponent<T>();
            return component;
        }
    }
}
