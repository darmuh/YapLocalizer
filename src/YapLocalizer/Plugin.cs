using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using YAPYAP;

namespace YapLocalizer;

[BepInAutoPlugin]
internal partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;
        Log.LogDebug($"{Name} startup...");
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        Log.LogInfo($"Plugin {Name} is loaded!");
        //Testing.TestText();
    }

    [HarmonyPatch(typeof(VoiceManager), nameof(VoiceManager.LoadLocalisationData))]
    internal class HookVoiceManager
    {
        public static void Postfix()
        {
            Log.LogDebug("Adding modded phrases");
            ModLocalizedPhrase.AddLocalizationsAsRecognizedPhrases();
        }
    }

    //ModLocalizedText hook
    [HarmonyPatch(typeof(Translator), nameof(Translator.Translate), argumentTypes: [typeof(string), typeof(UnityEngine.Object)])]
    internal class LocalizeModdedText
    {
        public static bool Prefix(Translator __instance, string key, ref string __result)
        {
            if (ModLocalizedText.ModLocalizationKeyLookup.Count == 0)
                return true;

            if (ModLocalizedText.ModLocalizationKeyLookup.TryGetValue(key, out ModLocalizedText moddedResult))
            {
                __result = moddedResult.GetLocalized(__instance.Language);
                return false;
            }

            return true;
        }
    }

    //ModLocalizedPhrase hook
    [HarmonyPatch(typeof(VoiceManager), nameof(VoiceManager.TryGetVoiceCommand))]
    internal class LocalizeModdedVoice
    {
        public static bool Prefix(VoiceManager __instance, string key, ref bool __result, out string[] command)
        {
            Log.LogDebug($"TryGetVoiceCommand - Checking key {key}");
            command = []; //this will get updated when original method runs

            if (ModLocalizedPhrase.ModLocalizedPhraseLookup.Count == 0)
                return true;

            if (ModLocalizedPhrase.ModLocalizedPhraseLookup.TryGetValue(key, out ModLocalizedPhrase moddedResult))
            {
                Log.LogDebug($"Returning command for modded localized phrase with key {moddedResult.LocalizationKey}");
                command = moddedResult.GetLocalized(__instance.CurrentVoskLocalisation.Language);
                __result = true;
                return false;
            }

            return true;
        }
    }

}
