using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

internal struct StringListEnumerator<T> where T : struct
{
	private readonly CultureInfo culture;

	private readonly string text;

	private readonly string listSeparator;

	private readonly TypeConverter converter;

	private int start;

	internal StringListEnumerator(CultureInfo culture, string text)
	{
		if (culture == null)
		{
			culture = CultureInfo.CurrentCulture;
		}
		this.culture = culture;
		listSeparator = culture.TextInfo.ListSeparator;
		this.text = text.Trim();
		converter = TypeDescriptor.GetConverter(typeof(T));
		start = 0;
	}

	internal T Next()
	{
		int num = text.IndexOf(listSeparator, start);
		if (num == -1)
		{
			num = text.Length;
		}
		T result = (T)converter.ConvertFromString(null, culture, text.Substring(start, num - start));
		start = num + listSeparator.Length;
		return result;
	}
}
