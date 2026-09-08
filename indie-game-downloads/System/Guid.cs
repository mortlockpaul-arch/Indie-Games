using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Versioning;
using System.Text;

namespace System;

[Serializable]
[NonVersionable]
[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public readonly struct Guid : ISpanFormattable, IFormattable, IComparable, IComparable<Guid>, IEquatable<Guid>, ISpanParsable<Guid>, IParsable<Guid>, IUtf8SpanFormattable, IUtf8SpanParsable<Guid>
{
	private enum GuidParseThrowStyle : byte
	{
		None,
		All,
		AllButOverflow
	}

	private enum ParseFailure
	{
		Format_ExtraJunkAtEnd,
		Format_GuidBraceAfterLastNumber,
		Format_GuidBrace,
		Format_GuidComma,
		Format_GuidDashes,
		Format_GuidEndBrace,
		Format_GuidHexPrefix,
		Format_GuidInvalidChar,
		Format_GuidInvLen,
		Format_GuidUnrecognized,
		Overflow_Byte,
		Overflow_UInt32
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct GuidResult
	{
		[FieldOffset(0)]
		internal uint _a;

		[FieldOffset(4)]
		internal uint _bc;

		[FieldOffset(4)]
		internal ushort _b;

		[FieldOffset(6)]
		internal ushort _c;

		[FieldOffset(8)]
		internal uint _defg;

		[FieldOffset(8)]
		internal ushort _de;

		[FieldOffset(8)]
		internal byte _d;

		[FieldOffset(10)]
		internal ushort _fg;

		[FieldOffset(12)]
		internal uint _hijk;

		[FieldOffset(16)]
		private readonly GuidParseThrowStyle _throwStyle;

		internal GuidResult(GuidParseThrowStyle canThrow)
		{
			this = default(GuidResult);
			_throwStyle = canThrow;
		}

		internal readonly void SetFailure(ParseFailure failureKind)
		{
			if (_throwStyle == GuidParseThrowStyle.None)
			{
				return;
			}
			if (failureKind == ParseFailure.Overflow_UInt32 && _throwStyle == GuidParseThrowStyle.All)
			{
				throw new OverflowException(SR.Overflow_UInt32);
			}
			throw new FormatException(failureKind switch
			{
				ParseFailure.Format_ExtraJunkAtEnd => SR.Format_ExtraJunkAtEnd, 
				ParseFailure.Format_GuidBraceAfterLastNumber => SR.Format_GuidBraceAfterLastNumber, 
				ParseFailure.Format_GuidBrace => SR.Format_GuidBrace, 
				ParseFailure.Format_GuidComma => SR.Format_GuidComma, 
				ParseFailure.Format_GuidDashes => SR.Format_GuidDashes, 
				ParseFailure.Format_GuidEndBrace => SR.Format_GuidEndBrace, 
				ParseFailure.Format_GuidHexPrefix => SR.Format_GuidHexPrefix, 
				ParseFailure.Format_GuidInvalidChar => SR.Format_GuidInvalidChar, 
				ParseFailure.Format_GuidInvLen => SR.Format_GuidInvLen, 
				_ => SR.Format_GuidUnrecognized, 
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Guid ToGuid()
		{
			return Unsafe.As<GuidResult, Guid>(ref Unsafe.AsRef<GuidResult>(this));
		}
	}

	public static readonly Guid Empty;

	private readonly int _a;

	private readonly short _b;

	private readonly short _c;

	private readonly byte _d;

	private readonly byte _e;

	private readonly byte _f;

	private readonly byte _g;

	private readonly byte _h;

	private readonly byte _i;

	private readonly byte _j;

	private readonly byte _k;

	public static Guid AllBitsSet => new Guid(uint.MaxValue, ushort.MaxValue, ushort.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public int Variant => _d >> 4;

	public int Version => (ushort)_c >>> 12;

	public Guid(byte[] b)
		: this(new ReadOnlySpan<byte>(b ?? throw new ArgumentNullException("b")))
	{
	}

	public Guid(ReadOnlySpan<byte> b)
	{
		if (b.Length != 16)
		{
			ThrowGuidArrayCtorArgumentException();
		}
		this = MemoryMarshal.Read<Guid>(b);
		if (BitConverter.IsLittleEndian)
		{
		}
	}

	public Guid(ReadOnlySpan<byte> b, bool bigEndian)
	{
		if (b.Length != 16)
		{
			ThrowGuidArrayCtorArgumentException();
		}
		this = MemoryMarshal.Read<Guid>(b);
		if (BitConverter.IsLittleEndian == bigEndian)
		{
			_a = BinaryPrimitives.ReverseEndianness(_a);
			_b = BinaryPrimitives.ReverseEndianness(_b);
			_c = BinaryPrimitives.ReverseEndianness(_c);
		}
	}

	[DoesNotReturn]
	[StackTraceHidden]
	private static void ThrowGuidArrayCtorArgumentException()
	{
		throw new ArgumentException(SR.Format(SR.Arg_GuidArrayCtor, "16"), "b");
	}

	[CLSCompliant(false)]
	public Guid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
	{
		_a = (int)a;
		_b = (short)b;
		_c = (short)c;
		_d = d;
		_e = e;
		_f = f;
		_g = g;
		_h = h;
		_i = i;
		_j = j;
		_k = k;
	}

	public Guid(int a, short b, short c, byte[] d)
	{
		ArgumentNullException.ThrowIfNull(d, "d");
		if (d.Length != 8)
		{
			throw new ArgumentException(SR.Format(SR.Arg_GuidArrayCtor, "8"), "d");
		}
		_a = a;
		_b = b;
		_c = c;
		_d = d[0];
		_e = d[1];
		_f = d[2];
		_g = d[3];
		_h = d[4];
		_i = d[5];
		_j = d[6];
		_k = d[7];
	}

	public Guid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
	{
		_a = a;
		_b = b;
		_c = c;
		_d = d;
		_e = e;
		_f = f;
		_g = g;
		_h = h;
		_i = i;
		_j = j;
		_k = k;
	}

	public Guid(string g)
	{
		ArgumentNullException.ThrowIfNull(g, "g");
		GuidResult result = new GuidResult(GuidParseThrowStyle.All);
		TryParseGuid(g.AsSpan(), ref result);
		this = result.ToGuid();
	}

	public static Guid CreateVersion7()
	{
		return CreateVersion7(DateTimeOffset.UtcNow);
	}

	public static Guid CreateVersion7(DateTimeOffset timestamp)
	{
		Guid result = NewGuid();
		long num = timestamp.ToUnixTimeMilliseconds();
		ArgumentOutOfRangeException.ThrowIfNegative(num, "timestamp");
		Unsafe.AsRef(in result._a) = (int)(num >> 16);
		Unsafe.AsRef(in result._b) = (short)num;
		Unsafe.AsRef(in result._c) = (short)((result._c & -61441) | 0x7000);
		Unsafe.AsRef(in result._d) = (byte)((result._d & -193) | 0x80);
		return result;
	}

	public static Guid Parse(string input)
	{
		ArgumentNullException.ThrowIfNull(input, "input");
		return Parse(input.AsSpan());
	}

	public static Guid Parse(ReadOnlySpan<char> input)
	{
		GuidResult result = new GuidResult(GuidParseThrowStyle.AllButOverflow);
		TryParseGuid(input, ref result);
		return result.ToGuid();
	}

	public static Guid Parse(ReadOnlySpan<byte> utf8Text)
	{
		GuidResult result = new GuidResult(GuidParseThrowStyle.AllButOverflow);
		TryParseGuid(utf8Text, ref result);
		return result.ToGuid();
	}

	public static bool TryParse([NotNullWhen(true)] string? input, out Guid result)
	{
		if (input == null)
		{
			result = default(Guid);
			return false;
		}
		return TryParse(input.AsSpan(), out result);
	}

	public static bool TryParse(ReadOnlySpan<char> input, out Guid result)
	{
		GuidResult result2 = new GuidResult(GuidParseThrowStyle.None);
		if (TryParseGuid(input, ref result2))
		{
			result = result2.ToGuid();
			return true;
		}
		result = default(Guid);
		return false;
	}

	public static bool TryParse(ReadOnlySpan<byte> utf8Text, out Guid result)
	{
		GuidResult result2 = new GuidResult(GuidParseThrowStyle.None);
		if (TryParseGuid(utf8Text, ref result2))
		{
			result = result2.ToGuid();
			return true;
		}
		result = default(Guid);
		return false;
	}

	public static Guid ParseExact(string input, [StringSyntax("GuidFormat")] string format)
	{
		ArgumentNullException.ThrowIfNull(input, "input");
		ArgumentNullException.ThrowIfNull(format, "format");
		return ParseExact(input.AsSpan(), format.AsSpan());
	}

	public static Guid ParseExact(ReadOnlySpan<char> input, [StringSyntax("GuidFormat")] ReadOnlySpan<char> format)
	{
		if (format.Length != 1)
		{
			ThrowBadGuidFormatSpecification();
		}
		input = input.Trim();
		GuidResult result = new GuidResult(GuidParseThrowStyle.AllButOverflow);
		switch ((char)(ushort)(format[0] | 0x20))
		{
		case 'd':
		{
			bool flag = TryParseExactD(input, ref result);
			break;
		}
		case 'n':
		{
			bool flag = TryParseExactN(input, ref result);
			break;
		}
		case 'b':
		{
			bool flag = TryParseExactB(input, ref result);
			break;
		}
		case 'p':
		{
			bool flag = TryParseExactP(input, ref result);
			break;
		}
		case 'x':
		{
			bool flag = TryParseExactX(input, ref result);
			break;
		}
		default:
			throw new FormatException(SR.Format_InvalidGuidFormatSpecification);
		}
		return result.ToGuid();
	}

	public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)][StringSyntax("GuidFormat")] string? format, out Guid result)
	{
		if (input == null)
		{
			result = default(Guid);
			return false;
		}
		return TryParseExact(input.AsSpan(), format.AsSpan(), out result);
	}

	public static bool TryParseExact(ReadOnlySpan<char> input, [StringSyntax("GuidFormat")] ReadOnlySpan<char> format, out Guid result)
	{
		if (format.Length != 1 || input.Length < 32)
		{
			result = default(Guid);
			return false;
		}
		input = input.Trim();
		GuidResult result2 = new GuidResult(GuidParseThrowStyle.None);
		if ((format[0] | 0x20) switch
		{
			100 => TryParseExactD(input, ref result2), 
			110 => TryParseExactN(input, ref result2), 
			98 => TryParseExactB(input, ref result2), 
			112 => TryParseExactP(input, ref result2), 
			120 => TryParseExactX(input, ref result2), 
			_ => false, 
		})
		{
			result = result2.ToGuid();
			return true;
		}
		result = default(Guid);
		return false;
	}

	private static bool TryParseGuid<TChar>(ReadOnlySpan<TChar> guidString, ref GuidResult result) where TChar : unmanaged, IUtfChar<TChar>
	{
		guidString = Number.SpanTrim(guidString);
		if (guidString.Length < 32)
		{
			result.SetFailure(ParseFailure.Format_GuidUnrecognized);
			return false;
		}
		return TChar.CastToUInt32(guidString[0]) switch
		{
			40u => TryParseExactP(guidString, ref result), 
			123u => (guidString[9] == TChar.CastFrom('-')) ? TryParseExactB(guidString, ref result) : TryParseExactX(guidString, ref result), 
			_ => (guidString[8] == TChar.CastFrom('-')) ? TryParseExactD(guidString, ref result) : TryParseExactN(guidString, ref result), 
		};
	}

	private static bool TryParseExactB<TChar>(ReadOnlySpan<TChar> guidString, ref GuidResult result) where TChar : unmanaged, IUtfChar<TChar>
	{
		if (guidString.Length != 38 || guidString[0] != TChar.CastFrom('{') || guidString[37] != TChar.CastFrom('}'))
		{
			result.SetFailure(ParseFailure.Format_GuidInvLen);
			return false;
		}
		return TryParseExactD(guidString.Slice(1, 36), ref result);
	}

	private static bool TryParseExactD<TChar>(ReadOnlySpan<TChar> guidString, ref GuidResult result) where TChar : unmanaged, IUtfChar<TChar>
	{
		if (guidString.Length != 36 || guidString[8] != TChar.CastFrom('-') || guidString[13] != TChar.CastFrom('-') || guidString[18] != TChar.CastFrom('-') || guidString[23] != TChar.CastFrom('-'))
		{
			result.SetFailure((guidString.Length != 36) ? ParseFailure.Format_GuidInvLen : ParseFailure.Format_GuidDashes);
			return false;
		}
		Span<byte> span = MemoryMarshal.AsBytes(new Span<GuidResult>(ref result));
		int invalidIfNegative = 0;
		span[0] = DecodeByte(guidString[6], guidString[7], ref invalidIfNegative);
		span[1] = DecodeByte(guidString[4], guidString[5], ref invalidIfNegative);
		span[2] = DecodeByte(guidString[2], guidString[3], ref invalidIfNegative);
		span[3] = DecodeByte(guidString[0], guidString[1], ref invalidIfNegative);
		span[4] = DecodeByte(guidString[11], guidString[12], ref invalidIfNegative);
		span[5] = DecodeByte(guidString[9], guidString[10], ref invalidIfNegative);
		span[6] = DecodeByte(guidString[16], guidString[17], ref invalidIfNegative);
		span[7] = DecodeByte(guidString[14], guidString[15], ref invalidIfNegative);
		span[8] = DecodeByte(guidString[19], guidString[20], ref invalidIfNegative);
		span[9] = DecodeByte(guidString[21], guidString[22], ref invalidIfNegative);
		span[10] = DecodeByte(guidString[24], guidString[25], ref invalidIfNegative);
		span[11] = DecodeByte(guidString[26], guidString[27], ref invalidIfNegative);
		span[12] = DecodeByte(guidString[28], guidString[29], ref invalidIfNegative);
		span[13] = DecodeByte(guidString[30], guidString[31], ref invalidIfNegative);
		span[14] = DecodeByte(guidString[32], guidString[33], ref invalidIfNegative);
		span[15] = DecodeByte(guidString[34], guidString[35], ref invalidIfNegative);
		if (invalidIfNegative >= 0)
		{
			if (!BitConverter.IsLittleEndian)
			{
			}
			return true;
		}
		if (guidString.ContainsAny(TChar.CastFrom('X'), TChar.CastFrom('x'), TChar.CastFrom('+')) && TryCompatParsing(guidString, ref result))
		{
			return true;
		}
		result.SetFailure(ParseFailure.Format_GuidInvalidChar);
		return false;
		static bool TryCompatParsing(ReadOnlySpan<TChar> readOnlySpan, ref GuidResult reference)
		{
			if (TryParseHex(readOnlySpan.Slice(0, 8), out reference._a) && TryParseHex(readOnlySpan.Slice(9, 4), out var result2))
			{
				reference._b = (ushort)result2;
				if (TryParseHex(readOnlySpan.Slice(14, 4), out result2))
				{
					reference._c = (ushort)result2;
					if (TryParseHex(readOnlySpan.Slice(19, 4), out result2))
					{
						if (!BitConverter.IsLittleEndian)
						{
						}
						reference._de = BinaryPrimitives.ReverseEndianness((ushort)result2);
						if (TryParseHex(readOnlySpan.Slice(24, 4), out result2))
						{
							if (!BitConverter.IsLittleEndian)
							{
							}
							reference._fg = BinaryPrimitives.ReverseEndianness((ushort)result2);
							if (Number.TryParseBinaryIntegerHexNumberStyle<TChar, uint>(readOnlySpan.Slice(28, 8), NumberStyles.AllowHexSpecifier, out result2) == Number.ParsingStatus.OK)
							{
								if (!BitConverter.IsLittleEndian)
								{
								}
								reference._hijk = BinaryPrimitives.ReverseEndianness(result2);
								return true;
							}
						}
					}
				}
			}
			return false;
		}
	}

	private static bool TryParseExactN<TChar>(ReadOnlySpan<TChar> guidString, ref GuidResult result) where TChar : unmanaged, IUtfChar<TChar>
	{
		if (guidString.Length != 32)
		{
			result.SetFailure(ParseFailure.Format_GuidInvLen);
			return false;
		}
		Span<byte> span = MemoryMarshal.AsBytes(new Span<GuidResult>(ref result));
		int invalidIfNegative = 0;
		span[0] = DecodeByte(guidString[6], guidString[7], ref invalidIfNegative);
		span[1] = DecodeByte(guidString[4], guidString[5], ref invalidIfNegative);
		span[2] = DecodeByte(guidString[2], guidString[3], ref invalidIfNegative);
		span[3] = DecodeByte(guidString[0], guidString[1], ref invalidIfNegative);
		span[4] = DecodeByte(guidString[10], guidString[11], ref invalidIfNegative);
		span[5] = DecodeByte(guidString[8], guidString[9], ref invalidIfNegative);
		span[6] = DecodeByte(guidString[14], guidString[15], ref invalidIfNegative);
		span[7] = DecodeByte(guidString[12], guidString[13], ref invalidIfNegative);
		span[8] = DecodeByte(guidString[16], guidString[17], ref invalidIfNegative);
		span[9] = DecodeByte(guidString[18], guidString[19], ref invalidIfNegative);
		span[10] = DecodeByte(guidString[20], guidString[21], ref invalidIfNegative);
		span[11] = DecodeByte(guidString[22], guidString[23], ref invalidIfNegative);
		span[12] = DecodeByte(guidString[24], guidString[25], ref invalidIfNegative);
		span[13] = DecodeByte(guidString[26], guidString[27], ref invalidIfNegative);
		span[14] = DecodeByte(guidString[28], guidString[29], ref invalidIfNegative);
		span[15] = DecodeByte(guidString[30], guidString[31], ref invalidIfNegative);
		if (invalidIfNegative >= 0)
		{
			if (!BitConverter.IsLittleEndian)
			{
			}
			return true;
		}
		result.SetFailure(ParseFailure.Format_GuidInvalidChar);
		return false;
	}

	private static bool TryParseExactP<TChar>(ReadOnlySpan<TChar> guidString, ref GuidResult result) where TChar : unmanaged, IUtfChar<TChar>
	{
		if (guidString.Length != 38 || guidString[0] != TChar.CastFrom('(') || guidString[37] != TChar.CastFrom(')'))
		{
			result.SetFailure(ParseFailure.Format_GuidInvLen);
			return false;
		}
		return TryParseExactD(guidString.Slice(1, 36), ref result);
	}

	private static bool TryParseExactX<TChar>(ReadOnlySpan<TChar> guidString, ref GuidResult result) where TChar : unmanaged, IUtfChar<TChar>
	{
		guidString = EatAllWhitespace(guidString, ref result);
		if (guidString.Length == 0 || guidString[0] != TChar.CastFrom('{'))
		{
			result.SetFailure(ParseFailure.Format_GuidBrace);
			return false;
		}
		if (!IsHexPrefix(guidString, 1))
		{
			result.SetFailure(ParseFailure.Format_GuidHexPrefix);
			return false;
		}
		int num = 3;
		int num2 = guidString.Slice(num).IndexOf(TChar.CastFrom(','));
		if (num2 <= 0)
		{
			result.SetFailure(ParseFailure.Format_GuidComma);
			return false;
		}
		bool overflow = false;
		if (!TryParseHex(guidString.Slice(num, num2), out result._a, ref overflow) | overflow)
		{
			result.SetFailure(overflow ? ParseFailure.Overflow_UInt32 : ParseFailure.Format_GuidInvalidChar);
			return false;
		}
		if (!IsHexPrefix(guidString, num + num2 + 1))
		{
			result.SetFailure(ParseFailure.Format_GuidHexPrefix);
			return false;
		}
		num = num + num2 + 3;
		num2 = guidString.Slice(num).IndexOf(TChar.CastFrom(','));
		if (num2 <= 0)
		{
			result.SetFailure(ParseFailure.Format_GuidComma);
			return false;
		}
		if (!TryParseHex(guidString.Slice(num, num2), out result._b, ref overflow) | overflow)
		{
			result.SetFailure(overflow ? ParseFailure.Overflow_UInt32 : ParseFailure.Format_GuidInvalidChar);
			return false;
		}
		if (!IsHexPrefix(guidString, num + num2 + 1))
		{
			result.SetFailure(ParseFailure.Format_GuidHexPrefix);
			return false;
		}
		num = num + num2 + 3;
		num2 = guidString.Slice(num).IndexOf(TChar.CastFrom(','));
		if (num2 <= 0)
		{
			result.SetFailure(ParseFailure.Format_GuidComma);
			return false;
		}
		if (!TryParseHex(guidString.Slice(num, num2), out result._c, ref overflow) | overflow)
		{
			result.SetFailure(overflow ? ParseFailure.Overflow_UInt32 : ParseFailure.Format_GuidInvalidChar);
			return false;
		}
		if ((uint)guidString.Length <= (uint)(num + num2 + 1) || guidString[num + num2 + 1] != TChar.CastFrom('{'))
		{
			result.SetFailure(ParseFailure.Format_GuidBrace);
			return false;
		}
		num2++;
		for (int i = 0; i < 8; i++)
		{
			if (!IsHexPrefix(guidString, num + num2 + 1))
			{
				result.SetFailure(ParseFailure.Format_GuidHexPrefix);
				return false;
			}
			num = num + num2 + 3;
			if (i < 7)
			{
				num2 = guidString.Slice(num).IndexOf(TChar.CastFrom(','));
				if (num2 <= 0)
				{
					result.SetFailure(ParseFailure.Format_GuidComma);
					return false;
				}
			}
			else
			{
				num2 = guidString.Slice(num).IndexOf(TChar.CastFrom('}'));
				if (num2 <= 0)
				{
					result.SetFailure(ParseFailure.Format_GuidBraceAfterLastNumber);
					return false;
				}
			}
			if ((!TryParseHex(guidString.Slice(num, num2), out uint result2, ref overflow) | overflow) || result2 > 255)
			{
				result.SetFailure(overflow ? ParseFailure.Overflow_UInt32 : ((result2 > 255) ? ParseFailure.Overflow_Byte : ParseFailure.Format_GuidInvalidChar));
				return false;
			}
			Unsafe.Add(ref result._d, i) = (byte)result2;
		}
		if (num + num2 + 1 >= guidString.Length || guidString[num + num2 + 1] != TChar.CastFrom('}'))
		{
			result.SetFailure(ParseFailure.Format_GuidEndBrace);
			return false;
		}
		if (num + num2 + 1 != guidString.Length - 1)
		{
			result.SetFailure(ParseFailure.Format_ExtraJunkAtEnd);
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte DecodeByte<TChar>(TChar ch1, TChar ch2, ref int invalidIfNegative) where TChar : unmanaged, IUtfChar<TChar>
	{
		ReadOnlySpan<byte> charToHexLookup = HexConverter.CharToHexLookup;
		sbyte num = (sbyte)charToHexLookup[byte.CreateTruncating(ch1)];
		int num2 = (sbyte)charToHexLookup[byte.CreateTruncating(ch2)];
		int num3 = (num << 4) | num2;
		uint num4 = TChar.CastToUInt32(ch1);
		uint num5 = TChar.CastToUInt32(ch2);
		num3 = (((num4 | num5) >> 8 == 0) ? num3 : (-1));
		invalidIfNegative |= num3;
		return (byte)num3;
	}

	private static bool TryParseHex<TChar>(ReadOnlySpan<TChar> guidString, out ushort result, ref bool overflow) where TChar : unmanaged, IUtfChar<TChar>
	{
		bool result2 = TryParseHex(guidString, out uint result3, ref overflow);
		result = (ushort)result3;
		return result2;
	}

	private static bool TryParseHex<TChar>(ReadOnlySpan<TChar> guidString, out uint result) where TChar : unmanaged, IUtfChar<TChar>
	{
		bool overflow = false;
		return TryParseHex(guidString, out result, ref overflow);
	}

	private static bool TryParseHex<TChar>(ReadOnlySpan<TChar> guidString, out uint result, ref bool overflow) where TChar : unmanaged, IUtfChar<TChar>
	{
		if (guidString.Length > 0)
		{
			if (guidString[0] == TChar.CastFrom('+'))
			{
				guidString = guidString.Slice(1);
			}
			if (guidString.Length > 1 && guidString[0] == TChar.CastFrom('0') && (guidString[1] | TChar.CastFrom(32)) == TChar.CastFrom('x'))
			{
				guidString = guidString.Slice(2);
			}
		}
		int i;
		for (i = 0; i < guidString.Length && guidString[i] == TChar.CastFrom('0'); i++)
		{
		}
		int num = 0;
		uint num2 = 0u;
		for (; i < guidString.Length; i++)
		{
			int num3 = HexConverter.FromChar(int.CreateTruncating(guidString[i]));
			if (num3 == 255)
			{
				if (num > 8)
				{
					overflow = true;
				}
				result = 0u;
				return false;
			}
			num2 = num2 * 16 + (uint)num3;
			num++;
		}
		if (num > 8)
		{
			overflow = true;
		}
		result = num2;
		return true;
	}

	private static ReadOnlySpan<TChar> EatAllWhitespace<TChar>(ReadOnlySpan<TChar> str, scoped ref GuidResult result) where TChar : unmanaged, IUtfChar<TChar>
	{
		if (typeof(TChar) == typeof(char))
		{
			ReadOnlySpan<char> readOnlySpan = Unsafe.BitCast<ReadOnlySpan<TChar>, ReadOnlySpan<char>>(str);
			int i;
			for (i = 0; i < readOnlySpan.Length && !char.IsWhiteSpace(readOnlySpan[i]); i++)
			{
			}
			if (i == readOnlySpan.Length)
			{
				return str;
			}
			char[] array = new char[readOnlySpan.Length];
			int length = 0;
			if (i > 0)
			{
				length = i;
				readOnlySpan.Slice(0, i).CopyTo(array);
			}
			for (; i < readOnlySpan.Length; i++)
			{
				char c = readOnlySpan[i];
				if (!char.IsWhiteSpace(c))
				{
					array[length++] = c;
				}
			}
			return Unsafe.BitCast<ReadOnlySpan<char>, ReadOnlySpan<TChar>>(new ReadOnlySpan<char>(array, 0, length));
		}
		ReadOnlySpan<byte> readOnlySpan2 = Unsafe.BitCast<ReadOnlySpan<TChar>, ReadOnlySpan<byte>>(str);
		int j;
		int bytesConsumed;
		for (j = 0; j < readOnlySpan2.Length; j += bytesConsumed)
		{
			if (Rune.DecodeFromUtf8(readOnlySpan2.Slice(j), out var result2, out bytesConsumed) != OperationStatus.Done)
			{
				result.SetFailure(ParseFailure.Format_GuidInvalidChar);
				return ReadOnlySpan<TChar>.Empty;
			}
			if (!Rune.IsWhiteSpace(result2))
			{
				break;
			}
		}
		if (j == readOnlySpan2.Length)
		{
			return str;
		}
		Span<byte> destination = new byte[readOnlySpan2.Length];
		int num = 0;
		if (j > 0)
		{
			num = j;
			readOnlySpan2.Slice(0, j).CopyTo(destination);
		}
		int bytesConsumed2;
		for (; j < readOnlySpan2.Length; j += bytesConsumed2)
		{
			if (Rune.DecodeFromUtf8(readOnlySpan2.Slice(j), out var result3, out bytesConsumed2) != OperationStatus.Done)
			{
				result.SetFailure(ParseFailure.Format_GuidInvalidChar);
				return ReadOnlySpan<TChar>.Empty;
			}
			if (!Rune.IsWhiteSpace(result3))
			{
				readOnlySpan2.Slice(j, bytesConsumed2).CopyTo(destination.Slice(num));
				num += bytesConsumed2;
			}
		}
		return Unsafe.BitCast<ReadOnlySpan<byte>, ReadOnlySpan<TChar>>((ReadOnlySpan<byte>)destination.Slice(0, num));
	}

	private static bool IsHexPrefix<TChar>(ReadOnlySpan<TChar> str, int i) where TChar : unmanaged, IUtfChar<TChar>
	{
		if (i + 1 < str.Length && str[i] == TChar.CastFrom('0'))
		{
			return (str[i + 1] | TChar.CastFrom(32)) == TChar.CastFrom('x');
		}
		return false;
	}

	public byte[] ToByteArray()
	{
		byte[] array = new byte[16];
		_ = BitConverter.IsLittleEndian;
		MemoryMarshal.Write<Guid>(array, this);
		return array;
	}

	public byte[] ToByteArray(bool bigEndian)
	{
		byte[] array = new byte[16];
		if (BitConverter.IsLittleEndian != bigEndian)
		{
			MemoryMarshal.Write<Guid>(array, this);
		}
		else
		{
			Guid value = new Guid(MemoryMarshal.AsBytes(new ReadOnlySpan<Guid>(this)), bigEndian);
			MemoryMarshal.Write(array, in value);
		}
		return array;
	}

	public bool TryWriteBytes(Span<byte> destination)
	{
		if (destination.Length < 16)
		{
			return false;
		}
		_ = BitConverter.IsLittleEndian;
		MemoryMarshal.Write<Guid>(destination, this);
		return true;
	}

	public bool TryWriteBytes(Span<byte> destination, bool bigEndian, out int bytesWritten)
	{
		if (destination.Length < 16)
		{
			bytesWritten = 0;
			return false;
		}
		if (BitConverter.IsLittleEndian != bigEndian)
		{
			MemoryMarshal.Write<Guid>(destination, this);
		}
		else
		{
			Guid value = new Guid(MemoryMarshal.AsBytes(new ReadOnlySpan<Guid>(this)), bigEndian);
			MemoryMarshal.Write(destination, in value);
		}
		bytesWritten = 16;
		return true;
	}

	public override int GetHashCode()
	{
		ref int reference = ref Unsafe.AsRef(in _a);
		return reference ^ Unsafe.Add(ref reference, 1) ^ Unsafe.Add(ref reference, 2) ^ Unsafe.Add(ref reference, 3);
	}

	public override bool Equals([NotNullWhen(true)] object? o)
	{
		if (o is Guid right)
		{
			return EqualsCore(this, in right);
		}
		return false;
	}

	public bool Equals(Guid g)
	{
		return EqualsCore(this, in g);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool EqualsCore(in Guid left, in Guid right)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			return Unsafe.BitCast<Guid, Vector128<byte>>(left) == Unsafe.BitCast<Guid, Vector128<byte>>(right);
		}
		ref int reference = ref Unsafe.AsRef(in left._a);
		ref int reference2 = ref Unsafe.AsRef(in right._a);
		if (reference == reference2 && Unsafe.Add(ref reference, 1) == Unsafe.Add(ref reference2, 1) && Unsafe.Add(ref reference, 2) == Unsafe.Add(ref reference2, 2))
		{
			return Unsafe.Add(ref reference, 3) == Unsafe.Add(ref reference2, 3);
		}
		return false;
	}

	private static int GetResult(uint me, uint them)
	{
		if (me >= them)
		{
			return 1;
		}
		return -1;
	}

	public int CompareTo(object? value)
	{
		if (value == null)
		{
			return 1;
		}
		if (!(value is Guid value2))
		{
			throw new ArgumentException(SR.Arg_MustBeGuid, "value");
		}
		return CompareTo(value2);
	}

	public int CompareTo(Guid value)
	{
		if (value._a != _a)
		{
			return GetResult((uint)_a, (uint)value._a);
		}
		if (value._b != _b)
		{
			return GetResult((uint)_b, (uint)value._b);
		}
		if (value._c != _c)
		{
			return GetResult((uint)_c, (uint)value._c);
		}
		if (value._d != _d)
		{
			return GetResult(_d, value._d);
		}
		if (value._e != _e)
		{
			return GetResult(_e, value._e);
		}
		if (value._f != _f)
		{
			return GetResult(_f, value._f);
		}
		if (value._g != _g)
		{
			return GetResult(_g, value._g);
		}
		if (value._h != _h)
		{
			return GetResult(_h, value._h);
		}
		if (value._i != _i)
		{
			return GetResult(_i, value._i);
		}
		if (value._j != _j)
		{
			return GetResult(_j, value._j);
		}
		if (value._k != _k)
		{
			return GetResult(_k, value._k);
		}
		return 0;
	}

	public static bool operator ==(Guid a, Guid b)
	{
		return EqualsCore(in a, in b);
	}

	public static bool operator !=(Guid a, Guid b)
	{
		return !EqualsCore(in a, in b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static int HexsToChars<TChar>(TChar* guidChars, int a, int b) where TChar : unmanaged, IUtfChar<TChar>
	{
		*guidChars = TChar.CastFrom(HexConverter.ToCharLower(a >> 4));
		guidChars[1] = TChar.CastFrom(HexConverter.ToCharLower(a));
		guidChars[2] = TChar.CastFrom(HexConverter.ToCharLower(b >> 4));
		guidChars[3] = TChar.CastFrom(HexConverter.ToCharLower(b));
		return 4;
	}

	public override string ToString()
	{
		return ToString("d", null);
	}

	public string ToString([StringSyntax("GuidFormat")] string? format)
	{
		return ToString(format, null);
	}

	public string ToString([StringSyntax("GuidFormat")] string? format, IFormatProvider? provider)
	{
		int num;
		if (string.IsNullOrEmpty(format))
		{
			num = 36;
		}
		else
		{
			if (format.Length != 1)
			{
				ThrowBadGuidFormatSpecification();
			}
			switch (format[0] | 0x20)
			{
			case 100:
				num = 36;
				break;
			case 110:
				num = 32;
				break;
			case 98:
			case 112:
				num = 38;
				break;
			case 120:
				num = 68;
				break;
			default:
				num = 0;
				ThrowBadGuidFormatSpecification();
				break;
			}
		}
		string text = string.FastAllocateString(num);
		TryFormatCore(new Span<char>(ref text.GetRawStringData(), text.Length), out var _, format.AsSpan());
		return text;
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax("GuidFormat")] ReadOnlySpan<char> format = default(ReadOnlySpan<char>))
	{
		return TryFormatCore(destination, out charsWritten, format);
	}

	bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, [StringSyntax("GuidFormat")] ReadOnlySpan<char> format, IFormatProvider provider)
	{
		return TryFormatCore(destination, out charsWritten, format);
	}

	public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, [StringSyntax("GuidFormat")] ReadOnlySpan<char> format = default(ReadOnlySpan<char>))
	{
		return TryFormatCore(utf8Destination, out bytesWritten, format);
	}

	bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, [StringSyntax("GuidFormat")] ReadOnlySpan<char> format, IFormatProvider provider)
	{
		return TryFormatCore(utf8Destination, out bytesWritten, format);
	}

	private bool TryFormatCore<TChar>(Span<TChar> destination, out int charsWritten, ReadOnlySpan<char> format) where TChar : unmanaged, IUtfChar<TChar>
	{
		int flags;
		if (format.Length == 0)
		{
			flags = -2147483612;
		}
		else
		{
			if (format.Length != 1)
			{
				ThrowBadGuidFormatSpecification();
			}
			switch (format[0] | 0x20)
			{
			case 100:
				flags = -2147483612;
				break;
			case 112:
				flags = -2144786394;
				break;
			case 98:
				flags = -2139260122;
				break;
			case 110:
				flags = 32;
				break;
			case 120:
				return TryFormatX(destination, out charsWritten);
			default:
				flags = 0;
				ThrowBadGuidFormatSpecification();
				break;
			}
		}
		return TryFormatCore(destination, out charsWritten, flags);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe bool TryFormatCore<TChar>(Span<TChar> destination, out int charsWritten, int flags) where TChar : unmanaged, IUtfChar<TChar>
	{
		if ((byte)flags > destination.Length)
		{
			charsWritten = 0;
			return false;
		}
		charsWritten = (byte)flags;
		flags >>= 8;
		fixed (TChar* reference = &MemoryMarshal.GetReference(destination))
		{
			TChar* ptr = reference;
			if ((byte)flags != 0)
			{
				*(ptr++) = TChar.CastFrom((byte)flags);
			}
			flags >>= 8;
			if (Ssse3.IsSupported ? true : false)
			{
				_ = BitConverter.IsLittleEndian;
				var (source, source2, source3) = FormatGuidVector128Utf8(this, flags < 0);
				if (typeof(TChar) == typeof(byte))
				{
					byte* ptr2 = (byte*)ptr;
					if (flags < 0)
					{
						source.Store(ptr2);
						source2.Store(ptr2 + 20);
						source3.Store(ptr2 + 8);
						ptr += 36;
					}
					else
					{
						source.Store(ptr2);
						source2.Store(ptr2 + 16);
						ptr += 32;
					}
				}
				else
				{
					(Vector128<ushort> Lower, Vector128<ushort> Upper) tuple2 = Vector128.Widen(source);
					Vector128<ushort> item = tuple2.Lower;
					Vector128<ushort> item2 = tuple2.Upper;
					(Vector128<ushort> Lower, Vector128<ushort> Upper) tuple3 = Vector128.Widen(source2);
					Vector128<ushort> item3 = tuple3.Lower;
					Vector128<ushort> item4 = tuple3.Upper;
					ushort* ptr3 = (ushort*)ptr;
					if (flags < 0)
					{
						var (source4, source5) = Vector128.Widen(source3);
						item.Store(ptr3);
						item3.Store(ptr3 + 20);
						item4.Store(ptr3 + 28);
						source4.Store(ptr3 + 8);
						source5.Store(ptr3 + 16);
						ptr += 36;
					}
					else
					{
						item.Store(ptr3);
						item2.Store(ptr3 + 8);
						item3.Store(ptr3 + 16);
						item4.Store(ptr3 + 24);
						ptr += 32;
					}
				}
			}
			else
			{
				ptr += HexsToChars(ptr, _a >> 24, _a >> 16);
				ptr += HexsToChars(ptr, _a >> 8, _a);
				if (flags < 0)
				{
					*(ptr++) = TChar.CastFrom('-');
				}
				ptr += HexsToChars(ptr, _b >> 8, _b);
				if (flags < 0)
				{
					*(ptr++) = TChar.CastFrom('-');
				}
				ptr += HexsToChars(ptr, _c >> 8, _c);
				if (flags < 0)
				{
					*(ptr++) = TChar.CastFrom('-');
				}
				ptr += HexsToChars(ptr, _d, _e);
				if (flags < 0)
				{
					*(ptr++) = TChar.CastFrom('-');
				}
				ptr += HexsToChars(ptr, _f, _g);
				ptr += HexsToChars(ptr, _h, _i);
				ptr += HexsToChars(ptr, _j, _k);
			}
			if ((byte)flags != 0)
			{
				*ptr = TChar.CastFrom((byte)flags);
			}
		}
		return true;
	}

	private bool TryFormatX<TChar>(Span<TChar> dest, out int charsWritten) where TChar : unmanaged, IUtfChar<TChar>
	{
		if (dest.Length < 68)
		{
			charsWritten = 0;
			return false;
		}
		dest[0] = TChar.CastFrom('{');
		dest[1] = TChar.CastFrom('0');
		dest[2] = TChar.CastFrom('x');
		dest[3] = TChar.CastFrom(HexConverter.ToCharLower(_a >> 28));
		dest[4] = TChar.CastFrom(HexConverter.ToCharLower(_a >> 24));
		dest[5] = TChar.CastFrom(HexConverter.ToCharLower(_a >> 20));
		dest[6] = TChar.CastFrom(HexConverter.ToCharLower(_a >> 16));
		dest[7] = TChar.CastFrom(HexConverter.ToCharLower(_a >> 12));
		dest[8] = TChar.CastFrom(HexConverter.ToCharLower(_a >> 8));
		dest[9] = TChar.CastFrom(HexConverter.ToCharLower(_a >> 4));
		dest[10] = TChar.CastFrom(HexConverter.ToCharLower(_a));
		dest[11] = TChar.CastFrom(',');
		dest[12] = TChar.CastFrom('0');
		dest[13] = TChar.CastFrom('x');
		dest[14] = TChar.CastFrom(HexConverter.ToCharLower(_b >> 12));
		dest[15] = TChar.CastFrom(HexConverter.ToCharLower(_b >> 8));
		dest[16] = TChar.CastFrom(HexConverter.ToCharLower(_b >> 4));
		dest[17] = TChar.CastFrom(HexConverter.ToCharLower(_b));
		dest[18] = TChar.CastFrom(',');
		dest[19] = TChar.CastFrom('0');
		dest[20] = TChar.CastFrom('x');
		dest[21] = TChar.CastFrom(HexConverter.ToCharLower(_c >> 12));
		dest[22] = TChar.CastFrom(HexConverter.ToCharLower(_c >> 8));
		dest[23] = TChar.CastFrom(HexConverter.ToCharLower(_c >> 4));
		dest[24] = TChar.CastFrom(HexConverter.ToCharLower(_c));
		dest[25] = TChar.CastFrom(',');
		dest[26] = TChar.CastFrom('{');
		WriteHex(dest, 27, _d);
		WriteHex(dest, 32, _e);
		WriteHex(dest, 37, _f);
		WriteHex(dest, 42, _g);
		WriteHex(dest, 47, _h);
		WriteHex(dest, 52, _i);
		WriteHex(dest, 57, _j);
		WriteHex(dest, 62, _k, appendComma: false);
		dest[66] = TChar.CastFrom('}');
		dest[67] = TChar.CastFrom('}');
		charsWritten = 68;
		return true;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void WriteHex(Span<TChar> span, int offset, int val, bool appendComma = true)
		{
			span[offset] = TChar.CastFrom('0');
			span[offset + 1] = TChar.CastFrom('x');
			span[offset + 2] = TChar.CastFrom(HexConverter.ToCharLower(val >> 4));
			span[offset + 3] = TChar.CastFrom(HexConverter.ToCharLower(val));
			if (appendComma)
			{
				span[offset + 4] = TChar.CastFrom(',');
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	private static (Vector128<byte>, Vector128<byte>, Vector128<byte>) FormatGuidVector128Utf8(Guid value, bool useDashes)
	{
		Vector128<byte> hexMap = Vector128.Create((byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102);
		(Vector128<byte>, Vector128<byte>) tuple = HexConverter.AsciiToHexVector128(Unsafe.BitCast<Guid, Vector128<byte>>(value), hexMap);
		Vector128<byte> item = tuple.Item1;
		Vector128<byte> item2 = tuple.Item2;
		item = Vector128.Shuffle(item.AsInt16(), Vector128.Create(3, 2, 1, 0, 5, 4, 7, 6)).AsByte();
		if (useDashes)
		{
			Vector128<byte> item3 = Vector128.Shuffle(item, Vector128.Create(506097522914230528L, 940406845091678463L).AsByte());
			Vector128<byte> item4 = Vector128.Shuffle(item2, Vector128.Create(506097527142154753L, 1084818905618843912L).AsByte());
			Vector128<byte> vector = Vector128.Create(49478023249965L, 3242591731709706240L).AsByte();
			if (false)
			{
			}
			Vector128<byte> vector2 = Vector128.Shuffle(item, Vector128.Create(940406845091678463uL, 18446744073709489934uL).AsByte());
			Vector128<byte> vector3 = Vector128.Shuffle(item2, Vector128.Create(ulong.MaxValue, 18375533107936755711uL).AsByte());
			Vector128<byte> item5 = vector2 | vector3 | vector;
			return (item3, item4, item5);
		}
		return (item, item2, default(Vector128<byte>));
	}

	public static bool operator <(Guid left, Guid right)
	{
		if (left._a != right._a)
		{
			return (uint)left._a < (uint)right._a;
		}
		if (left._b != right._b)
		{
			return (uint)left._b < (uint)right._b;
		}
		if (left._c != right._c)
		{
			return (uint)left._c < (uint)right._c;
		}
		if (left._d != right._d)
		{
			return left._d < right._d;
		}
		if (left._e != right._e)
		{
			return left._e < right._e;
		}
		if (left._f != right._f)
		{
			return left._f < right._f;
		}
		if (left._g != right._g)
		{
			return left._g < right._g;
		}
		if (left._h != right._h)
		{
			return left._h < right._h;
		}
		if (left._i != right._i)
		{
			return left._i < right._i;
		}
		if (left._j != right._j)
		{
			return left._j < right._j;
		}
		if (left._k != right._k)
		{
			return left._k < right._k;
		}
		return false;
	}

	public static bool operator <=(Guid left, Guid right)
	{
		if (left._a != right._a)
		{
			return (uint)left._a < (uint)right._a;
		}
		if (left._b != right._b)
		{
			return (uint)left._b < (uint)right._b;
		}
		if (left._c != right._c)
		{
			return (uint)left._c < (uint)right._c;
		}
		if (left._d != right._d)
		{
			return left._d < right._d;
		}
		if (left._e != right._e)
		{
			return left._e < right._e;
		}
		if (left._f != right._f)
		{
			return left._f < right._f;
		}
		if (left._g != right._g)
		{
			return left._g < right._g;
		}
		if (left._h != right._h)
		{
			return left._h < right._h;
		}
		if (left._i != right._i)
		{
			return left._i < right._i;
		}
		if (left._j != right._j)
		{
			return left._j < right._j;
		}
		if (left._k != right._k)
		{
			return left._k < right._k;
		}
		return true;
	}

	public static bool operator >(Guid left, Guid right)
	{
		if (left._a != right._a)
		{
			return (uint)left._a > (uint)right._a;
		}
		if (left._b != right._b)
		{
			return (uint)left._b > (uint)right._b;
		}
		if (left._c != right._c)
		{
			return (uint)left._c > (uint)right._c;
		}
		if (left._d != right._d)
		{
			return left._d > right._d;
		}
		if (left._e != right._e)
		{
			return left._e > right._e;
		}
		if (left._f != right._f)
		{
			return left._f > right._f;
		}
		if (left._g != right._g)
		{
			return left._g > right._g;
		}
		if (left._h != right._h)
		{
			return left._h > right._h;
		}
		if (left._i != right._i)
		{
			return left._i > right._i;
		}
		if (left._j != right._j)
		{
			return left._j > right._j;
		}
		if (left._k != right._k)
		{
			return left._k > right._k;
		}
		return false;
	}

	public static bool operator >=(Guid left, Guid right)
	{
		if (left._a != right._a)
		{
			return (uint)left._a > (uint)right._a;
		}
		if (left._b != right._b)
		{
			return (uint)left._b > (uint)right._b;
		}
		if (left._c != right._c)
		{
			return (uint)left._c > (uint)right._c;
		}
		if (left._d != right._d)
		{
			return left._d > right._d;
		}
		if (left._e != right._e)
		{
			return left._e > right._e;
		}
		if (left._f != right._f)
		{
			return left._f > right._f;
		}
		if (left._g != right._g)
		{
			return left._g > right._g;
		}
		if (left._h != right._h)
		{
			return left._h > right._h;
		}
		if (left._i != right._i)
		{
			return left._i > right._i;
		}
		if (left._j != right._j)
		{
			return left._j > right._j;
		}
		if (left._k != right._k)
		{
			return left._k > right._k;
		}
		return true;
	}

	public static Guid Parse(string s, IFormatProvider? provider)
	{
		return Parse(s);
	}

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Guid result)
	{
		return TryParse(s, out result);
	}

	public static Guid Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
	{
		return Parse(s);
	}

	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Guid result)
	{
		return TryParse(s, out result);
	}

	[DoesNotReturn]
	private static void ThrowBadGuidFormatSpecification()
	{
		throw new FormatException(SR.Format_InvalidGuidFormatSpecification);
	}

	public static Guid Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
	{
		return Parse(utf8Text);
	}

	public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out Guid result)
	{
		return TryParse(utf8Text, out result);
	}

	public unsafe static Guid NewGuid()
	{
		Unsafe.SkipInit(out Guid result);
		int num = Interop.Ole32.CoCreateGuid(&result);
		if (num != 0)
		{
			ThrowForHr(num);
		}
		return result;
	}

	private static void ThrowForHr(int hr)
	{
		throw new Exception
		{
			HResult = hr
		};
	}
}
