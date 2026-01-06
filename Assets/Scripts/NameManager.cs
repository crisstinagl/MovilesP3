using UnityEngine;
using TMPro; // Necesario para usar la caja de texto

public class NameManager : MonoBehaviour
{
    // Arrastra aquí tu InputField desde la jerarquía
    public TMP_InputField cajaDeTexto;

    void Start()
    {
        // Al empezar, leemos el nombre guardado y lo ponemos en la caja
        if (ScenesManager.Instance != null)
        {
            cajaDeTexto.text = ScenesManager.Instance.nombreJugador;
        }
    }

    // Esta es la función que llamará la caja cada vez que escribas una letra
    public void ActualizarNombre(string nuevoNombre)
    {
        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.nombreJugador = nuevoNombre;

            ScenesManager.Instance.SaveSettings();
        }
    }
}