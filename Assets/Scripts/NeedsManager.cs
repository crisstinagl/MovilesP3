using UnityEngine;
using UnityEngine.UI;

public class NeedsManager : MonoBehaviour
{
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

        // Calculamos el valor normalizado (de 0 a 1)
        float valorNormalizado = slider.value / slider.maxValue;
        fillImage.color = gradiente.Evaluate(valorNormalizado);
    }

    public void RecargarNecesidad(float cantidad)
    {
        // Suma la cantidad y evita que pase de 1 o baje de 0
        slider.value = Mathf.Clamp(slider.value + cantidad, 0f, 1f);

        Debug.Log("Necesidad recargada en: " + cantidad);
    }
}
