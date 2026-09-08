using System;
using System.ComponentModel;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class IntegerType
{
	public static int FromString(string Value)
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
					return (int)i64Value;
				}
				return (int)Math.Round(DoubleType.Parse(Value));
			}
			catch (FormatException innerException)
			{
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Integer"), innerException);
			}
		}
	}

	public static int FromObject(object Value)
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
					return unchecked(0 - (convertible.ToBoolean(null) ? 1 : 0));
				case TypeCode.Byte:
					if (!(Value is int result3))
					{
						return convertible.ToByte(null);
					}
					return result3;
				case TypeCode.Int16:
					if (!(Value is int result))
					{
						return convertible.ToInt16(null);
					}
					return result;
				case TypeCode.Int32:
					if (!(Value is int result2))
					{
						return convertible.ToInt32(null);
					}
					return result2;
				case TypeCode.Int64:
					if (Value is long)
					{
						return (int)(long)Value;
					}
					return (int)convertible.ToInt64(null);
				case TypeCode.Single:
					if (Value is float)
					{
						return (int)Math.Round((float)Value);
					}
					return (int)Math.Round(convertible.ToSingle(null));
				case TypeCode.Double:
					if (Value is double)
					{
						return (int)Math.Round((double)Value);
					}
					return (int)Math.Round(convertible.ToDouble(null));
				case TypeCode.Decimal:
					return DecimalToInteger(convertible);
				case TypeCode.String:
					return FromString(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Integer"));
		}
	}

	private static int DecimalToInteger(IConvertible ValueInterface)
	{
		return Convert.ToInt32(ValueInterface.ToDecimal(null));
	}
}
