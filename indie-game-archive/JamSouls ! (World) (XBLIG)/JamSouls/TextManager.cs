using System.Collections.Generic;
using System.IO;

namespace JamSouls;

public class TextManager
{
	public enum Languages
	{
		ENGLISH = 1,
		FRENCH,
		SPANISH,
		GERMAN,
		ITALIAN,
		CHINESE,
		JAPANESE
	}

	private const char delimiterChar = ';';

	private static List<string> textTable = new List<string>();

	private static Languages m_CurrentLanguage;

	public static bool LoadLanguage(Languages lang, string path)
	{
		m_CurrentLanguage = lang;
		StreamReader streamReader = new StreamReader(path);
		if (streamReader == null)
		{
			return false;
		}
		textTable.Clear();
		StringReader stringReader = new StringReader(streamReader.ReadToEnd());
		string[] array = stringReader.ReadToEnd().Split('|');
		string text = "";
		for (int i = 0; i < array.Length - 1; i++)
		{
			text = array[i];
			string[] array2 = text.Split(';');
			if (array2[(int)m_CurrentLanguage] == null)
			{
				break;
			}
			textTable.Add(array2[(int)m_CurrentLanguage]);
		}
		textTable.Add("NONE");
		stringReader.Close();
		streamReader.Close();
		return true;
	}

	public static string GetText(TextID id)
	{
		return textTable[(int)id];
	}
}
