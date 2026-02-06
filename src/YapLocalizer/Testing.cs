using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace YapLocalizer;

internal class Testing
{
    internal static void TestPhrase()
    {
        ModLocalizedPhrase newPhrase = new ModLocalizedPhrase("SPELL_BAS_UPDOG", ["up", "ease"])
            .AddLocalization(SystemLanguage.Italian, ["cane", "alto"]);
    }

    internal static void TestText()
    {
        ModLocalizedText testLocal = new ModLocalizedText("TEST_LOCAL_KEY")
            .SetLocalization(SystemLanguage.English, "Test");

        ModLocalizedText testNoTranslation = new ModLocalizedText("TEST2_KEY");

    }
}
