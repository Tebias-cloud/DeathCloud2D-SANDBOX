using UnityEngine;

/// <summary>
/// Maneja la zona de caída (respawn) para el jugador en el entorno de pruebas.
/// </summary>
public class FallZone : MonoBehaviour
{
    [Tooltip("El punto al que se teletransportará el jugador al caer.")]
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si el objeto que colisiona tiene el tag "Player"
        if (other.CompareTag("Player"))
        {
            if (respawnPoint != null)
            {
                // Al ser un NetworkObject con autoridad local, modificar el transform
                // en el cliente dueño es suficiente para un respawn simple.
                other.transform.position = respawnPoint.position;
            }
            else
            {
                Debug.LogWarning("FallZone: No se ha asignado un RespawnPoint en el inspector.");
            }
        }
    }
}
