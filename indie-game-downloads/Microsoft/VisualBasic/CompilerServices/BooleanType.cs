using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class BooleanType
{
	public static bool FromString(string Value)
	{
		if (Value == null)
		{
			Value = "";
		}
		try
		{
			CultureInfo cultureInfo = Utils.GetCultureInfo();
			if (string.Compare(Value, bool.FalseString, ignoreCase: true, cultureInfo) == 0)
			{
				return false;
			}
			if (string.Compare(Value, bool.TrueString, ignoreCase: true, cultureInfo) == 0)
			{
				return true;
			}
			long i64Value = default(long);
			if (Utils.IsHexOrOctValue(Value, ref i64Value))
			{
				return i64Value != 0;
			}
			return DoubleType.Parse(Value) != 0.0;
		}
		catch (FormatException innerException)
		{
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Boolean"), innerException);
		}
	}

	public static bool FromObject(object Value)
	{
		if (Value == null)
		{
			return false;
		}
		if (Value is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.Boolean:
				if (!(Value is bool result))
				{
					return convertible.ToBoolean(null);
				}
				return result;
			case TypeCode.Byte:
				if (Value is byte)
				{
					return (byte)Value != 0;
				}
				return convertible.ToByte(null) != 0;
			case TypeCode.Int16:
				if (Value is short)
				{
					return (short)Value != 0;
				}
				return convertible.ToInt16(null) != 0;
			case TypeCode.Int32:
				if (Value is int)
				{
					return (int)Value != 0;
				}
				return convertible.ToInt32(null) != 0;
			case TypeCode.Int64:
				if (Value is long)
				{
					return (long)Value != 0;
				}
				return convertible.ToInt64(null) != 0;
			case TypeCode.Single:
				if (Value is float)
				{
					return (float)Value != 0f;
				}
				return convertible.ToSingle(null) != 0f;
			case TypeCode.Double:
				if (Value is double)
				{
					return (double)Value != 0.0;
				}
				return convertible.ToDouble(null) != 0.0;
			case TypeCode.Decimal:
				return DecimalToBoolean(convertible);
			case TypeCode.String:
				if (Value is string value)
				{
					return FromString(value);
				}
				return FromString(convertible.ToString(null));
			}
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Boolean"));
	}

	private static bool DecimalToBoolean(IConvertible ValueInterface)
	{
		return Convert.ToBoolean(ValueInterface.ToDecimal(null));
	}
}
