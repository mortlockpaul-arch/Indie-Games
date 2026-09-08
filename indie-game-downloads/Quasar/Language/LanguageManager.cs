using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Quasar.ContentPipeline;
using Quasar.Global;

namespace Quasar.Language;

public class LanguageManager
{
	private const string LANGUAGE_DIR = "Texts/";

	private const string XML_LANGUAGE_ROOT = "Language";

	private const string XML_LANG_ATTRIBUTE = "lang";

	private const string XML_NAME_ATTRIBUTE = "name";

	private const string XML_TEXT_ELEMENT = "Text";

	private const string XML_ID_ELEMENT = "id";

	private const string XML_VALUE_ELEMENT = "value";

	private string currentLanguage = "EN";

	private static LanguageManager instance;

	private Dictionary<string, string> texts = new Dictionary<string, string>();

	private List<KeyValuePair<string, string>> languages = new List<KeyValuePair<string, string>>();

	public static string Language
	{
		get
		{
			return Texts.currentLanguage;
		}
		set
		{
			Texts.currentLanguage = value;
			Texts.reloadAllTexts();
		}
	}

	public static LanguageManager Texts
	{
		get
		{
			if (instance == null)
			{
				instance = new LanguageManager();
			}
			return instance;
		}
	}

	public static List<KeyValuePair<string, string>> AvailableLanguages => Texts.languages;

	public string this[string name]
	{
		get
		{
			string value = null;
			if (texts.TryGetValue(name, out value))
			{
				return value;
			}
			return name;
		}
	}

	private LanguageManager()
	{
		loadLanguages();
		checkDefaultLanguage();
		reloadAllTexts();
	}

	private void checkDefaultLanguage()
	{
		currentLanguage = "EN";
		string text = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToUpper(CultureInfo.InvariantCulture);
		foreach (KeyValuePair<string, string> language in languages)
		{
			if (language.Key == text)
			{
				currentLanguage = text;
				break;
			}
		}
	}

	public bool TryGetValue(string name, out string text)
	{
		return texts.TryGetValue(name, out text);
	}

	private void loadLanguages()
	{
		try
		{
			XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("Texts/languages");
			XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
			foreach (XElement item in xDocument.Root.Elements("Language"))
			{
				string attribute = XDocHelper.GetAttribute(item, "lang");
				string attribute2 = XDocHelper.GetAttribute(item, "name");
				languages.Add(new KeyValuePair<string, string>(attribute, attribute2));
			}
		}
		catch (Exception)
		{
		}
	}

	private void reloadAllTexts()
	{
		texts.Clear();
		AddSource(currentLanguage);
	}

	private void AddSource(string language)
	{
		try
		{
			string text = "Texts/" + language + "/";
			DirectoryManager.FileInfo[] files = DirectoryManager.GetFiles(text, recursive: false);
			DirectoryManager.FileInfo[] array = files;
			foreach (DirectoryManager.FileInfo fileInfo in array)
			{
				XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>(text + fileInfo.Name);
				XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
				foreach (XElement item in xDocument.Root.Elements("Text"))
				{
					string attribute = XDocHelper.GetAttribute(item, "id");
					string attribute2 = XDocHelper.GetAttribute(item, "value");
					if (!texts.ContainsKey(attribute))
					{
						texts.Add(attribute, attribute2);
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}
}
