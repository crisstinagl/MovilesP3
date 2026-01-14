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
    public Image suciedadImage;

    [Header("Identificador Único")]
    public string idGuardado;

    [Header("Ajustes de Tiempo")]
    public float velocidadDescenso = 0.001f; // Predeterminado

    void Start()
    {
        // Cargar valor
        CargarEstadoInicial();

        // Tiempo transcurrido
        if (PlayerPrefs.HasKey("UltimaVez"))
        {
            System.DateTime ultimaVez = System.DateTime.Parse(PlayerPrefs.GetString("UltimaVez"));
            float segundosFuera = (float)(System.DateTime.Now - ultimaVez).TotalSeconds;

            if (idGuardado == "Sueno") // Sueño
            {
                bool estabaDormido = PlayerPrefs.GetInt("EstaDurmiendo", 0) == 1;

                if (estabaDormido) slider.value += segundosFuera * 0.05f; // Subir valor
                else slider.value -= segundosFuera * velocidadDescenso; // Restar valor
            }
            else // Resto de necesidades
            {
                slider.value -= segundosFuera * velocidadDescenso;
            }

            // Ajustar limite slider
            slider.value = Mathf.Clamp(slider.value, 0f, 1f);
        }

        ActualizarVisualesAlEntrar();
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
                case "Sueno": ScenesManager.Instance.sleep = slider.value; break;
            }
        }

        // Color y graficos
        float valorNormalizado = slider.value / slider.maxValue;
        fillImage.color = gradiente.Evaluate(valorNormalizado);
        fillImage.fillAmount = valorNormalizado;

        // Chequeo constante de suciedad si es la barra de higiene
        if (objetoSuciedad != null) ActualizarSuciedadVisual();
    }

    void CargarEstadoInicial()
    {
        if (ScenesManager.Instance != null)
        {
            switch (idGuardado)
            {
                case "Hambre": slider.value = ScenesManager.Instance.hunger; break;
                case "Higiene": slider.value = ScenesManager.Instance.hygiene; break;
                case "Entretenimiento": slider.value = ScenesManager.Instance.fun; break;
                case "Sueno": slider.value = ScenesManager.Instance.sleep; break;
            }
        }
    }

    void ActualizarVisualesAlEntrar()
    {
        ActualizarSuciedadVisual();
        float valorNormalizado = slider.value / slider.maxValue;
        if (fillImage != null)
        {
            fillImage.color = gradiente.Evaluate(valorNormalizado);
            fillImage.fillAmount = valorNormalizado;
        }
    }

    void ActualizarSuciedadVisual()
    {
        if (objetoSuciedad == null) return;

        float valorNormalizado = slider.value / slider.maxValue;
        float opacidad = 1f - valorNormalizado;

        if (suciedadImage != null)
        {
            Color c = suciedadImage.color;
            c.a = opacidad;
            suciedadImage.color = c;
        }

        bool estaSucio = slider.value <= 0.1f;
        if (ScenesManager.Instance != null && ScenesManager.Instance.isDirty != estaSucio)
        {
            ScenesManager.Instance.isDirty = estaSucio;
        }

        objetoSuciedad.SetActive(opacidad > 0.01f);
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