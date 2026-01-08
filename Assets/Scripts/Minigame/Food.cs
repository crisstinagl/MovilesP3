using UnityEngine;

public class Food : MonoBehaviour
{
    public float velocidadCaida = 5f;
    public bool esComidaBuena = true; // False si es mala

    private float velocidadRotacion;

    void Start()
    {
        velocidadRotacion = Random.Range(-200f, 200f);
    }
    void Update()
    {
        // Mover hacia abajo constantemente
        transform.Translate(Vector2.down * velocidadCaida * Time.deltaTime, Space.World);

        transform.Rotate(0, 0, velocidadRotacion * Time.deltaTime);

        // Si baja demasiado (sale de la pantalla), se autodestruye para limpiar memoria
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}