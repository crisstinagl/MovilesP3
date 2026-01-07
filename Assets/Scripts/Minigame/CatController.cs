using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

public class CatController : MonoBehaviour
{
    [Header("Configuración Juego")]
    public float velocidad = 20f;
    public int vidas = 3;
    public int puntos = 0;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoVidas;

    [Header("Referencias UI (Game Over)")]
    public GameObject panelGameOver;
    public TextMeshProUGUI textoResultadoPuntos;
    public TextMeshProUGUI textoResultadoMonedas;
    public TextMeshProUGUI valorResultadoPuntos;
    public TextMeshProUGUI valorResultadoMonedas;

    // REFERENCIA AL OTRO SCRIPT
    public CatSkin miSkinVisuals;

    private bool juegoTerminado = false;

    void Start()
    {
        ActualizarMarcador();
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (juegoTerminado) return;

        float movimiento = Input.acceleration.x;
        

        transform.Translate(Vector2.right * movimiento * velocidad * Time.deltaTime);

        Vector3 posicion = transform.position;
        posicion.x = Mathf.Clamp(posicion.x, -1.8f, 1.7f);
        transform.position = posicion;
    }

    // CUANDO CHOCAMOS CON COMIDA
    private void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        Food food = otroObjeto.GetComponent<Food>();

        if (food != null)
        {
            // Llamamos a la función de lógica de juego
            ProcesarComida(food.esComidaBuena);

            // Borramos la comida
            Destroy(otroObjeto.gameObject);
        }
    }

    void ProcesarComida(bool esBuena)
    {
        if (esBuena)
        {
            // COMIDA BUENA: Sumamos puntos y cara feliz
            puntos += 5;
            StartCoroutine(RutinaCara("comiendo"));
        }
        else
        {
            // COMIDA MALA: Restamos vida y cara asco
            vidas--;
            StartCoroutine(RutinaCara("asco"));

            // Si nos quedamos sin vidas... ¡Game Over!
            if (vidas <= 0)
            {
                GameOver();
            }
        }

        // Siempre actualizamos el texto después de comer
        ActualizarMarcador();
    }

    void ActualizarMarcador()
    {
        // Traducciones
        string tabla = "P3 Texto";

        string txtPuntos = LocalizationSettings.StringDatabase.GetLocalizedString(tabla, "txt_puntos");
        string txtVidas = LocalizationSettings.StringDatabase.GetLocalizedString(tabla, "txt_vidas");

        textoPuntos.text = txtPuntos + " " + puntos;
        textoVidas.text = txtVidas + " " + vidas;

        // Dislexia
        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.UpdateText(textoPuntos);
            ScenesManager.Instance.UpdateText(textoVidas);
        }
    }

    IEnumerator RutinaCara(string tipo)
    {
        // 1. Le decimos al otro script que cambie la cara
        miSkinVisuals.PonerCara(tipo);

        yield return new WaitForSeconds(0.5f);

        // 2. Le decimos que vuelva a la normal
        miSkinVisuals.ResetearCara();
    }

    void GameOver()
    {
        juegoTerminado = true;
        int monedasGanadas = Mathf.CeilToInt(puntos / 10f);

        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.AddCoins(monedasGanadas);
            float cantidadASubir = puntos / 1000f;
            ScenesManager.Instance.SubirEntretenimiento(cantidadASubir);
            ScenesManager.Instance.isDirty = true;
            if (LeaderboardManager.Instance != null)
            {
                LeaderboardManager.Instance.GuardarPuntuacion(ScenesManager.Instance.nombreJugador, puntos);
            }
            ScenesManager.Instance.SaveSettings();
        }

        valorResultadoPuntos.text = "" + puntos;
        valorResultadoMonedas.text = "" + monedasGanadas;

        // Dislexia
        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.UpdateText(textoResultadoPuntos);
            ScenesManager.Instance.UpdateText(textoResultadoMonedas);
        }

        panelGameOver.SetActive(true);
        Time.timeScale = 0f;
    }

    public void BotonReiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BotonSalir()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

}