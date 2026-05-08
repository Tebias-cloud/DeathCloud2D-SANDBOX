using UnityEngine;
using System.Collections;
using DeathCloud.Core.Combat;
using DeathCloud.Core.Management;

namespace DeathCloud.Features.Combat
{
    public class DummyTarget : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 30;
        private int currentHealth;

        private SpriteRenderer _sprite;
        private Color _originalColor;

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            if (_sprite != null) _originalColor = _sprite.color;
            currentHealth = maxHealth;
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            Debug.Log($"[DummyTarget] ¡Recibí {amount} de daño! Vida restante: {currentHealth}");
            
            StartCoroutine(FlashRed());

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("[DummyTarget] Objetivo destruido. ¡Victoria!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.WinGame();
            }
            Destroy(gameObject);
        }

        private IEnumerator FlashRed()
        {
            if (_sprite == null) yield break;

            _sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            _sprite.color = _originalColor;
        }
    }
}
