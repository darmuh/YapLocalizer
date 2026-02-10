using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YapLocalizer;

/// <summary>
/// Allows for customized text localization using new keys.
/// </summary>
public class ModLocalizedText
{
    internal static Dictionary<string, ModLocalizedText> ModLocalizationKeyLookup = [];

    public string LocalizationKey = string.Empty;
    public string FallbackResult = string.Empty;
    internal Dictionary<SystemLanguage, string> ItemTranslations = [];
    internal bool LocalizationExists
    {
        get
        {
            return ItemTranslations.Count > 0;
        }
    }

    /// <summary>Create an instance of ModLocalizedText</summary>
    /// <param name="localizationKey">This is a unique string that identifies your modded text to be localized at runtime.</param>
    /// <param name="fallbackResult">This string represents the text to display when localization fails. If left empty, your localizationKey will be displayed.</param>
    /// <remarks>WARNING: Providing a localizationKey that is already in use will NOT replace the existing localization and will prevent this ModLocalizedText from being added to the list of ModLocalizationKeyLookups.</remarks>
    public ModLocalizedText(string localizationKey, string fallbackResult = "")
    {
        if(ModLocalizationKeyLookup.ContainsKey(localizationKey))
        {
            Plugin.Log.LogWarning($"WARNING: Unable to override existing modded text localization key - {localizationKey}");
            return;
        }

        if(!string.IsNullOrWhiteSpace(fallbackResult))
            FallbackResult = fallbackResult;
        else
            FallbackResult = localizationKey;

        LocalizationKey = localizationKey;
        ModLocalizationKeyLookup.Add(LocalizationKey, this);
    }

    /// <summary>Set localization value for a specific language</summary>
    /// <param name="language">This is the system language that will display the localization value</param>
    /// <param name="localizedText">This is the localization value that will be displayed for the language defined in the first parameter</param>
    /// <remarks>
    /// If the game does not have a native localization for the given system language, this localization will never be displayed.
    /// This method cannot be used to Update an existing language's localized text. For that, see UpdateLocalization.
    /// </remarks>
    public ModLocalizedText SetLocalization(SystemLanguage language, string localizedText)
    {
        if(!ItemTranslations.TryAdd(language, localizedText))
        {
            Plugin.Log.LogWarning($"Unable to add {language} translation for key {LocalizationKey}");
        }

        return this;
    }

    /// <summary>Update localization value for a specific language</summary>
    /// <param name="language">This is the system language that will display the localization value</param>
    /// <param name="localizedText">This is the localization value that will be displayed for the language defined in the first parameter</param>
    /// <remarks>If the game does not have a native localization for the given system language, this localization will never be displayed.</remarks>
    public void UpdateLocalization(SystemLanguage language, string localizedText)
    {
        if (!ItemTranslations.TryAdd(language, localizedText))
        {
            ItemTranslations[language] = localizedText;
        }
    }

    /// <summary>Get this ModLocalizedText's localization value for a specific language</summary>
    /// <param name="language">This is the system language we are checking</param>
    /// <remarks>If there is no localization for this language, the FallbackResult is returned.</remarks>
    public string GetLocalized(SystemLanguage language)
    {
        if (!LocalizationExists || !ItemTranslations.TryGetValue(language, out string? value))
        {
            return FallbackResult;
        }
        else
        {
            return value;
        }
    }

    /// <summary>Check if this ModLocalizedText has a specific localization</summary>
    /// <param name="language">This is the system language we are checking</param>
    /// <param name="value">This is the system language we are checking</param>
    public bool HasLocal(SystemLanguage language, string value)
    {
        if (ItemTranslations.TryGetValue(language, out string? result))
        {
            return result.Equals(value, System.StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }
}