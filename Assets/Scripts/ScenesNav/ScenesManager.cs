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

    [Header("Necesidades")]
    public float hunger = 1.0f;
    public float hygiene = 1.0f;
    public float fun = 1.0f;

    [Header("Ajustes de Audio")]
	public AudioSource musicaSource;
	public AudioSource uiSource;
	public AudioClip sonidoClick;

	[Header("Daltonismo")]
	public int actualColorFilter; // 0: Normal, 1: Protan, 2: Deutan, 3: Tritan

    [System.Serializable]
	public struct AccesibilityFont
	{
		public TMP_FontAsset originalFont;
		public TMP_FontAsset dyslexicFont;
	};

	[Header("Mapeo de Fuentes")]
	public List<AccesibilityFont> fontCatalog;
	private Dictionary<TMP_Text, TMP_FontAsset> fontsMemory = new Dictionary<TMP_Text, TMP_FontAsset>();

	[HideInInspector] public float actualVol = 1.0f;
	[HideInInspector] public float actualBrightness = 1.0f;
	[HideInInspector] public bool isDyslexicMode;

    [Header("Economía e Inventario")]
    public string nombreJugador = "PouPlayer";
    public int monedas;
	public List<int> skinsCompradas = new List<int>();
	public int skinEquipada = -1;

    public bool isDirty = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
            PlayMusic();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
	void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fontsMemory != null) fontsMemory.Clear();
        ApplyChanges();
    }

    public void AddCoins(int amount)
    {
        monedas += amount;
        SaveSettings(); // Guardamos inmediatamente
        RefreshCoinsUI();
    }



    // FUNCIONES PARA LA GESTION DE LA FUENTE PARA DISLEXICOS
    public void ApplyDislexicFont(bool activate)
    {
        isDyslexicMode = activate;
        PlayerPrefs.SetInt("Dislexia", activate ? 1 : 0);
        PlayerPrefs.Save();

        TMP_Text[] textos = Resources.FindObjectsOfTypeAll<TMP_Text>();
        foreach (var txt in textos) UpdateText(txt);
    }

    public void UpdateText(TMP_Text text)
    {
        if (text == null) return;
        if (!fontsMemory.ContainsKey(text)) fontsMemory.Add(text, text.font);
        TMP_FontAsset baseFont = fontsMemory[text];

        foreach (var par in fontCatalog)
        {
            if (baseFont == par.originalFont)
            {
                text.font = isDyslexicMode ? par.dyslexicFont : par.originalFont;
                break;
            }
        }
    }

    // FUNCIONES PARA LA GESTION DEL GUARDADO Y CARGA DE DATOS
    private void LoadSettings()
	{
		// Ajustes
		actualVol = PlayerPrefs.GetFloat("Volumen", 1f);
		actualBrightness = PlayerPrefs.GetFloat("Brillo", 1f);
		isDyslexicMode = PlayerPrefs.GetInt("Dislexia", 0) == 1;
		actualColorFilter = PlayerPrefs.GetInt("Daltonismo", 0);

        int savedLanguage = PlayerPrefs.GetInt("IdiomaIndex", 0);
        ChangeLanguage(savedLanguage);

        nombreJugador = PlayerPrefs.GetString("NombreJugador", "Gatito");

        // Tienda
        monedas = PlayerPrefs.GetInt("Monedas", 25);
		skinEquipada = PlayerPrefs.GetInt("SkinEquipada", -1);
        isDirty = PlayerPrefs.GetInt("EstaSucio", 0) == 1;

        string skinsData = PlayerPrefs.GetString("SkinsPoseidas", "");
		if (!string.IsNullOrEmpty(skinsData))
		{
			string[] ids = skinsData.Split(',');
			skinsCompradas.Clear();
			foreach (var id in ids) skinsCompradas.Add(int.Parse(id));
		}

        hunger = PlayerPrefs.GetFloat("HambreValue", 1.0f);
        hygiene = PlayerPrefs.GetFloat("HigieneValue", 1.0f);
        fun = PlayerPrefs.GetFloat("Entretenimiento", 1.0f);

        ApplyChanges();
	}

	public void SaveSettings()
	{
        // Ajustes
		PlayerPrefs.SetFloat("Volumen", actualVol);
		PlayerPrefs.SetFloat("Brillo", actualBrightness);
		PlayerPrefs.SetInt("Dislexia", isDyslexicMode ? 1 : 0);
		PlayerPrefs.SetInt("Daltonismo", actualColorFilter);

        var locales = LocalizationSettings.AvailableLocales.Locales;
        int indiceActual = locales.IndexOf(LocalizationSettings.SelectedLocale);
        PlayerPrefs.SetInt("IdiomaIndex", indiceActual);

        PlayerPrefs.SetString("NombreJugador", nombreJugador);

        // Tienda
        PlayerPrefs.SetInt("Monedas", monedas);
		PlayerPrefs.SetInt("SkinEquipada", skinEquipada);
		PlayerPrefs.SetString("SkinsPoseidas", string.Join(",", skinsCompradas));
        PlayerPrefs.SetInt("EstaSucio", isDirty ? 1 : 0);

        PlayerPrefs.SetFloat("HambreValue", hunger);
        PlayerPrefs.SetFloat("HigieneValue", hygiene);
        PlayerPrefs.SetFloat("Entretenimiento", fun);

        PlayerPrefs.Save();
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
            img.color = new Color(0, 0, 0, Mathf.Clamp(1.0f - actualBrightness, 0f, 0.8f));
        }

        // Fuente dislexia
        TMP_Text[] allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
        foreach (var text in allTexts)
        {
            if (text == null || text.gameObject.hideFlags == HideFlags.NotEditable || text.gameObject.hideFlags == HideFlags.HideAndDontSave)
                continue;

            UpdateText(text);
            text.ForceMeshUpdate();
            text.SetAllDirty();
        }

        // Daltonismo
        GameObject filtroGO = GameObject.FindWithTag("FiltroColor");
        if (filtroGO != null)
        {
            var img = filtroGO.GetComponent<UnityEngine.UI.Image>();
            Color[] filterColors = {
                new Color(0,0,0,0),             // Normal
                new Color(0,0.4f,0.7f,0.15f),    // Protan
                new Color(0.7f,0,0.7f,0.15f),    // Deutan
                new Color(0.7f,0.7f,0,0.15f)     // Tritan
            };
            img.color = filterColors[actualColorFilter];
        }

        //Monedas
        RefreshCoinsUI();
    }

    public void UpdateValues(float vol, float bright, bool dyslexic, int colorIndex)
    {
        actualVol = vol;
        actualBrightness = bright;
        isDyslexicMode = dyslexic;
        actualColorFilter = colorIndex;

        SaveSettings();
        ApplyChanges();
    }

    // Funcion para gastar monedas 
    public bool TrySpendCoins(int amount)
	{
		if (monedas >= amount)
		{
			monedas -= amount;
			SaveSettings();
			return true;
		}
		return false;
	}

    // Función global para actualizar el texto de monedas en cualquier parte
    public void RefreshCoinsUI()
    {
        GameObject[] coinTexts = GameObject.FindGameObjectsWithTag("MonedasText");
        foreach (GameObject go in coinTexts)
        {
            TMP_Text t = go.GetComponent<TMP_Text>();
            if (t != null) t.text = monedas.ToString();
        }
    }

    // FUNCIONES PARA REPRODUCIR LA MUSICA Y LOS EFECTOS DE SONIDO
    private void PlayMusic()
    {
        if (musicaSource != null && !musicaSource.isPlaying)
        {
            musicaSource.loop = true; // Asegurar que sea en bucle
            musicaSource.playOnAwake = true;
            musicaSource.Play();
        }
    }

    public void PlayClickSound()
	{
		if (uiSource != null && sonidoClick != null) uiSource.PlayOneShot(sonidoClick);
	}

    // Funcion para cambiar de escena
    public void ChangeScene(string nombre) 
	{	
        fontsMemory.Clear(); 
		SceneManager.LoadScene(nombre); 
	}

    // FUNCIONES PARA CAMBIAR EL IDIOMA
    public void ChangeLanguage(int index) 
	{ 
		StartCoroutine(SetLocale(index)); 
	}

	private IEnumerator SetLocale(int _localeID)
	{
		yield return LocalizationSettings.InitializationOperation;
        var locales = LocalizationSettings.AvailableLocales.Locales;

        if (_localeID >= 0 && _localeID < locales.Count)
        {
            LocalizationSettings.SelectedLocale = locales[_localeID];
            PlayerPrefs.SetInt("IdiomaIndex", _localeID);
            PlayerPrefs.Save();
        }
    }

    public void SubirEntretenimiento(float cantidad)
    {
        fun += cantidad;
        if (fun > 1.0f) fun = 1.0f;

        SaveSettings();
    }
}