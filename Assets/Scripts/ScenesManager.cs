using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    [Header("Ajustes de Audio")]
    public AudioSource musicaSource;

    [HideInInspector] public float volumenActual;
    [HideInInspector] public float brilloActual;

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

    private void PlayMusic()
    {
        if (musicaSource != null && !musicaSource.isPlaying)
        {
            musicaSource.loop = true; // Asegurar que sea en bucle
            musicaSource.playOnAwake = true;
            musicaSource.Play();
        }
    }

    private void LoadSettings()
    {
        volumenActual = PlayerPrefs.GetFloat("Volumen", 0.5f);
        brilloActual = PlayerPrefs.GetFloat("Brillo", 1.0f);
        ApplyChanges();
    }

    public void UpdateValues(float vol, float bri)
    {
        volumenActual = vol;
        brilloActual = bri;

        PlayerPrefs.SetFloat("Volumen", vol);
        PlayerPrefs.SetFloat("Brillo", bri);
        PlayerPrefs.Save();

        ApplyChanges();
    }

    public void ApplyChanges()
    {
        AudioListener.volume = volumenActual;

        GameObject overlay = GameObject.FindWithTag("BrilloOverlay");
        if (overlay != null)
        {
            var img = overlay.GetComponent<UnityEngine.UI.Image>();
            Color c = img.color;
            c.a = Mathf.Clamp(1.0f - brilloActual, 0f, 0.8f);
            img.color = c;
        }
    }

    public void ChangeScene(string nombre)
    {
        Time.timeScale = 1; // Siempre resetear el tiempo al cambiar
        SceneManager.LoadScene(nombre);
    }
}
