using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

// Clase encargada de cambiar el idioma de las opciones del daltonismo
public class DropdownLanguage : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public string tableName = "P3 Texto";
    public List<string> languageKeys;

    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        ActualizarTextos();
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        ActualizarTextos();
    }

    public void ActualizarTextos()
    {
        if (dropdown == null || languageKeys.Count == 0) return;

        // Recorremos las opciones del dropdown y las traducimos una a una
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (i < languageKeys.Count)
            {
                dropdown.options[i].text = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, languageKeys[i]);
            }
        }

        dropdown.RefreshShownValue();
    }
}
