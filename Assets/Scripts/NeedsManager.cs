using UnityEngine;
using UnityEngine.UI;

// Clase para gestionar las necesidades de la mascota
public class NeedsManager : MonoBehaviour
{
    [Header("Referencias")]
    public Slider slider;
    public Image fillImage;
    public Gradient gradiente;

    [Header("Identificador Único")]
    [Tooltip("Escribe aquí: HigieneValue, HambreValue, etc.")]
    public string idGuardado;

    [Header("Ajustes de Tiempo")]
    public float velocidadDescenso = 0.01f;

    void Start()
    {
        if (!string.IsNullOrEmpty(idGuardado) && PlayerPrefs.HasKey(idGuardado))
        {
            slider.value = PlayerPrefs.GetFloat(idGuardado);
        }
    }
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

    void OnDestroy()
    {
        GuardarDatos();
    }

    public void GuardarDatos()
    {
        // Solo guardamos si le hemos puesto un nombre
        if (!string.IsNullOrEmpty(idGuardado))
        {
            PlayerPrefs.SetFloat(idGuardado, slider.value);
            PlayerPrefs.Save();
        }
    }

    // Funcion para recargar una necesidad
    public void ReloadNeed(float amount)
    {
        // Suma la cantidad y evita que pase de 1 o baje de 0
        slider.value = Mathf.Clamp(slider.value + amount, 0f, 1f);
        GuardarDatos();
        Debug.Log("Necesidad recargada en: " + amount);
    }

    // Funcion para decrementar una necesidad
    public void DecreaseNeed(float amount)
    {
        // Resta la cantidad y evita que pase de 1 o baje de 0
        slider.value = Mathf.Clamp(slider.value - amount, 0f, 1f);
    }
}
