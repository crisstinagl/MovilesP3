using UnityEngine;
using UnityEngine.UI;

// Clase para gestionar las necesidades de la mascota
public class NeedsManager : MonoBehaviour
{
    [Header("Referencias")]
    public Slider slider;
    public Image fillImage;
    public Gradient gradiente;

    [Header("Ajustes de Tiempo")]
    public float velocidadDescenso = 0.01f;

    void Update()
    {
        if (slider.value > 0)
        {
            slider.value -= velocidadDescenso * Time.deltaTime;
        }

        // Se calcula el valor normalizado (de 0 a 1)
        float valorNormalizado = slider.value / slider.maxValue;
        fillImage.color = gradiente.Evaluate(valorNormalizado);
        fillImage.fillAmount = valorNormalizado;
    }

    // Funcion para recargar una necesidad
    public void ReloadNeed(float amount)
    {
        // Suma la cantidad y evita que pase de 1 o baje de 0
        slider.value = Mathf.Clamp(slider.value + amount, 0f, 1f);

        Debug.Log("Necesidad recargada en: " + amount);
    }

    // Funcion para decrementar una necesidad
    public void DecreaseNeed(float amount)
    {
        // Resta la cantidad y evita que pase de 1 o baje de 0
        slider.value = Mathf.Clamp(slider.value - amount, 0f, 1f);
    }
}
