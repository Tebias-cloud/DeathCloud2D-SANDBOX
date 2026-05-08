using UnityEngine;
using UnityEngine.Events;

namespace DeathCloud.Core.Management
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;

        public UnityEvent OnGameWin;
        public UnityEvent OnGameLose;

        private bool _isGameOver = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // Seguridad: Asegurar que el tiempo no esté pausado al iniciar/reiniciar
            Time.timeScale = 1f; 
        }

        private void Start()
        {
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
            Time.timeScale = 1f;
        }

        public void WinGame()
        {
            if (_isGameOver) return;
            _isGameOver = true;

            Debug.Log("[GameManager] ¡VICTORIA!");
            Time.timeScale = 0f; // Pausar el juego
            
            if (winPanel != null) winPanel.SetActive(true);
            OnGameWin?.Invoke();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void LoseGame()
        {
            if (_isGameOver) return;
            _isGameOver = true;

            Debug.Log("[GameManager] ¡DERROTA!");
            Time.timeScale = 0f; // Pausar el juego

            if (losePanel != null) losePanel.SetActive(true);
            OnGameLose?.Invoke();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            // Si el juego terminó, permitimos reiniciar rápido con R o salir con ESC
            if (_isGameOver)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.R))
                {
                    Time.timeScale = 1f;
                    UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                }
            }
        }
    }
}
