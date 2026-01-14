using UnityEngine;
using UnityEngine.SceneManagement;

// Clase utilizada para la gestion de paneles y cargas de escena
public class UIManager : MonoBehaviour
{
	[Header("Paneles")]
	public GameObject panelAjustes;
	public GameObject panelRanking;
	public GameObject panelPausa;
	public GameObject panelShop;
	public GameObject panelTutorial;

	// Abrir/Cerrar el panel de ajustes
	public void OpenSettings() => panelAjustes.SetActive(true);
	public void CloseSettings() => panelAjustes.SetActive(false);

    // Abrir/Cerrar el panel de ranking
    public void OpenRanking() => panelRanking.SetActive(true);
	public void CloseRanking() => panelRanking.SetActive(false);

    // Abrir/Cerrar el panel de tienda
    public void OpenShop() => panelShop.SetActive(true);
	public void CloseShop() => panelShop.SetActive(false);

    // Abrir/Cerrar el panel de tutorial
    public void OpenTutorial() => panelTutorial.SetActive(true);
    public void CloseTutorial() => panelTutorial.SetActive(false);

    // Alternar el panel de pausa
    public void TogglePauseMenu()
	{
		if (panelPausa != null)
		{
			bool pause = !panelPausa.activeSelf;
			panelPausa.SetActive(pause);
			Time.timeScale = pause ? 0 : 1;
			if (ScenesManager.Instance.musicaSource != null)
				ScenesManager.Instance.musicaSource.volume = pause ? 0.2f : ScenesManager.Instance.actualVol;
		}
	}

    // Cargar escena de menu principal
    public void LoadMainMenu()
	{
		 ScenesManager.Instance.ChangeScene("MainMenu");
    }

    // Cargar escena de juego
    public void LoadGame()
	{
		 ScenesManager.Instance.ChangeScene("Game");
	}

    // Cargar escena de minijuego
    public void LoadMinigame()
    {
        ScenesManager.Instance.ChangeScene("Minigame");
    }

    // Salir del juego
    public void ExitGame()
	{
		Application.Quit();
	}
}