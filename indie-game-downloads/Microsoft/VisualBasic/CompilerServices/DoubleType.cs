using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class DoubleType
{
	public static double FromString(string Value)
	{
		return FromString(Value, null);
	}

	public static double FromString(string Value, NumberFormatInfo NumberFormat)
	{
		if (Value == null)
		{
			return 0.0;
		}
		try
		{
			long i64Value = default(long);
			if (Utils.IsHexOrOctValue(Value, ref i64Value))
			{
				return i64Value;
			}
			return Parse(Value, NumberFormat);
		}
		catch (FormatException innerException)
		{
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Double"), innerException);
		}
	}

	public static double FromObject(object Value)
	{
		return FromObject(Value, null);
	}

	public static double FromObject(object Value, NumberFormatInfo NumberFormat)
	{
		if (Value == null)
		{
			return 0.0;
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
				if (Value is float)
				{
					return (float)Value;
				}
				return convertible.ToSingle(null);
			case TypeCode.Double:
				if (Value is double)
				{
					return (double)Value;
				}
				return convertible.ToDouble(null);
			case TypeCode.Decimal:
				return DecimalToDouble(convertible);
			case TypeCode.String:
				return FromString(convertible.ToString(null), NumberFormat);
			}
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Double"));
	}

	private static double DecimalToDouble(IConvertible ValueInterface)
	{
		return Convert.ToDouble(ValueInterface.ToDecimal(null));
	}

	public static double Parse(string Value)
	{
		return Parse(Value, null);
	}

	internal static bool TryParse(string Value, ref double Result)
	{
		CultureInfo cultureInfo = Utils.GetCultureInfo();
		NumberFormatInfo numberFormat = cultureInfo.NumberFormat;
		NumberFormatInfo normalizedNumberFormat = DecimalType.GetNormalizedNumberFormat(numberFormat);
		Value = Utils.ToHalfwidthNumbers(Value, cultureInfo);
		if (numberFormat == normalizedNumberFormat)
		{
			return double.TryParse(Value, NumberStyles.Any, normalizedNumberFormat, out Result);
		}
		try
		{
			Result = double.Parse(Value, NumberStyles.Any, normalizedNumberFormat);
			return true;
		}
		catch (FormatException)
		{
			try
			{
				return double.TryParse(Value, NumberStyles.Any, numberFormat, out Result);
			}
			catch (ArgumentException)
			{
				return false;
			}
		}
		catch (StackOverflowException ex3)
		{
			throw ex3;
		}
		catch (OutOfMemoryException ex4)
		{
			throw ex4;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static double Parse(string Value, NumberFormatInfo NumberFormat)
	{
		CultureInfo cultureInfo = Utils.GetCultureInfo();
		if (NumberFormat == null)
		{
			NumberFormat = cultureInfo.NumberFormat;
		}
		NumberFormatInfo normalizedNumberFormat = DecimalType.GetNormalizedNumberFormat(NumberFormat);
		Value = Utils.ToHalfwidthNumbers(Value, cultureInfo);
		try
		{
			return double.Parse(Value, NumberStyles.Any, normalizedNumberFormat);
		}
		catch (FormatException) when (NumberFormat != normalizedNumberFormat)
		{
			return double.Parse(Value, NumberStyles.Any, NumberFormat);
		}
		catch (Exception ex2)
		{
			throw ex2;
		}
	}
}
