# Changelog

## 0.1.3
 - Fixed ModLocalizedPhrase for latest game update (not sure when this broke)
	- If a localization is added for a language, I toggle it's optional value so it shows up in the menu.
	- This allows for some localizations that have not been set up by the developers to be used, the following languages are possible:
		- ChineseSimplified, English, French, German, Italian, Japanese, Korean, Polish, Portuguese, Russian, Spanish, Turkish, Ukrainian 
 - Made some logging adjustments
 - Now generating documentation file from xml comments to include in release

## 0.1.2
 - Per recommendations from Robyn, converted some internal List<T> to Dictionary types for better lookup performance.
	- No more LINQ :)
 - LocalizationKeys are also no longer case sensitive, go wIlD!

## 0.1.1
 - Added ``HasLocal`` to ModLocalizedText
 - Uploaded to nuget for those who do not want to depend on this mod via Aaron's thunderstore nuget

## 0.1.0
 - Initial release.
