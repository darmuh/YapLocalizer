
using System.Collections.Generic;
using UnityEngine;
using YAPYAP;

namespace YapLocalizer;

public static class UtilExtensions
{
    public static string PhraseToString(this string[] phrase)
    {
        return string.Join(" ", phrase);
    }

    public static string[] StringToPhrase(this string value)
    {
        return value.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
    }

    public static string SafeCommandString(this string input)
    {
        string returnString = string.Empty;
        foreach (char c in input)
        {
            if (char.IsLetter(c) || c == ' ')
                returnString += c;
        }

        return returnString;
    }

    //helper method now that the dictionaries have been removed from the game
    internal static bool TryGetVoskTranslator(this VoiceManager service, SystemLanguage language, out VoiceManager.VoskTranslator voskTranslator, out int LangIndex)
    {
        voskTranslator = null!;
        List<VoskLocalisation> localList = [.. service._voskLocalisations];
        LangIndex = localList.FindIndex(x => x.Language == language);

        if (LangIndex == -1)
            return false;

        if (service._voskTranslators.Count <= LangIndex)
            return false;

        voskTranslator = service._voskTranslators[LangIndex];

        return voskTranslator != null;
    }
}
