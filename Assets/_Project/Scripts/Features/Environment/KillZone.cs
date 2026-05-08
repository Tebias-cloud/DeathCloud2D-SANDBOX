using UnityEngine;
using DeathCloud.Features.Player;

namespace DeathCloud.Features.Environment
{
    public class KillZone : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"[KillZone] Algo entró en el trigger: {other.name}");

            // Verificamos si es el jugador por Tag o por Componente
            bool isPlayer = other.CompareTag(playerTag);
            
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health == null) health = other.GetComponentInParent<PlayerHealth>();
            if (health == null) health = other.GetComponentInChildren<PlayerHealth>();

            if (isPlayer || health != null)
            {
                Debug.Log("[KillZone] ¡JUGADOR DETECTADO! Activando muerte instantánea.");
                health.Die();
            }
            else
            {
                Debug.LogWarning($"[KillZone] {other.name} no tiene el componente PlayerHealth.");
            }
        }
    }
}
