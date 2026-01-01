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

	[Header("Sonidos UI")]
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

	private bool activeDyslexicMode = false;

	[HideInInspector] public float actualVol;
	[HideInInspector] public float actualBrightness;
	[HideInInspector] public bool isDyslexicMode;

	[Header("Economía")]
	public int monedas;

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

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		ApplyChanges();
	}

	public void UpdateValues(float vol, float bright, bool dyslexic, int colorFilter)
	{
		actualVol = vol;
		actualBrightness = bright;
		isDyslexicMode = dyslexic;
		actualColorFilter = colorFilter;

		SaveSettings();
		ApplyChanges();
	}

	private void SaveSettings()
	{
		PlayerPrefs.SetFloat("Volumen", actualVol);
		PlayerPrefs.SetFloat("Brillo", actualBrightness);
		PlayerPrefs.SetInt("Dislexia", isDyslexicMode ? 1 : 0);
		PlayerPrefs.SetInt("Daltonismo", actualColorFilter);
		PlayerPrefs.SetInt("Monedas", monedas);
		PlayerPrefs.Save();
	}

	private void LoadSettings()
	{
		actualVol = PlayerPrefs.GetFloat("Volumen", 1f);
		actualBrightness = PlayerPrefs.GetFloat("Brillo", 1f);
		isDyslexicMode = PlayerPrefs.GetInt("Dislexia", 0) == 1;
		actualColorFilter = PlayerPrefs.GetInt("Daltonismo", 0);
		monedas = PlayerPrefs.GetInt("Monedas", 100);
	}

	public void ApplyChanges()
	{
		if (musicaSource != null) musicaSource.volume = actualVol;

		activeDyslexicMode = isDyslexicMode;
		TMP_Text[] allTexts = GameObject.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);

		foreach (TMP_Text txt in allTexts)
		{
			if (!fontsMemory.ContainsKey(txt))
			{
				fontsMemory.Add(txt, txt.font);
			}

			TMP_FontAsset fontToApply = fontsMemory[txt];

			if (activeDyslexicMode)
			{
				foreach (var item in fontCatalog)
				{
					if (item.originalFont == fontToApply)
					{
						fontToApply = item.dyslexicFont;
						break;
					}
				}
			}
			txt.font = fontToApply;
		}

		GameObject brilloGO = GameObject.FindWithTag("BrilloOverlay");
		if (brilloGO != null)
		{
			var img = brilloGO.GetComponent<UnityEngine.UI.Image>();
			img.color = new Color(0, 0, 0, 1f - actualBrightness);
		}

		GameObject filtroGO = GameObject.FindWithTag("FiltroColor");
		if (filtroGO != null)
		{
			var img = filtroGO.GetComponent<UnityEngine.UI.Image>();
			switch (actualColorFilter)
			{
				case 0:
					img.color = new Color(0, 0, 0, 0);
					break;
				case 1:
					img.color = new Color(0, 0.4f, 0.7f, 0.15f);
					break;
				case 2:
					img.color = new Color(0.7f, 0, 0.7f, 0.15f);
					break;
				case 3:
					img.color = new Color(0.7f, 0.7f, 0, 0.15f);
					break;
			}
		}
	}

	public void ChangeScene(string nombre)
	{
		fontsMemory.Clear();
		Time.timeScale = 1;
		SceneManager.LoadScene(nombre);
	}

	public void ChangeLanguage(int indice)
	{
		StartCoroutine(SetLocale(indice));
	}

	IEnumerator SetLocale(int _localeID)
	{
		yield return LocalizationSettings.InitializationOperation;
		LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
	}

	public void PlayClickSound()
	{
		if (uiSource != null && sonidoClick != null)
		{
			uiSource.PlayOneShot(sonidoClick);
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

	public void AddCoins(int amount)
	{
		monedas += amount;
		SaveSettings();
	}
}