using UnityEngine;
using DeathCloud.Core.Combat;
using DeathCloud.Core.Management;

namespace DeathCloud.Features.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            Debug.Log($"[PlayerHealth] Jugador recibió daño. Vida actual: {currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("[PlayerHealth] El jugador ha muerto.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseGame();
            }
        }
    }
}
