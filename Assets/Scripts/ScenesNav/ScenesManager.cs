using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

// Clase para gestionar la navegacion entre las escenas y el flujo del juego
public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    [Header("Ajustes de Audio")]
    public AudioSource musicaSource;

    [HideInInspector] public float actualVol;
    [HideInInspector] public float actualBrightness;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (musicaSource == null)
                musicaSource = GetComponent<AudioSource>();

            LoadSettings();
            PlayMusic();
        }
        else { Destroy(gameObject); }
    }

    // Funcion para reproducir la musica
    private void PlayMusic()
    {
        if (musicaSource != null && !musicaSource.isPlaying)
        {
            musicaSource.loop = true; // Asegurar que sea en bucle
            musicaSource.playOnAwake = true;
            musicaSource.Play();
        }
    }


    // FUNCIONES PARA CARGAR Y APLICAR LOS AJUSTES DE BRILLO Y VOLUMEN
    private void LoadSettings()
    {
        actualVol = PlayerPrefs.GetFloat("Volumen", 0.5f);
        actualBrightness = PlayerPrefs.GetFloat("Brillo", 1.0f);
        ApplyChanges();
    }

    public void UpdateValues(float vol, float bright)
    {
        actualVol = vol;
        actualBrightness = bright;

        PlayerPrefs.SetFloat("Volumen", vol);
        PlayerPrefs.SetFloat("Brillo", bright);
        PlayerPrefs.Save();

        ApplyChanges();
    }

    public void ApplyChanges()
    {
        AudioListener.volume = actualVol;

        GameObject overlay = GameObject.FindWithTag("BrilloOverlay");
        if (overlay != null)
        {
            var img = overlay.GetComponent<UnityEngine.UI.Image>();
            Color c = img.color;
            c.a = Mathf.Clamp(1.0f - actualBrightness, 0f, 0.8f);
            img.color = c;
        }
    }

    // Funcion para cambiar de escena
    public void ChangeScene(string nombre)
    {
        Time.timeScale = 1; // Siempre resetear el tiempo al cambiar
        SceneManager.LoadScene(nombre);
    }

    // FUNCIONES PARA CAMBIAR EL IDIOMA
    public void ChangeLanguage(int indice)
    {
        StartCoroutine(SetLocale(indice));
    }

    private IEnumerator SetLocale(int _localeID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
    }
}
