using UnityEngine;
using TMPro;

public class MainMenuNameController : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelNombre;
    public TMP_InputField inputNombre;

    void Start()
    {
        // Se comprueba si existe "NombreJugador" en los datos guardados, si no, es la primera vez en entrar al juego
        if (!PlayerPrefs.HasKey("NombreJugador"))
        {
            AbrirPanel();
        }
        else
        {
            // Si ya existe el panel estara cerrado
            panelNombre.SetActive(false);
        }
    }

    // Función para el botón "Cambiar Nombre" del menú
    public void AbrirPanel()
    {
        panelNombre.SetActive(true);

        // Rellenar el input con el nombre que ya tiene (para editarlo)
        if (ScenesManager.Instance != null)
        {
            inputNombre.text = ScenesManager.Instance.nombreJugador;
        }
    }

    // Función para el botón "Confirmar" dentro del panel
    public void ConfirmarNombre()
    {
        // Validar que no esté vacío
        if (string.IsNullOrWhiteSpace(inputNombre.text))
        {
            Debug.Log("El nombre no puede estar vacío");
            return;
        }

        // Guardar en el ScenesManager
        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.nombreJugador = inputNombre.text;
            ScenesManager.Instance.SaveSettings();
        }

        // Cerrar el panel
        panelNombre.SetActive(false);
    }
}