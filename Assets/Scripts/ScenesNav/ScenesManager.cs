using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

// Clase para gestionar la navegacion entre las escenas y el flujo del juego
public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    [Header("Ajustes de Audio")]
    public AudioSource musicaSource;

    [System.Serializable]
    public struct AccesibilityFont
    {
        public TMP_FontAsset originalFont;
        public TMP_FontAsset dyslexicFont;
    };

    [Header("Mapeo de Fuentes")]
    public List<AccesibilityFont> fontCatalog;
    private Dictionary<TMP_Text, TMP_FontAsset> fontsMemory = new Dictionary<TMP_Text, TMP_FontAsset>();

    private bool activeDyslexicMode = false;

    [HideInInspector] public float actualVol;
    [HideInInspector] public float actualBrightness;
    [HideInInspector] public bool isDyslexicMode;

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

    // FUNCIONES PARA LA GESTION DE LA FUENTE PARA DISLEXICOS
    public void ApplyDislexicFont(bool activate)
    {
        activeDyslexicMode = activate;
        PlayerPrefs.SetInt("AccesibilidadDislexia", activate ? 1 : 0);

        TMP_Text[] textos = Resources.FindObjectsOfTypeAll<TMP_Text>();

        foreach (var txt in textos)
        {
            UpdateText(txt);
        }
    }

    public void UpdateText(TMP_Text text)
    {
        foreach (var par in fontCatalog)
        {
            if (activeDyslexicMode)
            {
                if (text.font == par.originalFont)
                {
                    text.font = par.dyslexicFont;
                    break;
                }
            }
            else
            {
                if (text.font == par.dyslexicFont)
                {
                    text.font = par.originalFont;
                    break;
                }
            }
        }
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

    // FUNCIONES PARA CARGAR Y APLICAR LOS AJUSTES
    private void LoadSettings()
    {
        actualVol = PlayerPrefs.GetFloat("Volumen", 0.5f);
        actualBrightness = PlayerPrefs.GetFloat("Brillo", 1.0f);
        isDyslexicMode = PlayerPrefs.GetInt("Dislexia", 0) == 1;

        ApplyChanges();
    }

    public void UpdateValues(float vol, float bright, bool dyslexic)
    {
        actualVol = vol;
        actualBrightness = bright;
        isDyslexicMode = dyslexic;

        PlayerPrefs.SetFloat("Volumen", vol);
        PlayerPrefs.SetFloat("Brillo", bright);
        PlayerPrefs.SetInt("Dislexia", dyslexic ? 1 : 0);
        PlayerPrefs.Save();

        ApplyChanges();
    }

    public void ApplyChanges()
    {
        // Volumen
        AudioListener.volume = actualVol;

        // Brillo
        GameObject overlay = GameObject.FindWithTag("BrilloOverlay");
        if (overlay != null)
        {
            var img = overlay.GetComponent<UnityEngine.UI.Image>();
            Color c = img.color;
            c.a = Mathf.Clamp(1.0f - actualBrightness, 0f, 0.8f);
            img.color = c;
        }

        // Fuente dislexia
        TMP_Text[] allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
        foreach (var text in allTexts)
        {
            if (text == null || text.gameObject.hideFlags == HideFlags.NotEditable || text.gameObject.hideFlags == HideFlags.HideAndDontSave)
                continue;

            if (!fontsMemory.ContainsKey(text))
            {
                fontsMemory.Add(text, text.font);
            }

            TMP_FontAsset originalTextFont = fontsMemory[text];

            foreach (var mapping in fontCatalog)
            {
                if (isDyslexicMode)
                {
                    if (originalTextFont == mapping.originalFont)
                    {
                        text.font = mapping.dyslexicFont;
                        break;
                    }
                }
                else
                {
                    text.font = originalTextFont;
                    break;
                }
            }
            text.SetAllDirty();
        }
    }

    // Funcion para cambiar de escena
    public void ChangeScene(string nombre)
    {
        fontsMemory.Clear();
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
