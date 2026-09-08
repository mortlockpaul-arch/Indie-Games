using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Reflection;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class Conversions
{
	public static bool ToBoolean(string Value)
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
			return ParseDouble(Value) != 0.0;
		}
		catch (FormatException innerException)
		{
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Boolean"), innerException);
		}
	}

	public static bool ToBoolean(object Value)
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
			case TypeCode.SByte:
				if (Value is sbyte)
				{
					return (sbyte)Value != 0;
				}
				return convertible.ToSByte(null) != 0;
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
			case TypeCode.UInt16:
				if (Value is ushort)
				{
					return (ushort)Value != 0;
				}
				return convertible.ToUInt16(null) != 0;
			case TypeCode.Int32:
				if (Value is int)
				{
					return (int)Value != 0;
				}
				return convertible.ToInt32(null) != 0;
			case TypeCode.UInt32:
				if (Value is uint)
				{
					return (uint)Value != 0;
				}
				return convertible.ToUInt32(null) != 0;
			case TypeCode.Int64:
				if (Value is long)
				{
					return (long)Value != 0;
				}
				return convertible.ToInt64(null) != 0;
			case TypeCode.UInt64:
				if (Value is ulong)
				{
					return (ulong)Value != 0;
				}
				return convertible.ToUInt64(null) != 0;
			case TypeCode.Decimal:
				if (Value is decimal)
				{
					return convertible.ToBoolean(null);
				}
				return Convert.ToBoolean(convertible.ToDecimal(null));
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
			case TypeCode.String:
				if (Value is string value)
				{
					return ToBoolean(value);
				}
				return ToBoolean(convertible.ToString(null));
			}
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Boolean"));
	}

	public static byte ToByte(string Value)
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
				return (byte)Math.Round(ParseDouble(Value));
			}
			catch (FormatException innerException)
			{
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Byte"), innerException);
			}
		}
	}

	public static byte ToByte(object Value)
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
					unchecked
					{
						if (Value is bool)
						{
							return (byte)(0u - (((bool)Value) ? 1u : 0u));
						}
						return (byte)(0u - (convertible.ToBoolean(null) ? 1u : 0u));
					}
				case TypeCode.SByte:
					if (Value is sbyte)
					{
						return (byte)(sbyte)Value;
					}
					return (byte)convertible.ToSByte(null);
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
				case TypeCode.UInt16:
					if (Value is ushort)
					{
						return (byte)(ushort)Value;
					}
					return (byte)convertible.ToUInt16(null);
				case TypeCode.Int32:
					if (Value is int)
					{
						return (byte)(int)Value;
					}
					return (byte)convertible.ToInt32(null);
				case TypeCode.UInt32:
					if (Value is uint)
					{
						return (byte)(uint)Value;
					}
					return (byte)convertible.ToUInt32(null);
				case TypeCode.Int64:
					if (Value is long)
					{
						return (byte)(long)Value;
					}
					return (byte)convertible.ToInt64(null);
				case TypeCode.UInt64:
					if (Value is ulong)
					{
						return (byte)(ulong)Value;
					}
					return (byte)convertible.ToUInt64(null);
				case TypeCode.Decimal:
					if (Value is decimal)
					{
						return convertible.ToByte(null);
					}
					return Convert.ToByte(convertible.ToDecimal(null));
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
				case TypeCode.String:
					if (Value is string value)
					{
						return ToByte(value);
					}
					return ToByte(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Byte"));
		}
	}

	[CLSCompliant(false)]
	public static sbyte ToSByte(string Value)
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
					return (sbyte)i64Value;
				}
				return (sbyte)Math.Round(ParseDouble(Value));
			}
			catch (FormatException innerException)
			{
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "SByte"), innerException);
			}
		}
	}

	[CLSCompliant(false)]
	public static sbyte ToSByte(object Value)
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
					unchecked
					{
						if (Value is bool)
						{
							return (sbyte)(0 - (((bool)Value) ? 1 : 0));
						}
						return (sbyte)(0 - (convertible.ToBoolean(null) ? 1 : 0));
					}
				case TypeCode.SByte:
					if (!(Value is sbyte result))
					{
						return convertible.ToSByte(null);
					}
					return result;
				case TypeCode.Byte:
					if (Value is byte)
					{
						return (sbyte)(byte)Value;
					}
					return (sbyte)convertible.ToByte(null);
				case TypeCode.Int16:
					if (Value is short)
					{
						return (sbyte)(short)Value;
					}
					return (sbyte)convertible.ToInt16(null);
				case TypeCode.UInt16:
					if (Value is ushort)
					{
						return (sbyte)(ushort)Value;
					}
					return (sbyte)convertible.ToUInt16(null);
				case TypeCode.Int32:
					if (Value is int)
					{
						return (sbyte)(int)Value;
					}
					return (sbyte)convertible.ToInt32(null);
				case TypeCode.UInt32:
					if (Value is uint)
					{
						return (sbyte)(uint)Value;
					}
					return (sbyte)convertible.ToUInt32(null);
				case TypeCode.Int64:
					if (Value is long)
					{
						return (sbyte)(long)Value;
					}
					return (sbyte)convertible.ToInt64(null);
				case TypeCode.UInt64:
					if (Value is ulong)
					{
						return (sbyte)(ulong)Value;
					}
					return (sbyte)convertible.ToUInt64(null);
				case TypeCode.Decimal:
					if (Value is decimal)
					{
						return convertible.ToSByte(null);
					}
					return Convert.ToSByte(convertible.ToDecimal(null));
				case TypeCode.Single:
					if (Value is float)
					{
						return (sbyte)Math.Round((float)Value);
					}
					return (sbyte)Math.Round(convertible.ToSingle(null));
				case TypeCode.Double:
					if (Value is double)
					{
						return (sbyte)Math.Round((double)Value);
					}
					return (sbyte)Math.Round(convertible.ToDouble(null));
				case TypeCode.String:
					if (Value is string value)
					{
						return ToSByte(value);
					}
					return ToSByte(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "SByte"));
		}
	}

	public static short ToShort(string Value)
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
				return (short)Math.Round(ParseDouble(Value));
			}
			catch (FormatException innerException)
			{
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Short"), innerException);
			}
		}
	}

	public static short ToShort(object Value)
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
					unchecked
					{
						if (Value is bool)
						{
							return (short)(0 - (((bool)Value) ? 1 : 0));
						}
						return (short)(0 - (convertible.ToBoolean(null) ? 1 : 0));
					}
				case TypeCode.SByte:
					if (!(Value is short result2))
					{
						return convertible.ToSByte(null);
					}
					return result2;
				case TypeCode.Byte:
					if (!(Value is short result3))
					{
						return convertible.ToByte(null);
					}
					return result3;
				case TypeCode.Int16:
					if (!(Value is short result))
					{
						return convertible.ToInt16(null);
					}
					return result;
				case TypeCode.UInt16:
					if (Value is ushort)
					{
						return (short)(ushort)Value;
					}
					return (short)convertible.ToUInt16(null);
				case TypeCode.Int32:
					if (Value is int)
					{
						return (short)(int)Value;
					}
					return (short)convertible.ToInt32(null);
				case TypeCode.UInt32:
					if (Value is uint)
					{
						return (short)(uint)Value;
					}
					return (short)convertible.ToUInt32(null);
				case TypeCode.Int64:
					if (Value is long)
					{
						return (short)(long)Value;
					}
					return (short)convertible.ToInt64(null);
				case TypeCode.UInt64:
					if (Value is ulong)
					{
						return (short)(ulong)Value;
					}
					return (short)convertible.ToUInt64(null);
				case TypeCode.Decimal:
					if (Value is decimal)
					{
						return convertible.ToInt16(null);
					}
					return Convert.ToInt16(convertible.ToDecimal(null));
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
				case TypeCode.String:
					if (Value is string value)
					{
						return ToShort(value);
					}
					return ToShort(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Short"));
		}
	}

	[CLSCompliant(false)]
	public static ushort ToUShort(string Value)
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
					return (ushort)i64Value;
				}
				return (ushort)Math.Round(ParseDouble(Value));
			}
			catch (FormatException innerException)
			{
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "UShort"), innerException);
			}
		}
	}

	[CLSCompliant(false)]
	public static ushort ToUShort(object Value)
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
					unchecked
					{
						if (Value is bool)
						{
							return (ushort)(0u - (((bool)Value) ? 1u : 0u));
						}
						return (ushort)(0u - (convertible.ToBoolean(null) ? 1u : 0u));
					}
				case TypeCode.SByte:
					if (Value is sbyte)
					{
						return (ushort)(sbyte)Value;
					}
					return (ushort)convertible.ToSByte(null);
				case TypeCode.Byte:
					if (!(Value is ushort result2))
					{
						return convertible.ToByte(null);
					}
					return result2;
				case TypeCode.Int16:
					if (Value is short)
					{
						return (ushort)(short)Value;
					}
					return (ushort)convertible.ToInt16(null);
				case TypeCode.UInt16:
					if (!(Value is ushort result))
					{
						return convertible.ToUInt16(null);
					}
					return result;
				case TypeCode.Int32:
					if (Value is int)
					{
						return (ushort)(int)Value;
					}
					return (ushort)convertible.ToInt32(null);
				case TypeCode.UInt32:
					if (Value is uint)
					{
						return (ushort)(uint)Value;
					}
					return (ushort)convertible.ToUInt32(null);
				case TypeCode.Int64:
					if (Value is long)
					{
						return (ushort)(long)Value;
					}
					return (ushort)convertible.ToInt64(null);
				case TypeCode.UInt64:
					if (Value is ulong)
					{
						return (ushort)(ulong)Value;
					}
					return (ushort)convertible.ToUInt64(null);
				case TypeCode.Decimal:
					if (Value is decimal)
					{
						return convertible.ToUInt16(null);
					}
					return Convert.ToUInt16(convertible.ToDecimal(null));
				case TypeCode.Single:
					if (Value is float)
					{
						return (ushort)Math.Round((float)Value);
					}
					return (ushort)Math.Round(convertible.ToSingle(null));
				case TypeCode.Double:
					if (Value is double)
					{
						return (ushort)Math.Round((double)Value);
					}
					return (ushort)Math.Round(convertible.ToDouble(null));
				case TypeCode.String:
					if (Value is string value)
					{
						return ToUShort(value);
					}
					return ToUShort(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "UShort"));
		}
	}

	public static int ToInteger(string Value)
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
				return (int)Math.Round(ParseDouble(Value));
			}
			catch (FormatException innerException)
			{
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Integer"), innerException);
			}
		}
	}

	public static int ToInteger(object Value)
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
					unchecked
					{
						if (Value is bool)
						{
							return 0 - (((bool)Value) ? 1 : 0);
						}
						return 0 - (convertible.ToBoolean(null) ? 1 : 0);
					}
				case TypeCode.SByte:
					if (!(Value is int result))
					{
						return convertible.ToSByte(null);
					}
					return result;
				case TypeCode.Byte:
					if (!(Value is int result5))
					{
						return convertible.ToByte(null);
					}
					return result5;
				case TypeCode.Int16:
					if (!(Value is int result3))
					{
						return convertible.ToInt16(null);
					}
					return result3;
				case TypeCode.UInt16:
					if (!(Value is int result4))
					{
						return convertible.ToUInt16(null);
					}
					return result4;
				case TypeCode.Int32:
					if (!(Value is int result2))
					{
						return convertible.ToInt32(null);
					}
					return result2;
				case TypeCode.UInt32:
					if (Value is uint)
					{
						return (int)(uint)Value;
					}
					return (int)convertible.ToUInt32(null);
				case TypeCode.Int64:
					if (Value is long)
					{
						return (int)(long)Value;
					}
					return (int)convertible.ToInt64(null);
				case TypeCode.UInt64:
					if (Value is ulong)
					{
						return (int)(ulong)Value;
					}
					return (int)convertible.ToUInt64(null);
				case TypeCode.Decimal:
					if (Value is decimal)
					{
						return convertible.ToInt32(null);
					}
					return Convert.ToInt32(convertible.ToDecimal(null));
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
				case TypeCode.String:
					if (Value is string value)
					{
						return ToInteger(value);
					}
					return ToInteger(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Integer"));
		}
	}

	[CLSCompliant(false)]
	public static uint ToUInteger(string Value)
	{
		if (Value == null)
		{
			return 0u;
		}
		checked
		{
			try
			{
				long i64Value = default(long);
				if (Utils.IsHexOrOctValue(Value, ref i64Value))
				{
					return (uint)i64Value;
				}
				return (uint)Math.Round(ParseDouble(Value));
			}
			catch (FormatException innerException)
			{
				throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "UInteger"), innerException);
			}
		}
	}

	[CLSCompliant(false)]
	public static uint ToUInteger(object Value)
	{
		if (Value == null)
		{
			return 0u;
		}
		checked
		{
			if (Value is IConvertible convertible)
			{
				switch (convertible.GetTypeCode())
				{
				case TypeCode.Boolean:
					unchecked
					{
						if (Value is bool)
						{
							return 0u - (((bool)Value) ? 1u : 0u);
						}
						return 0u - (convertible.ToBoolean(null) ? 1u : 0u);
					}
				case TypeCode.SByte:
					if (Value is sbyte)
					{
						return (uint)(sbyte)Value;
					}
					return (uint)convertible.ToSByte(null);
				case TypeCode.Byte:
					if (!(Value is uint result3))
					{
						return convertible.ToByte(null);
					}
					return result3;
				case TypeCode.Int16:
					if (Value is short)
					{
						return (uint)(short)Value;
					}
					return (uint)convertible.ToInt16(null);
				case TypeCode.UInt16:
					if (!(Value is uint result))
					{
						return convertible.ToUInt16(null);
					}
					return result;
				case TypeCode.Int32:
					if (Value is int)
					{
						return (uint)(int)Value;
					}
					return (uint)convertible.ToInt32(null);
				case TypeCode.UInt32:
					if (!(Value is uint result2))
					{
						return convertible.ToUInt32(null);
					}
					return result2;
				case TypeCode.Int64:
					if (Value is long)
					{
						return (uint)(long)Value;
					}
					return (uint)convertible.ToInt64(null);
				case TypeCode.UInt64:
					if (Value is ulong)
					{
						return (uint)(ulong)Value;
					}
					return (uint)convertible.ToUInt64(null);
				case TypeCode.Decimal:
					if (Value is decimal)
					{
						return convertible.ToUInt32(null);
					}
					return Convert.ToUInt32(convertible.ToDecimal(null));
				case TypeCode.Single:
					if (Value is float)
					{
						return (uint)Math.Round((float)Value);
					}
					return (uint)Math.Round(convertible.ToSingle(null));
				case TypeCode.Double:
					if (Value is double)
					{
						return (uint)Math.Round((double)Value);
					}
					return (uint)Math.Round(convertible.ToDouble(null));
				case TypeCode.String:
					if (Value is string value)
					{
						return ToUInteger(value);
					}
					return ToUInteger(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "UInteger"));
		}
	}

	public static long ToLong(string Value)
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
			return Convert.ToInt64(ParseDecimal(Value, null));
		}
		catch (FormatException innerException)
		{
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Long"), innerException);
		}
	}

	public static long ToLong(object Value)
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
					unchecked
					{
						if (Value is bool)
						{
							return 0 - (((bool)Value) ? 1 : 0);
						}
						return 0 - (convertible.ToBoolean(null) ? 1 : 0);
					}
				case TypeCode.SByte:
					if (Value is sbyte)
					{
						return (sbyte)Value;
					}
					return convertible.ToSByte(null);
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
				case TypeCode.UInt16:
					if (Value is ushort)
					{
						return (ushort)Value;
					}
					return convertible.ToUInt16(null);
				case TypeCode.Int32:
					if (Value is int)
					{
						return (int)Value;
					}
					return convertible.ToInt32(null);
				case TypeCode.UInt32:
					if (Value is uint)
					{
						return (uint)Value;
					}
					return convertible.ToUInt32(null);
				case TypeCode.Int64:
					if (!(Value is long result))
					{
						return convertible.ToInt64(null);
					}
					return result;
				case TypeCode.UInt64:
					if (Value is ulong)
					{
						return (long)(ulong)Value;
					}
					return (long)convertible.ToUInt64(null);
				case TypeCode.Decimal:
					if (Value is decimal)
					{
						return convertible.ToInt64(null);
					}
					return Convert.ToInt64(convertible.ToDecimal(null));
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
				case TypeCode.String:
					if (Value is string value)
					{
						return ToLong(value);
					}
					return ToLong(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Long"));
		}
	}

	[CLSCompliant(false)]
	public static ulong ToULong(string Value)
	{
		if (Value == null)
		{
			return 0uL;
		}
		try
		{
			ulong ui64Value = default(ulong);
			if (Utils.IsHexOrOctValue(Value, ref ui64Value))
			{
				return ui64Value;
			}
			return Convert.ToUInt64(ParseDecimal(Value, null));
		}
		catch (FormatException innerException)
		{
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "ULong"), innerException);
		}
	}

	[CLSCompliant(false)]
	public static ulong ToULong(object Value)
	{
		if (Value == null)
		{
			return 0uL;
		}
		checked
		{
			if (Value is IConvertible convertible)
			{
				switch (convertible.GetTypeCode())
				{
				case TypeCode.Boolean:
					unchecked
					{
						if (Value is bool)
						{
							return (ulong)(int)(0u - (((bool)Value) ? 1u : 0u));
						}
						return (ulong)(int)(0u - (convertible.ToBoolean(null) ? 1u : 0u));
					}
				case TypeCode.SByte:
					if (Value is sbyte)
					{
						return (ulong)(sbyte)Value;
					}
					return (ulong)convertible.ToSByte(null);
				case TypeCode.Byte:
					if (Value is byte)
					{
						return (byte)Value;
					}
					return convertible.ToByte(null);
				case TypeCode.Int16:
					if (Value is short)
					{
						return (ulong)(short)Value;
					}
					return (ulong)convertible.ToInt16(null);
				case TypeCode.UInt16:
					if (Value is ushort)
					{
						return (ushort)Value;
					}
					return convertible.ToUInt16(null);
				case TypeCode.Int32:
					if (Value is int)
					{
						return (ulong)(int)Value;
					}
					return (ulong)convertible.ToInt32(null);
				case TypeCode.UInt32:
					if (Value is uint)
					{
						return (uint)Value;
					}
					return convertible.ToUInt32(null);
				case TypeCode.Int64:
					if (Value is long)
					{
						return (ulong)(long)Value;
					}
					return (ulong)convertible.ToInt64(null);
				case TypeCode.UInt64:
					if (!(Value is ulong result))
					{
						return convertible.ToUInt64(null);
					}
					return result;
				case TypeCode.Decimal:
					if (Value is decimal)
					{
						return convertible.ToUInt64(null);
					}
					return Convert.ToUInt64(convertible.ToDecimal(null));
				case TypeCode.Single:
					if (Value is float)
					{
						return (ulong)Math.Round((float)Value);
					}
					return (ulong)Math.Round(convertible.ToSingle(null));
				case TypeCode.Double:
					if (Value is double)
					{
						return (ulong)Math.Round((double)Value);
					}
					return (ulong)Math.Round(convertible.ToDouble(null));
				case TypeCode.String:
					if (Value is string value)
					{
						return ToULong(value);
					}
					return ToULong(convertible.ToString(null));
				}
			}
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "ULong"));
		}
	}

	public static decimal ToDecimal(bool Value)
	{
		if (Value)
		{
			return -1m;
		}
		return 0m;
	}

	public static decimal ToDecimal(string Value)
	{
		return ToDecimal(Value, null);
	}

	internal static decimal ToDecimal(string Value, NumberFormatInfo NumberFormat)
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
			return ParseDecimal(Value, NumberFormat);
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

	public static decimal ToDecimal(object Value)
	{
		return ToDecimal(Value, null);
	}

	internal static decimal ToDecimal(object Value, NumberFormatInfo NumberFormat)
	{
		decimal result;
		if (Value == null)
		{
			result = 0m;
			goto IL_0287;
		}
		if (Value is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.Boolean:
				break;
			case TypeCode.SByte:
				goto IL_009c;
			case TypeCode.Byte:
				goto IL_00c9;
			case TypeCode.Int16:
				goto IL_00f6;
			case TypeCode.UInt16:
				goto IL_0123;
			case TypeCode.Int32:
				goto IL_0150;
			case TypeCode.UInt32:
				goto IL_017d;
			case TypeCode.Int64:
				goto IL_01aa;
			case TypeCode.UInt64:
				goto IL_01d7;
			case TypeCode.Decimal:
				return convertible.ToDecimal(null);
			case TypeCode.Single:
				goto IL_020e;
			case TypeCode.Double:
				goto IL_0235;
			case TypeCode.String:
				return ToDecimal(convertible.ToString(null), NumberFormat);
			default:
				goto IL_026c;
			}
			if (Value is bool)
			{
				return ToDecimal((bool)Value);
			}
			return ToDecimal(convertible.ToBoolean(null));
		}
		goto IL_026c;
		IL_0235:
		result = ((!(Value is double)) ? new decimal(convertible.ToDouble(null)) : new decimal((double)Value));
		goto IL_0287;
		IL_0123:
		result = ((!(Value is ushort)) ? new decimal(convertible.ToUInt16(null)) : new decimal((ushort)Value));
		goto IL_0287;
		IL_017d:
		result = ((!(Value is uint)) ? new decimal(convertible.ToUInt32(null)) : new decimal((uint)Value));
		goto IL_0287;
		IL_0150:
		result = ((!(Value is int)) ? new decimal(convertible.ToInt32(null)) : new decimal((int)Value));
		goto IL_0287;
		IL_01aa:
		result = ((!(Value is long)) ? new decimal(convertible.ToInt64(null)) : new decimal((long)Value));
		goto IL_0287;
		IL_009c:
		result = ((!(Value is sbyte)) ? new decimal(convertible.ToSByte(null)) : new decimal((sbyte)Value));
		goto IL_0287;
		IL_026c:
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Decimal"));
		IL_0287:
		return result;
		IL_00c9:
		result = ((!(Value is byte)) ? new decimal(convertible.ToByte(null)) : new decimal((byte)Value));
		goto IL_0287;
		IL_01d7:
		result = ((!(Value is ulong)) ? new decimal(convertible.ToUInt64(null)) : new decimal((ulong)Value));
		goto IL_0287;
		IL_020e:
		result = ((!(Value is float)) ? new decimal(convertible.ToSingle(null)) : new decimal((float)Value));
		goto IL_0287;
		IL_00f6:
		result = ((!(Value is short)) ? new decimal(convertible.ToInt16(null)) : new decimal((short)Value));
		goto IL_0287;
	}

	private static decimal ParseDecimal(string Value, NumberFormatInfo NumberFormat)
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

	private static NumberFormatInfo GetNormalizedNumberFormat(NumberFormatInfo InNumberFormat)
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

	public static float ToSingle(string Value)
	{
		return ToSingle(Value, null);
	}

	internal static float ToSingle(string Value, NumberFormatInfo NumberFormat)
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
			double num = ParseDouble(Value, NumberFormat);
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

	public static float ToSingle(object Value)
	{
		return ToSingle(Value, null);
	}

	internal static float ToSingle(object Value, NumberFormatInfo NumberFormat)
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
				if (Value is bool)
				{
					return 0 - (((bool)Value) ? 1 : 0);
				}
				return 0 - (convertible.ToBoolean(null) ? 1 : 0);
			case TypeCode.SByte:
				if (Value is sbyte)
				{
					return (sbyte)Value;
				}
				return convertible.ToSByte(null);
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
			case TypeCode.UInt16:
				if (Value is ushort)
				{
					return (int)(ushort)Value;
				}
				return (int)convertible.ToUInt16(null);
			case TypeCode.Int32:
				if (Value is int)
				{
					return (int)Value;
				}
				return convertible.ToInt32(null);
			case TypeCode.UInt32:
				if (!(Value is uint))
				{
					return convertible.ToUInt32(null);
				}
				return (uint)Value;
			case TypeCode.Int64:
				if (Value is long)
				{
					return (long)Value;
				}
				return convertible.ToInt64(null);
			case TypeCode.UInt64:
				if (!(Value is ulong))
				{
					return convertible.ToUInt64(null);
				}
				return (ulong)Value;
			case TypeCode.Decimal:
				if (Value is decimal)
				{
					return convertible.ToSingle(null);
				}
				return Convert.ToSingle(convertible.ToDecimal(null));
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
			case TypeCode.String:
				return ToSingle(convertible.ToString(null), NumberFormat);
			}
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Single"));
	}

	public static double ToDouble(string Value)
	{
		return ToDouble(Value, null);
	}

	internal static double ToDouble(string Value, NumberFormatInfo NumberFormat)
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
			return ParseDouble(Value, NumberFormat);
		}
		catch (FormatException innerException)
		{
			throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Double"), innerException);
		}
	}

	public static double ToDouble(object Value)
	{
		return ToDouble(Value, null);
	}

	internal static double ToDouble(object Value, NumberFormatInfo NumberFormat)
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
				if (Value is bool)
				{
					return 0 - (((bool)Value) ? 1 : 0);
				}
				return 0 - (convertible.ToBoolean(null) ? 1 : 0);
			case TypeCode.SByte:
				if (Value is sbyte)
				{
					return (sbyte)Value;
				}
				return convertible.ToSByte(null);
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
			case TypeCode.UInt16:
				if (Value is ushort)
				{
					return (int)(ushort)Value;
				}
				return (int)convertible.ToUInt16(null);
			case TypeCode.Int32:
				if (Value is int)
				{
					return (int)Value;
				}
				return convertible.ToInt32(null);
			case TypeCode.UInt32:
				if (!(Value is uint))
				{
					return convertible.ToUInt32(null);
				}
				return (uint)Value;
			case TypeCode.Int64:
				if (Value is long)
				{
					return (long)Value;
				}
				return convertible.ToInt64(null);
			case TypeCode.UInt64:
				if (!(Value is ulong))
				{
					return convertible.ToUInt64(null);
				}
				return (ulong)Value;
			case TypeCode.Decimal:
				if (Value is decimal)
				{
					return convertible.ToDouble(null);
				}
				return Convert.ToDouble(convertible.ToDecimal(null));
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
			case TypeCode.String:
				return ToDouble(convertible.ToString(null), NumberFormat);
			}
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Double"));
	}

	private static double ParseDouble(string Value)
	{
		return ParseDouble(Value, null);
	}

	internal static bool TryParseDouble(string Value, ref double Result)
	{
		CultureInfo cultureInfo = Utils.GetCultureInfo();
		NumberFormatInfo numberFormat = cultureInfo.NumberFormat;
		NumberFormatInfo normalizedNumberFormat = GetNormalizedNumberFormat(numberFormat);
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

	private static double ParseDouble(string Value, NumberFormatInfo NumberFormat)
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

	public static DateTime ToDate(string Value)
	{
		DateTime Result = default(DateTime);
		if (TryParseDate(Value, ref Result))
		{
			return Result;
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromStringTo, Strings.Left(Value, 32), "Date"));
	}

	public static DateTime ToDate(object Value)
	{
		if (Value == null)
		{
			return DateTime.MinValue;
		}
		if (Value is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.DateTime:
				if (!(Value is DateTime result))
				{
					return convertible.ToDateTime(null);
				}
				return result;
			case TypeCode.String:
				if (Value is string value)
				{
					return ToDate(value);
				}
				return ToDate(convertible.ToString(null));
			}
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Date"));
	}

	internal static bool TryParseDate(string Value, ref DateTime Result)
	{
		CultureInfo cultureInfo = Utils.GetCultureInfo();
		return DateTime.TryParse(Utils.ToHalfwidthNumbers(Value, cultureInfo), cultureInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault, out Result);
	}

	public static char ToChar(string Value)
	{
		if (Value == null || Value.Length == 0)
		{
			return '\0';
		}
		return Value[0];
	}

	public static char ToChar(object Value)
	{
		if (Value == null)
		{
			return '\0';
		}
		if (Value is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.Char:
				if (!(Value is char result))
				{
					return convertible.ToChar(null);
				}
				return result;
			case TypeCode.String:
				if (Value is string value)
				{
					return ToChar(value);
				}
				return ToChar(convertible.ToString(null));
			}
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "Char"));
	}

	public static char[] ToCharArrayRankOne(string Value)
	{
		if (Value == null)
		{
			Value = "";
		}
		return Value.ToCharArray();
	}

	public static char[] ToCharArrayRankOne(object Value)
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

	public static string ToString(bool Value)
	{
		if (Value)
		{
			return bool.TrueString;
		}
		return bool.FalseString;
	}

	public static string ToString(byte Value)
	{
		return Value.ToString(null, null);
	}

	public static string ToString(char Value)
	{
		return Value.ToString();
	}

	public static string FromCharArray(char[] Value)
	{
		return new string(Value);
	}

	public static string FromCharAndCount(char Value, int Count)
	{
		return new string(Value, Count);
	}

	public static string FromCharArraySubset(char[] Value, int StartIndex, int Length)
	{
		return new string(Value, StartIndex, Length);
	}

	public static string ToString(short Value)
	{
		return Value.ToString(null, null);
	}

	public static string ToString(int Value)
	{
		return Value.ToString(null, null);
	}

	[CLSCompliant(false)]
	public static string ToString(uint Value)
	{
		return Value.ToString(null, null);
	}

	public static string ToString(long Value)
	{
		return Value.ToString(null, null);
	}

	[CLSCompliant(false)]
	public static string ToString(ulong Value)
	{
		return Value.ToString(null, null);
	}

	public static string ToString(float Value)
	{
		return ToString(Value, null);
	}

	public static string ToString(double Value)
	{
		return ToString(Value, null);
	}

	public static string ToString(float Value, NumberFormatInfo NumberFormat)
	{
		return Value.ToString(null, NumberFormat);
	}

	public static string ToString(double Value, NumberFormatInfo NumberFormat)
	{
		return Value.ToString("G", NumberFormat);
	}

	public static string ToString(DateTime Value)
	{
		long ticks = Value.TimeOfDay.Ticks;
		if (ticks == Value.Ticks || (Value.Year == 1899 && Value.Month == 12 && Value.Day == 30))
		{
			return Value.ToString("T", null);
		}
		if (ticks == 0L)
		{
			return Value.ToString("d", null);
		}
		return Value.ToString("G", null);
	}

	public static string ToString(decimal Value)
	{
		return ToString(Value, null);
	}

	public static string ToString(decimal Value, NumberFormatInfo NumberFormat)
	{
		return Value.ToString("G", NumberFormat);
	}

	public static string ToString(object Value)
	{
		if (Value == null)
		{
			return null;
		}
		if (Value is string result)
		{
			return result;
		}
		if (Value is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.Boolean:
				return ToString(convertible.ToBoolean(null));
			case TypeCode.SByte:
				return ToString((int)convertible.ToSByte(null));
			case TypeCode.Byte:
				return ToString(convertible.ToByte(null));
			case TypeCode.Int16:
				return ToString((int)convertible.ToInt16(null));
			case TypeCode.UInt16:
				return ToString((uint)convertible.ToUInt16(null));
			case TypeCode.Int32:
				return ToString(convertible.ToInt32(null));
			case TypeCode.UInt32:
				return ToString(convertible.ToUInt32(null));
			case TypeCode.Int64:
				return ToString(convertible.ToInt64(null));
			case TypeCode.UInt64:
				return ToString(convertible.ToUInt64(null));
			case TypeCode.Decimal:
				return ToString(convertible.ToDecimal(null));
			case TypeCode.Single:
				return ToString(convertible.ToSingle(null));
			case TypeCode.Double:
				return ToString(convertible.ToDouble(null));
			case TypeCode.Char:
				return ToString(convertible.ToChar(null));
			case TypeCode.DateTime:
				return ToString(convertible.ToDateTime(null));
			case TypeCode.String:
				return convertible.ToString(null);
			}
		}
		else if (Value is char[] value)
		{
			return new string(value);
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(Value), "String"));
	}

	public static T ToGenericParameter<T>(object Value)
	{
		if (Value == null)
		{
			return default(T);
		}
		return ReflectionExtensions.GetTypeCode(typeof(T)) switch
		{
			TypeCode.Boolean => (T)(object)ToBoolean(Value), 
			TypeCode.SByte => (T)(object)ToSByte(Value), 
			TypeCode.Byte => (T)(object)ToByte(Value), 
			TypeCode.Int16 => (T)(object)ToShort(Value), 
			TypeCode.UInt16 => (T)(object)ToUShort(Value), 
			TypeCode.Int32 => (T)(object)ToInteger(Value), 
			TypeCode.UInt32 => (T)(object)ToUInteger(Value), 
			TypeCode.Int64 => (T)(object)ToLong(Value), 
			TypeCode.UInt64 => (T)(object)ToULong(Value), 
			TypeCode.Decimal => (T)(object)ToDecimal(Value), 
			TypeCode.Single => (T)(object)ToSingle(Value), 
			TypeCode.Double => (T)(object)ToDouble(Value), 
			TypeCode.DateTime => (T)(object)ToDate(Value), 
			TypeCode.Char => (T)(object)ToChar(Value), 
			TypeCode.String => (T)(object)ToString(Value), 
			_ => (T)Value, 
		};
	}

	private static object CastSByteEnum(sbyte Expression, Type TargetType)
	{
		if (Symbols.IsEnum(TargetType))
		{
			return Enum.ToObject(TargetType, Expression);
		}
		return Expression;
	}

	private static object CastByteEnum(byte Expression, Type TargetType)
	{
		if (Symbols.IsEnum(TargetType))
		{
			return Enum.ToObject(TargetType, Expression);
		}
		return Expression;
	}

	private static object CastInt16Enum(short Expression, Type TargetType)
	{
		if (Symbols.IsEnum(TargetType))
		{
			return Enum.ToObject(TargetType, Expression);
		}
		return Expression;
	}

	private static object CastUInt16Enum(ushort Expression, Type TargetType)
	{
		if (Symbols.IsEnum(TargetType))
		{
			return Enum.ToObject(TargetType, Expression);
		}
		return Expression;
	}

	private static object CastInt32Enum(int Expression, Type TargetType)
	{
		if (Symbols.IsEnum(TargetType))
		{
			return Enum.ToObject(TargetType, Expression);
		}
		return Expression;
	}

	private static object CastUInt32Enum(uint Expression, Type TargetType)
	{
		if (Symbols.IsEnum(TargetType))
		{
			return Enum.ToObject(TargetType, Expression);
		}
		return Expression;
	}

	private static object CastInt64Enum(long Expression, Type TargetType)
	{
		if (Symbols.IsEnum(TargetType))
		{
			return Enum.ToObject(TargetType, Expression);
		}
		return Expression;
	}

	private static object CastUInt64Enum(ulong Expression, Type TargetType)
	{
		if (Symbols.IsEnum(TargetType))
		{
			return Enum.ToObject(TargetType, Expression);
		}
		return Expression;
	}

	internal static object ForceValueCopy(object Expression, Type TargetType)
	{
		if (!(Expression is IConvertible convertible))
		{
			return Expression;
		}
		return convertible.GetTypeCode() switch
		{
			TypeCode.Boolean => convertible.ToBoolean(null), 
			TypeCode.SByte => CastSByteEnum(convertible.ToSByte(null), TargetType), 
			TypeCode.Byte => CastByteEnum(convertible.ToByte(null), TargetType), 
			TypeCode.Int16 => CastInt16Enum(convertible.ToInt16(null), TargetType), 
			TypeCode.UInt16 => CastUInt16Enum(convertible.ToUInt16(null), TargetType), 
			TypeCode.Int32 => CastInt32Enum(convertible.ToInt32(null), TargetType), 
			TypeCode.UInt32 => CastUInt32Enum(convertible.ToUInt32(null), TargetType), 
			TypeCode.Int64 => CastInt64Enum(convertible.ToInt64(null), TargetType), 
			TypeCode.UInt64 => CastUInt64Enum(convertible.ToUInt64(null), TargetType), 
			TypeCode.Decimal => convertible.ToDecimal(null), 
			TypeCode.Single => convertible.ToSingle(null), 
			TypeCode.Double => convertible.ToDouble(null), 
			TypeCode.DateTime => convertible.ToDateTime(null), 
			TypeCode.Char => convertible.ToChar(null), 
			_ => Expression, 
		};
	}

	private static object ChangeIntrinsicType(object Expression, Type TargetType)
	{
		return ReflectionExtensions.GetTypeCode(TargetType) switch
		{
			TypeCode.Boolean => ToBoolean(Expression), 
			TypeCode.SByte => CastSByteEnum(ToSByte(Expression), TargetType), 
			TypeCode.Byte => CastByteEnum(ToByte(Expression), TargetType), 
			TypeCode.Int16 => CastInt16Enum(ToShort(Expression), TargetType), 
			TypeCode.UInt16 => CastUInt16Enum(ToUShort(Expression), TargetType), 
			TypeCode.Int32 => CastInt32Enum(ToInteger(Expression), TargetType), 
			TypeCode.UInt32 => CastUInt32Enum(ToUInteger(Expression), TargetType), 
			TypeCode.Int64 => CastInt64Enum(ToLong(Expression), TargetType), 
			TypeCode.UInt64 => CastUInt64Enum(ToULong(Expression), TargetType), 
			TypeCode.Decimal => ToDecimal(Expression), 
			TypeCode.Single => ToSingle(Expression), 
			TypeCode.Double => ToDouble(Expression), 
			TypeCode.DateTime => ToDate(Expression), 
			TypeCode.Char => ToChar(Expression), 
			TypeCode.String => ToString(Expression), 
			_ => throw new Exception(), 
		};
	}

	[RequiresUnreferencedCode("The Expression origin object cannot be statically analyzed and may be trimmed")]
	public static object ChangeType(object Expression, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type TargetType)
	{
		return ChangeType(Expression, TargetType, Dynamic: false);
	}

	[RequiresUnreferencedCode("Calls ObjectUserDefinedConversion")]
	internal static object ChangeType(object Expression, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type TargetType, bool Dynamic)
	{
		if ((object)TargetType == null)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "TargetType"));
		}
		if (Expression == null)
		{
			if (Symbols.IsValueType(TargetType))
			{
				return Activator.CreateInstance(TargetType);
			}
			return null;
		}
		Type type = Expression.GetType();
		if (TargetType.IsByRef)
		{
			TargetType = TargetType.GetElementType();
		}
		if ((object)TargetType == type || Symbols.IsRootObjectType(TargetType))
		{
			return Expression;
		}
		if (Symbols.IsIntrinsicType(ReflectionExtensions.GetTypeCode(TargetType)) && Symbols.IsIntrinsicType(ReflectionExtensions.GetTypeCode(type)))
		{
			return ChangeIntrinsicType(Expression, TargetType);
		}
		if (TargetType.IsInstanceOfType(Expression))
		{
			return Expression;
		}
		if (Symbols.IsCharArrayRankOne(TargetType) && Symbols.IsStringType(type))
		{
			return ToCharArrayRankOne((string)Expression);
		}
		if (Symbols.IsStringType(TargetType) && Symbols.IsCharArrayRankOne(type))
		{
			return new string((char[])Expression);
		}
		if (Dynamic)
		{
			IDynamicMetaObjectProvider dynamicMetaObjectProvider = IDOUtils.TryCastToIDMOP(Expression);
			if (dynamicMetaObjectProvider != null)
			{
				return IDOBinder.UserDefinedConversion(dynamicMetaObjectProvider, TargetType);
			}
		}
		return ObjectUserDefinedConversion(Expression, TargetType);
	}

	[Obsolete("FallbackUserDefinedConversion has been deprecated and is not supported.", true)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("The Expression origin object cannot be statically analyzed and may be trimmed")]
	public static object FallbackUserDefinedConversion(object Expression, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type TargetType)
	{
		return ObjectUserDefinedConversion(Expression, TargetType);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Calls Container.InvokeMethod which is unsafe.")]
	private static object ObjectUserDefinedConversion(object Expression, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type TargetType)
	{
		Type type = Expression.GetType();
		if (ConversionResolution.ClassifyPredefinedConversion(TargetType, type) == ConversionResolution.ConversionClass.None && (Symbols.IsClassOrValueType(type) || Symbols.IsClassOrValueType(TargetType)) && (!Symbols.IsIntrinsicType(type) || !Symbols.IsIntrinsicType(TargetType)))
		{
			Symbols.Method operatorMethod = null;
			ConversionResolution.ConversionClass conversionClass = ConversionResolution.ClassifyUserDefinedConversion(TargetType, type, ref operatorMethod);
			if ((object)operatorMethod != null)
			{
				return ChangeType(new Symbols.Container(operatorMethod.DeclaringType).InvokeMethod(operatorMethod, new object[1] { Expression }, null, BindingFlags.InvokeMethod), TargetType);
			}
			if (conversionClass == ConversionResolution.ConversionClass.Ambiguous)
			{
				throw new InvalidCastException(System.SR.Format(System.SR.AmbiguousCast2, Utils.VBFriendlyName(type), Utils.VBFriendlyName(TargetType)));
			}
		}
		throw new InvalidCastException(System.SR.Format(System.SR.InvalidCast_FromTo, Utils.VBFriendlyName(type), Utils.VBFriendlyName(TargetType)));
	}

	[RequiresUnreferencedCode("Calls ClassifyUserDefinedConversion")]
	internal static bool CanUserDefinedConvert(object Expression, Type TargetType)
	{
		Type type = Expression.GetType();
		if (ConversionResolution.ClassifyPredefinedConversion(TargetType, type) == ConversionResolution.ConversionClass.None && (Symbols.IsClassOrValueType(type) || Symbols.IsClassOrValueType(TargetType)) && (!Symbols.IsIntrinsicType(type) || !Symbols.IsIntrinsicType(TargetType)))
		{
			Symbols.Method operatorMethod = null;
			ConversionResolution.ClassifyUserDefinedConversion(TargetType, type, ref operatorMethod);
			return (object)operatorMethod != null;
		}
		return false;
	}
}
