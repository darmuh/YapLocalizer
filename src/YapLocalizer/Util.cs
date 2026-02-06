
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
}
