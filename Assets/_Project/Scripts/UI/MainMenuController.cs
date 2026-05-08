using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DeathCloud.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Navegación")]
        [SerializeField] private string gameSceneName = "SampleScene";

        [Header("Paneles (Asignar en Inspector)")]
        [SerializeField] private GameObject mainButtonsPanel;
        [SerializeField] private GameObject settingsPanel;

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            // Estado inicial limpio
            ToggleSettings(false);
        }

        public void StartGame() => SceneManager.LoadScene(gameSceneName);

        // Control maestro de ventanas
        public void ToggleSettings(bool show)
        {
            if (settingsPanel != null) settingsPanel.SetActive(show);
            if (mainButtonsPanel != null) mainButtonsPanel.SetActive(!show); // Oculta los botones principales para que no estorben
        }

        // ==========================================
        // HOOKS PREPARADOS PARA FUTURO TUNEO (UI)
        // ==========================================
        public void SetMasterVolume(float sliderValue) 
        { 
            // TODO: Conectar a AudioMixer 
            Debug.Log($"[UI] Volumen cambiado a {sliderValue}");
        }

        public void SetFullscreen(bool isFullscreen) 
        { 
            Screen.fullScreen = isFullscreen; 
            Debug.Log($"[UI] Pantalla completa: {isFullscreen}");
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
