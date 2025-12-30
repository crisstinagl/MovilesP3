using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelAjustes;
    public GameObject panelRanking;
    public GameObject panelPausa;

    // --- CONTROL DE PANELES ---
    public void OpenSettings() => panelAjustes.SetActive(true);
    public void CloseSettings() => panelAjustes.SetActive(false);

    public void OpenRanking() => panelRanking.SetActive(true);
    public void CloseRanking() => panelRanking.SetActive(false);

    // --- SISTEMA DE PAUSA ---
    public void TogglePauseMenu()
    {
        if (panelPausa != null)
        {
            bool pausado = !panelPausa.activeSelf;
            panelPausa.SetActive(pausado);
            Time.timeScale = pausado ? 0 : 1;
        }
    }

    // Función para llamar al Singleton desde los botones
    public void LoadMainMenu() => ScenesManager.Instance.ChangeScene("MainMenu");
    public void LoadGame() => ScenesManager.Instance.ChangeScene("Game");
}
