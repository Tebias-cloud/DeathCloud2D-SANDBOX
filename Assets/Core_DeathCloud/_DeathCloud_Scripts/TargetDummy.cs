using UnityEngine;
using System.Collections;

public class TargetDummy : MonoBehaviour
{
    [Header("Estadísticas")]
    public float maxHealth = 1000000f; // Casi inmortal, como el del LoL
    private float currentHealth;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    // Esta función la llamará tu espada cuando te golpee
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        
        // Imprimimos el log para ver los números exactos
        Debug.Log($"<color=orange>[DUMMY]</color> Recibió {damageAmount} de daño. HP: {currentHealth}");

        // Feedback visual de impacto
        StartCoroutine(DamageFeedback());
    }

    private IEnumerator DamageFeedback()
    {
        if (spriteRenderer != null)
        {
            // Parpadeo rojo y un pequeño encogimiento (Squash)
            spriteRenderer.color = Color.red;
            transform.localScale = new Vector3(0.9f, 1.1f, 1f); 
            
            yield return new WaitForSeconds(0.1f);
            
            // Vuelve a la normalidad
            spriteRenderer.color = originalColor;
            transform.localScale = Vector3.one;
        }
    }
}