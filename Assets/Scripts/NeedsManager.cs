using UnityEngine;
using UnityEngine.UI;

public class NeedsManager : MonoBehaviour
{
    [Header("Referencias")]
    public Slider slider;
    public Image fillImage;
    public Gradient gradiente;

    [Header("Sistema de Suciedad (Solo para Higiene)")]
    public GameObject objetoSuciedad;

    [Header("Identificador Único")]
    [Tooltip("IMPORTANTE: Usa 'Hambre', 'Higiene', 'Sueño' o 'Entretenimiento'")]
    public string idGuardado;

    [Header("Ajustes de Tiempo")]
    public float velocidadDescenso = 0.01f;

    void Start()
    {
        if (ScenesManager.Instance != null)
        {
            switch (idGuardado)
            {
                case "Hambre":
                    slider.value = ScenesManager.Instance.hunger;
                    break;
                case "Higiene":
                    slider.value = ScenesManager.Instance.hygiene;
                    break;
                case "Entretenimiento":
                    slider.value = ScenesManager.Instance.fun;
                    break;
                default:
                    if (PlayerPrefs.HasKey(idGuardado))
                        slider.value = PlayerPrefs.GetFloat(idGuardado);
                    break;
            }
        }
        else if (PlayerPrefs.HasKey(idGuardado))
        {
            slider.value = PlayerPrefs.GetFloat(idGuardado);
        }

        // Logica de suciedad (Visual)
        ActualizarSuciedadVisual();
    }

    void Update()
    {
        // Descenso de la barra
        if (slider.value > 0)
        {
            slider.value -= velocidadDescenso * Time.deltaTime;
        }

        if (ScenesManager.Instance != null)
        {
            switch (idGuardado)
            {
                case "Hambre": ScenesManager.Instance.hunger = slider.value; break;
                case "Higiene": ScenesManager.Instance.hygiene = slider.value; break;
                case "Entretenimiento": ScenesManager.Instance.fun = slider.value; break;
            }
        }

        // Color y Gráficos
        float valorNormalizado = slider.value / slider.maxValue;
        fillImage.color = gradiente.Evaluate(valorNormalizado);
        fillImage.fillAmount = valorNormalizado;

        // Chequeo constante de suciedad si es la barra de higiene
        if (objetoSuciedad != null) ActualizarSuciedadVisual();
    }

    void ActualizarSuciedadVisual()
    {
        if (objetoSuciedad == null) return;

        bool estaSucio = slider.value <= 0;

        // Solo cambiamos si el estado es diferente para no parpadear
        if (objetoSuciedad.activeSelf != estaSucio)
        {
            objetoSuciedad.SetActive(estaSucio);
            if (ScenesManager.Instance != null)
            {
                ScenesManager.Instance.isDirty = estaSucio;
            }
        }
    }

    void OnDestroy()
    {
        GuardarDatos();
    }

    public void GuardarDatos()
    {
        if (!string.IsNullOrEmpty(idGuardado))
        {
            PlayerPrefs.SetFloat(idGuardado, slider.value);
            if (ScenesManager.Instance != null) ScenesManager.Instance.SaveSettings();
            else PlayerPrefs.Save();
        }
    }

    // Funciones para botones de comida/baño
    public void ReloadNeed(float amount)
    {
        slider.value = Mathf.Clamp(slider.value + amount, 0f, 1f);
        ActualizarSuciedadVisual();
    }

    public void DecreaseNeed(float amount)
    {
        slider.value = Mathf.Clamp(slider.value - amount, 0f, 1f);
    }
}