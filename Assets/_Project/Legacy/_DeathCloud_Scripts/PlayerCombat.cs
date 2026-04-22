using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Arma Actual")]
    public WeaponDataSO currentWeapon; 

    [Header("Ajustes de la Hitbox")]
    public Transform attackPoint; 
    public LayerMask enemyLayers; 

    [Header("Visual del Ataque")]
    public GameObject swordTrailParent; // ¡AQUÍ ESTÁ LA VARIABLE QUE BUSCAMOS!

    [Header("Feedback Visual Jugador")]
    public SpriteRenderer playerSprite; 
    public Color specialAuraColor = Color.cyan; 
    private Color originalColor;

    private bool isSpecialModeActive = false;
    private float nextAttackTime = 0f;

    void Start()
    {
        if (playerSprite != null) originalColor = playerSprite.color;
        if (swordTrailParent != null) swordTrailParent.SetActive(false);
    }

    void Update()
    {
        if (currentWeapon == null) return;

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            PerformAttack();
        }

        if (Input.GetKeyDown(KeyCode.E) && !isSpecialModeActive)
        {
            TryActivateSpecialPower();
        }
    }

    void PerformAttack()
    {
        nextAttackTime = Time.time + currentWeapon.attackRate;

        float finalDamage = currentWeapon.damage;
        if (isSpecialModeActive) finalDamage *= currentWeapon.specialDamageMultiplier;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, currentWeapon.attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            TargetDummy dummy = enemy.GetComponent<TargetDummy>();
            if (dummy != null) dummy.TakeDamage(finalDamage);
        }

        if (swordTrailParent != null)
        {
            StopAllCoroutines(); 
            StartCoroutine(SwordVisualRoutine());
            StartCoroutine(PlayerFlashRoutine()); 
        }
    }

    private IEnumerator SwordVisualRoutine()
    {
        swordTrailParent.SetActive(true);
        yield return new WaitForSeconds(currentWeapon.attackRate * 0.25f); 
        swordTrailParent.SetActive(false);
    }

    private IEnumerator PlayerFlashRoutine()
    {
        if (playerSprite != null)
        {
            playerSprite.color = Color.white;
            yield return new WaitForSeconds(0.05f);
            playerSprite.color = isSpecialModeActive ? specialAuraColor : originalColor;
        }
    }

    void TryActivateSpecialPower()
    {
        if (ElitioManager.Instance != null && ElitioManager.Instance.ConsumeElitio(currentWeapon.elitioCost))
        {
            StartCoroutine(SpecialPowerRoutine());
        }
    }

    private IEnumerator SpecialPowerRoutine()
    {
        isSpecialModeActive = true;
        if (playerSprite != null) playerSprite.color = specialAuraColor;
        yield return new WaitForSeconds(currentWeapon.specialDuration);
        isSpecialModeActive = false;
        if (playerSprite != null) playerSprite.color = originalColor;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || currentWeapon == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, currentWeapon.attackRange);
    }
}