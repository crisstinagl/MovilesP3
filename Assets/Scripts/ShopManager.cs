using UnityEngine;
using UnityEngine.UI;
using TMP_Generic = TMPro.TMP_Text;

public class ShopManager : MonoBehaviour
{
	[System.Serializable]
	public struct Skin
	{
		public string nombre;
		public int precio;
		public Sprite imagenSkin;
	}

	public Skin[] skinsDisponibles;
	public TMPro.TMP_Text textoMonedas;
	public Image visualizadorBicho;

	[Header("UI de Error")]
	public GameObject cartelNoDinero;

	void Start()
	{
		ActualizarUI();
		if (cartelNoDinero != null) cartelNoDinero.SetActive(false);
	}

	public void ComprarSkin(int index)
	{
		if (index < 0 || index >= skinsDisponibles.Length) return;

		if (ScenesManager.Instance.TrySpendCoins(skinsDisponibles[index].precio))
		{
			if (visualizadorBicho != null)
			{
				visualizadorBicho.sprite = skinsDisponibles[index].imagenSkin;
			}
			ActualizarUI();
		}
		else
		{
			MostrarError();
		}
	}

	public void ActualizarUI()
	{
		if (textoMonedas != null)
		{
			textoMonedas.text = ScenesManager.Instance.monedas.ToString() + " Monedas";
		}
	}

	private void MostrarError()
	{
		if (cartelNoDinero != null)
		{
			cartelNoDinero.SetActive(true);
			Invoke("CerrarError", 2f);
		}
	}

	private void CerrarError()
	{
		if (cartelNoDinero != null) cartelNoDinero.SetActive(false);
	}
}