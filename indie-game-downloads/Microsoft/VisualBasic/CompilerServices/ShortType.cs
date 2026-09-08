using System;
using System.ComponentModel;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ShortType
{
	public static short FromString(string Value)
	{
		if (Value == null)
		{
			return 0;
		}
		checked
		{
			try
			{
				long i64Value = default(long);
				if (Utils.IsHexOrOctValue(Value, ref i64Value))
				{
					return (short)i64Value;
				}
				return (short)Math.Round(DoubleType.Parse(Value));
			}
			catch (FormatException innerException)
			{
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Short"), innerException);
			}
		}
	}

	public static short FromObject(object Value)
	{
		if (Value == null)
		{
			return 0;
		}
		checked
		{
			if (Value is IConvertible convertible)
			{
				switch (convertible.GetTypeCode())
				{
				case TypeCode.Boolean:
					return unchecked((short)(0 - (convertible.ToBoolean(null) ? 1 : 0)));
				case TypeCode.Byte:
					if (!(Value is short result))
					{
						return convertible.ToByte(null);
					}
					return result;
				case TypeCode.Int16:
					if (!(Value is short result2))
					{
						return convertible.ToInt16(null);
					}
					return result2;
				case TypeCode.Int32:
					if (Value is int)
					{
						return (short)(int)Value;
					}
					return (short)convertible.ToInt32(null);
				case TypeCode.Int64:
					if (Value is long)
					{
						return (short)(long)Value;
					}
					return (short)convertible.ToInt64(null);
				case TypeCode.Single:
					if (Value is float)
					{
						return (short)Math.Round((float)Value);
					}
					return (short)Math.Round(convertible.ToSingle(null));
				case TypeCode.Double:
					if (Value is double)
					{
						return (short)Math.Round((double)Value);
					}
					return (short)Math.Round(convertible.ToDouble(null));
				case TypeCode.Decimal:
					return DecimalToShort(convertible);
				case TypeCode.String:
					return FromString(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Short"));
		}
	}

	private static short DecimalToShort(IConvertible ValueInterface)
	{
		return Convert.ToInt16(ValueInterface.ToDecimal(null));
	}
}
