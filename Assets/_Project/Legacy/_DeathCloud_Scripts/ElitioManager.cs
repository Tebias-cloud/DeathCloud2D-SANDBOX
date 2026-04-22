using UnityEngine;
using UnityEngine.Events;

public class ElitioManager : MonoBehaviour
{
    // Patrón Singleton para acceso fácil
    public static ElitioManager Instance { get; private set; }

    [Header("Estadísticas")]
    [SerializeField] private float maxElitio = 100f;
    [SerializeField] private float currentElitio = 0f;

    // Eventos para que la interfaz (UI) se entere sin preguntar cada frame
    public UnityEvent<float> OnElitioChanged; // Pasa el porcentaje (0-1)

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;
    }

    private void Start()
    {
        currentElitio = maxElitio; // Empezamos llenos para testear
        UpdateUI();
    }

    // Gasta el recurso. Devuelve true si pudo, false si no tenía suficiente.
    public bool ConsumeElitio(float amount)
    {
        if (currentElitio >= amount)
        {
            currentElitio -= amount;
            UpdateUI();
            Debug.Log($"Elitio gastado. Queda: {currentElitio}");
            return true;
        }
        
        Debug.Log("No hay suficiente Elitio");
        return false;
    }

    // Los enemigos y objetos llaman a esto al morir/romperse
    public void AddElitio(float amount)
    {
        currentElitio = Mathf.Clamp(currentElitio + amount, 0, maxElitio);
        UpdateUI();
        Debug.Log($"Elitio recogido. Total: {currentElitio}");
    }

    private void UpdateUI()
    {
        OnElitioChanged.Invoke(currentElitio / maxElitio);
    }
}