using UnityEngine;
using UnityEngine.UI;

public class Seetings : MonoBehaviour
{
    public Slider sliderVolumen;
    public Slider sliderBrillo;

    void Start()
    {
        sliderVolumen.value = ScenesManager.Instance.volumenActual;
        sliderBrillo.value = ScenesManager.Instance.brilloActual;

        sliderVolumen.onValueChanged.AddListener(delegate { NotificarCambio(); });
        sliderBrillo.onValueChanged.AddListener(delegate { NotificarCambio(); });

        ScenesManager.Instance.ApplyChanges();
    }

    public void NotificarCambio()
    {
        ScenesManager.Instance.UpdateValues(sliderVolumen.value, sliderBrillo.value);
    }
}
