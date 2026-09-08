using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System;

internal static class Number
{
	internal readonly ref struct PowersOf1e9
	{
		private readonly ReadOnlySpan<uint> pow1E9;

		public const uint TenPowMaxPartial = 1000000000u;

		public const int MaxPartialDigits = 9;

		private static ReadOnlySpan<int> Indexes => new int[32]
		{
			0, 1, 3, 6, 12, 23, 44, 86, 170, 338,
			673, 1342, 2680, 5355, 10705, 21405, 42804, 85602, 171198, 342390,
			684773, 1369538, 2739067, 5478125, 10956241, 21912473, 43824936, 87649862, 175299713, 484817143,
			969634274, 1939268536
		};

		private static ReadOnlySpan<uint> LeadingPowers1E9 => new uint[44]
		{
			1000000000u, 2808348672u, 232830643u, 3008077584u, 2076772117u, 12621774u, 4130660608u, 835571558u, 1441351422u, 977976457u,
			264170013u, 37092u, 767623168u, 4241160024u, 1260959332u, 2541775228u, 2965753944u, 1796720685u, 484800439u, 1311835347u,
			2945126454u, 3563705203u, 1375821026u, 3940379521u, 184513341u, 2872588323u, 2214530454u, 38258512u, 2980860351u, 114267010u,
			2188874685u, 234079247u, 2101059099u, 1948702207u, 947446250u, 864457656u, 507589568u, 1321007357u, 3911984176u, 1011110295u,
			2382358050u, 2389730781u, 730678769u, 440721283u
		};

		public PowersOf1e9(Span<uint> pow1E9)
		{
			if (pow1E9.Length <= LeadingPowers1E9.Length)
			{
				this.pow1E9 = LeadingPowers1E9;
				return;
			}
			LeadingPowers1E9.CopyTo(pow1E9.Slice(0, LeadingPowers1E9.Length));
			this.pow1E9 = pow1E9;
			ReadOnlySpan<uint> value = pow1E9.Slice(Indexes[5], Indexes[6] - Indexes[5]);
			int num = Indexes[6];
			for (int i = 6; i + 1 < Indexes.Length; i++)
			{
				if (pow1E9.Length - num < value.Length << 1)
				{
					break;
				}
				Span<uint> bits = pow1E9.Slice(num, value.Length << 1);
				BigIntegerCalculator.Square(value, bits);
				int num2 = num;
				num = Indexes[i + 1];
				value = pow1E9.Slice(num2, num - num2);
			}
		}

		public static int GetBufferSize(int digits, out int maxIndex)
		{
			uint value = (uint)(digits - 1) / 9u;
			maxIndex = BitOperations.Log2(value);
			int num = maxIndex + 1;
			int num2;
			if ((uint)num < (uint)Indexes.Length)
			{
				num2 = Indexes[num];
			}
			else
			{
				maxIndex = Indexes.Length - 2;
				ReadOnlySpan<int> indexes = Indexes;
				num2 = indexes[indexes.Length - 1];
			}
			return ++num2;
		}

		public ReadOnlySpan<uint> GetSpan(int index)
		{
			int num = Indexes[index];
			int num2 = Indexes[index + 1];
			return pow1E9.Slice(num, num2 - num);
		}

		public static int OmittedLength(int index)
		{
			return 9 * (1 << index) >> 5;
		}

		public void MultiplyPowerOfTen(ReadOnlySpan<uint> left, int trailingZeroCount, Span<uint> bits)
		{
			if (trailingZeroCount < UInt32PowersOfTen.Length)
			{
				BigIntegerCalculator.Multiply(left, UInt32PowersOfTen[trailingZeroCount], bits.Slice(0, left.Length + 1));
				return;
			}
			uint[] array = null;
			Span<uint> span = ((bits.Length > 64) ? ((Span<uint>)(array = ArrayPool<uint>.Shared.Rent(bits.Length))) : stackalloc uint[64]).Slice(0, bits.Length);
			Span<uint> span2 = bits;
			int num = Math.DivRem(trailingZeroCount, 9, out var result);
			int num2 = BitOperations.TrailingZeroCount(num);
			int num3 = OmittedLength(num2);
			ReadOnlySpan<uint> span3 = GetSpan(num2);
			int num4 = span3.Length;
			num >>= num2;
			num >>= 1;
			if ((BitOperations.PopCount((uint)num) & 1) != 0)
			{
				span2 = span;
				span = bits;
				span2.Clear();
			}
			span3.CopyTo(span);
			num2++;
			while (num != 0)
			{
				if ((num & 1) != 0)
				{
					num3 += OmittedLength(num2);
					ReadOnlySpan<uint> span4 = GetSpan(num2);
					Span<uint> span5 = span.Slice(0, num4);
					Span<uint> bits2 = span2.Slice(0, num4 += span4.Length);
					if (span4.Length < span5.Length)
					{
						BigIntegerCalculator.Multiply(span5, span4, bits2);
					}
					else
					{
						BigIntegerCalculator.Multiply(span4, span5, bits2);
					}
					Span<uint> span6 = span;
					span = span2;
					span2 = span6;
					span2.Clear();
					while (--num4 >= 0 && span[num4] == 0)
					{
					}
					num4++;
				}
				num2++;
				num >>= 1;
			}
			span = span.Slice(0, num4);
			Span<uint> bits3 = bits.Slice(num3, num4 += left.Length);
			if (left.Length < span.Length)
			{
				BigIntegerCalculator.Multiply(span, left, bits3);
			}
			else
			{
				BigIntegerCalculator.Multiply(left, span, bits3);
			}
			if (array != null)
			{
				ArrayPool<uint>.Shared.Return(array);
			}
			if (result > 0)
			{
				uint num5 = UInt32PowersOfTen[result];
				uint num6 = 0u;
				for (int i = 0; i < bits3.Length; i++)
				{
					ulong num7 = (ulong)((long)num5 * (long)bits3[i] + num6);
					bits3[i] = (uint)num7;
					num6 = (uint)(num7 >> 32);
				}
				if (num6 != 0)
				{
					bits[num3 + num4] = num6;
				}
			}
		}
	}

