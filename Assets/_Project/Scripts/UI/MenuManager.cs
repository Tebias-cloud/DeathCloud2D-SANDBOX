using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DeathCloud.Core.Audio;

namespace DeathCloud.UI
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Navegación")]
        [SerializeField] private string gameSceneName = "SampleScene";

        [Header("Paneles (Asignar en Inspector)")]
        [SerializeField] private GameObject mainButtonsPanel;
        [SerializeField] private GameObject settingsPanel;
        
        [Header("Audio")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip uiClickSound;

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (menuMusic != null && AudioManager.Instance != null)
            {
                Core.Audio.AudioManager.Instance.PlayMusic(menuMusic);
            }
        }

        public void PlayClickSound()
        {
            if (uiClickSound != null && Core.Audio.AudioManager.Instance != null)
            {
                Core.Audio.AudioManager.Instance.PlaySFX(uiClickSound);
            }
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
            Debug.Log("[UI] Saliendo del juego...");
            Application.Quit();
        }

        public void OnVolumeChanged(float volume)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMasterVolume(volume);
        }

        public void OnMuteToggled(bool isMuted)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.MuteAll(isMuted);
        }
    }
}
