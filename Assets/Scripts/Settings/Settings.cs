using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Clase para gestionar los ajustes
public class Settings : MonoBehaviour
{
    [Header("Referencias")]
    public Slider volSlider;
    public Slider brightnessSlider;
    public Toggle dyslexiaToggle;
    public TMPro.TMP_Dropdown colorblindDropdown;

    void Start()
    {
        volSlider.value = ScenesManager.Instance.actualVol;
        brightnessSlider.value = ScenesManager.Instance.actualBrightness;
        dyslexiaToggle.isOn = ScenesManager.Instance.isDyslexicMode;
        colorblindDropdown.value = ScenesManager.Instance.actualColorFilter;

        volSlider.onValueChanged.AddListener(delegate { NotifyChange(); });
        brightnessSlider.onValueChanged.AddListener(delegate { NotifyChange(); });
        dyslexiaToggle.onValueChanged.AddListener(delegate { NotifyChange(); });
        colorblindDropdown.onValueChanged.AddListener(delegate { NotifyChange(); });

        ScenesManager.Instance.ApplyChanges();

        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button btn in allButtons)
        {
            btn.onClick.RemoveListener(() => ScenesManager.Instance.PlayClickSound());
            btn.onClick.AddListener(() => ScenesManager.Instance.PlayClickSound());
        }
    }

    public void NotifyChange()
    {
        ScenesManager.Instance.UpdateValues(
            volSlider.value, 
            brightnessSlider.value,
            dyslexiaToggle.isOn,
            colorblindDropdown.value
        );
    }
}
