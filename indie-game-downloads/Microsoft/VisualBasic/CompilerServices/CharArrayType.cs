using System;
using System.ComponentModel;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class CharArrayType
{
	public static char[] FromString(string Value)
	{
		if (Value == null)
		{
			Value = "";
		}
		return Value.ToCharArray();
	}

	public static char[] FromObject(object Value)
	{
		if (Value == null)
		{
			return "".ToCharArray();
		}
		if (Value is char[] { Rank: 1 } array)
		{
			return array;
		}
		if (Value is IConvertible convertible && convertible.GetTypeCode() == TypeCode.String)
		{
			return convertible.ToString(null).ToCharArray();
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Char()"));
	}
}
