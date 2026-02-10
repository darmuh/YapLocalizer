using System.Collections.Generic;
using System.Linq;
using MonoMod.Utils;
using UnityEngine;
using YAPYAP;

namespace YapLocalizer;

public class ModLocalizedPhrase
{
    internal static Dictionary<string, ModLocalizedPhrase> ModLocalizedPhraseLookup = [];

    public string LocalizationKey = string.Empty;
    public string[] DefaultPhrase = [];
    internal Dictionary<SystemLanguage, string[]> TranslatedPhrases = [];
    internal bool LocalizationExists
    {
        get
        {
            return TranslatedPhrases.Count > 0;
        }
    }

    /// <summary>Create an instance of ModLocalizedPhrase</summary>
    /// <param name="localizationKey">This is a unique string that identifies your modded phrase to be localized at runtime.</param>
    /// <param name="defaultCommands">This is the default set of words that make up your Modded Phrase. If your phrase cannot be localized this will be the set of commands that are returned.</param>
    /// <remarks>
    /// WARNING: Providing a localizationKey that is already in use (by another mod) will NOT replace the existing localization and will prevent this ModLocalizedPhrase from being added to the list of ModdedLocalizedPhrases.
    /// However, replacing a vanilla localizationKey IS possible.
    /// </remarks>
    public ModLocalizedPhrase(string localizationKey, string[] defaultCommands)
    {
        List<string> ToUpper = [];
        foreach (string command in defaultCommands)
            ToUpper.Add(command.ToUpperInvariant());

        if (ModLocalizedPhraseLookup.ContainsKey(localizationKey))
        {
            Plugin.Log.LogWarning($"WARNING: Unable to override existing modded phrase localization key - {localizationKey}");
            return;
        }

        DefaultPhrase = [.. ToUpper];
        LocalizationKey = localizationKey;
        ModLocalizedPhraseLookup.Add(LocalizationKey, this);
    }

    /// <summary>Add localization value for a specific language</summary>
    /// <param name="language">This is the system language that will display the localization value</param>
    /// <param name="localizedPhrase">This is the localized set of words that corresponds with this ModdedPhrase's LocalizationKey</param>
    /// <remarks>
    /// NOTE: This method can NOT be used to replace an existing localization!
    /// If the game does not have a native localization for the given system language, this localization will never be used.
    /// Typical usecase for this method is during initial setup of the ModLocalizedPhrase
    /// </remarks>
    public ModLocalizedPhrase AddLocalization(SystemLanguage language, string[] localizedPhrase)
    {
        List<string> toUpper = [];
        foreach(string word in localizedPhrase)
        {
            toUpper.Add(word.ToUpperInvariant());
        }

        if (!TranslatedPhrases.TryAdd(language, [.. toUpper]))
        {
            Plugin.Log.LogWarning($"Unable to add {language} translation for key {LocalizationKey}");
            //AddRecognizedPhrase(localizedPhrase, language);
        }

        return this;
    }

    /// <summary>Update existing localization value for a specific language</summary>
    /// <param name="language">This is the system language that will display the localization value</param>
    /// <param name="localizedPhrase">This is the localized set of words that corresponds with this ModdedPhrase's LocalizationKey</param>
    /// <remarks>If the game does not have a native localization for the given system language, this localization will never be used.</remarks>

    public void UpdateLocalization(SystemLanguage language, string[] localizedPhrase)
    {
        List<string> toUpper = [];
        foreach (string word in localizedPhrase)
        {
            toUpper.Add(word.ToUpperInvariant());
        }

        if (!TranslatedPhrases.TryAdd(language, [.. toUpper]))
        {
            Plugin.Log.LogMessage($"Overriding existing localized phrase ({TranslatedPhrases[language].PhraseToString()}) with new localized phrase ({localizedPhrase.PhraseToString()})");
            TranslatedPhrases[language] = [.. toUpper];
        }
    }

