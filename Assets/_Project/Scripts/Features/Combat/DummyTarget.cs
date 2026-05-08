using UnityEngine;
using System.Collections;
using DeathCloud.Core.Combat;
using DeathCloud.Core.Management;
using DeathCloud.Core.Audio;

namespace DeathCloud.Features.Combat
{
    public class DummyTarget : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 30;
        [SerializeField] private AudioClip impactSound;
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
            
            if (impactSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(impactSound);
            }

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

        public void ApplyStun(float duration)
        {
            Debug.Log($"[DummyTarget] {gameObject.name} aturdido por {duration}s.");
            StartCoroutine(StunRoutine(duration));
        }

        private IEnumerator StunRoutine(float duration)
        {
            // feedback visual de aturdimiento (opcional: podrías ponerlo azul o gris)
            Color stunnedColor = new Color(0.5f, 0.5f, 1f); // Azul claro
            if (_sprite != null) _sprite.color = stunnedColor;

            // Aquí podrías desactivar componentes de movimiento del enemigo
            yield return new WaitForSeconds(duration);

            if (_sprite != null) _sprite.color = _originalColor;
        }
    }
}
