using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class DateType
{
	public static DateTime FromString(string Value)
	{
		return FromString(Value, Utils.GetCultureInfo());
	}

	public static DateTime FromString(string Value, CultureInfo culture)
	{
		DateTime Result = default(DateTime);
		if (TryParse(Value, ref Result))
		{
			return Result;
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Date"));
	}

	public static DateTime FromObject(object Value)
	{
		if (Value == null)
		{
			return DateTime.MinValue;
		}
		if (Value is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.DateTime:
				return convertible.ToDateTime(null);
			case TypeCode.String:
				return FromString(convertible.ToString(null), Utils.GetCultureInfo());
			}
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Date"));
	}

	internal static bool TryParse(string Value, ref DateTime Result)
	{
		CultureInfo cultureInfo = Utils.GetCultureInfo();
		return DateTime.TryParse(Utils.ToHalfwidthNumbers(Value, cultureInfo), cultureInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault, out Result);
	}
}
