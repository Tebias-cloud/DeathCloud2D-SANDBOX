using UnityEngine;
using DeathCloud.Core.Combat;
using DeathCloud.Core.Management;
using DeathCloud.Core.Audio;

namespace DeathCloud.Features.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private AudioClip impactSound;
        private int currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            Debug.Log($"[PlayerHealth] Jugador recibió daño. Vida actual: {currentHealth}");

            if (impactSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(impactSound);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            Debug.Log("[PlayerHealth] El jugador ha muerto.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseGame();
            }
        }
    }
}
