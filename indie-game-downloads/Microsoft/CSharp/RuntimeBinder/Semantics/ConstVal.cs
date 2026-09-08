using System;
using System.Globalization;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal readonly struct ConstVal
{
	private static readonly object s_false = false;

	private static readonly object s_true = true;

	private static readonly object s_zeroInt32 = 0;

	public object ObjectVal { get; }

	public bool BooleanVal => SpecialUnbox<bool>(ObjectVal);

	public sbyte SByteVal => SpecialUnbox<sbyte>(ObjectVal);

	public byte ByteVal => SpecialUnbox<byte>(ObjectVal);

	public short Int16Val => SpecialUnbox<short>(ObjectVal);

	public ushort UInt16Val => SpecialUnbox<ushort>(ObjectVal);

	public int Int32Val => SpecialUnbox<int>(ObjectVal);

	public uint UInt32Val => SpecialUnbox<uint>(ObjectVal);

	public long Int64Val => SpecialUnbox<long>(ObjectVal);

	public ulong UInt64Val => SpecialUnbox<ulong>(ObjectVal);

	public float SingleVal => SpecialUnbox<float>(ObjectVal);

	public double DoubleVal => SpecialUnbox<double>(ObjectVal);

	public decimal DecimalVal => SpecialUnbox<decimal>(ObjectVal);

	public char CharVal => SpecialUnbox<char>(ObjectVal);

	public string StringVal => SpecialUnbox<string>(ObjectVal);

	public bool IsNullRef => ObjectVal == null;

	private ConstVal(object value)
	{
		ObjectVal = value;
	}

	public bool IsZero(ConstValKind kind)
	{
		return kind switch
		{
			ConstValKind.Decimal => DecimalVal == 0m, 
			ConstValKind.String => false, 
			_ => IsDefault(ObjectVal), 
		};
	}

	private static T SpecialUnbox<T>(object o)
	{
		if (IsDefault(o))
		{
			return default(T);
		}
		return (T)Convert.ChangeType(o, typeof(T), CultureInfo.InvariantCulture);
	}

	private static bool IsDefault(object o)
	{
		bool flag = o == null;
		if (!flag)
		{
			bool flag2;
			switch (Type.GetTypeCode(o.GetType()))
			{
			case TypeCode.Boolean:
				flag2 = false.Equals(o);
				break;
			case TypeCode.SByte:
				flag2 = ((sbyte)0).Equals(o);
				break;
			case TypeCode.Byte:
				flag2 = ((byte)0).Equals(o);
				break;
			case TypeCode.Int16:
				flag2 = ((short)0).Equals(o);
				break;
			case TypeCode.UInt16:
				flag2 = ((ushort)0).Equals(o);
				break;
			case TypeCode.Int32:
				flag2 = 0.Equals(o);
				break;
			case TypeCode.UInt32:
				flag2 = 0u.Equals(o);
				break;
			case TypeCode.Int64:
				flag2 = 0L.Equals(o);
				break;
			case TypeCode.UInt64:
				flag2 = 0uL.Equals(o);
				break;
			case TypeCode.Single:
				flag2 = 0f.Equals(o);
				break;
			case TypeCode.Double:
				flag2 = 0.0.Equals(o);
				break;
			case TypeCode.Decimal:
			{
				decimal num = 0m;
				flag2 = num.Equals(o);
				break;
			}
			case TypeCode.Char:
				flag2 = '\0'.Equals(o);
				break;
			default:
				flag2 = false;
				break;
			}
			flag = flag2;
		}
		return flag;
	}

	public static ConstVal GetDefaultValue(ConstValKind kind)
	{
		return kind switch
		{
			ConstValKind.Int => new ConstVal(s_zeroInt32), 
			ConstValKind.Double => new ConstVal(0.0), 
			ConstValKind.Long => new ConstVal(0L), 
			ConstValKind.Decimal => new ConstVal(0m), 
			ConstValKind.Float => new ConstVal(0f), 
			ConstValKind.Boolean => new ConstVal(s_false), 
			_ => default(ConstVal), 
		};
	}

	public static ConstVal Get(bool value)
	{
		return new ConstVal(value ? s_true : s_false);
	}

	public static ConstVal Get(int value)
	{
		return new ConstVal((value == 0) ? s_zeroInt32 : ((object)value));
	}

	public static ConstVal Get(uint value)
	{
		return new ConstVal(value);
	}

	public static ConstVal Get(decimal value)
	{
		return new ConstVal(value);
	}

	public static ConstVal Get(string value)
	{
		return new ConstVal(value);
	}

	public static ConstVal Get(float value)
	{
		return new ConstVal(value);
	}

	public static ConstVal Get(double value)
	{
		return new ConstVal(value);
	}

	public static ConstVal Get(long value)
	{
		return new ConstVal(value);
	}

	public static ConstVal Get(ulong value)
	{
		return new ConstVal(value);
	}

	public static ConstVal Get(object p)
	{
		return new ConstVal(p);
	}
}
