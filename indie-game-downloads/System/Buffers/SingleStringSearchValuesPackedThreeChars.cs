using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal sealed class SingleStringSearchValuesPackedThreeChars<TValueLength, TCaseSensitivity> : StringSearchValuesBase where TValueLength : struct, StringSearchValuesHelper.IValueLength where TCaseSensitivity : struct, StringSearchValuesHelper.ICaseSensitivity
{
	private readonly StringSearchValuesHelper.SingleValueState _valueState;

	private readonly nint _minusValueTailLength;

	private readonly nuint _ch2ByteOffset;

	private readonly nuint _ch3ByteOffset;

	private readonly byte _ch1;

	private readonly byte _ch2;

	private readonly byte _ch3;

	private static bool IgnoreCase => typeof(TCaseSensitivity) != typeof(StringSearchValuesHelper.CaseSensitive);

	private static bool CanSkipAnchorMatchVerification
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (Sse2.IsSupported && typeof(TValueLength) == typeof(StringSearchValuesHelper.ValueLengthLessThan4))
			{
				if (!(typeof(TCaseSensitivity) == typeof(StringSearchValuesHelper.CaseSensitive)))
				{
					return typeof(TCaseSensitivity) == typeof(StringSearchValuesHelper.CaseInsensitiveAsciiLetters);
				}
				return true;
			}
			return false;
		}
	}

	public SingleStringSearchValuesPackedThreeChars(HashSet<string> uniqueValues, string value, int ch2Offset, int ch3Offset)
		: base(uniqueValues)
	{
		_valueState = new StringSearchValuesHelper.SingleValueState(value, IgnoreCase);
		_minusValueTailLength = -(value.Length - 1);
		_ch1 = (byte)value[0];
		_ch2 = (byte)value[ch2Offset];
		_ch3 = (byte)value[ch3Offset];
		if (IgnoreCase)
		{
			_ch1 &= 223;
			_ch2 &= 223;
			_ch3 &= 223;
		}
		_ch2ByteOffset = (nuint)ch2Offset * (nuint)2u;
		_ch3ByteOffset = (nuint)ch3Offset * (nuint)2u;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int IndexOfAnyMultiString(ReadOnlySpan<char> span)
	{
		return IndexOf(ref MemoryMarshal.GetReference(span), span.Length);
	}

	private int IndexOf(ref char searchSpace, int searchSpaceLength)
	{
		ref char searchSpaceStart = ref searchSpace;
		nint num = searchSpaceLength + _minusValueTailLength;
		nuint ch2ByteOffset = _ch2ByteOffset;
		nuint ch3ByteOffset = _ch3ByteOffset;
		if (Vector512.IsHardwareAccelerated && Avx512BW.IsSupported && num - Vector512<byte>.Count >= 0)
		{
			Vector512<byte> ch = Vector512.Create(_ch1);
			Vector512<byte> ch2 = Vector512.Create(_ch2);
			Vector512<byte> ch3 = Vector512.Create(_ch3);
			ref char reference = ref Unsafe.Add(ref searchSpace, num - Vector512<byte>.Count);
			int offsetFromStart;
			while (true)
			{
				Vector512<byte> comparisonResult = GetComparisonResult(ref searchSpace, ch2ByteOffset, ch3ByteOffset, ch, ch2, ch3);
				if (comparisonResult != Vector512<byte>.Zero && TryMatch(ref searchSpaceStart, searchSpaceLength, ref searchSpace, PackedSpanHelpers.FixUpPackedVector512Result(comparisonResult).ExtractMostSignificantBits(), out offsetFromStart))
				{
					break;
				}
				searchSpace = ref Unsafe.Add(ref searchSpace, Vector512<byte>.Count);
				if (Unsafe.IsAddressGreaterThan(in searchSpace, in reference))
				{
					if (Unsafe.AreSame(in searchSpace, in Unsafe.Add(ref reference, Vector512<byte>.Count)))
					{
						return -1;
					}
					searchSpace = ref reference;
				}
			}
			return offsetFromStart;
		}
		if (Vector256.IsHardwareAccelerated && Avx2.IsSupported && num - Vector256<byte>.Count >= 0)
		{
			Vector256<byte> ch4 = Vector256.Create(_ch1);
			Vector256<byte> ch5 = Vector256.Create(_ch2);
			Vector256<byte> ch6 = Vector256.Create(_ch3);
			ref char reference2 = ref Unsafe.Add(ref searchSpace, num - Vector256<byte>.Count);
			int offsetFromStart2;
			while (true)
			{
				Vector256<byte> comparisonResult2 = GetComparisonResult(ref searchSpace, ch2ByteOffset, ch3ByteOffset, ch4, ch5, ch6);
				if (comparisonResult2 != Vector256<byte>.Zero && TryMatch(ref searchSpaceStart, searchSpaceLength, ref searchSpace, PackedSpanHelpers.FixUpPackedVector256Result(comparisonResult2).ExtractMostSignificantBits(), out offsetFromStart2))
				{
					break;
				}
				searchSpace = ref Unsafe.Add(ref searchSpace, Vector256<byte>.Count);
				if (Unsafe.IsAddressGreaterThan(in searchSpace, in reference2))
				{
					if (Unsafe.AreSame(in searchSpace, in Unsafe.Add(ref reference2, Vector256<byte>.Count)))
					{
						return -1;
					}
					searchSpace = ref reference2;
				}
			}
			return offsetFromStart2;
		}
		if ((Sse2.IsSupported ? true : false) && num - Vector128<byte>.Count >= 0)
		{
			Vector128<byte> ch7 = Vector128.Create(_ch1);
			Vector128<byte> ch8 = Vector128.Create(_ch2);
			Vector128<byte> ch9 = Vector128.Create(_ch3);
			ref char reference3 = ref Unsafe.Add(ref searchSpace, num - Vector128<byte>.Count);
			int offsetFromStart3;
			while (true)
			{
				Vector128<byte> comparisonResult3 = GetComparisonResult(ref searchSpace, ch2ByteOffset, ch3ByteOffset, ch7, ch8, ch9);
				if (comparisonResult3 != Vector128<byte>.Zero && TryMatch(ref searchSpaceStart, searchSpaceLength, ref searchSpace, comparisonResult3.ExtractMostSignificantBits(), out offsetFromStart3))
				{
					break;
				}
				searchSpace = ref Unsafe.Add(ref searchSpace, Vector128<byte>.Count);
				if (Unsafe.IsAddressGreaterThan(in searchSpace, in reference3))
				{
					if (Unsafe.AreSame(in searchSpace, in Unsafe.Add(ref reference3, Vector128<byte>.Count)))
					{
						return -1;
					}
					searchSpace = ref reference3;
				}
			}
			return offsetFromStart3;
		}
		char rawStringData = _valueState.Value.GetRawStringData();
		for (nint num2 = 0; num2 < num; num2++)
		{
			ref char reference4 = ref Unsafe.Add(ref searchSpace, num2);
			if ((typeof(TCaseSensitivity) == typeof(StringSearchValuesHelper.CaseInsensitiveUnicode) || TCaseSensitivity.TransformInput(reference4) == rawStringData) && TCaseSensitivity.Equals<TValueLength>(ref reference4, in _valueState))
			{
				return (int)num2;
			}
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Sse2))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	private static Vector128<byte> GetComparisonResult(ref char searchSpace, nuint ch2ByteOffset, nuint ch3ByteOffset, Vector128<byte> ch1, Vector128<byte> ch2, Vector128<byte> ch3)
	{
		if (typeof(TCaseSensitivity) == typeof(StringSearchValuesHelper.CaseSensitive))
		{
			Vector128<byte> vector = Vector128.Equals(ch1, LoadPacked128(ref searchSpace, 0u));
			Vector128<byte> vector2 = Vector128.Equals(ch2, LoadPacked128(ref searchSpace, ch2ByteOffset));
			Vector128<byte> vector3 = Vector128.Equals(ch3, LoadPacked128(ref searchSpace, ch3ByteOffset));
			return (vector & vector2 & vector3).AsByte();
		}
		Vector128<byte> vector4 = Vector128.Create((byte)223);
		Vector128<byte> vector5 = Vector128.Equals(ch1, LoadPacked128(ref searchSpace, 0u) & vector4);
		Vector128<byte> vector6 = Vector128.Equals(ch2, LoadPacked128(ref searchSpace, ch2ByteOffset) & vector4);
		Vector128<byte> vector7 = Vector128.Equals(ch3, LoadPacked128(ref searchSpace, ch3ByteOffset) & vector4);
		return (vector5 & vector6 & vector7).AsByte();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static Vector256<byte> GetComparisonResult(ref char searchSpace, nuint ch2ByteOffset, nuint ch3ByteOffset, Vector256<byte> ch1, Vector256<byte> ch2, Vector256<byte> ch3)
	{
		if (typeof(TCaseSensitivity) == typeof(StringSearchValuesHelper.CaseSensitive))
		{
			Vector256<byte> vector = Vector256.Equals(ch1, LoadPacked256(ref searchSpace, 0u));
			Vector256<byte> vector2 = Vector256.Equals(ch2, LoadPacked256(ref searchSpace, ch2ByteOffset));
			Vector256<byte> vector3 = Vector256.Equals(ch3, LoadPacked256(ref searchSpace, ch3ByteOffset));
			return (vector & vector2 & vector3).AsByte();
		}
		Vector256<byte> vector4 = Vector256.Create((byte)223);
		Vector256<byte> vector5 = Vector256.Equals(ch1, LoadPacked256(ref searchSpace, 0u) & vector4);
		Vector256<byte> vector6 = Vector256.Equals(ch2, LoadPacked256(ref searchSpace, ch2ByteOffset) & vector4);
		Vector256<byte> vector7 = Vector256.Equals(ch3, LoadPacked256(ref searchSpace, ch3ByteOffset) & vector4);
		return (vector5 & vector6 & vector7).AsByte();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512BW))]
	private static Vector512<byte> GetComparisonResult(ref char searchSpace, nuint ch2ByteOffset, nuint ch3ByteOffset, Vector512<byte> ch1, Vector512<byte> ch2, Vector512<byte> ch3)
	{
		if (typeof(TCaseSensitivity) == typeof(StringSearchValuesHelper.CaseSensitive))
		{
			Vector512<byte> vector = Vector512.Equals(ch1, LoadPacked512(ref searchSpace, 0u));
			Vector512<byte> vector2 = Vector512.Equals(ch2, LoadPacked512(ref searchSpace, ch2ByteOffset));
			Vector512<byte> vector3 = Vector512.Equals(ch3, LoadPacked512(ref searchSpace, ch3ByteOffset));
			return (vector & vector2 & vector3).AsByte();
		}
		Vector512<byte> vector4 = Vector512.Create((byte)223);
		Vector512<byte> vector5 = Vector512.Equals(ch1, LoadPacked512(ref searchSpace, 0u) & vector4);
		Vector512<byte> vector6 = Vector512.Equals(ch2, LoadPacked512(ref searchSpace, ch2ByteOffset) & vector4);
		Vector512<byte> vector7 = Vector512.Equals(ch3, LoadPacked512(ref searchSpace, ch3ByteOffset) & vector4);
		return (vector5 & vector6 & vector7).AsByte();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool TryMatch(ref char searchSpaceStart, int searchSpaceLength, ref char searchSpace, uint mask, out int offsetFromStart)
	{
		do
		{
			int elementOffset = BitOperations.TrailingZeroCount(mask);
			ref char reference = ref Unsafe.Add(ref searchSpace, elementOffset);
			if (CanSkipAnchorMatchVerification || TCaseSensitivity.Equals<TValueLength>(ref reference, in _valueState))
			{
				offsetFromStart = (int)((nuint)Unsafe.ByteOffset(in searchSpaceStart, in reference) / (nuint)2u);
				return true;
			}
			mask = BitOperations.ResetLowestSetBit(mask);
		}
		while (mask != 0);
		offsetFromStart = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool TryMatch(ref char searchSpaceStart, int searchSpaceLength, ref char searchSpace, ulong mask, out int offsetFromStart)
	{
		do
		{
			int elementOffset = BitOperations.TrailingZeroCount(mask);
			ref char reference = ref Unsafe.Add(ref searchSpace, elementOffset);
			if (CanSkipAnchorMatchVerification || TCaseSensitivity.Equals<TValueLength>(ref reference, in _valueState))
			{
				offsetFromStart = (int)((nuint)Unsafe.ByteOffset(in searchSpaceStart, in reference) / (nuint)2u);
				return true;
			}
			mask = BitOperations.ResetLowestSetBit(mask);
		}
		while (mask != 0L);
		offsetFromStart = 0;
		return false;
	}

	internal override bool ContainsCore(string value)
	{
		if (!base.HasUniqueValues)
		{
			return _valueState.Value.Equals(value, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		return base.ContainsCore(value);
	}

	internal override string[] GetValues()
	{
		if (!base.HasUniqueValues)
		{
			return new string[1] { _valueState.Value };
		}
		return base.GetValues();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Sse2))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	private static Vector128<byte> LoadPacked128(ref char searchSpace, nuint byteOffset)
	{
		Vector128<ushort> vector = Vector128.LoadUnsafe(ref Unsafe.AddByteOffset(ref searchSpace, byteOffset));
		Vector128<ushort> vector2 = Vector128.LoadUnsafe(ref Unsafe.AddByteOffset(ref searchSpace, byteOffset + (uint)Vector128<byte>.Count));
		if (!Sse2.IsSupported)
		{
			return AdvSimd.Arm64.UnzipEven(vector.AsByte(), vector2.AsByte());
		}
		return Sse2.PackUnsignedSaturate(vector.AsInt16(), vector2.AsInt16());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static Vector256<byte> LoadPacked256(ref char searchSpace, nuint byteOffset)
	{
		return Avx2.PackUnsignedSaturate(Vector256.LoadUnsafe(ref Unsafe.AddByteOffset(ref searchSpace, byteOffset)).AsInt16(), Vector256.LoadUnsafe(ref Unsafe.AddByteOffset(ref searchSpace, byteOffset + (uint)Vector256<byte>.Count)).AsInt16());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512BW))]
	private static Vector512<byte> LoadPacked512(ref char searchSpace, nuint byteOffset)
	{
		return Avx512BW.PackUnsignedSaturate(Vector512.LoadUnsafe(ref Unsafe.AddByteOffset(ref searchSpace, byteOffset)).AsInt16(), Vector512.LoadUnsafe(ref Unsafe.AddByteOffset(ref searchSpace, byteOffset + (uint)Vector512<byte>.Count)).AsInt16());
	}
}
