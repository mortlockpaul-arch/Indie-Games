using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class SingleType
{
	public static float FromString(string Value)
	{
		return FromString(Value, null);
	}

	public static float FromString(string Value, NumberFormatInfo NumberFormat)
	{
		if (Value == null)
		{
			return 0f;
		}
		try
		{
			long i64Value = default(long);
			if (Utils.IsHexOrOctValue(Value, ref i64Value))
			{
				return i64Value;
			}
			double num = DoubleType.Parse(Value, NumberFormat);
			if ((num < -3.4028234663852886E+38 || num > 3.4028234663852886E+38) && !double.IsInfinity(num))
			{
				throw new OverflowException();
			}
			return (float)num;
		}
		catch (FormatException innerException)
		{
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Single"), innerException);
		}
	}

	public static float FromObject(object Value)
	{
		return FromObject(Value, null);
	}

	public static float FromObject(object Value, NumberFormatInfo NumberFormat)
	{
		if (Value == null)
		{
			return 0f;
		}
		if (Value is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.Boolean:
				return 0 - (convertible.ToBoolean(null) ? 1 : 0);
			case TypeCode.Byte:
				if (Value is byte)
				{
					return (int)(byte)Value;
				}
				return (int)convertible.ToByte(null);
			case TypeCode.Int16:
				if (Value is short)
				{
					return (short)Value;
				}
				return convertible.ToInt16(null);
			case TypeCode.Int32:
				if (Value is int)
				{
					return (int)Value;
				}
				return convertible.ToInt32(null);
			case TypeCode.Int64:
				if (Value is long)
				{
					return (long)Value;
				}
				return convertible.ToInt64(null);
			case TypeCode.Single:
				if (!(Value is float result))
				{
					return convertible.ToSingle(null);
				}
				return result;
			case TypeCode.Double:
				if (Value is double)
				{
					return (float)(double)Value;
				}
				return (float)convertible.ToDouble(null);
			case TypeCode.Decimal:
				return DecimalToSingle(convertible);
			case TypeCode.String:
				return FromString(convertible.ToString(null), NumberFormat);
			}
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Single"));
	}

	private static float DecimalToSingle(IConvertible ValueInterface)
	{
		return Convert.ToSingle(ValueInterface.ToDecimal(null));
	}
}
