using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

// Clase encargada de gestionar la tienda y las skins
public class ShopManager : MonoBehaviour
{
	[System.Serializable]
	public struct Skin
	{
		public string nombre;
		public int precio;
		public Sprite imagenSkin;
		public Button botonAccion;
		public TMP_Text textoBoton;
	}

	public Skin[] skinsDisponibles;
	public TMP_Text textoMonedas;
	public Image visualizadorBicho;
	public Sprite skinOriginal;

	[Header("UI de Error")]
	public GameObject cartelNoDinero;

	// Para la traduccion
    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += (locale) => ActualizarUI();
        ActualizarUI(); // Actualizamos al abrir el panel
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= (locale) => ActualizarUI();
    }

    void Start()
	{
		if (cartelNoDinero != null) cartelNoDinero.SetActive(false);
        int skinGuardada = ScenesManager.Instance.skinEquipada;
        if (skinGuardada != -1 && skinGuardada < skinsDisponibles.Length)
        {
            visualizadorBicho.sprite = skinsDisponibles[skinGuardada].imagenSkin;
        }
        else
        {
            visualizadorBicho.sprite = skinOriginal;
        }

        ActualizarUI();
	}

	public void ComprarSkin(int index)
	{
		if (index < 0 || index >= skinsDisponibles.Length) return;

		// Si ya la tiene comprada, equipar/desequipar
		if (ScenesManager.Instance.skinsCompradas.Contains(index))
		{
			EquiparSkin(index);
			return;
		}

		// Si no la tiene, intentar comprar
		if (ScenesManager.Instance.TrySpendCoins(skinsDisponibles[index].precio))
		{
			ScenesManager.Instance.skinsCompradas.Add(index);
			ScenesManager.Instance.SaveSettings();
			EquiparSkin(index);
		}
		else
		{
			MostrarError(); // Muestra el cartel si no hay dinero
		}
	}

	private void EquiparSkin(int index)
	{
		if (ScenesManager.Instance.skinEquipada == index)
		{
			ScenesManager.Instance.skinEquipada = -1;
			visualizadorBicho.sprite = skinOriginal;
		}
		else
		{
			ScenesManager.Instance.skinEquipada = index;
			visualizadorBicho.sprite = skinsDisponibles[index].imagenSkin;
		}
		ScenesManager.Instance.SaveSettings();
		ActualizarUI();
	}

	public void ActualizarUI()
	{
		if (textoMonedas != null)
			textoMonedas.text = ScenesManager.Instance.monedas.ToString();

		string tabla = "P3 Texto";

        for (int i = 0; i < skinsDisponibles.Length; i++)
		{
			bool comprada = ScenesManager.Instance.skinsCompradas.Contains(i);
			bool equipada = ScenesManager.Instance.skinEquipada == i;

			if (!comprada)
			{
                skinsDisponibles[i].textoBoton.text = LocalizationSettings.StringDatabase.GetLocalizedString(tabla, "btn_comprar"); skinsDisponibles[i].botonAccion.image.color = Color.white;
			}
			else
			{
                string llave = equipada ? "btn_equipada" : "btn_equipar";

                skinsDisponibles[i].textoBoton.text = LocalizationSettings.StringDatabase.GetLocalizedString(tabla, llave); 
				skinsDisponibles[i].botonAccion.image.color = equipada ? Color.gray : Color.white;
			}
            ScenesManager.Instance.UpdateText(skinsDisponibles[i].textoBoton);
        }
	}

	public void MostrarError()
	{
		if (cartelNoDinero != null) cartelNoDinero.SetActive(true);
	}

	public void CerrarError()
	{
		if (cartelNoDinero != null) cartelNoDinero.SetActive(false);
	}
}