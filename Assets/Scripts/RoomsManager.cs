using UnityEngine;

public class RoomsManager : MonoBehaviour
{
    public GameObject[] salas;

    public void CambiarSala(int indiceNuevaSala)
    {
        // 1. Recorremos todas las salas de la lista
        for (int i = 0; i < salas.Length; i++)
        {
            // 2. Si i es igual al indice que pedimos, se activa. Si no, se apaga
            salas[i].SetActive(i == indiceNuevaSala);
        }

        Debug.Log("Cambiando a la sala número: " + indiceNuevaSala);
    }
}
