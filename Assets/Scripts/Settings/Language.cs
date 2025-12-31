using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

// Clase para gestionar el cambio de idioma
public class Language : MonoBehaviour
{
    // Referencia
    public TMP_Dropdown dropdown;

    void OnEnable()
    {
        StartCoroutine(UpdateDropdown());
    }

    IEnumerator UpdateDropdown()
    {
        yield return LocalizationSettings.InitializationOperation;

        var selectedLocale = LocalizationSettings.SelectedLocale;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            if (LocalizationSettings.AvailableLocales.Locales[i] == selectedLocale)
            {
                dropdown.SetValueWithoutNotify(i);
                break;
            }
        }
    }

    void Start()
    {
        dropdown.onValueChanged.AddListener(indice => {
            ScenesManager.Instance.ChangeLanguage(indice);
        });
    }
}
