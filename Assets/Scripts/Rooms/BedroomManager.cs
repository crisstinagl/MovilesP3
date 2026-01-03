using UnityEngine;
using UnityEngine.UI;

public class BedroomManager : MonoBehaviour
{
    [Header("Referencias Visuales")]
    public Image imagenLampara;
    public Sprite spriteLamparaOn;
    public Sprite spriteLamparaOff;

    public Image imagenCaraMascota;
    public Sprite caraDespierta;
    public Sprite caraDormida;

    public GameObject filtroOscuridad;

    [Header("Lógica de Sueño")]
    public NeedsManager managerSueño;
    public float velocidadRecuperacion = 0.05f;

    private bool estaDurmiendo = false;

    void Start()
    {
        // Aseguramos que empiece despierto
        estaDurmiendo = false;
        ActualizarVisuales();
    }

    void Update()
    {
        // Si está durmiendo, recargamos la necesidad de sueño
        if (estaDurmiendo && managerSueño != null)
        {
            // Usamos Time.deltaTime para que la recarga sea suave en el tiempo
            managerSueño.ReloadNeed(velocidadRecuperacion * Time.deltaTime);
        }
    }

    // Esta función la llamaremos desde el Botón de la lámpara
    public void ToggleLampara()
    {
        estaDurmiendo = !estaDurmiendo; // Invierte el valor 
        ActualizarVisuales();
    }

    void ActualizarVisuales()
    {
        if (estaDurmiendo)
        {
            // Estado DORMIDO
            if (imagenLampara != null) imagenLampara.sprite = spriteLamparaOff;
            if (imagenCaraMascota != null) imagenCaraMascota.sprite = caraDormida;
            if (filtroOscuridad != null) filtroOscuridad.SetActive(true);
        }
        else
        {
            // Estado DESPIERTO
            if (imagenLampara != null) imagenLampara.sprite = spriteLamparaOn;
            if (imagenCaraMascota != null) imagenCaraMascota.sprite = caraDespierta;
            if (filtroOscuridad != null) filtroOscuridad.SetActive(false);
        }
    }

    public void DespertarForzado()
    {
        estaDurmiendo = false;
        ActualizarVisuales();
    }
}