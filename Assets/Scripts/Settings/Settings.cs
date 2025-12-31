using UnityEngine;
using UnityEngine.UI;

// Clase para gestionar los ajustes
public class Settings : MonoBehaviour
{
    [Header("Referencias")]
    public Slider volSlider;
    public Slider brightnessSlider;
    public Toggle dyslexiaToggle;

    void Start()
    {
        volSlider.value = ScenesManager.Instance.actualVol;
        brightnessSlider.value = ScenesManager.Instance.actualBrightness;
        dyslexiaToggle.isOn = ScenesManager.Instance.isDyslexicMode;

        volSlider.onValueChanged.AddListener(delegate { NotifyChange(); });
        brightnessSlider.onValueChanged.AddListener(delegate { NotifyChange(); });
        dyslexiaToggle.onValueChanged.AddListener(delegate { NotifyChange(); });

        ScenesManager.Instance.ApplyChanges();
    }

    public void NotifyChange()
    {
        ScenesManager.Instance.UpdateValues(
            volSlider.value, 
            brightnessSlider.value,
            dyslexiaToggle.isOn
        );
    }
}
