using UnityEngine;

public class FoodGenerator : MonoBehaviour
{
    [Header("Listas de Prefabs")]
    public GameObject[] comidasBuenas;
    public GameObject[] comidasMalas;

    [Header("Configuración")]
    public float tiempoEntreSpawn = 1.5f;
    [Range(0, 100)]
    public float probabilidadDeMalo = 20f;

    float contador;

    void Update()
    {
        contador -= Time.deltaTime;

        if (contador <= 0)
        {
            SpawnearObjeto();
            contador = tiempoEntreSpawn;
        }
    }

    void SpawnearObjeto()
    {
        float randomX = Random.Range(-2.5f, 2.5f);
        Vector3 posicionSpawn = new Vector3(randomX, transform.position.y, 0);

        GameObject prefabElegido;

        if (Random.Range(0, 100) < probabilidadDeMalo)
        {
            int indiceAleatorio = Random.Range(0, comidasMalas.Length);
            prefabElegido = comidasMalas[indiceAleatorio];
        }
        else
        {
            int indiceAleatorio = Random.Range(0, comidasBuenas.Length);
            prefabElegido = comidasBuenas[indiceAleatorio];
        }

        Instantiate(prefabElegido, posicionSpawn, Quaternion.identity);
    }
}