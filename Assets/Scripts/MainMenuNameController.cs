using UnityEngine;
using TMPro; // Necesario para TextMeshPro

public class MainMenuNameController : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelNombre;      // Arrastra aquí el panel 'PanelNombre'
    public TMP_InputField inputNombre;  // Arrastra aquí el InputField del panel

    void Start()
    {
        // COMPROBACIÓN DE "PRIMERA VEZ"
        // Verificamos si ya existe la clave "NombreJugador" en los datos guardados.
        // Si NO existe, significa que es la primera vez que entra al juego.
        if (!PlayerPrefs.HasKey("NombreJugador"))
        {
            AbrirPanel();
        }
        else
        {
            // Si ya existe, nos aseguramos de que el panel esté cerrado
            panelNombre.SetActive(false);
        }
    }

    // Función para el botón "Cambiar Nombre" del menú
    public void AbrirPanel()
    {
        panelNombre.SetActive(true);

        // Opcional: Rellenar el input con el nombre que ya tiene (para editarlo)
        if (ScenesManager.Instance != null)
        {
            inputNombre.text = ScenesManager.Instance.nombreJugador;
        }
    }

    // Función para el botón "Confirmar" dentro del panel
    public void ConfirmarNombre()
    {
        // 1. Validar que no esté vacío
        if (string.IsNullOrWhiteSpace(inputNombre.text))
        {
            Debug.Log("El nombre no puede estar vacío");
            return;
        }

        // 2. Guardar en el ScenesManager
        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.nombreJugador = inputNombre.text;

            // Forzamos el guardado en disco para que PlayerPrefs.HasKey("NombreJugador") sea true la próxima vez
            ScenesManager.Instance.SaveSettings();
        }

        // 3. Cerrar el panel
        panelNombre.SetActive(false);
    }
}