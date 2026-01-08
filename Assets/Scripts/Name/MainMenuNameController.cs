using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    // Función para el botón "Cambiar Nombre" del menú
    public void AbrirPanel()
    {
        panelNombre.SetActive(true);

        // Rellenar el input con el nombre que ya tiene (para editarlo)
        if (ScenesManager.Instance != null)
        {
            inputNombre.text = ScenesManager.Instance.nombreJugador;
        }
        if (textoError != null) textoError.text = "";
    }

    // Función para el botón "Confirmar" dentro del panel
    public void ConfirmarNombre()
    {
        string nombreEscrito = inputNombre.text;

        // 1. Validar vacío
        if (string.IsNullOrWhiteSpace(nombreEscrito))
        {
            MostrarError("¡Escribe un nombre!");
            return;
        }

        // 2. Desactivar botón para que no le den dos veces mientras carga
        if (botonConfirmar != null) botonConfirmar.interactable = false;

        // 3. CONSULTAR A LEADERBOARD MANAGER
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.VerificarNombreDisponible(nombreEscrito, (esValido) => {

                // Reactivamos botón por si hay que reintentar
                if (botonConfirmar != null) botonConfirmar.interactable = true;

                if (esValido)
                {
                    // ¡NOMBRE VÁLIDO!
                    // a) Lo reservamos en la nube YA MISMO
                    LeaderboardManager.Instance.RegistrarNombre(nombreEscrito);

                    // b) Lo guardamos en el móvil
                    if (ScenesManager.Instance != null)
                    {
                        ScenesManager.Instance.nombreJugador = nombreEscrito;
                        ScenesManager.Instance.SaveSettings();
                    }

                    // c) Cerramos
                    panelNombre.SetActive(false);
                }
                else
                {
                    // ¡NOMBRE OCUPADO!
                    MostrarError("Ese nombre ya existe. Elige otro.");
                }
            });
        }
        else
        {
            // Si estamos jugando offline (sin el manager), guardamos y ya
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