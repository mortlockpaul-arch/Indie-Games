using System;
using System.ComponentModel;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class LongType
{
	public static long FromString(string Value)
	{
		if (Value == null)
		{
			return 0L;
		}
		try
		{
			long i64Value = default(long);
			if (Utils.IsHexOrOctValue(Value, ref i64Value))
			{
				return i64Value;
			}
			return Convert.ToInt64(DecimalType.Parse(Value, null));
		}
		catch (FormatException innerException)
		{
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Long"), innerException);
		}
	}

	public static long FromObject(object Value)
	{
		if (Value == null)
		{
			return 0L;
		}
		checked
		{
			if (Value is IConvertible convertible)
			{
				switch (convertible.GetTypeCode())
				{
				case TypeCode.Boolean:
					return unchecked(0 - (convertible.ToBoolean(null) ? 1 : 0));
				case TypeCode.Byte:
					if (Value is byte)
					{
						return (byte)Value;
					}
					return convertible.ToByte(null);
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
					if (!(Value is long result))
					{
						return convertible.ToInt64(null);
					}
					return result;
				case TypeCode.Single:
					if (Value is float)
					{
						return (long)Math.Round((float)Value);
					}
					return (long)Math.Round(convertible.ToSingle(null));
				case TypeCode.Double:
					if (Value is double)
					{
						return (long)Math.Round((double)Value);
					}
					return (long)Math.Round(convertible.ToDouble(null));
				case TypeCode.Decimal:
					return DecimalToLong(convertible);
				case TypeCode.String:
					return FromString(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Long"));
		}
	}

	private static long DecimalToLong(IConvertible ValueInterface)
	{
		return Convert.ToInt64(ValueInterface.ToDecimal(null));
	}
}
