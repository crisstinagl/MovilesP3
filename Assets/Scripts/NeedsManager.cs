using UnityEngine;
using UnityEngine.UI;

// Clase para gestionar las necesidades de la mascota
public class NeedsManager : MonoBehaviour
{
    [Header("Referencias")]
    public Slider slider;
    public Image fillImage;
    public Gradient gradiente;

    [Header("Sistema de Suciedad (Solo para Higiene)")]
    [Tooltip("Arrastra aquí la imagen 'ManchasSuciedad' SOLO si este es el slider de Higiene.")]
    public GameObject objetoSuciedad;

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

        if (objetoSuciedad != null && ScenesManager.Instance != null)
        {
            if (ScenesManager.Instance.isDirty)
            {
                slider.value = 0;
                objetoSuciedad.SetActive(true);
            }
            else
            {
                if (slider.value <= 0) objetoSuciedad.SetActive(true);
                else objetoSuciedad.SetActive(false);
            }
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
        if (slider.value > 0)
        {
            CambiarEstadoSuciedad(false);
        }
        GuardarDatos();
        Debug.Log("Necesidad recargada en: " + amount);
    }

    // Funcion para decrementar una necesidad
    public void DecreaseNeed(float amount)
    {
        // Resta la cantidad y evita que pase de 1 o baje de 0
        slider.value = Mathf.Clamp(slider.value - amount, 0f, 1f);
    }

    void CambiarEstadoSuciedad(bool estaSucio)
    {
        // Solo hacemos esto si tenemos la imagen asignada (Barra de Higiene)
        if (objetoSuciedad != null)
        {
            objetoSuciedad.SetActive(estaSucio);

            // Avisamos al Manager Global para que lo recuerde
            if (ScenesManager.Instance != null)
            {
                ScenesManager.Instance.isDirty = estaSucio;
                ScenesManager.Instance.SaveSettings();
            }
        }
    }
}