    /// <summary>Update game localization value for a specific language at runtime</summary>
    /// <param name="language">This is the system language that we are updating</param>
    /// <remarks>
    /// NOTE: This will restart Vosk and update it's grammar/command values if updating the current language. 
    /// This is somewhat performance intensive and should not be run often.
    /// </remarks>
    public void UpdateGameLocalization(SystemLanguage language)
    {
        if (!Service.Get<VoiceManager>(out var service))
        {
            Plugin.Log.LogWarning("UpdateGameLocalization: Unable to get VoiceManager service!");
            return;
        }

        if (TranslatedPhrases.TryGetValue(language, out string[] command))
        {
            Plugin.Log.LogInfo($"Adding custom localization for {LocalizationKey} in Language {language} - {command.PhraseToString()}");
            AddRecognizedPhrase(service, command, language);
        }
        else
        {
            Plugin.Log.LogInfo($"No custom localization for {LocalizationKey} found in Language {language}\nSetting to DefaultPhrase - {DefaultPhrase.PhraseToString()}");
            AddRecognizedPhrase(service, DefaultPhrase, language);
        }
    }

    internal static void AddLocalizationsAsRecognizedPhrases()
    {
        if (!Service.Get<VoiceManager>(out var service))
        {
            Plugin.Log.LogWarning("Unable to get VoiceManager service!");
            return;
        }
        
        foreach (var localization in ModLocalizedPhraseLookup)
        {
            foreach(var local in service.VoskLocalisations)
            {
                localization.Value.UpdateGameLocalization(local.Language);
            }
        }
    }

    //i dont expect this to be called externally
    internal void AddRecognizedPhrase(VoiceManager service, string[] phrase, SystemLanguage language)
    {
        string phraseString = phrase.PhraseToString();
        Plugin.Log.LogDebug($"AddRecognizedPhrase {LocalizationKey}");
        if(service._voskTranslatorLookup.TryGetValue(language, out var voskTranslator))
        {
            bool isDefault = service._defaultTranslator == voskTranslator;
            bool isCurrent = service._currentVoskTranslator == voskTranslator;
            Plugin.Log.LogDebug($"Working on translator isDefault - {isDefault} isCurrent - {isCurrent}");

            Dictionary<string, string[]> languageCommands = [];
            languageCommands.AddRange(voskTranslator._commands);

            if (languageCommands.TryGetValue(LocalizationKey, out string[]? value))
            {
                Plugin.Log.LogMessage($"Replacing commands ({value.PhraseToString()}) for {LocalizationKey} in language {language} with modded phrase - {phraseString}");
                languageCommands[LocalizationKey] = phrase;
            }
            else
            {
                Plugin.Log.LogInfo($"Adding modded phrase ({phraseString}) to key - {LocalizationKey}");
            }
                
            List<string> grammar = [.. voskTranslator._grammar];

            foreach(string word in phrase)
            {
                if (grammar.Any(x => x.Equals(word, System.StringComparison.InvariantCultureIgnoreCase)))
                    continue;
                else
                {
                    grammar.Add(word);
                    Plugin.Log.LogDebug($"Added recognized word - {word}");
                }    
            }

            VoiceManager.VoskTranslator updatedTranslator = new(languageCommands, [.. grammar]);

            service._voskTranslatorLookup[language] = updatedTranslator;

            if (isDefault)
                service._defaultTranslator = updatedTranslator;

            if (isCurrent)
            {
                service._currentVoskTranslator = updatedTranslator;

                //only reset language if initializing is done and not currently initializing
                if(service.Vosk._didInit && !service.Vosk._isInitializing)
                    service.SetLanguage(language);
            }
        }
    }

    /// <summary>Get this ModLocalizedPhrase's localization value for a specific language</summary>
    /// <param name="language">This is the system language we are checking</param>
    /// <remarks>If there is no localization for this language, the DefaultPhrase is returned.</remarks>
    public string[] GetLocalized(SystemLanguage language)
    {
        if (!LocalizationExists || !TranslatedPhrases.TryGetValue(language, out string[]? value))
        {
            return DefaultPhrase;
        }
        else
        {
            return value;
        }
    }

    /// <summary>Get ModLocalizedPhrase instance via existing key</summary>
    /// <param name="key">This is the LocalizationKey that we are searching for</param>
    /// <remarks>Used by SpellCustomizer to update localizations of vanilla keys</remarks>
    public static bool TryGetExistingKey(string key, out ModLocalizedPhrase phrase)
    {
        if(ModLocalizedPhraseLookup.TryGetValue(key, out phrase))
        {
            return true;
        }

        return false;
    }
}
