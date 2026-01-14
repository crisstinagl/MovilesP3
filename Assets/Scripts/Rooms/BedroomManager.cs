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
        estaDurmiendo = PlayerPrefs.GetInt("EstaDurmiendo", 0) == 1;
        ActualizarVisuales();
    }

    void Update()
    {
        // Si esta durmiendo recarga la necesidad de sueño
        if (estaDurmiendo && managerSueño != null)
        {
            managerSueño.ReloadNeed(velocidadRecuperacion * Time.deltaTime);
        }
    }

    public void ToggleLampara()
    {
        estaDurmiendo = !estaDurmiendo; // Invierte el valor 
        ActualizarVisuales();
    }

    void ActualizarVisuales()
    {
        PlayerPrefs.SetInt("EstaDurmiendo", estaDurmiendo ? 1 : 0);
        PlayerPrefs.Save();

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