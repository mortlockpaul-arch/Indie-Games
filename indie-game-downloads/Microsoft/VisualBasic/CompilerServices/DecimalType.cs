using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class DecimalType
{
	public static decimal FromBoolean(bool Value)
	{
		if (Value)
		{
			return -1m;
		}
		return 0m;
	}

	public static decimal FromString(string Value)
	{
		return FromString(Value, null);
	}

	public static decimal FromString(string Value, NumberFormatInfo NumberFormat)
	{
		if (Value == null)
		{
			return 0m;
		}
		try
		{
			long i64Value = default(long);
			if (Utils.IsHexOrOctValue(Value, ref i64Value))
			{
				return new decimal(i64Value);
			}
			return Parse(Value, NumberFormat);
		}
		catch (OverflowException)
		{
			throw ExceptionUtils.VbMakeException(6);
		}
		catch (FormatException)
		{
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Decimal"));
		}
	}

	public static decimal FromObject(object Value)
	{
		return FromObject(Value, null);
	}

	public static decimal FromObject(object Value, NumberFormatInfo NumberFormat)
	{
		decimal result;
		if (Value == null)
		{
			result = 0m;
		}
		else
		{
			if (!(Value is IConvertible convertible))
			{
				goto IL_0100;
			}
			switch (convertible.GetTypeCode())
			{
			case TypeCode.Boolean:
				return FromBoolean(convertible.ToBoolean(null));
			case TypeCode.Byte:
				break;
			case TypeCode.Int16:
				goto IL_0096;
			case TypeCode.Int32:
				goto IL_00a6;
			case TypeCode.Int64:
				goto IL_00b6;
			case TypeCode.Single:
				goto IL_00c6;
			case TypeCode.Double:
				goto IL_00d6;
			case TypeCode.Decimal:
				return convertible.ToDecimal(null);
			case TypeCode.String:
				return FromString(convertible.ToString(null), NumberFormat);
			default:
				goto IL_0100;
			}
			result = new decimal(convertible.ToByte(null));
		}
		goto IL_011b;
		IL_00d6:
		result = new decimal(convertible.ToDouble(null));
		goto IL_011b;
		IL_00c6:
		result = new decimal(convertible.ToSingle(null));
		goto IL_011b;
		IL_0100:
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Decimal"));
		IL_011b:
		return result;
		IL_0096:
		result = new decimal(convertible.ToInt16(null));
		goto IL_011b;
		IL_00a6:
		result = new decimal(convertible.ToInt32(null));
		goto IL_011b;
		IL_00b6:
		result = new decimal(convertible.ToInt64(null));
		goto IL_011b;
	}

	public static decimal Parse(string Value, NumberFormatInfo NumberFormat)
	{
		CultureInfo cultureInfo = Utils.GetCultureInfo();
		if (NumberFormat == null)
		{
			NumberFormat = cultureInfo.NumberFormat;
		}
		NumberFormatInfo normalizedNumberFormat = GetNormalizedNumberFormat(NumberFormat);
		Value = Utils.ToHalfwidthNumbers(Value, cultureInfo);
		try
		{
			return decimal.Parse(Value, NumberStyles.Any, normalizedNumberFormat);
		}
		catch (FormatException) when (NumberFormat != normalizedNumberFormat)
		{
			return decimal.Parse(Value, NumberStyles.Any, NumberFormat);
		}
		catch (Exception ex2)
		{
			throw ex2;
		}
	}

	internal static NumberFormatInfo GetNormalizedNumberFormat(NumberFormatInfo InNumberFormat)
	{
		NumberFormatInfo numberFormatInfo = InNumberFormat;
		if (numberFormatInfo.CurrencyDecimalSeparator != null && numberFormatInfo.NumberDecimalSeparator != null && numberFormatInfo.CurrencyGroupSeparator != null && numberFormatInfo.NumberGroupSeparator != null && numberFormatInfo.CurrencyDecimalSeparator.Length == 1 && numberFormatInfo.NumberDecimalSeparator.Length == 1 && numberFormatInfo.CurrencyGroupSeparator.Length == 1 && numberFormatInfo.NumberGroupSeparator.Length == 1 && numberFormatInfo.CurrencyDecimalSeparator[0] == numberFormatInfo.NumberDecimalSeparator[0] && numberFormatInfo.CurrencyGroupSeparator[0] == numberFormatInfo.NumberGroupSeparator[0] && numberFormatInfo.CurrencyDecimalDigits == numberFormatInfo.NumberDecimalDigits)
		{
			return InNumberFormat;
		}
		numberFormatInfo = null;
		NumberFormatInfo numberFormatInfo2 = InNumberFormat;
		checked
		{
			if (numberFormatInfo2.CurrencyDecimalSeparator != null && numberFormatInfo2.NumberDecimalSeparator != null && numberFormatInfo2.CurrencyDecimalSeparator.Length == numberFormatInfo2.NumberDecimalSeparator.Length && numberFormatInfo2.CurrencyGroupSeparator != null && numberFormatInfo2.NumberGroupSeparator != null && numberFormatInfo2.CurrencyGroupSeparator.Length == numberFormatInfo2.NumberGroupSeparator.Length)
			{
				int num = numberFormatInfo2.CurrencyDecimalSeparator.Length - 1;
				int num2 = 0;
				while (true)
				{
					if (num2 <= num)
					{
						if (numberFormatInfo2.CurrencyDecimalSeparator[num2] != numberFormatInfo2.NumberDecimalSeparator[num2])
						{
							break;
						}
						num2++;
						continue;
					}
					int num3 = numberFormatInfo2.CurrencyGroupSeparator.Length - 1;
					num2 = 0;
					while (true)
					{
						if (num2 <= num3)
						{
							if (numberFormatInfo2.CurrencyGroupSeparator[num2] != numberFormatInfo2.NumberGroupSeparator[num2])
							{
								break;
							}
							num2++;
							continue;
						}
						return InNumberFormat;
					}
					break;
				}
			}
			else
			{
				numberFormatInfo2 = null;
			}
			NumberFormatInfo obj = (NumberFormatInfo)InNumberFormat.Clone();
			obj.CurrencyDecimalSeparator = obj.NumberDecimalSeparator;
			obj.CurrencyGroupSeparator = obj.NumberGroupSeparator;
			obj.CurrencyDecimalDigits = obj.NumberDecimalDigits;
			_ = null;
			return obj;
		}
	}
}
