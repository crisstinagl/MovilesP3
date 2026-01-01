using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
	[Header("Paneles")]
	public GameObject panelAjustes;
	public GameObject panelRanking;
	public GameObject panelPausa;
	public GameObject panelShop;

	public void OpenSettings() => panelAjustes.SetActive(true);
	public void CloseSettings() => panelAjustes.SetActive(false);

	public void OpenRanking() => panelRanking.SetActive(true);
	public void CloseRanking() => panelRanking.SetActive(false);

	public void OpenShop() => panelShop.SetActive(true);
	public void CloseShop() => panelShop.SetActive(false);

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

	public void LoadMainMenu() => ScenesManager.Instance.ChangeScene("MainMenu");
	public void LoadGame() => ScenesManager.Instance.ChangeScene("Game");

	public void ExitGame()
	{
		Application.Quit();
	}
}