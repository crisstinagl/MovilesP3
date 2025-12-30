using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    public void ChangeScene(string nombre)
    {
        Time.timeScale = 1; // Siempre resetear el tiempo al cambiar
        SceneManager.LoadScene(nombre);
    }
}
