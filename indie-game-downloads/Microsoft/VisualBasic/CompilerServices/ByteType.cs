using System;
using System.ComponentModel;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ByteType
{
	public static byte FromString(string Value)
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
					return (byte)i64Value;
				}
				return (byte)Math.Round(DoubleType.Parse(Value));
			}
			catch (FormatException innerException)
			{
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Byte"), innerException);
			}
		}
	}

	public static byte FromObject(object Value)
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
					return unchecked((byte)(0u - (convertible.ToBoolean(null) ? 1u : 0u)));
				case TypeCode.Byte:
					if (!(Value is byte result))
					{
						return convertible.ToByte(null);
					}
					return result;
				case TypeCode.Int16:
					if (Value is short)
					{
						return (byte)(short)Value;
					}
					return (byte)convertible.ToInt16(null);
				case TypeCode.Int32:
					if (Value is int)
					{
						return (byte)(int)Value;
					}
					return (byte)convertible.ToInt32(null);
				case TypeCode.Int64:
					if (Value is long)
					{
						return (byte)(long)Value;
					}
					return (byte)convertible.ToInt64(null);
				case TypeCode.Single:
					if (Value is float)
					{
						return (byte)Math.Round((float)Value);
					}
					return (byte)Math.Round(convertible.ToSingle(null));
				case TypeCode.Double:
					if (Value is double)
					{
						return (byte)Math.Round((double)Value);
					}
					return (byte)Math.Round(convertible.ToDouble(null));
				case TypeCode.Decimal:
					return DecimalToByte(convertible);
				case TypeCode.String:
					return FromString(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Byte"));
		}
	}

	private static byte DecimalToByte(IConvertible ValueInterface)
	{
		return Convert.ToByte(ValueInterface.ToDecimal(null));
	}
}
