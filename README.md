# YapLocalizer

**Utility mod for adding localized text to the game!**

## Features

This utility mod is designed for mod developers to make adding new localizations quick and easy!

- Add Localized Text or Verbal Commands easily with ``ModLocalizedText`` and ``ModLocalizedPhrase``


### ModLocalizedText - Adds modded localized text with optional translations
Simply define a key and any translations you'd like to add. 

Example:
``` ModLocalizedText testLocal = new ModLocalizedText("TEST_LOCAL_KEY")
            .SetLocalization(SystemLanguage.English, "Test");
```
- The above example would translate any localized text with key ``TEST_LOCAL_KEY`` to ``Test`` when the game language is set to english.

*NOTE: If you do not set any translations, the LocalizationKey will be displayed instead.*  
An Optional ``fallbackResult`` can be defined that will be displayed when localization fails.  

### ModLocalizedPhrase - Adds modded voice commands with optional localizations
Define a key and any translations you'd like to add. Constructor takes a "Default Command" for when localization fails.

Example:
```ModLocalizedPhrase newPhrase = new ModLocalizedPhrase("SPELL_BAS_UPDOG", ["up", "ease"])
            .AddLocalization(SystemLanguage.Italian, ["cane", "alto"]);
```
- The above example would replace ``UP DOG's`` localization values.
- It defines a localization for Italian, which currently is not supported in-game.
- If YAPYAP were to include an updated localization for italian, the above localization would be used.
- Any other undefined language currently in the game will use the default command, ``UP EASE`` unless otherwise specified via ``AddLocalization``
