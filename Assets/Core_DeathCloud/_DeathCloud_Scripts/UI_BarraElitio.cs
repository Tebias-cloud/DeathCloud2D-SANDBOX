using UnityEngine;
using UnityEngine.UI; // Importante para que Unity entienda qué es un Slider

public class UI_BarraElitio : MonoBehaviour
{
    private Slider miSlider;

    void Start()
    {
        // El script busca automáticamente el Slider en el objeto donde lo pongas
        miSlider = GetComponent<Slider>();

        // Le decimos a la barra que se conecte al tanque central de Elitio
        if (ElitioManager.Instance != null)
        {
            ElitioManager.Instance.OnElitioChanged.AddListener(ActualizarBarra);
        }
    }

    // El ElitioManager llamará a esta función automáticamente enviando un número entre 0 y 1
    void ActualizarBarra(float nuevoPorcentaje)
    {
        if (miSlider != null)
        {
            miSlider.value = nuevoPorcentaje;
        }
    }
}