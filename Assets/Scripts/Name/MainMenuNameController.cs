using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class MainMenuNameController : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelNombre;
    public TMP_InputField inputNombre;
    public Button botonConfirmar;
    public TMP_Text textoError;

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

    // Función para el boton "Cambiar Nombre"
    public void AbrirPanel()
    {
        panelNombre.SetActive(true);

        if (ScenesManager.Instance != null)
        {
            inputNombre.text = ScenesManager.Instance.nombreJugador;
        }
        if (textoError != null) textoError.text = "";
    }

    // Funcion para el boton "Confirmar"
    public void ConfirmarNombre()
    {
        inputNombre.image.color = Color.white;
        string nombreEscrito = inputNombre.text;
        string tabla = "P3 Texto";

        if (string.IsNullOrWhiteSpace(nombreEscrito))
        {
            string msgVacio = LocalizationSettings.StringDatabase.GetLocalizedString(tabla, "txt_introNombre");
            MostrarError(msgVacio); 
            return;
        }

        if (botonConfirmar != null) botonConfirmar.interactable = false;

        // CONSULTAR A LEADERBOARD MANAGER
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.VerificarNombreDisponible(nombreEscrito, (esValido) => {

                if (botonConfirmar != null) botonConfirmar.interactable = true;

                if (esValido)
                {
                    // Nombre valido - guardar
                    LeaderboardManager.Instance.RegistrarNombre(nombreEscrito);

                    if (ScenesManager.Instance != null)
                    {
                        ScenesManager.Instance.nombreJugador = nombreEscrito;
                        ScenesManager.Instance.SaveSettings();
                    }

                    panelNombre.SetActive(false);
                }
                else
                {
                    // Nombre no valido - mostrar error
                    string msgDuplicado = LocalizationSettings.StringDatabase.GetLocalizedString(tabla, "txt_errorNombre");
                    MostrarError(msgDuplicado);
                }
            });
        }
        else
        {
            // offline - guardar
            if (ScenesManager.Instance != null)
            {
                ScenesManager.Instance.nombreJugador = nombreEscrito;
                ScenesManager.Instance.SaveSettings();
            }
            panelNombre.SetActive(false);
        }
    }

    void MostrarError(string mensaje)
    {
        if (textoError != null)
        {
            textoError.text = mensaje;
            textoError.color = Color.red;

            if (ScenesManager.Instance != null)
            {
                ScenesManager.Instance.UpdateText(textoError);
            }
        }
        else
        {
            Debug.Log(mensaje);
            inputNombre.image.color = new Color(1, 0.8f, 0.8f);
        }
    }

    public void ValidarTexto()
    {
        if (botonConfirmar != null) botonConfirmar.interactable = !string.IsNullOrWhiteSpace(inputNombre.text);
        if (inputNombre != null) inputNombre.image.color = Color.white;
        if (textoError != null) textoError.text = "";
    }
}