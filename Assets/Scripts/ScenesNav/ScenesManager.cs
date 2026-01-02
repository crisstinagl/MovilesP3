using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
	public static ScenesManager Instance;

	[Header("Ajustes de Audio")]
	public AudioSource musicaSource;
	public AudioSource uiSource;
	public AudioClip sonidoClick;

	[Header("Daltonismo")]
	public int actualColorFilter;

	[System.Serializable]
	public struct AccesibilityFont
	{
		public TMP_FontAsset originalFont;
		public TMP_FontAsset dyslexicFont;
	};

	[Header("Mapeo de Fuentes")]
	public List<AccesibilityFont> fontCatalog;
	private Dictionary<TMP_Text, TMP_FontAsset> fontsMemory = new Dictionary<TMP_Text, TMP_FontAsset>();

	[HideInInspector] public float actualVol;
	[HideInInspector] public float actualBrightness;
	[HideInInspector] public bool isDyslexicMode;

	[Header("Economía e Inventario")]
	public int monedas;
	public List<int> skinsCompradas = new List<int>();
	public int skinEquipada = -1;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			LoadSettings();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
	void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ApplyChanges();

	private void LoadSettings()
	{
		actualVol = PlayerPrefs.GetFloat("Volumen", 1f);
		actualBrightness = PlayerPrefs.GetFloat("Brillo", 1f);
		isDyslexicMode = PlayerPrefs.GetInt("Dislexia", 0) == 1;
		actualColorFilter = PlayerPrefs.GetInt("Daltonismo", 0);
		monedas = PlayerPrefs.GetInt("Monedas", 100);
		skinEquipada = PlayerPrefs.GetInt("SkinEquipada", -1);

		string skinsData = PlayerPrefs.GetString("SkinsPoseidas", "");
		if (!string.IsNullOrEmpty(skinsData))
		{
			string[] ids = skinsData.Split(',');
			skinsCompradas.Clear();
			foreach (var id in ids) skinsCompradas.Add(int.Parse(id));
		}
	}

	public void SaveSettings()
	{
		PlayerPrefs.SetFloat("Volumen", actualVol);
		PlayerPrefs.SetFloat("Brillo", actualBrightness);
		PlayerPrefs.SetInt("Dislexia", isDyslexicMode ? 1 : 0);
		PlayerPrefs.SetInt("Daltonismo", actualColorFilter);
		PlayerPrefs.SetInt("Monedas", monedas);
		PlayerPrefs.SetInt("SkinEquipada", skinEquipada);
		PlayerPrefs.SetString("SkinsPoseidas", string.Join(",", skinsCompradas));
		PlayerPrefs.Save();
	}

	public void ApplyChanges()
	{
		AudioListener.volume = actualVol;
		TMP_Text[] allTexts = GameObject.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
		foreach (var txt in allTexts)
		{
			if (!fontsMemory.ContainsKey(txt)) fontsMemory.Add(txt, txt.font);
			TMP_FontAsset original = fontsMemory[txt];
			if (isDyslexicMode)
			{
				foreach (var f in fontCatalog) if (original == f.originalFont) { txt.font = f.dyslexicFont; break; }
			}
			else txt.font = original;
		}

		GameObject overlay = GameObject.FindWithTag("BrilloOverlay");
		if (overlay != null)
		{
			var img = overlay.GetComponent<UnityEngine.UI.Image>();
			img.color = new Color(0, 0, 0, 1f - actualBrightness);
		}
	}

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

	public void PlayClickSound()
	{
		if (uiSource != null && sonidoClick != null) uiSource.PlayOneShot(sonidoClick);
	}

	public void ChangeScene(string nombre) { fontsMemory.Clear(); SceneManager.LoadScene(nombre); }

	public void ChangeLanguage(int indice) { StartCoroutine(SetLocale(indice)); }

	private IEnumerator SetLocale(int _localeID)
	{
		yield return LocalizationSettings.InitializationOperation;
		LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
	}

	public void UpdateValues(float vol, float bright, bool dyslexic, int colorIndex)
	{
		actualVol = vol; actualBrightness = bright; isDyslexicMode = dyslexic; actualColorFilter = colorIndex;
		SaveSettings(); ApplyChanges();
	}
}