	internal ref struct NumberBuffer
	{
		public int DigitsCount;

		public int Scale;

		public bool IsNegative;

		public bool HasNonZeroTail;

		public NumberBufferKind Kind;

		public Span<byte> Digits;

		public unsafe readonly byte* DigitsPtr => (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference(Digits));

		public unsafe NumberBuffer(NumberBufferKind kind, byte* digits, int digitsLength)
			: this(kind, new Span<byte>(digits, digitsLength))
		{
		}

		public NumberBuffer(NumberBufferKind kind, Span<byte> digits)
		{
			DigitsCount = 0;
			Scale = 0;
			IsNegative = false;
			HasNonZeroTail = false;
			Kind = kind;
			Digits = digits;
			Digits[0] = 0;
		}

		[Conditional("DEBUG")]
		public void CheckConsistency()
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			stringBuilder.Append('"');
			for (int i = 0; i < Digits.Length; i++)
			{
				byte b = Digits[i];
				if (b == 0)
				{
					break;
				}
				stringBuilder.Append((char)b);
			}
			stringBuilder.Append('"');
			stringBuilder.Append(", Length = ").Append(DigitsCount);
			stringBuilder.Append(", Scale = ").Append(Scale);
			stringBuilder.Append(", IsNegative = ").Append(IsNegative);
			stringBuilder.Append(", HasNonZeroTail = ").Append(HasNonZeroTail);
			stringBuilder.Append(", Kind = ").Append(Kind);
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}
	}

	internal enum NumberBufferKind : byte
	{
		Unknown,
		Integer,
		Decimal,
		FloatingPoint
	}

	internal enum ParsingStatus
	{
		OK,
		Failed,
		Overflow
	}

	public const int BigIntegerParseNaiveThreshold = 1233;

	public const int BigIntegerParseNaiveThresholdInRecursive = 128;

	private static readonly string[] s_posCurrencyFormats = new string[4] { "$#", "#$", "$ #", "# $" };

	private static readonly string[] s_negCurrencyFormats = new string[17]
	{
		"($#)", "-$#", "$-#", "$#-", "(#$)", "-#$", "#-$", "#$-", "-# $", "-$ #",
		"# $-", "$ #-", "$ -#", "#- $", "($ #)", "(# $)", "$- #"
	};

	private static readonly string[] s_posPercentFormats = new string[4] { "# %", "#%", "%#", "% #" };

	private static readonly string[] s_negPercentFormats = new string[12]
	{
		"-# %", "-#%", "-%#", "%-#", "%#-", "#-%", "#%-", "-% #", "# %-", "% #-",
		"% -#", "#- %"
	};

	private static readonly string[] s_negNumberFormats = new string[5] { "(#)", "-#", "- #", "#-", "# -" };

	internal const int DecimalNumberBufferLength = 31;

	internal const int DoubleNumberBufferLength = 769;

	internal const int Int32NumberBufferLength = 11;

	internal const int Int64NumberBufferLength = 20;

	internal const int Int128NumberBufferLength = 40;

	internal const int SingleNumberBufferLength = 114;

	internal const int HalfNumberBufferLength = 23;

	internal const int UInt32NumberBufferLength = 11;

	internal const int UInt64NumberBufferLength = 21;

	internal const int UInt128NumberBufferLength = 40;

	private static ReadOnlySpan<uint> UInt32PowersOfTen => new uint[10] { 1u, 10u, 100u, 1000u, 10000u, 100000u, 1000000u, 10000000u, 100000000u, 1000000000u };

	[DoesNotReturn]
	internal static void ThrowOverflowOrFormatException(ParsingStatus status)
	{
		throw GetException(status);
	}

	private static Exception GetException(ParsingStatus status)
	{
		if (status != ParsingStatus.Failed)
		{
			return new OverflowException(System.SR.Overflow_ParseBigInteger);
		}
		return new FormatException(System.SR.Overflow_ParseBigInteger);
	}

	internal static bool TryValidateParseStyleInteger(NumberStyles style, [NotNullWhen(false)] out ArgumentException? e)
	{
		if ((style & ~(NumberStyles.Any | NumberStyles.AllowHexSpecifier | NumberStyles.AllowBinarySpecifier)) != NumberStyles.None)
		{
			e = new ArgumentException(System.SR.Argument_InvalidNumberStyles, "style");
			return false;
		}
		if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None && (style & ~NumberStyles.HexNumber) != NumberStyles.None)
		{
			e = new ArgumentException(System.SR.Argument_InvalidHexStyle, "style");
			return false;
		}
		e = null;
		return true;
	}

	internal static ParsingStatus TryParseBigInteger(ReadOnlySpan<char> value, NumberStyles style, NumberFormatInfo info, out BigInteger result)
	{
		if (!TryValidateParseStyleInteger(style, out ArgumentException e))
		{
			throw e;
		}
		if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
		{
			return TryParseBigIntegerHexOrBinaryNumberStyle<BigIntegerHexParser<char>, char>(value, style, out result);
		}
		if ((style & NumberStyles.AllowBinarySpecifier) != NumberStyles.None)
		{
			return TryParseBigIntegerHexOrBinaryNumberStyle<BigIntegerBinaryParser<char>, char>(value, style, out result);
		}
		return TryParseBigIntegerNumber(value, style, info, out result);
	}

	internal unsafe static ParsingStatus TryParseBigIntegerNumber(ReadOnlySpan<char> value, NumberStyles style, NumberFormatInfo info, out BigInteger result)
	{
		byte[] array = null;
		if (value.Length == 0)
		{
			result = default(BigInteger);
			return ParsingStatus.Failed;
		}
		Span<byte> digits = ((value.Length >= 255) ? ((Span<byte>)(array = ArrayPool<byte>.Shared.Rent(value.Length + 1 + 1))) : stackalloc byte[value.Length + 1 + 1]);
		ParsingStatus result2;
		fixed (byte* ptr = digits)
		{
			NumberBuffer number = new NumberBuffer(NumberBufferKind.Integer, digits);
			if (!TryStringToNumber(MemoryMarshal.Cast<char, Utf16Char>(value), style, ref number, info))
			{
				result = default(BigInteger);
				result2 = ParsingStatus.Failed;
			}
			else
			{
				result2 = NumberToBigInteger(ref number, out result);
			}
		}
		if (array != null)
		{
			ArrayPool<byte>.Shared.Return(array);
		}
		return result2;
	}

	internal static BigInteger ParseBigInteger(ReadOnlySpan<char> value, NumberStyles style, NumberFormatInfo info)
	{
		if (!TryValidateParseStyleInteger(style, out ArgumentException e))
		{
			throw e;
		}
		ParsingStatus parsingStatus = TryParseBigInteger(value, style, info, out var result);
		if (parsingStatus != ParsingStatus.OK)
		{
			ThrowOverflowOrFormatException(parsingStatus);
		}
		return result;
	}

	internal static ParsingStatus TryParseBigIntegerHexOrBinaryNumberStyle<TParser, TChar>(ReadOnlySpan<TChar> value, NumberStyles style, out BigInteger result) where TParser : struct, IBigIntegerHexOrBinaryParser<TParser, TChar> where TChar : unmanaged, IBinaryInteger<TChar>
	{
		if ((style & NumberStyles.AllowLeadingWhite) != NumberStyles.None)
		{
			int i;
			for (i = 0; i < value.Length && IsWhite(uint.CreateTruncating(value[i])); i++)
			{
			}
			int num = i;
			value = value.Slice(num, value.Length - num);
		}
		if ((style & NumberStyles.AllowTrailingWhite) != NumberStyles.None)
		{
			int i = value.Length - 1;
			while (i >= 0 && IsWhite(uint.CreateTruncating(value[i])))
			{
				i--;
			}
			value = value.Slice(0, i + 1);
		}
		if (!value.IsEmpty)
		{
			uint signBitsIfValid = TParser.GetSignBitsIfValid(uint.CreateTruncating(value[0]));
			int num2 = value.Length % TParser.DigitsPerBlock;
			uint result2 = signBitsIfValid;
			if (num2 != 0)
			{
				if (!TParser.TryParseUnalignedBlock(value.Slice(0, num2), out result2))
				{
					goto IL_0283;
				}
				result2 |= signBitsIfValid << num2 * TParser.BitsPerDigit;
				int num = num2;
				value = value.Slice(num, value.Length - num);
			}
			while (true)
			{
				if (!value.IsEmpty && result2 == signBitsIfValid)
				{
					if (!TParser.TryParseSingleBlock(value.Slice(0, TParser.DigitsPerBlock), out result2))
					{
						break;
					}
					int num = TParser.DigitsPerBlock;
					value = value.Slice(num, value.Length - num);
					continue;
				}
				if (value.IsEmpty)
				{
					if ((int)(result2 ^ signBitsIfValid) >= 0)
					{
						result = new BigInteger((int)result2);
						return ParsingStatus.OK;
					}
					if (result2 != 0)
					{
						result = new BigInteger((int)(signBitsIfValid | 1), new uint[1] { (result2 ^ signBitsIfValid) - signBitsIfValid });
						return ParsingStatus.OK;
					}
					result = new BigInteger(-1, new uint[2] { 0u, 1u });
					return ParsingStatus.OK;
				}
				int num3 = value.Length / TParser.DigitsPerBlock;
				int num4 = num3 + 1;
				if (num4 > BigInteger.MaxLength)
				{
					result = default(BigInteger);
					return ParsingStatus.Overflow;
				}
				uint[] array = new uint[num4];
				Span<uint> destination = array.AsSpan(0, num3);
				if (!TParser.TryParseWholeBlocks(value, destination))
				{
					break;
				}
				array[^1] = result2;
				if (signBitsIfValid != 0)
				{
					if (((ReadOnlySpan<uint>)array.AsSpan()).ContainsAnyExcept(0u))
					{
						NumericsHelpers.DangerousMakeTwosComplement(array);
					}
					else
					{
						array = new uint[array.Length + 1];
						array[^1] = 1u;
					}
					result = new BigInteger(-1, array);
					return ParsingStatus.OK;
				}
				result = new BigInteger(1, array);
				return ParsingStatus.OK;
			}
		}
		goto IL_0283;
		IL_0283:
		result = default(BigInteger);
		return ParsingStatus.Failed;
	}

	private static ParsingStatus NumberToBigInteger(ref NumberBuffer number, out BigInteger result)
	{
		if (number.Scale == int.MaxValue)
		{
			result = default(BigInteger);
			return ParsingStatus.Overflow;
		}
		if (number.Scale < 0)
		{
			result = default(BigInteger);
			return ParsingStatus.Failed;
		}
		uint[] array = null;
		ReadOnlySpan<byte> span = number.Digits.Slice(0, Math.Min(number.Scale, number.DigitsCount));
		int num = span.IndexOf((byte)0);
		if (num < 0)
		{
			ReadOnlySpan<byte> readOnlySpan = number.Digits.Slice(span.Length);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				switch (readOnlySpan[i])
				{
				default:
					result = default(BigInteger);
					return ParsingStatus.Failed;
				case 48:
					continue;
				case 0:
					break;
				}
				break;
			}
		}
		else
		{
			span = span.Slice(0, num);
		}
		int num2 = (span.Length + 9 - 1) / 9;
		Span<uint> span2 = ((num2 > 64) ? ((Span<uint>)(array = ArrayPool<uint>.Shared.Rent(num2))) : stackalloc uint[64]).Slice(0, num2);
		int num3 = num2;
		ReadOnlySpan<byte> utf8Text = span.Slice(0, span.Length % 9);
		if (utf8Text.Length != 0)
		{
			uint.TryParse(utf8Text, out span2[--num3]);
		}
		span = span.Slice(utf8Text.Length);
		for (num3--; num3 >= 0; num3--)
		{
			uint.TryParse(span.Slice(0, 9), out span2[num3]);
			span = span.Slice(9);
		}
		int num4 = checked((int)(0.10381025297 * (double)number.Scale) + 1 + 2);
		uint[] array2 = null;
		Span<uint> span3 = ((num4 > 64) ? ((Span<uint>)(array2 = ArrayPool<uint>.Shared.Rent(num4))) : stackalloc uint[64]).Slice(0, num4);
		span3.Clear();
		int num5 = Math.Min(number.DigitsCount, number.Scale);
		int trailingZeroCount = number.Scale - num5;
		if (number.Scale <= 1233)
		{
			Naive(span2, trailingZeroCount, span3);
		}
		else
		{
			DivideAndConquer(span2, trailingZeroCount, span3);
		}
		result = new BigInteger(span3, number.IsNegative);
		if (array != null)
		{
			ArrayPool<uint>.Shared.Return(array);
		}
		if (array2 != null)
		{
			ArrayPool<uint>.Shared.Return(array2);
		}
		return ParsingStatus.OK;
		static void DivideAndConquer(ReadOnlySpan<uint> base1E9, int num6, scoped Span<uint> bits)
		{
			int bufferSize = PowersOf1e9.GetBufferSize(Math.Max((base1E9.Length - 1) * 9 + System.Buffers.Text.FormattingHelpers.CountDigits(base1E9[base1E9.Length - 1]), num6 + 1), out var maxIndex);
			uint[] array3 = null;
			Span<uint> pow1E = ((bufferSize > 64) ? ((Span<uint>)(array3 = ArrayPool<uint>.Shared.Rent(bufferSize))) : stackalloc uint[64]).Slice(0, bufferSize);
			pow1E.Clear();
			PowersOf1e9 powersOf1e = new PowersOf1e9(pow1E);
			if (num6 > 0)
			{
				int num7 = checked((int)(0.93429227673 * (double)base1E9.Length) + 3);
				uint[] array4 = null;
				Span<uint> span4 = ((num7 > 64) ? ((Span<uint>)(array4 = ArrayPool<uint>.Shared.Rent(num7))) : stackalloc uint[64]).Slice(0, num7);
				span4.Clear();
				Recursive(in powersOf1e, maxIndex, base1E9, span4);
				span4 = span4.Slice(0, BigIntegerCalculator.ActualLength(span4));
				powersOf1e.MultiplyPowerOfTen(span4, num6, bits);
				if (array4 != null)
				{
					ArrayPool<uint>.Shared.Return(array4);
				}
			}
			else
			{
				Recursive(in powersOf1e, maxIndex, base1E9, bits);
			}
			if (array3 != null)
			{
				ArrayPool<uint>.Shared.Return(array3);
			}
		}
		static uint MultiplyAdd(Span<uint> bits, uint multiplier, uint addValue)
		{
			uint num6 = addValue;
			for (int j = 0; j < bits.Length; j++)
			{
				ulong num7 = (ulong)((long)multiplier * (long)bits[j] + num6);
				bits[j] = (uint)num7;
				num6 = (uint)(num7 >> 32);
			}
			return num6;
		}
		static void Naive(ReadOnlySpan<uint> base1E9, int a, scoped Span<uint> bits)
		{
			if (base1E9.Length != 0)
			{
				int length = NaiveBase1E9ToBits(base1E9, bits);
				int num6 = Math.DivRem(a, 9, out var result2);
				for (int j = 0; j < num6; j++)
				{
					uint num7 = MultiplyAdd(bits.Slice(0, length), 1000000000u, 0u);
					if (num7 != 0)
					{
						bits[length++] = num7;
					}
				}
				if (result2 != 0)
				{
					uint multiplier = UInt32PowersOfTen[result2];
					uint num8 = MultiplyAdd(bits.Slice(0, length), multiplier, 0u);
					if (num8 != 0)
					{
						bits[length++] = num8;
					}
				}
			}
		}
		static int NaiveBase1E9ToBits(ReadOnlySpan<uint> base1E9, Span<uint> bits)
		{
			if (base1E9.Length == 0)
			{
				return 0;
			}
			int num6 = 1;
			bits[0] = base1E9[base1E9.Length - 1];
			for (int num7 = base1E9.Length - 2; num7 >= 0; num7--)
			{
				uint num8 = MultiplyAdd(bits.Slice(0, num6), 1000000000u, base1E9[num7]);
				if (num8 != 0)
				{
					bits[num6++] = num8;
				}
			}
			return num6;
		}
		static void Recursive(in PowersOf1e9 powersOf1e9, int powersOf1e9Index, ReadOnlySpan<uint> base1E9, Span<uint> bits)
		{
			base1E9 = base1E9.Slice(0, BigIntegerCalculator.ActualLength(base1E9));
			if (base1E9.Length < 128)
			{
				NaiveBase1E9ToBits(base1E9, bits);
			}
			else
			{
				int num6 = 1 << powersOf1e9Index;
				while (base1E9.Length <= num6)
				{
					num6 = 1 << --powersOf1e9Index;
				}
				ReadOnlySpan<uint> span4 = powersOf1e9.GetSpan(powersOf1e9Index);
				int start = PowersOf1e9.OmittedLength(powersOf1e9Index);
				int num7 = checked((int)(0.93429227673 * (double)num6) + 1 + 2);
				uint[] array3 = null;
				Span<uint> span5 = ((num7 > 64) ? ((Span<uint>)(array3 = ArrayPool<uint>.Shared.Rent(num7))) : stackalloc uint[64]).Slice(0, num7);
				span5.Clear();
				int powersOf1e9Index2 = powersOf1e9Index - 1;
				int num8 = num6;
				Recursive(in powersOf1e9, powersOf1e9Index2, base1E9.Slice(num8, base1E9.Length - num8), span5);
				ReadOnlySpan<uint> readOnlySpan2 = span5.Slice(0, BigIntegerCalculator.ActualLength(span5));
				Span<uint> bits2 = bits.Slice(start, readOnlySpan2.Length + span4.Length);
				if (span4.Length < readOnlySpan2.Length)
				{
					BigIntegerCalculator.Multiply(readOnlySpan2, span4, bits2);
				}
				else
				{
					BigIntegerCalculator.Multiply(span4, readOnlySpan2, bits2);
				}
				span5.Clear();
				Recursive(in powersOf1e9, powersOf1e9Index - 1, base1E9.Slice(0, num6), span5);
				BigIntegerCalculator.AddSelf(bits, span5.Slice(0, BigIntegerCalculator.ActualLength(span5)));
				if (array3 != null)
				{
					ArrayPool<uint>.Shared.Return(array3);
				}
			}
		}
	}

	private static string FormatBigIntegerToHex(bool targetSpan, BigInteger value, char format, int digits, NumberFormatInfo info, Span<char> destination, out int charsWritten, out bool spanSuccess)
	{
		byte[] array = null;
		Span<byte> destination2 = stackalloc byte[64];
		if (!value.TryWriteOrCountBytes(destination2, out var bytesWritten))
		{
			destination2 = (array = ArrayPool<byte>.Shared.Rent(bytesWritten));
			value.TryWriteBytes(destination2, out bytesWritten);
		}
		destination2 = destination2.Slice(0, bytesWritten);
		Span<char> initialBuffer = stackalloc char[128];
		System.Text.ValueStringBuilder valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
		int num = destination2.Length - 1;
		if (num > -1)
		{
			bool flag = false;
			byte b = destination2[num];
			if (b > 247)
			{
				b -= 240;
				flag = true;
			}
			if ((b < 8) | flag)
			{
				valueStringBuilder.Append((b < 10) ? ((char)(b + 48)) : ((format == 'X') ? ((char)((b & 0xF) - 10 + 65)) : ((char)((b & 0xF) - 10 + 97))));
				num--;
			}
		}
		if (num > -1)
		{
			Span<char> span = valueStringBuilder.AppendSpan((num + 1) * 2);
			int num2 = 0;
			string text = ((format == 'x') ? "0123456789abcdef" : "0123456789ABCDEF");
			while (num > -1)
			{
				byte b2 = destination2[num--];
				span[num2++] = text[b2 >> 4];
				span[num2++] = text[b2 & 0xF];
			}
		}
		if (digits > valueStringBuilder.Length)
		{
			valueStringBuilder.Insert(0, (value._sign >= 0) ? '0' : ((format == 'x') ? 'f' : 'F'), digits - valueStringBuilder.Length);
		}
		if (array != null)
		{
			ArrayPool<byte>.Shared.Return(array);
		}
		if (targetSpan)
		{
			spanSuccess = valueStringBuilder.TryCopyTo(destination, out charsWritten);
			return null;
		}
		charsWritten = 0;
		spanSuccess = false;
		return valueStringBuilder.ToString();
	}

	private static string FormatBigIntegerToBinary(bool targetSpan, BigInteger value, int digits, Span<char> destination, out int charsWritten, out bool spanSuccess)
	{
		byte[] array = null;
		Span<byte> destination2 = stackalloc byte[64];
		if (!value.TryWriteOrCountBytes(destination2, out var bytesWritten))
		{
			destination2 = (array = ArrayPool<byte>.Shared.Rent(bytesWritten));
			value.TryWriteBytes(destination2, out var _);
		}
		destination2 = destination2.Slice(0, bytesWritten);
		byte b = destination2[destination2.Length - 1];
		int num = 9 - byte.LeadingZeroCount((value._sign >= 0) ? b : ((byte)(~b)));
		long num2 = num + ((long)(destination2.Length - 1) << 3);
		if (num2 > Array.MaxLength)
		{
			ArrayPool<byte>.Shared.Return(array);
			throw new FormatException(System.SR.Format_TooLarge);
		}
		int num3 = (int)num2;
		int num4 = Math.Max(digits, num3);
		try
		{
			System.Text.ValueStringBuilder sb;
			if (targetSpan)
			{
				if (num4 > destination.Length)
				{
					charsWritten = 0;
					spanSuccess = false;
					return null;
				}
				sb = new System.Text.ValueStringBuilder(destination);
			}
			else
			{
				System.Text.ValueStringBuilder valueStringBuilder;
				if (num4 > 512)
				{
					valueStringBuilder = new System.Text.ValueStringBuilder(num4);
				}
				else
				{
					Span<char> initialBuffer = stackalloc char[512];
					valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
				}
				sb = valueStringBuilder;
			}
			if (digits > num3)
			{
				sb.Append((value._sign >= 0) ? '0' : '1', digits - num3);
			}
			AppendByte(ref sb, b, num - 1);
			for (int num5 = destination2.Length - 2; num5 >= 0; num5--)
			{
				AppendByte(ref sb, destination2[num5]);
			}
			if (targetSpan)
			{
				charsWritten = num4;
				spanSuccess = true;
				return null;
			}
			charsWritten = 0;
			spanSuccess = false;
			return sb.ToString();
		}
		finally
		{
			if (array != null)
			{
				ArrayPool<byte>.Shared.Return(array);
			}
		}
		static void AppendByte(ref System.Text.ValueStringBuilder reference, byte b2, int startHighBit = 7)
		{
			for (int num6 = startHighBit; num6 >= 0; num6--)
			{
				reference.Append((char)(48 + ((b2 >> num6) & 1)));
			}
		}
	}

	internal static string FormatBigInteger(BigInteger value, string? format, NumberFormatInfo info)
	{
		int charsWritten;
		bool spanSuccess;
		return FormatBigInteger(targetSpan: false, value, format, format.AsSpan(), info, default(Span<char>), out charsWritten, out spanSuccess);
	}

	internal static bool TryFormatBigInteger(BigInteger value, ReadOnlySpan<char> format, NumberFormatInfo info, Span<char> destination, out int charsWritten)
	{
		FormatBigInteger(targetSpan: true, value, null, format, info, destination, out charsWritten, out var spanSuccess);
		return spanSuccess;
	}

	private unsafe static string FormatBigInteger(bool targetSpan, BigInteger value, string formatString, ReadOnlySpan<char> formatSpan, NumberFormatInfo info, Span<char> destination, out int charsWritten, out bool spanSuccess)
	{
		int digits = 0;
		char c = ParseFormatSpecifier(formatSpan, out digits);
		switch (c)
		{
		case 'X':
		case 'x':
			return FormatBigIntegerToHex(targetSpan, value, c, digits, info, destination, out charsWritten, out spanSuccess);
		case 'B':
		case 'b':
			return FormatBigIntegerToBinary(targetSpan, value, digits, destination, out charsWritten, out spanSuccess);
		default:
		{
			if (value._bits == null)
			{
				if (c == 'g' || c == 'G' || c == 'r' || c == 'R')
				{
					formatSpan = (formatString = ((digits > 0) ? $"D{digits}" : "D")).AsSpan();
				}
				if (targetSpan)
				{
					spanSuccess = value._sign.TryFormat(destination, out charsWritten, formatSpan, info);
					return null;
				}
				charsWritten = 0;
				spanSuccess = false;
				return value._sign.ToString(formatString, info);
			}
			int num = value._bits.Length;
			int num2 = num * 10 / 9 + 1;
			uint[] array = null;
			Span<uint> span = ((num2 >= 64) ? ((Span<uint>)(array = ArrayPool<uint>.Shared.Rent(num2))) : stackalloc uint[num2]);
			Span<uint> span2 = span;
			int num3 = 0;
			int num4 = num;
			while (--num4 >= 0)
			{
				uint num5 = value._bits[num4];
				for (int i = 0; i < num3; i++)
				{
					(ulong Quotient, ulong Remainder) tuple = Math.DivRem(NumericsHelpers.MakeUInt64(span2[i], num5), 1000000000uL);
					ulong item = tuple.Quotient;
					ulong item2 = tuple.Remainder;
					num5 = (uint)item;
					span2[i] = (uint)item2;
				}
				if (num5 != 0)
				{
					(num5, span2[num3++]) = Math.DivRem(num5, 1000000000u);
					if (num5 != 0)
					{
						span2[num3++] = num5;
					}
				}
			}
			ReadOnlySpan<uint> base1E9Value = span2.Slice(0, num3);
			int num6 = (base1E9Value.Length - 1) * 9 + System.Buffers.Text.FormattingHelpers.CountDigits(base1E9Value[base1E9Value.Length - 1]);
			string result;
			if (c == 'g' || c == 'G' || c == 'd' || c == 'D' || c == 'r' || c == 'R')
			{
				int num7 = Math.Max(digits, num6);
				string text = ((value.Sign < 0) ? info.NegativeSign : null);
				int num8 = num7 + (text?.Length ?? 0);
				if (targetSpan)
				{
					if (destination.Length < num8)
					{
						spanSuccess = false;
						charsWritten = 0;
					}
					else
					{
						text?.CopyTo(destination);
						fixed (char* reference2 = &MemoryMarshal.GetReference(destination))
						{
							BigIntegerToDecChars((Utf16Char*)reference2 + num8, base1E9Value, digits);
						}
						charsWritten = num8;
						spanSuccess = true;
					}
					result = null;
				}
				else
				{
					spanSuccess = false;
					charsWritten = 0;
					fixed (uint* item3 = base1E9Value)
					{
						result = string.Create(num8, (digits, (nint)item3, base1E9Value.Length, text), delegate(Span<char> span5, (int digits, nint ptr, int Length, string sNegative) state)
						{
							state.sNegative?.CopyTo(span5);
							fixed (char* reference3 = &MemoryMarshal.GetReference(span5))
							{
								BigIntegerToDecChars((Utf16Char*)reference3 + span5.Length, new ReadOnlySpan<uint>((void*)state.ptr, state.Length), state.digits);
							}
						});
					}
				}
			}
			else
			{
				byte[] array2 = null;
				Span<byte> span3 = ((num6 + 1 > 32) ? ((Span<byte>)(array2 = ArrayPool<byte>.Shared.Rent(num6 + 1))) : stackalloc byte[num6 + 1]);
				Span<byte> span4 = span3;
				fixed (byte* ptr = span4)
				{
					NumberBuffer number = new NumberBuffer(NumberBufferKind.Integer, ptr, num6 + 1);
					BigIntegerToDecChars((Utf8Char*)ptr + num6, base1E9Value, num6);
					ref Span<byte> digits2 = ref number.Digits;
					digits2[digits2.Length - 1] = 0;
					number.DigitsCount = num6;
					number.Scale = num6;
					number.IsNegative = value.Sign < 0;
					Span<Utf16Char> scratchBuffer = stackalloc Utf16Char[32];
					System.Collections.Generic.ValueListBuilder<Utf16Char> vlb = new System.Collections.Generic.ValueListBuilder<Utf16Char>(scratchBuffer);
					if (c != 0)
					{
						NumberToString(ref vlb, ref number, c, digits, info);
					}
					else
					{
						NumberToStringFormat(ref vlb, ref number, formatSpan, info);
					}
					if (targetSpan)
					{
						spanSuccess = vlb.TryCopyTo(MemoryMarshal.Cast<char, Utf16Char>(destination), out charsWritten);
						result = null;
					}
					else
					{
						charsWritten = 0;
						spanSuccess = false;
						result = MemoryMarshal.Cast<Utf16Char, char>(vlb.AsSpan()).ToString();
					}
					vlb.Dispose();
					if (array2 != null)
					{
						ArrayPool<byte>.Shared.Return(array2);
					}
				}
			}
			if (array != null)
			{
				ArrayPool<uint>.Shared.Return(array);
			}
			return result;
		}
		}
	}

	private unsafe static TChar* BigIntegerToDecChars<TChar>(TChar* bufferEnd, ReadOnlySpan<uint> base1E9Value, int digits) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		for (int i = 0; i < base1E9Value.Length - 1; i++)
		{
			bufferEnd = UInt32ToDecChars(bufferEnd, base1E9Value[i], 9);
			digits -= 9;
		}
		return UInt32ToDecChars(bufferEnd, base1E9Value[base1E9Value.Length - 1], digits);
	}

	internal static bool AllowHyphenDuringParsing(this NumberFormatInfo info)
	{
		string negativeSign = info.NegativeSign;
		bool flag = negativeSign.Length == 1;
		if (flag)
		{
			bool flag2;
			switch (negativeSign[0])
			{
			case '‒':
			case '⁻':
			case '₋':
			case '−':
			case '➖':
			case '﹣':
			case '－':
				flag2 = true;
				break;
			default:
				flag2 = false;
				break;
			}
			flag = flag2;
		}
		return flag;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<TChar> PositiveSignTChar<TChar>(this NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return MemoryMarshal.Cast<char, TChar>(info.PositiveSign.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<TChar> NegativeSignTChar<TChar>(this NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return MemoryMarshal.Cast<char, TChar>(info.NegativeSign.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<TChar> CurrencySymbolTChar<TChar>(this NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return MemoryMarshal.Cast<char, TChar>(info.CurrencySymbol.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<TChar> PercentSymbolTChar<TChar>(this NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return MemoryMarshal.Cast<char, TChar>(info.PercentSymbol.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<TChar> PerMilleSymbolTChar<TChar>(this NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return MemoryMarshal.Cast<char, TChar>(info.PerMilleSymbol.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<TChar> CurrencyDecimalSeparatorTChar<TChar>(this NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return MemoryMarshal.Cast<char, TChar>(info.CurrencyDecimalSeparator.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<TChar> CurrencyGroupSeparatorTChar<TChar>(this NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return MemoryMarshal.Cast<char, TChar>(info.CurrencyGroupSeparator.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<TChar> NumberDecimalSeparatorTChar<TChar>(this NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return MemoryMarshal.Cast<char, TChar>(info.NumberDecimalSeparator.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<TChar> NumberGroupSeparatorTChar<TChar>(this NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return MemoryMarshal.Cast<char, TChar>(info.NumberGroupSeparator.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<TChar> PercentDecimalSeparatorTChar<TChar>(this NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return MemoryMarshal.Cast<char, TChar>(info.PercentDecimalSeparator.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ReadOnlySpan<TChar> PercentGroupSeparatorTChar<TChar>(this NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return MemoryMarshal.Cast<char, TChar>(info.PercentGroupSeparator.AsSpan());
	}

	internal static char ParseFormatSpecifier(ReadOnlySpan<char> format, out int digits)
	{
		char c = '\0';
		if (format.Length > 0)
		{
			c = format[0];
			if (char.IsAsciiLetter(c))
			{
				if (format.Length == 1)
				{
					digits = -1;
					return c;
				}
				if (format.Length == 2)
				{
					int num = format[1] - 48;
					if ((uint)num < 10u)
					{
						digits = num;
						return c;
					}
				}
				else if (format.Length == 3)
				{
					int num2 = format[1] - 48;
					int num3 = format[2] - 48;
					if ((uint)num2 < 10u && (uint)num3 < 10u)
					{
						digits = num2 * 10 + num3;
						return c;
					}
				}
				int num4 = 0;
				int num5 = 1;
				while ((uint)num5 < (uint)format.Length && char.IsAsciiDigit(format[num5]))
				{
					if (num4 >= 100000000)
					{
						System.ThrowHelper.ThrowFormatException_BadFormatSpecifier();
					}
					num4 = num4 * 10 + format[num5++] - 48;
				}
				if ((uint)num5 >= (uint)format.Length || format[num5] == '\0')
				{
					digits = num4;
					return c;
				}
			}
		}
		digits = -1;
		if (format.Length != 0 && c != 0)
		{
			return '\0';
		}
		return 'G';
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static TChar* UInt32ToDecChars<TChar>(TChar* bufferEnd, uint value, int digits) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		while (value != 0 || digits > 0)
		{
			digits--;
			uint num;
			(value, num) = Math.DivRem(value, 10u);
			*(--bufferEnd) = TChar.CastFrom(num + 48);
		}
		return bufferEnd;
	}

	internal static void NumberToString<TChar>(ref System.Collections.Generic.ValueListBuilder<TChar> vlb, ref NumberBuffer number, char format, int nMaxDigits, NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		bool isCorrectlyRounded = number.Kind == NumberBufferKind.FloatingPoint;
		bool suppressScientific;
		switch (format)
		{
		case 'C':
		case 'c':
			if (nMaxDigits < 0)
			{
				nMaxDigits = info.CurrencyDecimalDigits;
			}
			RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
			FormatCurrency(ref vlb, ref number, nMaxDigits, info);
			break;
		case 'F':
		case 'f':
			if (nMaxDigits < 0)
			{
				nMaxDigits = info.NumberDecimalDigits;
			}
			RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
			if (number.IsNegative)
			{
				vlb.Append(info.NegativeSignTChar<TChar>());
			}
			FormatFixed(ref vlb, ref number, nMaxDigits, null, info.NumberDecimalSeparatorTChar<TChar>(), null);
			break;
		case 'N':
		case 'n':
			if (nMaxDigits < 0)
			{
				nMaxDigits = info.NumberDecimalDigits;
			}
			RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
			FormatNumber(ref vlb, ref number, nMaxDigits, info);
			break;
		case 'E':
		case 'e':
			if (nMaxDigits < 0)
			{
				nMaxDigits = 6;
			}
			nMaxDigits++;
			RoundNumber(ref number, nMaxDigits, isCorrectlyRounded);
			if (number.IsNegative)
			{
				vlb.Append(info.NegativeSignTChar<TChar>());
			}
			FormatScientific(ref vlb, ref number, nMaxDigits, info, format);
			break;
		case 'G':
		case 'g':
			suppressScientific = false;
			if (nMaxDigits < 1)
			{
				if (number.Kind == NumberBufferKind.Decimal && nMaxDigits == -1)
				{
					suppressScientific = true;
					if (number.Digits[0] != 0)
					{
						goto IL_018e;
					}
					goto IL_01a3;
				}
				nMaxDigits = number.DigitsCount;
			}
			RoundNumber(ref number, nMaxDigits, isCorrectlyRounded);
			goto IL_018e;
		case 'P':
		case 'p':
			if (nMaxDigits < 0)
			{
				nMaxDigits = info.PercentDecimalDigits;
			}
			number.Scale += 2;
			RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
			FormatPercent(ref vlb, ref number, nMaxDigits, info);
			break;
		case 'R':
		case 'r':
			format = (char)(format - 11);
			goto case 'G';
		default:
			{
				System.ThrowHelper.ThrowFormatException_BadFormatSpecifier();
				break;
			}
			IL_018e:
			if (number.IsNegative)
			{
				vlb.Append(info.NegativeSignTChar<TChar>());
			}
			goto IL_01a3;
			IL_01a3:
			FormatGeneral(ref vlb, ref number, nMaxDigits, info, (char)(format - 2), suppressScientific);
			break;
		}
	}

	internal unsafe static void NumberToStringFormat<TChar>(ref System.Collections.Generic.ValueListBuilder<TChar> vlb, ref NumberBuffer number, ReadOnlySpan<char> format, NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		int num = 0;
		byte* digitsPtr = number.DigitsPtr;
		int num2 = FindSection(format, (*digitsPtr == 0) ? 2 : (number.IsNegative ? 1 : 0));
		int num3;
		int num4;
		bool flag;
		bool flag2;
		int num5;
		int num6;
		int num9;
		while (true)
		{
			num3 = 0;
			num4 = -1;
			num5 = int.MaxValue;
			num6 = 0;
			flag = false;
			int num7 = -1;
			flag2 = false;
			int num8 = 0;
			num9 = num2;
			fixed (char* reference = &MemoryMarshal.GetReference(format))
			{
				char c;
				while (num9 < format.Length && (c = reference[num9++]) != 0)
				{
					switch (c)
					{
					case ';':
						break;
					case '#':
						num3++;
						continue;
					case '0':
						if (num5 == int.MaxValue)
						{
							num5 = num3;
						}
						num3++;
						num6 = num3;
						continue;
					case '.':
						if (num4 < 0)
						{
							num4 = num3;
						}
						continue;
					case ',':
						if (num3 <= 0 || num4 >= 0)
						{
							continue;
						}
						if (num7 >= 0)
						{
							if (num7 == num3)
							{
								num++;
								continue;
							}
							flag2 = true;
						}
						num7 = num3;
						num = 1;
						continue;
					case '%':
						num8 += 2;
						continue;
					case '‰':
						num8 += 3;
						continue;
					case '"':
					case '\'':
						while (num9 < format.Length && reference[num9] != 0 && reference[num9++] != c)
						{
						}
						continue;
					case '\\':
						if (num9 < format.Length && reference[num9] != 0)
						{
							num9++;
						}
						continue;
					case 'E':
					case 'e':
						if ((num9 < format.Length && reference[num9] == '0') || (num9 + 1 < format.Length && (reference[num9] == '+' || reference[num9] == '-') && reference[num9 + 1] == '0'))
						{
							while (++num9 < format.Length && reference[num9] == '0')
							{
							}
							flag = true;
						}
						continue;
					default:
						continue;
					}
					break;
				}
			}
			if (num4 < 0)
			{
				num4 = num3;
			}
			if (num7 >= 0)
			{
				if (num7 == num4)
				{
					num8 -= num * 3;
				}
				else
				{
					flag2 = true;
				}
			}
			if (*digitsPtr != 0)
			{
				number.Scale += num8;
				int pos = (flag ? num3 : (number.Scale + num3 - num4));
				RoundNumber(ref number, pos, isCorrectlyRounded: false);
				if (*digitsPtr != 0)
				{
					break;
				}
				num9 = FindSection(format, 2);
				if (num9 == num2)
				{
					break;
				}
				num2 = num9;
				continue;
			}
			if (number.Kind != NumberBufferKind.FloatingPoint)
			{
				number.IsNegative = false;
			}
			number.Scale = 0;
			break;
		}
		num5 = ((num5 < num4) ? (num4 - num5) : 0);
		num6 = ((num6 > num4) ? (num4 - num6) : 0);
		int num10;
		int num11;
		if (flag)
		{
			num10 = num4;
			num11 = 0;
		}
		else
		{
			num10 = ((number.Scale > num4) ? number.Scale : num4);
			num11 = number.Scale - num4;
		}
		num9 = num2;
		Span<int> span = stackalloc int[4];
		int num12 = -1;
		if (flag2 && info.NumberGroupSeparator.Length > 0)
		{
			int[] array = info.NumberGroupSizes();
			int num13 = 0;
			int i = 0;
			int num14 = array.Length;
			if (num14 != 0)
			{
				i = array[num13];
			}
			int num15 = i;
			int num16 = num10 + ((num11 < 0) ? num11 : 0);
			for (int num17 = ((num5 > num16) ? num5 : num16); num17 > i; i += num15)
			{
				if (num15 == 0)
				{
					break;
				}
				num12++;
				if (num12 >= span.Length)
				{
					int[] array2 = new int[span.Length * 2];
					span.CopyTo(array2);
					span = array2;
				}
				span[num12] = i;
				if (num13 < num14 - 1)
				{
					num13++;
					num15 = array[num13];
				}
			}
		}
		if (number.IsNegative && num2 == 0 && number.Scale != 0)
		{
			vlb.Append(info.NegativeSignTChar<TChar>());
		}
		bool flag3 = false;
		fixed (char* reference2 = &MemoryMarshal.GetReference(format))
		{
			byte* ptr = digitsPtr;
			char c;
			while (num9 < format.Length && (c = reference2[num9++]) != 0 && c != ';')
			{
				if (num11 > 0 && (c == '#' || c == '.' || c == '0'))
				{
					while (num11 > 0)
					{
						vlb.Append(TChar.CastFrom((char)((*ptr != 0) ? (*(ptr++)) : 48)));
						if (flag2 && num10 > 1 && num12 >= 0 && num10 == span[num12] + 1)
						{
							vlb.Append(info.NumberGroupSeparatorTChar<TChar>());
							num12--;
						}
						num10--;
						num11--;
					}
				}
				switch (c)
				{
				case '#':
				case '0':
					if (num11 < 0)
					{
						num11++;
						c = ((num10 <= num5) ? '0' : '\0');
					}
					else
					{
						c = ((*ptr != 0) ? ((char)(*(ptr++))) : ((num10 > num6) ? '0' : '\0'));
					}
					if (c != 0)
					{
						vlb.Append(TChar.CastFrom(c));
						if (flag2 && num10 > 1 && num12 >= 0 && num10 == span[num12] + 1)
						{
							vlb.Append(info.NumberGroupSeparatorTChar<TChar>());
							num12--;
						}
					}
					num10--;
					break;
				case '.':
					if (!((num10 != 0) | flag3) && (num6 < 0 || (num4 < num3 && *ptr != 0)))
					{
						vlb.Append(info.NumberDecimalSeparatorTChar<TChar>());
						flag3 = true;
					}
					break;
				case '‰':
					vlb.Append(info.PerMilleSymbolTChar<TChar>());
					break;
				case '%':
					vlb.Append(info.PercentSymbolTChar<TChar>());
					break;
				case '"':
				case '\'':
					while (num9 < format.Length && reference2[num9] != 0 && reference2[num9] != c)
					{
						AppendUnknownChar(ref vlb, reference2[num9++]);
					}
					if (num9 < format.Length && reference2[num9] != 0)
					{
						num9++;
					}
					break;
				case '\\':
					if (num9 < format.Length && reference2[num9] != 0)
					{
						AppendUnknownChar(ref vlb, reference2[num9++]);
					}
					break;
				case 'E':
				case 'e':
				{
					bool positiveSign = false;
					int num18 = 0;
					if (flag)
					{
						if (num9 < format.Length && reference2[num9] == '0')
						{
							num18++;
						}
						else if (num9 + 1 < format.Length && reference2[num9] == '+' && reference2[num9 + 1] == '0')
						{
							positiveSign = true;
						}
						else if (num9 + 1 >= format.Length || reference2[num9] != '-' || reference2[num9 + 1] != '0')
						{
							vlb.Append(TChar.CastFrom(c));
							break;
						}
						while (++num9 < format.Length && reference2[num9] == '0')
						{
							num18++;
						}
						if (num18 > 10)
						{
							num18 = 10;
						}
						int value = ((*digitsPtr != 0) ? (number.Scale - num4) : 0);
						FormatExponent(ref vlb, info, value, c, num18, positiveSign);
						flag = false;
						break;
					}
					vlb.Append(TChar.CastFrom(c));
					if (num9 < format.Length)
					{
						if (reference2[num9] == '+' || reference2[num9] == '-')
						{
							AppendUnknownChar(ref vlb, reference2[num9++]);
						}
						while (num9 < format.Length && reference2[num9] == '0')
						{
							AppendUnknownChar(ref vlb, reference2[num9++]);
						}
					}
					break;
				}
				default:
					AppendUnknownChar(ref vlb, c);
					break;
				case ',':
					break;
				}
			}
		}
		if (number.IsNegative && num2 == 0 && number.Scale == 0 && vlb.Length > 0)
		{
			vlb.Insert(0, info.NegativeSignTChar<TChar>());
		}
	}

	private static void FormatCurrency<TChar>(ref System.Collections.Generic.ValueListBuilder<TChar> vlb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		string text = (number.IsNegative ? s_negCurrencyFormats[info.CurrencyNegativePattern] : s_posCurrencyFormats[info.CurrencyPositivePattern]);
		foreach (char c in text)
		{
			switch (c)
			{
			case '#':
				FormatFixed(ref vlb, ref number, nMaxDigits, info.CurrencyGroupSizes(), info.CurrencyDecimalSeparatorTChar<TChar>(), info.CurrencyGroupSeparatorTChar<TChar>());
				break;
			case '-':
				vlb.Append(info.NegativeSignTChar<TChar>());
				break;
			case '$':
				vlb.Append(info.CurrencySymbolTChar<TChar>());
				break;
			default:
				vlb.Append(TChar.CastFrom(c));
				break;
			}
		}
	}

	private unsafe static void FormatFixed<TChar>(ref System.Collections.Generic.ValueListBuilder<TChar> vlb, ref NumberBuffer number, int nMaxDigits, int[] groupDigits, ReadOnlySpan<TChar> sDecimal, ReadOnlySpan<TChar> sGroup) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		int num = number.Scale;
		byte* ptr = number.DigitsPtr;
		if (num > 0)
		{
			if (groupDigits != null)
			{
				int num2 = 0;
				int num3 = num;
				int num4 = 0;
				if (groupDigits.Length != 0)
				{
					int num5 = groupDigits[num2];
					while (num > num5 && groupDigits[num2] != 0)
					{
						num3 += sGroup.Length;
						if (num2 < groupDigits.Length - 1)
						{
							num2++;
						}
						num5 += groupDigits[num2];
						ArgumentOutOfRangeException.ThrowIfNegative(num5 | num3, string.Empty);
					}
					num4 = ((num5 != 0) ? groupDigits[0] : 0);
				}
				num2 = 0;
				int num6 = 0;
				int digitsCount = number.DigitsCount;
				int num7 = ((num < digitsCount) ? num : digitsCount);
				fixed (TChar* reference = &MemoryMarshal.GetReference(vlb.AppendSpan(num3)))
				{
					TChar* ptr2 = reference + num3 - 1;
					for (int num8 = num - 1; num8 >= 0; num8--)
					{
						*(ptr2--) = TChar.CastFrom((char)((num8 < num7) ? ptr[num8] : 48));
						if (num4 > 0)
						{
							num6++;
							if (num6 == num4 && num8 != 0)
							{
								for (int num9 = sGroup.Length - 1; num9 >= 0; num9--)
								{
									*(ptr2--) = sGroup[num9];
								}
								if (num2 < groupDigits.Length - 1)
								{
									num2++;
									num4 = groupDigits[num2];
								}
								num6 = 0;
							}
						}
					}
					ptr += num7;
				}
			}
			else
			{
				do
				{
					vlb.Append(TChar.CastFrom((char)((*ptr != 0) ? (*(ptr++)) : 48)));
				}
				while (--num > 0);
			}
		}
		else
		{
			vlb.Append(TChar.CastFrom('0'));
		}
		if (nMaxDigits <= 0)
		{
			return;
		}
		vlb.Append(sDecimal);
		if (num < 0 && nMaxDigits > 0)
		{
			int num10 = Math.Min(-num, nMaxDigits);
			for (int i = 0; i < num10; i++)
			{
				vlb.Append(TChar.CastFrom('0'));
			}
			num += num10;
			nMaxDigits -= num10;
		}
		while (nMaxDigits > 0)
		{
			vlb.Append(TChar.CastFrom((char)((*ptr != 0) ? (*(ptr++)) : 48)));
			nMaxDigits--;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void AppendUnknownChar<TChar>(ref System.Collections.Generic.ValueListBuilder<TChar> vlb, char ch) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		if (sizeof(TChar) == 2 || char.IsAscii(ch))
		{
			vlb.Append(TChar.CastFrom(ch));
		}
		else
		{
			AppendNonAsciiBytes(ref vlb, ch);
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void AppendNonAsciiBytes(ref System.Collections.Generic.ValueListBuilder<TChar> reference, char ch2)
		{
			Rune rune = new Rune(ch2);
			rune.EncodeToUtf8(MemoryMarshal.AsBytes(reference.AppendSpan(rune.Utf8SequenceLength)));
		}
	}

	private static void FormatNumber<TChar>(ref System.Collections.Generic.ValueListBuilder<TChar> vlb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		string text = (number.IsNegative ? s_negNumberFormats[info.NumberNegativePattern] : "#");
		foreach (char c in text)
		{
			switch (c)
			{
			case '#':
				FormatFixed(ref vlb, ref number, nMaxDigits, info.NumberGroupSizes(), info.NumberDecimalSeparatorTChar<TChar>(), info.NumberGroupSeparatorTChar<TChar>());
				break;
			case '-':
				vlb.Append(info.NegativeSignTChar<TChar>());
				break;
			default:
				vlb.Append(TChar.CastFrom(c));
				break;
			}
		}
	}

	private unsafe static void FormatScientific<TChar>(ref System.Collections.Generic.ValueListBuilder<TChar> vlb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info, char expChar) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		byte* digitsPtr = number.DigitsPtr;
		vlb.Append(TChar.CastFrom((char)((*digitsPtr != 0) ? (*(digitsPtr++)) : 48)));
		if (nMaxDigits != 1)
		{
			vlb.Append(info.NumberDecimalSeparatorTChar<TChar>());
		}
		while (--nMaxDigits > 0)
		{
			vlb.Append(TChar.CastFrom((char)((*digitsPtr != 0) ? (*(digitsPtr++)) : 48)));
		}
		int value = ((number.Digits[0] != 0) ? (number.Scale - 1) : 0);
		FormatExponent(ref vlb, info, value, expChar, 3, positiveSign: true);
	}

	private unsafe static void FormatExponent<TChar>(ref System.Collections.Generic.ValueListBuilder<TChar> vlb, NumberFormatInfo info, int value, char expChar, int minDigits, bool positiveSign) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		vlb.Append(TChar.CastFrom(expChar));
		if (value < 0)
		{
			vlb.Append(info.NegativeSignTChar<TChar>());
			value = -value;
		}
		else if (positiveSign)
		{
			vlb.Append(info.PositiveSignTChar<TChar>());
		}
		TChar* ptr = stackalloc TChar[10];
		TChar* ptr2 = UInt32ToDecChars(ptr + 10, (uint)value, minDigits);
		vlb.Append(new ReadOnlySpan<TChar>(ptr2, (int)(ptr + 10 - ptr2)));
	}

	private unsafe static void FormatGeneral<TChar>(ref System.Collections.Generic.ValueListBuilder<TChar> vlb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info, char expChar, bool suppressScientific) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		int i = number.Scale;
		bool flag = false;
		if (!suppressScientific && (i > nMaxDigits || i < -3))
		{
			i = 1;
			flag = true;
		}
		byte* digitsPtr = number.DigitsPtr;
		if (i > 0)
		{
			do
			{
				vlb.Append(TChar.CastFrom((char)((*digitsPtr != 0) ? (*(digitsPtr++)) : 48)));
			}
			while (--i > 0);
		}
		else
		{
			vlb.Append(TChar.CastFrom('0'));
		}
		if (*digitsPtr != 0 || i < 0)
		{
			vlb.Append(info.NumberDecimalSeparatorTChar<TChar>());
			for (; i < 0; i++)
			{
				vlb.Append(TChar.CastFrom('0'));
			}
			while (*digitsPtr != 0)
			{
				vlb.Append(TChar.CastFrom(*(digitsPtr++)));
			}
		}
		if (flag)
		{
			FormatExponent(ref vlb, info, number.Scale - 1, expChar, 2, positiveSign: true);
		}
	}

	private static void FormatPercent<TChar>(ref System.Collections.Generic.ValueListBuilder<TChar> vlb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		string text = (number.IsNegative ? s_negPercentFormats[info.PercentNegativePattern] : s_posPercentFormats[info.PercentPositivePattern]);
		foreach (char c in text)
		{
			switch (c)
			{
			case '#':
				FormatFixed(ref vlb, ref number, nMaxDigits, info.PercentGroupSizes(), info.PercentDecimalSeparatorTChar<TChar>(), info.PercentGroupSeparatorTChar<TChar>());
				break;
			case '-':
				vlb.Append(info.NegativeSignTChar<TChar>());
				break;
			case '%':
				vlb.Append(info.PercentSymbolTChar<TChar>());
				break;
			default:
				vlb.Append(TChar.CastFrom(c));
				break;
			}
		}
	}

	internal unsafe static void RoundNumber(ref NumberBuffer number, int pos, bool isCorrectlyRounded)
	{
		byte* digitsPtr = number.DigitsPtr;
		int i;
		for (i = 0; i < pos && digitsPtr[i] != 0; i++)
		{
		}
		if (i == pos && ShouldRoundUp(digitsPtr, i, number.Kind, isCorrectlyRounded))
		{
			while (i > 0 && digitsPtr[i - 1] == 57)
			{
				i--;
			}
			if (i > 0)
			{
				byte* num = digitsPtr + (i - 1);
				(*num)++;
			}
			else
			{
				number.Scale++;
				*digitsPtr = 49;
				i = 1;
			}
		}
		else
		{
			while (i > 0 && digitsPtr[i - 1] == 48)
			{
				i--;
			}
		}
		if (i == 0)
		{
			if (number.Kind != NumberBufferKind.FloatingPoint)
			{
				number.IsNegative = false;
			}
			number.Scale = 0;
		}
		digitsPtr[i] = 0;
		number.DigitsCount = i;
		unsafe static bool ShouldRoundUp(byte* dig, int num2, NumberBufferKind numberKind, bool flag)
		{
			byte b = dig[num2];
			if ((b == 0) | flag)
			{
				return false;
			}
			return b >= 53;
		}
	}

	private unsafe static int FindSection(ReadOnlySpan<char> format, int section)
	{
		if (section == 0)
		{
			return 0;
		}
		fixed (char* reference = &MemoryMarshal.GetReference(format))
		{
			int num = 0;
			while (true)
			{
				if (num >= format.Length)
				{
					return 0;
				}
				char c2;
				char c = (c2 = reference[num++]);
				if ((uint)c <= 34u)
				{
					if (c == '\0')
					{
						break;
					}
					if (c != '"')
					{
						continue;
					}
				}
				else if (c != '\'')
				{
					switch (c)
					{
					default:
						continue;
					case '\\':
						if (num < format.Length && reference[num] != 0)
						{
							num++;
						}
						continue;
					case ';':
						break;
					}
					if (--section == 0)
					{
						if (num >= format.Length || reference[num] == '\0' || reference[num] == ';')
						{
							break;
						}
						return num;
					}
					continue;
				}
				while (num < format.Length && reference[num] != 0 && reference[num++] != c2)
				{
				}
			}
			return 0;
		}
	}

	private static int[] NumberGroupSizes(this NumberFormatInfo info)
	{
		return info.NumberGroupSizes;
	}

	private static int[] CurrencyGroupSizes(this NumberFormatInfo info)
	{
		return info.CurrencyGroupSizes;
	}

	private static int[] PercentGroupSizes(this NumberFormatInfo info)
	{
		return info.PercentGroupSizes;
	}

	private unsafe static bool TryParseNumber<TChar>(scoped ref TChar* str, TChar* strEnd, NumberStyles styles, ref NumberBuffer number, NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		ReadOnlySpan<TChar> value = ReadOnlySpan<TChar>.Empty;
		bool flag = false;
		ReadOnlySpan<TChar> value2;
		ReadOnlySpan<TChar> value3;
		if ((styles & NumberStyles.AllowCurrencySymbol) != NumberStyles.None)
		{
			value = info.CurrencySymbolTChar<TChar>();
			value2 = info.CurrencyDecimalSeparatorTChar<TChar>();
			value3 = info.CurrencyGroupSeparatorTChar<TChar>();
			flag = true;
		}
		else
		{
			value2 = info.NumberDecimalSeparatorTChar<TChar>();
			value3 = info.NumberGroupSeparatorTChar<TChar>();
		}
		int num = 0;
		TChar* ptr = str;
		uint num2 = ((ptr < strEnd) ? TChar.CastToUInt32(*ptr) : 0u);
		while (true)
		{
			if (!IsWhite(num2) || (styles & NumberStyles.AllowLeadingWhite) == 0 || ((num & 1) != 0 && (num & 0x20) == 0 && info.NumberNegativePattern != 2))
			{
				TChar* ptr2;
				if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None && (num & 1) == 0 && ((ptr2 = MatchChars(ptr, strEnd, info.PositiveSignTChar<TChar>())) != null || ((ptr2 = MatchNegativeSignChars(ptr, strEnd, info)) != null && (number.IsNegative = true))))
				{
					num |= 1;
					ptr = ptr2 - 1;
				}
				else if (num2 == 40 && (styles & NumberStyles.AllowParentheses) != NumberStyles.None && (num & 1) == 0)
				{
					num |= 3;
					number.IsNegative = true;
				}
				else
				{
					if (value.IsEmpty || (ptr2 = MatchChars(ptr, strEnd, value)) == null)
					{
						break;
					}
					num |= 0x20;
					value = ReadOnlySpan<TChar>.Empty;
					ptr = ptr2 - 1;
				}
			}
			num2 = ((++ptr < strEnd) ? TChar.CastToUInt32(*ptr) : 0u);
		}
		int num3 = 0;
		int num4 = 0;
		int num5 = number.Digits.Length - 1;
		int num6 = 0;
		while (true)
		{
			TChar* ptr2;
			if (IsDigit(num2))
			{
				num |= 4;
				if (num2 != 48 || (num & 8) != 0)
				{
					if (num3 < num5)
					{
						number.Digits[num3] = (byte)num2;
						if (num2 != 48 || number.Kind != NumberBufferKind.Integer)
						{
							num4 = num3 + 1;
						}
					}
					else if (num2 != 48)
					{
						number.HasNonZeroTail = true;
					}
					if ((num & 0x10) == 0)
					{
						number.Scale++;
					}
					if (num3 < num5)
					{
						num6 = ((num2 == 48) ? (num6 + 1) : 0);
					}
					num3++;
					num |= 8;
				}
				else if ((num & 0x10) != 0)
				{
					number.Scale--;
				}
			}
			else if ((styles & NumberStyles.AllowDecimalPoint) != NumberStyles.None && (num & 0x10) == 0 && ((ptr2 = MatchChars(ptr, strEnd, value2)) != null || (flag && (num & 0x20) == 0 && (ptr2 = MatchChars(ptr, strEnd, info.NumberDecimalSeparatorTChar<TChar>())) != null)))
			{
				num |= 0x10;
				ptr = ptr2 - 1;
			}
			else
			{
				if ((styles & NumberStyles.AllowThousands) == 0 || (num & 4) == 0 || (num & 0x10) != 0 || ((ptr2 = MatchChars(ptr, strEnd, value3)) == null && (!flag || (num & 0x20) != 0 || (ptr2 = MatchChars(ptr, strEnd, info.NumberGroupSeparatorTChar<TChar>())) == null)))
				{
					break;
				}
				ptr = ptr2 - 1;
			}
			num2 = ((++ptr < strEnd) ? TChar.CastToUInt32(*ptr) : 0u);
		}
		bool flag2 = false;
		number.DigitsCount = num4;
		number.Digits[num4] = 0;
		if ((num & 4) != 0)
		{
			if ((num2 == 69 || num2 == 101) && (styles & NumberStyles.AllowExponent) != NumberStyles.None)
			{
				TChar* ptr3 = ptr;
				num2 = ((++ptr < strEnd) ? TChar.CastToUInt32(*ptr) : 0u);
				TChar* ptr2;
				if ((ptr2 = MatchChars(ptr, strEnd, info.PositiveSignTChar<TChar>())) != null)
				{
					num2 = (((ptr = ptr2) < strEnd) ? TChar.CastToUInt32(*ptr) : 0u);
				}
				else if ((ptr2 = MatchNegativeSignChars(ptr, strEnd, info)) != null)
				{
					num2 = (((ptr = ptr2) < strEnd) ? TChar.CastToUInt32(*ptr) : 0u);
					flag2 = true;
				}
				if (IsDigit(num2))
				{
					int num7 = 0;
					do
					{
						if (num7 >= 100000000)
						{
							num7 = int.MaxValue;
							number.Scale = 0;
							while (IsDigit(num2))
							{
								num2 = ((++ptr < strEnd) ? TChar.CastToUInt32(*ptr) : 0u);
							}
							break;
						}
						num7 = num7 * 10 + (int)(num2 - 48);
						num2 = ((++ptr < strEnd) ? TChar.CastToUInt32(*ptr) : 0u);
					}
					while (IsDigit(num2));
					if (flag2)
					{
						num7 = -num7;
					}
					number.Scale += num7;
				}
				else
				{
					ptr = ptr3;
					num2 = ((ptr < strEnd) ? TChar.CastToUInt32(*ptr) : 0u);
				}
			}
			if (number.Kind == NumberBufferKind.FloatingPoint && !number.HasNonZeroTail)
			{
				int num8 = num4 - number.Scale;
				if (num8 > 0)
				{
					num6 = Math.Min(num6, num8);
					number.DigitsCount = num4 - num6;
					number.Digits[number.DigitsCount] = 0;
				}
			}
			while (true)
			{
				if (!IsWhite(num2) || (styles & NumberStyles.AllowTrailingWhite) == 0)
				{
					TChar* ptr2;
					if ((styles & NumberStyles.AllowTrailingSign) != NumberStyles.None && (num & 1) == 0 && ((ptr2 = MatchChars(ptr, strEnd, info.PositiveSignTChar<TChar>())) != null || ((ptr2 = MatchNegativeSignChars(ptr, strEnd, info)) != null && (number.IsNegative = true))))
					{
						num |= 1;
						ptr = ptr2 - 1;
					}
					else if (num2 == 41 && (num & 2) != 0)
					{
						num &= -3;
					}
					else
					{
						if (value.IsEmpty || (ptr2 = MatchChars(ptr, strEnd, value)) == null)
						{
							break;
						}
						value = ReadOnlySpan<TChar>.Empty;
						ptr = ptr2 - 1;
					}
				}
				num2 = ((++ptr < strEnd) ? TChar.CastToUInt32(*ptr) : 0u);
			}
			if ((num & 2) == 0)
			{
				if ((num & 8) == 0)
				{
					if (number.Kind != NumberBufferKind.Decimal)
					{
						number.Scale = 0;
					}
					if (number.Kind == NumberBufferKind.Integer && (num & 0x10) == 0)
					{
						number.IsNegative = false;
					}
				}
				str = ptr;
				return true;
			}
		}
		str = ptr;
		return false;
	}

	internal unsafe static bool TryStringToNumber<TChar>(ReadOnlySpan<TChar> value, NumberStyles styles, ref NumberBuffer number, NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		fixed (TChar* reference = &MemoryMarshal.GetReference(value))
		{
			TChar* str = reference;
			if (!TryParseNumber(ref str, str + value.Length, styles, ref number, info) || ((int)(str - reference) < value.Length && !TrailingZeros(value, (int)(str - reference))))
			{
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static bool TrailingZeros<TChar>(ReadOnlySpan<TChar> value, int index) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		return !value.Slice(index).ContainsAnyExcept(TChar.CastFrom('\0'));
	}

	private static bool IsWhite(uint ch)
	{
		if (ch != 32)
		{
			return ch - 9 <= 4;
		}
		return true;
	}

	private static bool IsDigit(uint ch)
	{
		return ch - 48 <= 9;
	}

	private static bool IsSpaceReplacingChar(uint c)
	{
		if (c != 160)
		{
			return c == 8239;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static TChar* MatchNegativeSignChars<TChar>(TChar* p, TChar* pEnd, NumberFormatInfo info) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		TChar* ptr = MatchChars(p, pEnd, info.NegativeSignTChar<TChar>());
		if (ptr == null && info.AllowHyphenDuringParsing() && p < pEnd && TChar.CastToUInt32(*p) == 45)
		{
			ptr = p + 1;
		}
		return ptr;
	}

	private unsafe static TChar* MatchChars<TChar>(TChar* p, TChar* pEnd, ReadOnlySpan<TChar> value) where TChar : unmanaged, System.IUtfChar<TChar>
	{
		fixed (TChar* reference = &MemoryMarshal.GetReference(value))
		{
			TChar* ptr = reference;
			if (TChar.CastToUInt32(*ptr) != 0)
			{
				while (true)
				{
					uint num = ((p < pEnd) ? TChar.CastToUInt32(*p) : 0u);
					uint num2 = TChar.CastToUInt32(*ptr);
					if (num != num2 && (!IsSpaceReplacingChar(num2) || num != 32))
					{
						break;
					}
					p++;
					ptr++;
					if (TChar.CastToUInt32(*ptr) == 0)
					{
						return p;
					}
				}
			}
		}
		return null;
	}
}
