using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic;

[StandardModule]
public sealed class Conversion
{
	public static string ErrorToString()
	{
		return Information.Err().Description;
	}

	public static string ErrorToString(int ErrorNumber)
	{
		if (ErrorNumber >= 65535)
		{
			throw new ArgumentException(System.SR.MaxErrNumber);
		}
		if (ErrorNumber > 0)
		{
			ErrorNumber = -2146828288 | ErrorNumber;
		}
		if ((ErrorNumber & 0x1FFF0000) == 655360)
		{
			ErrorNumber &= 0xFFFF;
			return Utils.GetResourceString((vbErrors)ErrorNumber);
		}
		if (ErrorNumber != 0)
		{
			return Utils.GetResourceString(vbErrors.UserDefined);
		}
		return "";
	}

	public static short Fix(short Number)
	{
		return Number;
	}

	public static int Fix(int Number)
	{
		return Number;
	}

	public static long Fix(long Number)
	{
		return Number;
	}

	public static double Fix(double Number)
	{
		if (Number >= 0.0)
		{
			return Math.Floor(Number);
		}
		return 0.0 - Math.Floor(0.0 - Number);
	}

	public static float Fix(float Number)
	{
		if (Number >= 0f)
		{
			return (float)Math.Floor(Number);
		}
		return (float)(0.0 - Math.Floor(0f - Number));
	}

	public static decimal Fix(decimal Number)
	{
		if (Number < 0m)
		{
			return decimal.Negate(decimal.Floor(decimal.Negate(Number)));
		}
		return decimal.Floor(Number);
	}

