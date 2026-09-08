using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MonoMod.Utils;

public static class StringInjectExtension
{
	public static string Inject(this string formatString, object injectionObject)
	{
		if (injectionObject is IDictionary)
		{
			return formatString.Inject(new Hashtable((IDictionary)injectionObject));
		}
		return formatString.Inject(GetPropertyHash(injectionObject));
	}

	public static string Inject(this string formatString, IDictionary dictionary)
	{
		return formatString.Inject(new Hashtable(dictionary));
	}

	public static string Inject(this string formatString, Hashtable attributes)
	{
		string text = formatString;
		if (attributes == null || formatString == null)
		{
			return text;
		}
		foreach (string key in attributes.Keys)
		{
			text = text.InjectSingleValue(key, attributes[key]);
		}
		return text;
	}

	public static string InjectSingleValue(this string formatString, string key, object replacementValue)
	{
		string text = formatString;
		foreach (Match item in new Regex("{(" + key + ")(?:}|(?::(.[^}]*)}))").Matches(formatString))
		{
			string text2 = item.ToString();
			if (item.Groups[2].Length > 0)
			{
				string format = string.Format(CultureInfo.InvariantCulture, "{{0:{0}}}", item.Groups[2]);
				text2 = string.Format(CultureInfo.CurrentCulture, format, replacementValue);
			}
			else
			{
				text2 = (replacementValue ?? string.Empty).ToString();
			}
			text = text.Replace(item.ToString(), text2);
		}
		return text;
	}

	private static Hashtable GetPropertyHash(object properties)
	{
		Hashtable hashtable = null;
		if (properties != null)
		{
			hashtable = new Hashtable();
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(properties))
			{
				hashtable.Add(property.Name, property.GetValue(properties));
			}
		}
		return hashtable;
	}
}
