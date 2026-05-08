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
            if (DeathCloud.UI.GameUIController.Instance != null)
            {
                DeathCloud.UI.GameUIController.Instance.ShowGameOver();
            }
            else if (respawnPoint != null)
            {
                other.transform.position = respawnPoint.position;
            }
        }
    }
}
