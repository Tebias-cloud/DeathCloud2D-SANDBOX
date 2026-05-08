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

        private bool isPaused = false;

        private void Start()
        {
            if (pausePanel != null) pausePanel.SetActive(false);
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

        // Función para el botón "Salir al Menú" dentro de la pausa
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f; // CRÍTICO: Despausar antes de cambiar de escena
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // CRÍTICO PARA NGO: Apagar la sesión actual para poder re-entrar después
            if (Unity.Netcode.NetworkManager.Singleton != null)
            {
                Unity.Netcode.NetworkManager.Singleton.Shutdown();
            }

            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
