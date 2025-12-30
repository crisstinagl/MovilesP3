using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Language : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    void Start()
    {
        int currentLocaleIndex = 0;
        var selectedLocale = LocalizationSettings.SelectedLocale;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            if (LocalizationSettings.AvailableLocales.Locales[i] == selectedLocale)
            {
                currentLocaleIndex = i;
                break;
            }
        }
        dropdown.value = currentLocaleIndex;

        dropdown.onValueChanged.AddListener(CambiarIdioma);
    }

    public void CambiarIdioma(int indice)
    {
        StartCoroutine(SetLocale(indice));
    }

    IEnumerator SetLocale(int _localeID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
    }
}
