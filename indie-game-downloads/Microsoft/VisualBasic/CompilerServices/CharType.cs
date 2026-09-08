using System;
using System.ComponentModel;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class CharType
{
	public static char FromString(string Value)
	{
		if (Value == null || Value.Length == 0)
		{
			return '\0';
		}
		return Value[0];
	}

	public static char FromObject(object Value)
	{
		if (Value == null)
		{
			return '\0';
		}
		if (Value is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.Char:
				return convertible.ToChar(null);
			case TypeCode.String:
				return FromString(convertible.ToString(null));
			}
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Char"));
	}
}
