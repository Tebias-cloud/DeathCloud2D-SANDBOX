using UnityEngine;

// Esto añade la opción de crear armas en el menú de clic derecho
[CreateAssetMenu(fileName = "NuevaArma", menuName = "DeathCloud/Arma")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Información Básica")]
    public string weaponName;
    public Sprite weaponSprite; // Para la interfaz de inventario
    public GameObject weaponPrefab; // El objeto visual que sostienes

    [Header("Estadísticas Normales")]
    public float damage = 10f;
    public float attackRate = 0.5f; // Segundos entre golpes
    public float attackRange = 2f; // Para melé o radar

    [Header("Habilidad Especial (Elitio)")]
    public float elitioCost = 20f; // Cuánto gasta
    public float specialDuration = 5f; // Cuánto dura el aura/modo
    public float specialDamageMultiplier = 2f; // Daño extra
    
    // Aquí podrías añadir prefabs de VFX como el aura azul o flechas especiales
    public GameObject specialVFXPrefab; 
}