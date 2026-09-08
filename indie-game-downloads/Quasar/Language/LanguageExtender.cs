namespace Quasar.Language;

public static class LanguageExtender
{
	public static string Translate(this string textKey)
	{
		return LanguageManager.Texts[textKey];
	}
}