	public static object Fix(object Number)
	{
		if (Number == null)
		{
			throw new ArgumentNullException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "Number"));
		}
		if (Number is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return Number;
			case TypeCode.Single:
				return Fix(convertible.ToSingle(null));
			case TypeCode.Double:
				return Fix(convertible.ToDouble(null));
			case TypeCode.Decimal:
				return Fix(convertible.ToDecimal(null));
			case TypeCode.Boolean:
				return convertible.ToInt32(null);
			case TypeCode.String:
				return Fix(Conversions.ToDouble(convertible.ToString(null)));
			}
		}
		throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_NotNumericType2, "Number", Number.GetType().FullName)), 13);
	}

	public static short Int(short Number)
	{
		return Number;
	}

	public static int Int(int Number)
	{
		return Number;
	}

	public static long Int(long Number)
	{
		return Number;
	}

	public static double Int(double Number)
	{
		return Math.Floor(Number);
	}

	public static float Int(float Number)
	{
		return (float)Math.Floor(Number);
	}

	public static decimal Int(decimal Number)
	{
		return decimal.Floor(Number);
	}

	public static object Int(object Number)
	{
		if (Number == null)
		{
			throw new ArgumentNullException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "Number"));
		}
		if (Number is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return Number;
			case TypeCode.Single:
				return Int(convertible.ToSingle(null));
			case TypeCode.Double:
				return Int(convertible.ToDouble(null));
			case TypeCode.Decimal:
				return Int(convertible.ToDecimal(null));
			case TypeCode.Boolean:
				return convertible.ToInt32(null);
			case TypeCode.String:
				return Int(Conversions.ToDouble(convertible.ToString(null)));
			}
		}
		throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_NotNumericType2, "Number", Number.GetType().FullName)), 13);
	}

	[CLSCompliant(false)]
	public static string Hex(sbyte Number)
	{
		return Number.ToString("X");
	}

	public static string Hex(byte Number)
	{
		return Number.ToString("X");
	}

	public static string Hex(short Number)
	{
		return Number.ToString("X");
	}

	[CLSCompliant(false)]
	public static string Hex(ushort Number)
	{
		return Number.ToString("X");
	}

	public static string Hex(int Number)
	{
		return Number.ToString("X");
	}

	[CLSCompliant(false)]
	public static string Hex(uint Number)
	{
		return Number.ToString("X");
	}

	public static string Hex(long Number)
	{
		return Number.ToString("X");
	}

	[CLSCompliant(false)]
	public static string Hex(ulong Number)
	{
		return Number.ToString("X");
	}

	public static string Hex(object Number)
	{
		if (Number == null)
		{
			throw new ArgumentNullException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "Number"));
		}
		if (Number is IConvertible convertible)
		{
			long num;
			switch (convertible.GetTypeCode())
			{
			case TypeCode.SByte:
				return Hex(convertible.ToSByte(null));
			case TypeCode.Byte:
				return Hex(convertible.ToByte(null));
			case TypeCode.Int16:
				return Hex(convertible.ToInt16(null));
			case TypeCode.UInt16:
				return Hex(convertible.ToUInt16(null));
			case TypeCode.Int32:
				return Hex(convertible.ToInt32(null));
			case TypeCode.UInt32:
				return Hex(convertible.ToUInt32(null));
			case TypeCode.Int64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				num = convertible.ToInt64(null);
				goto IL_0124;
			case TypeCode.UInt64:
				return Hex(convertible.ToUInt64(null));
			case TypeCode.String:
				{
					try
					{
						num = Conversions.ToLong(convertible.ToString(null));
					}
					catch (OverflowException)
					{
						return Hex(Conversions.ToULong(convertible.ToString(null)));
					}
					goto IL_0124;
				}
				IL_0124:
				if (num == 0L)
				{
					return "0";
				}
				if (num > 0)
				{
					return Hex(num);
				}
				if (num >= int.MinValue)
				{
					return Hex(checked((int)num));
				}
				return Hex(num);
			}
		}
		throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValueType2, "Number", Utils.VBFriendlyName(Number)));
	}

	[CLSCompliant(false)]
	public static string Oct(sbyte Number)
	{
		return Utils.OctFromLong((long)Number & 0xFFL);
	}

	public static string Oct(byte Number)
	{
		return Utils.OctFromULong(Number);
	}

	public static string Oct(short Number)
	{
		return Utils.OctFromLong((long)Number & 0xFFFFL);
	}

	[CLSCompliant(false)]
	public static string Oct(ushort Number)
	{
		return Utils.OctFromULong(Number);
	}

	public static string Oct(int Number)
	{
		return Utils.OctFromLong(Number & 0xFFFFFFFFu);
	}

	[CLSCompliant(false)]
	public static string Oct(uint Number)
	{
		return Utils.OctFromULong(Number);
	}

	public static string Oct(long Number)
	{
		return Utils.OctFromLong(Number);
	}

	[CLSCompliant(false)]
	public static string Oct(ulong Number)
	{
		return Utils.OctFromULong(Number);
	}

	public static string Oct(object Number)
	{
		if (Number == null)
		{
			throw new ArgumentNullException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "Number"));
		}
		if (Number is IConvertible convertible)
		{
			long num;
			switch (convertible.GetTypeCode())
			{
			case TypeCode.SByte:
				return Oct(convertible.ToSByte(null));
			case TypeCode.Byte:
				return Oct(convertible.ToByte(null));
			case TypeCode.Int16:
				return Oct(convertible.ToInt16(null));
			case TypeCode.UInt16:
				return Oct(convertible.ToUInt16(null));
			case TypeCode.Int32:
				return Oct(convertible.ToInt32(null));
			case TypeCode.UInt32:
				return Oct(convertible.ToUInt32(null));
			case TypeCode.Int64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				num = convertible.ToInt64(null);
				goto IL_0124;
			case TypeCode.UInt64:
				return Oct(convertible.ToUInt64(null));
			case TypeCode.String:
				{
					try
					{
						num = Conversions.ToLong(convertible.ToString(null));
					}
					catch (OverflowException)
					{
						return Oct(Conversions.ToULong(convertible.ToString(null)));
					}
					goto IL_0124;
				}
				IL_0124:
				if (num == 0L)
				{
					return "0";
				}
				if (num > 0)
				{
					return Oct(num);
				}
				if (num >= int.MinValue)
				{
					return Oct(checked((int)num));
				}
				return Oct(num);
			}
		}
		throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValueType2, "Number", Utils.VBFriendlyName(Number)));
	}

	public static string Str(object Number)
	{
		if (Number == null)
		{
			throw new ArgumentNullException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "Number"));
		}
		if (!(Number is IConvertible convertible))
		{
			throw new InvalidCastException(System.SR.Format(System.SR.ArgumentNotNumeric1, "Number"));
		}
		string text;
		switch (convertible.GetTypeCode())
		{
		case TypeCode.DBNull:
			return "Null";
		case TypeCode.Boolean:
			if (convertible.ToBoolean(null))
			{
				return "True";
			}
			return "False";
		case TypeCode.SByte:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.UInt16:
		case TypeCode.Int32:
		case TypeCode.UInt32:
		case TypeCode.Int64:
		case TypeCode.UInt64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			text = Conversions.ToString(Number);
			break;
		case TypeCode.String:
			try
			{
				text = Conversions.ToString(Conversions.ToDouble(convertible.ToString(null)));
			}
			catch (StackOverflowException ex)
			{
				throw ex;
			}
			catch (OutOfMemoryException ex2)
			{
				throw ex2;
			}
			catch (Exception)
			{
				goto default;
			}
			break;
		default:
			throw new InvalidCastException(System.SR.Format(System.SR.ArgumentNotNumeric1, "Number"));
		}
		if (text.Length > 0 && text[0] != '-')
		{
			return " " + Utils.StdFormat(text);
		}
		return Utils.StdFormat(text);
	}

	private static double HexOrOctValue(string InputStr, int i)
	{
		int num = 0;
		int length = InputStr.Length;
		char c = InputStr[i];
		checked
		{
			i++;
			long num3 = default(long);
			if (c == 'H' || c == 'h')
			{
				while (i < length && num < 17)
				{
					c = InputStr[i];
					i++;
					char c2 = c;
					if (c2 == '\t' || c2 == '\n' || c2 == '\r' || c2 == ' ' || c2 == '\u3000')
					{
						continue;
					}
					int num2;
					if (c2 == '0')
					{
						if (num == 0)
						{
							continue;
						}
						num2 = 0;
					}
					else if (c2 >= '1' && c2 <= '9')
					{
						num2 = c - 48;
					}
					else if (c2 >= 'A' && c2 <= 'F')
					{
						num2 = c - 55;
					}
					else
					{
						if (c2 < 'a' || c2 > 'f')
						{
							break;
						}
						num2 = c - 87;
					}
					if (num == 15 && num3 > 576460752303423487L)
					{
						num3 = (num3 & 0x7FFFFFFFFFFFFFFL) * 16;
						num3 |= long.MinValue;
					}
					else
					{
						num3 *= 16;
					}
					num3 += num2;
					num++;
				}
				if (num == 16)
				{
					i++;
					if (i < length)
					{
						c = InputStr[i];
					}
				}
				if (num <= 8)
				{
					if (num > 4 || c == '&')
					{
						if (num3 > int.MaxValue)
						{
							num3 = int.MinValue + (num3 & 0x7FFFFFFF);
						}
					}
					else if ((num > 2 || c == '%') && num3 > 32767)
					{
						num3 = -32768 + (num3 & 0x7FFF);
					}
				}
				switch (c)
				{
				case '%':
					num3 = (short)num3;
					break;
				case '&':
					num3 = (int)num3;
					break;
				}
				return num3;
			}
			if (c == 'O' || c == 'o')
			{
				while (i < length && num < 22)
				{
					c = InputStr[i];
					i++;
					char c3 = c;
					if (c3 == '\t' || c3 == '\n' || c3 == '\r' || c3 == ' ' || c3 == '\u3000')
					{
						continue;
					}
					int num2;
					if (c3 == '0')
					{
						if (num == 0)
						{
							continue;
						}
						num2 = 0;
					}
					else
					{
						if (c3 < '1' || c3 > '7')
						{
							break;
						}
						num2 = c - 48;
					}
					if (num3 >= 1152921504606846976L)
					{
						num3 = (num3 & 0xFFFFFFFFFFFFFFFL) * 8;
						num3 |= 0x1000000000000000L;
					}
					else
					{
						num3 *= 8;
					}
					num3 += num2;
					num++;
				}
				if (num == 22)
				{
					i++;
					if (i < length)
					{
						c = InputStr[i];
					}
				}
				if (num3 <= 4294967296L)
				{
					if (num3 > 65535 || c == '&')
					{
						if (num3 > int.MaxValue)
						{
							num3 = int.MinValue + (num3 & 0x7FFFFFFF);
						}
					}
					else if ((num3 > 255 || c == '%') && num3 > 32767)
					{
						num3 = -32768 + (num3 & 0x7FFF);
					}
				}
				switch (c)
				{
				case '%':
					num3 = (short)num3;
					break;
				case '&':
					num3 = (int)num3;
					break;
				}
				return num3;
			}
			return 0.0;
		}
	}

	public static double Val(string InputStr)
	{
		int num = InputStr?.Length ?? 0;
		checked
		{
			int i;
			for (i = 0; i < num; i++)
			{
				switch (InputStr[i])
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
				case '\u3000':
					continue;
				}
				break;
			}
			if (i >= num)
			{
				return 0.0;
			}
			char c = InputStr[i];
			if (c == '&')
			{
				return HexOrOctValue(InputStr, i + 1);
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			double num2 = 0.0;
			c = InputStr[i];
			switch (c)
			{
			case '-':
				flag3 = true;
				i++;
				break;
			case '+':
				i++;
				break;
			}
			int num4 = default(int);
			double num3 = default(double);
			int num5 = default(int);
			while (i < num)
			{
				c = InputStr[i];
				char c2 = c;
				switch (c2)
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
				case '\u3000':
					i++;
					continue;
				case '0':
					if (num4 != 0 || flag)
					{
						num3 = num3 * 10.0 + (double)unchecked((int)c) - 48.0;
						i++;
						num4++;
					}
					else
					{
						i++;
					}
					continue;
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					num3 = num3 * 10.0 + (double)unchecked((int)c) - 48.0;
					i++;
					num4++;
					continue;
				}
				switch (c2)
				{
				case '.':
					i++;
					if (!flag)
					{
						flag = true;
						num5 = num4;
						continue;
					}
					break;
				case 'D':
				case 'E':
				case 'd':
				case 'e':
					flag2 = true;
					i++;
					break;
				}
				break;
			}
			int num6 = default(int);
			if (flag)
			{
				num6 = num4 - num5;
			}
			if (flag2)
			{
				bool flag4 = false;
				bool flag5 = false;
				while (i < num)
				{
					c = InputStr[i];
					char c3 = c;
					switch (c3)
					{
					case '\t':
					case '\n':
					case '\r':
					case ' ':
					case '\u3000':
						i++;
						continue;
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						num2 = num2 * 10.0 + (double)unchecked((int)c) - 48.0;
						i++;
						continue;
					}
					switch (c3)
					{
					case '+':
						if (!flag4)
						{
							flag4 = true;
							i++;
							continue;
						}
						break;
					case '-':
						if (!flag4)
						{
							flag4 = true;
							flag5 = true;
							i++;
							continue;
						}
						break;
					}
					break;
				}
				if (flag5)
				{
					num2 += (double)num6;
					num3 *= Math.Pow(10.0, 0.0 - num2);
				}
				else
				{
					num2 -= (double)num6;
					num3 *= Math.Pow(10.0, num2);
				}
			}
			else if (flag && num6 != 0)
			{
				num3 /= Math.Pow(10.0, num6);
			}
			if (double.IsInfinity(num3))
			{
				throw ExceptionUtils.VbMakeException(6);
			}
			if (flag3)
			{
				num3 = 0.0 - num3;
			}
			switch (c)
			{
			case '%':
				if (num6 > 0)
				{
					throw ExceptionUtils.VbMakeException(13);
				}
				num3 = (short)Math.Round(num3);
				break;
			case '&':
				if (num6 > 0)
				{
					throw ExceptionUtils.VbMakeException(13);
				}
				num3 = (int)Math.Round(num3);
				break;
			case '!':
				num3 = (float)num3;
				break;
			case '@':
				num3 = Convert.ToDouble(new decimal(num3));
				break;
			}
			return num3;
		}
	}

	public static int Val(char Expression)
	{
		if (Expression >= '1' && Expression <= '9')
		{
			return checked(Expression - 48);
		}
		return 0;
	}

	public static double Val(object Expression)
	{
		if (Expression is string inputStr)
		{
			return Val(inputStr);
		}
		if (Expression is char)
		{
			return Val((char)Expression);
		}
		if (Versioned.IsNumeric(Expression))
		{
			return Conversions.ToDouble(Expression);
		}
		string inputStr2;
		try
		{
			inputStr2 = Conversions.ToString(Expression);
		}
		catch (StackOverflowException ex)
		{
			throw ex;
		}
		catch (OutOfMemoryException ex2)
		{
			throw ex2;
		}
		catch (Exception)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValueType2, "Expression", Utils.VBFriendlyName(Expression))), 438);
		}
		return Val(inputStr2);
	}

	[RequiresUnreferencedCode("Calls UnsafeNativeMethods.VariantChangeType")]
	internal static object ParseInputField(object Value, VariantType vtInput)
	{
		string text = Conversions.ToString(Value);
		if (vtInput == VariantType.Empty && (Value == null || Strings.Len(Conversions.ToString(Value)) == 0))
		{
			return null;
		}
		ProjectData projectData = ProjectData.GetProjectData();
		byte[] numprsPtr = projectData.m_numprsPtr;
		byte[] digitArray = projectData.m_DigitArray;
		Array.Copy(BitConverter.GetBytes(Convert.ToInt32(digitArray.Length)), 0, numprsPtr, 0, 4);
		Array.Copy(BitConverter.GetBytes(Convert.ToInt32(2388)), 0, numprsPtr, 4, 4);
		if (UnsafeNativeMethods.VarParseNumFromStr(text, 1033, int.MinValue, numprsPtr, digitArray) < 0)
		{
			if (vtInput != VariantType.Empty)
			{
				return 0;
			}
			return text;
		}
		int num = BitConverter.ToInt32(numprsPtr, 8);
		int num2 = BitConverter.ToInt32(numprsPtr, 12);
		int num3 = BitConverter.ToInt32(numprsPtr, 16);
		int num4 = BitConverter.ToInt32(numprsPtr, 20);
		char c = default(char);
		if (num2 < text.Length)
		{
			c = text[num2];
		}
		checked
		{
			int vt;
			int num6;
			switch (c)
			{
			case '%':
				vt = 2;
				num6 = 0;
				break;
			case '&':
				vt = 3;
				num6 = 0;
				break;
			case '@':
				vt = 14;
				num6 = 4;
				break;
			case '!':
				vt = ((vtInput != VariantType.Double) ? 4 : 5);
				num6 = int.MaxValue;
				break;
			case '#':
				vt = 5;
				num6 = int.MaxValue;
				break;
			default:
				if (vtInput == VariantType.Empty)
				{
					int dwVtBits = 16428;
					if ((num & 0x800) != 0)
					{
						dwVtBits = 32;
					}
					return UnsafeNativeMethods.VarNumFromParseNum(numprsPtr, digitArray, dwVtBits);
				}
				if (num3 != 0)
				{
					Value = UnsafeNativeMethods.VarNumFromParseNum(numprsPtr, digitArray, 8);
					int num5 = Conversions.ToInteger(Value);
					if ((num5 & -65536) == 0)
					{
						num5 = (short)num5;
					}
					UnsafeNativeMethods.VariantChangeType(out Value, ref Value, 0, (short)vtInput);
					return Value;
				}
				return UnsafeNativeMethods.VarNumFromParseNum(numprsPtr, digitArray, ShiftVTBits(unchecked((int)vtInput)));
			}
			if (-num4 > num6)
			{
				throw ExceptionUtils.VbMakeException(13);
			}
			Value = UnsafeNativeMethods.VarNumFromParseNum(numprsPtr, digitArray, ShiftVTBits(vt));
			if (vtInput == VariantType.Empty)
			{
				return Value;
			}
			UnsafeNativeMethods.VariantChangeType(out Value, ref Value, 0, (short)vtInput);
			return Value;
		}
	}

	private static int ShiftVTBits(int vt)
	{
		switch (vt)
		{
		case 2:
			return 4;
		case 3:
			return 8;
		case 4:
			return 16;
		case 5:
			return 32;
		case 6:
		case 14:
			return 16384;
		case 7:
			return 128;
		case 8:
			return 256;
		case 9:
			return 512;
		case 10:
			return 1024;
		case 11:
			return 2048;
		case 12:
			return 4096;
		case 13:
			return 8192;
		case 17:
			return 131072;
		case 18:
			return 262144;
		case 20:
			return 1048576;
		default:
			return 0;
		}
	}

	[RequiresUnreferencedCode("The Expression's underlying type cannot be statically analyzed and its members may be trimmed")]
	public static object CTypeDynamic(object Expression, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type TargetType)
	{
		return Conversions.ChangeType(Expression, TargetType, Dynamic: true);
	}

	[RequiresUnreferencedCode("The Expression's underlying type cannot be statically analyzed and its members may be trimmed")]
	public static TargetType CTypeDynamic<TargetType>(object Expression)
	{
		return (TargetType)Conversions.ChangeType(Expression, typeof(TargetType), Dynamic: true);
	}
}
