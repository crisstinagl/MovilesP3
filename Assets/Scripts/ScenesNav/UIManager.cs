using UnityEngine;
using UnityEngine.SceneManagement;

// Clase para gestionar los paneles 
public class UIManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelAjustes;
    public GameObject panelRanking;
    public GameObject panelPausa;
    public GameObject panelShop;

    // --- CONTROL DE PANELES ---
    public void OpenSettings() => panelAjustes.SetActive(true);
    public void CloseSettings() => panelAjustes.SetActive(false);

    public void OpenRanking() => panelRanking.SetActive(true);
    public void CloseRanking() => panelRanking.SetActive(false);

    public void OpenShop() => panelShop.SetActive(true);
    public void CloseShop() => panelShop.SetActive(false);

    // --- PANEL DE PAUSA ---
    public void TogglePauseMenu()
    {
        if (panelPausa != null)
        {
            bool pause = !panelPausa.activeSelf;
            panelPausa.SetActive(pause);
            Time.timeScale = pause ? 0 : 1;
            ScenesManager.Instance.musicaSource.volume = pause ? 0.2f : 1.0f;
        }
    }

    // --- CARGA DE ESCENAS ---
    public void LoadMainMenu() => ScenesManager.Instance.ChangeScene("MainMenu");
    public void LoadGame() => ScenesManager.Instance.ChangeScene("Game");

    // Funcion para salir del juego
    public void ExitGame() 
    {
        Application.Quit();
        Debug.Log("Cerrando app");
    }
}
