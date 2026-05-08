using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeathCloud.UI
{
    public class GameUIController : MonoBehaviour
    {
        [Header("Navegación")]
        [SerializeField] private string mainMenuSceneName = "MainMenu_Scene";

        [Header("Paneles (Asignar en Inspector)")]
        [SerializeField] private GameObject pausePanel;
        
        [Header("Audio")]
        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private AudioClip uiClickSound;

        private bool isPaused = false;

        private void Start()
        {
            if (pausePanel != null) pausePanel.SetActive(false);

            if (gameMusic != null && Core.Audio.AudioManager.Instance != null)
            {
                Core.Audio.AudioManager.Instance.PlayMusic(gameMusic);
            }
        }

        public void PlayClickSound()
        {
            if (uiClickSound != null && Core.Audio.AudioManager.Instance != null)
            {
                Core.Audio.AudioManager.Instance.PlaySFX(uiClickSound);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
            if (pausePanel != null) pausePanel.SetActive(isPaused);

            // Arquitectura preparada para Netcode: 
            // Si es multijugador, en el futuro aquí verificaremos si somos el Host antes de parar el tiempo.
            Time.timeScale = isPaused ? 0f : 1f; 
            
            Cursor.visible = isPaused;
            // IMPORTANTE: En juegos 2D donde apuntas con el mouse (Gancho), NUNCA uses Locked. 
            // Locked ancla el mouse al centro de la pantalla. Confined lo mantiene en la ventana.
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Confined;
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            
            // Limpieza de red antes de reiniciar para evitar conflictos de puerto
            CleanupNetwork();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
 
            CleanupNetwork();

            SceneManager.LoadScene(mainMenuSceneName);
        }

        private void CleanupNetwork()
        {
            if (Unity.Netcode.NetworkManager.Singleton != null)
            {
                var nm = Unity.Netcode.NetworkManager.Singleton;
                nm.Shutdown();
                Destroy(nm.gameObject);
            }

            var persistents = FindObjectsByType<DeathCloud.Core.Network.NetworkManagerPersistent>(FindObjectsSortMode.None);
            foreach (var p in persistents)
            {
                Destroy(p.gameObject);
            }
        }
    }
}
