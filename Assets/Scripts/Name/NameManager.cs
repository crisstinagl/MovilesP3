using UnityEngine;
using TMPro;

public class NameManager : MonoBehaviour
{
    public TMP_InputField cajaDeTexto;

    void Start()
    {
        // Al empezar, leemos el nombre guardado y lo ponemos en la caja
        if (ScenesManager.Instance != null)
        {
            cajaDeTexto.text = ScenesManager.Instance.nombreJugador;
        }
    }

    public void ActualizarNombre(string nuevoNombre)
    {
        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.nombreJugador = nuevoNombre;

            ScenesManager.Instance.SaveSettings();
        }
    }
}