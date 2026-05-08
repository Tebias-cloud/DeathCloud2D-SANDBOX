using UnityEngine;
using UnityEngine.SceneManagement;
using DeathCloud.Core.Audio;

namespace DeathCloud.UI
{
    public class GameUIController : MonoBehaviour
    {
        [Header("Navegación")]
        [SerializeField] private string mainMenuSceneName = "MainMenu_Scene";

        [Header("Paneles (Asignar en Inspector)")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject gameOverPanel;
        
        [Header("Audio")]
        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private AudioClip uiClickSound;

        public static GameUIController Instance { get; private set; }
        private bool isPaused = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // Asegurar que el ratón sea visible para apuntar el gancho
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (pausePanel != null) pausePanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);

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

        public void ShowVictory()
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
                Time.timeScale = 0f; 
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Debug.Log("[GameUI] Pantalla de Victoria mostrada.");
            }
            else
            {
                Debug.LogError("[GameUI] ¡ERROR! No has asignado el Victory Panel en el Inspector.");
            }
        }

        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                Time.timeScale = 0f; 
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Debug.Log("[GameUI] Pantalla de Game Over mostrada.");
            }
            else
            {
                Debug.LogError("[GameUI] ¡ERROR! No has asignado el Game Over Panel en el Inspector.");
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

        // Alias para compatibilidad con botones de la escena
        public void ReturnToMenu() => GoToMainMenu();

        private void CleanupNetwork()
        {
            if (Unity.Netcode.NetworkManager.Singleton != null && !Unity.Netcode.NetworkManager.Singleton.ShutdownInProgress)
            {
                Unity.Netcode.NetworkManager.Singleton.Shutdown();
            }

            // Destruir managers persistentes para evitar el error de "Singleton is not null"
            var persistents = FindObjectsByType<DeathCloud.Core.Network.NetworkManagerPersistent>(FindObjectsSortMode.None);
            foreach (var p in persistents)
            {
                Destroy(p.gameObject);
            }
        }
    }
}
