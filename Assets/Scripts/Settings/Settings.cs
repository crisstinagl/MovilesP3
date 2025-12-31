using UnityEngine;
using UnityEngine.UI;

// Clase para gestionar los ajustes
public class Seetings : MonoBehaviour
{
    [Header("Referencias")]
    public Slider volSlider;
    public Slider brightnessSlider;

    void Start()
    {
        volSlider.value = ScenesManager.Instance.actualVol;
        brightnessSlider.value = ScenesManager.Instance.actualBrightness;

        volSlider.onValueChanged.AddListener(delegate { NotifyChange(); });
        brightnessSlider.onValueChanged.AddListener(delegate { NotifyChange(); });

        ScenesManager.Instance.ApplyChanges();
    }

    public void NotifyChange()
    {
        ScenesManager.Instance.UpdateValues(volSlider.value, brightnessSlider.value);
    }
}
