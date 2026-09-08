using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal abstract class AsciiStringSearchValuesTeddyBase<TBucketized, TStartCaseSensitivity, TCaseSensitivity> : StringSearchValuesRabinKarp<TCaseSensitivity> where TBucketized : struct, SearchValues.IRuntimeConst where TStartCaseSensitivity : struct, StringSearchValuesHelper.ICaseSensitivity where TCaseSensitivity : struct, StringSearchValuesHelper.ICaseSensitivity
{
	private readonly EightObjects _buckets;

	private readonly Vector512<byte> _n0Low;

	private readonly Vector512<byte> _n0High;

	private readonly Vector512<byte> _n1Low;

	private readonly Vector512<byte> _n1High;

	private readonly Vector512<byte> _n2Low;

	private readonly Vector512<byte> _n2High;

	protected AsciiStringSearchValuesTeddyBase(ReadOnlySpan<string> values, HashSet<string> uniqueValues, int n)
		: base(values, uniqueValues)
	{
		ReadOnlySpan<object>.CastUp(values).CopyTo(_buckets);
		(_n0Low, _n0High) = TeddyBucketizer.GenerateNonBucketizedFingerprint(values, 0);
		(_n1Low, _n1High) = TeddyBucketizer.GenerateNonBucketizedFingerprint(values, 1);
		if (n == 3)
		{
			(_n2Low, _n2High) = TeddyBucketizer.GenerateNonBucketizedFingerprint(values, 2);
		}
	}

	protected AsciiStringSearchValuesTeddyBase(string[][] buckets, ReadOnlySpan<string> values, HashSet<string> uniqueValues, int n)
		: base(values, uniqueValues)
	{
		((ReadOnlySpan<object>)(object[]?)buckets).CopyTo(_buckets);
		(_n0Low, _n0High) = TeddyBucketizer.GenerateBucketizedFingerprint(buckets, 0);
		(_n1Low, _n1High) = TeddyBucketizer.GenerateBucketizedFingerprint(buckets, 1);
		if (n == 3)
		{
			(_n2Low, _n2High) = TeddyBucketizer.GenerateBucketizedFingerprint(buckets, 2);
		}
	}

	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	protected int IndexOfAnyN2(ReadOnlySpan<char> span)
	{
		if (Vector512.IsHardwareAccelerated && Avx512Vbmi.IsSupported && span.Length >= 65)
		{
			return IndexOfAnyN2Avx512(span);
		}
		if (Avx2.IsSupported && span.Length >= 33)
		{
			return IndexOfAnyN2Avx2(span);
		}
		return IndexOfAnyN2Vector128(span);
	}

	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	protected int IndexOfAnyN3(ReadOnlySpan<char> span)
	{
		if (Vector512.IsHardwareAccelerated && Avx512Vbmi.IsSupported && span.Length >= 66)
		{
			return IndexOfAnyN3Avx512(span);
		}
		if (Avx2.IsSupported && span.Length >= 34)
		{
			return IndexOfAnyN3Avx2(span);
		}
		return IndexOfAnyN3Vector128(span);
	}

	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	private int IndexOfAnyN2Vector128(ReadOnlySpan<char> span)
	{
		if (span.Length < 17)
		{
			return ShortInputFallback(span);
		}
		ref char reference = ref MemoryMarshal.GetReference(span);
		ref char reference2 = ref Unsafe.Add(ref reference, span.Length - 16);
		reference = ref Unsafe.Add(ref reference, 1);
		Vector128<byte> lower = _n0Low._lower._lower;
		Vector128<byte> lower2 = _n0High._lower._lower;
		Vector128<byte> lower3 = _n1Low._lower._lower;
		Vector128<byte> lower4 = _n1High._lower._lower;
		Vector128<byte> prev = Vector128<byte>.AllBitsSet;
		int offsetFromStart;
		while (true)
		{
			Vector128<byte> vector;
			(vector, prev) = TeddyHelper.ProcessInputN2(TStartCaseSensitivity.TransformInput(TeddyHelper.LoadAndPack16AsciiChars(ref reference)), prev, lower, lower2, lower3, lower4);
			if (vector != Vector128<byte>.Zero && TryFindMatch(span, ref reference, vector, 1, out offsetFromStart))
			{
				break;
			}
			reference = ref Unsafe.Add(ref reference, 16);
			if (Unsafe.IsAddressGreaterThan(in reference, in reference2))
			{
				if (Unsafe.AreSame(in reference, in Unsafe.Add(ref reference2, 16)))
				{
					return -1;
				}
				prev = Vector128<byte>.AllBitsSet;
				reference = ref reference2;
			}
		}
		return offsetFromStart;
	}

	[CompExactlyDependsOn(typeof(Avx2))]
	private int IndexOfAnyN2Avx2(ReadOnlySpan<char> span)
	{
		ref char reference = ref MemoryMarshal.GetReference(span);
		ref char reference2 = ref Unsafe.Add(ref reference, span.Length - 32);
		reference = ref Unsafe.Add(ref reference, 1);
		Vector256<byte> lower = _n0Low._lower;
		Vector256<byte> lower2 = _n0High._lower;
		Vector256<byte> lower3 = _n1Low._lower;
		Vector256<byte> lower4 = _n1High._lower;
		Vector256<byte> prev = Vector256<byte>.AllBitsSet;
		int offsetFromStart;
		while (true)
		{
			Vector256<byte> vector;
			(vector, prev) = TeddyHelper.ProcessInputN2(TStartCaseSensitivity.TransformInput(TeddyHelper.LoadAndPack32AsciiChars(ref reference)), prev, lower, lower2, lower3, lower4);
			if (vector != Vector256<byte>.Zero && TryFindMatch(span, ref reference, vector, 1, out offsetFromStart))
			{
				break;
			}
			reference = ref Unsafe.Add(ref reference, 32);
			if (Unsafe.IsAddressGreaterThan(in reference, in reference2))
			{
				if (Unsafe.AreSame(in reference, in Unsafe.Add(ref reference2, 32)))
				{
					return -1;
				}
				prev = Vector256<byte>.AllBitsSet;
				reference = ref reference2;
			}
		}
		return offsetFromStart;
	}

	[CompExactlyDependsOn(typeof(Avx512Vbmi))]
	private int IndexOfAnyN2Avx512(ReadOnlySpan<char> span)
	{
		ref char reference = ref MemoryMarshal.GetReference(span);
		ref char reference2 = ref Unsafe.Add(ref reference, span.Length - 64);
		reference = ref Unsafe.Add(ref reference, 1);
		Vector512<byte> n0Low = _n0Low;
		Vector512<byte> n0High = _n0High;
		Vector512<byte> n1Low = _n1Low;
		Vector512<byte> n1High = _n1High;
		Vector512<byte> prev = Vector512<byte>.AllBitsSet;
		int offsetFromStart;
		while (true)
		{
			Vector512<byte> vector;
			(vector, prev) = TeddyHelper.ProcessInputN2(TStartCaseSensitivity.TransformInput(TeddyHelper.LoadAndPack64AsciiChars(ref reference)), prev, n0Low, n0High, n1Low, n1High);
			if (vector != Vector512<byte>.Zero && TryFindMatch(span, ref reference, vector, 1, out offsetFromStart))
			{
				break;
			}
			reference = ref Unsafe.Add(ref reference, 64);
			if (Unsafe.IsAddressGreaterThan(in reference, in reference2))
			{
				if (Unsafe.AreSame(in reference, in Unsafe.Add(ref reference2, 64)))
				{
					return -1;
				}
				prev = Vector512<byte>.AllBitsSet;
				reference = ref reference2;
			}
		}
		return offsetFromStart;
	}

	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	private int IndexOfAnyN3Vector128(ReadOnlySpan<char> span)
	{
		if (span.Length < 18)
		{
			return ShortInputFallback(span);
		}
		ref char reference = ref MemoryMarshal.GetReference(span);
		ref char reference2 = ref Unsafe.Add(ref reference, span.Length - 16);
		reference = ref Unsafe.Add(ref reference, 2);
		Vector128<byte> lower = _n0Low._lower._lower;
		Vector128<byte> lower2 = _n0High._lower._lower;
		Vector128<byte> lower3 = _n1Low._lower._lower;
		Vector128<byte> lower4 = _n1High._lower._lower;
		Vector128<byte> lower5 = _n2Low._lower._lower;
		Vector128<byte> lower6 = _n2High._lower._lower;
		Vector128<byte> prev = Vector128<byte>.AllBitsSet;
		Vector128<byte> prev2 = Vector128<byte>.AllBitsSet;
		int offsetFromStart;
		while (true)
		{
			Vector128<byte> vector;
			(vector, prev, prev2) = TeddyHelper.ProcessInputN3(TStartCaseSensitivity.TransformInput(TeddyHelper.LoadAndPack16AsciiChars(ref reference)), prev, prev2, lower, lower2, lower3, lower4, lower5, lower6);
			if (vector != Vector128<byte>.Zero && TryFindMatch(span, ref reference, vector, 2, out offsetFromStart))
			{
				break;
			}
			reference = ref Unsafe.Add(ref reference, 16);
			if (Unsafe.IsAddressGreaterThan(in reference, in reference2))
			{
				if (Unsafe.AreSame(in reference, in Unsafe.Add(ref reference2, 16)))
				{
					return -1;
				}
				prev = Vector128<byte>.AllBitsSet;
				prev2 = Vector128<byte>.AllBitsSet;
				reference = ref reference2;
			}
		}
		return offsetFromStart;
	}

	[CompExactlyDependsOn(typeof(Avx2))]
	private int IndexOfAnyN3Avx2(ReadOnlySpan<char> span)
	{
		ref char reference = ref MemoryMarshal.GetReference(span);
		ref char reference2 = ref Unsafe.Add(ref reference, span.Length - 32);
		reference = ref Unsafe.Add(ref reference, 2);
		Vector256<byte> lower = _n0Low._lower;
		Vector256<byte> lower2 = _n0High._lower;
		Vector256<byte> lower3 = _n1Low._lower;
		Vector256<byte> lower4 = _n1High._lower;
		Vector256<byte> lower5 = _n2Low._lower;
		Vector256<byte> lower6 = _n2High._lower;
		Vector256<byte> prev = Vector256<byte>.AllBitsSet;
		Vector256<byte> prev2 = Vector256<byte>.AllBitsSet;
		int offsetFromStart;
		while (true)
		{
			Vector256<byte> vector;
			(vector, prev, prev2) = TeddyHelper.ProcessInputN3(TStartCaseSensitivity.TransformInput(TeddyHelper.LoadAndPack32AsciiChars(ref reference)), prev, prev2, lower, lower2, lower3, lower4, lower5, lower6);
			if (vector != Vector256<byte>.Zero && TryFindMatch(span, ref reference, vector, 2, out offsetFromStart))
			{
				break;
			}
			reference = ref Unsafe.Add(ref reference, 32);
			if (Unsafe.IsAddressGreaterThan(in reference, in reference2))
			{
				if (Unsafe.AreSame(in reference, in Unsafe.Add(ref reference2, 32)))
				{
					return -1;
				}
				prev = Vector256<byte>.AllBitsSet;
				prev2 = Vector256<byte>.AllBitsSet;
				reference = ref reference2;
			}
		}
		return offsetFromStart;
	}

	[CompExactlyDependsOn(typeof(Avx512Vbmi))]
	private int IndexOfAnyN3Avx512(ReadOnlySpan<char> span)
	{
		ref char reference = ref MemoryMarshal.GetReference(span);
		ref char reference2 = ref Unsafe.Add(ref reference, span.Length - 64);
		reference = ref Unsafe.Add(ref reference, 2);
		Vector512<byte> n0Low = _n0Low;
		Vector512<byte> n0High = _n0High;
		Vector512<byte> n1Low = _n1Low;
		Vector512<byte> n1High = _n1High;
		Vector512<byte> n2Low = _n2Low;
		Vector512<byte> n2High = _n2High;
		Vector512<byte> prev = Vector512<byte>.AllBitsSet;
		Vector512<byte> prev2 = Vector512<byte>.AllBitsSet;
		int offsetFromStart;
		while (true)
		{
			Vector512<byte> vector;
			(vector, prev, prev2) = TeddyHelper.ProcessInputN3(TStartCaseSensitivity.TransformInput(TeddyHelper.LoadAndPack64AsciiChars(ref reference)), prev, prev2, n0Low, n0High, n1Low, n1High, n2Low, n2High);
			if (vector != Vector512<byte>.Zero && TryFindMatch(span, ref reference, vector, 2, out offsetFromStart))
			{
				break;
			}
			reference = ref Unsafe.Add(ref reference, 64);
			if (Unsafe.IsAddressGreaterThan(in reference, in reference2))
			{
				if (Unsafe.AreSame(in reference, in Unsafe.Add(ref reference2, 64)))
				{
					return -1;
				}
				prev = Vector512<byte>.AllBitsSet;
				prev2 = Vector512<byte>.AllBitsSet;
				reference = ref reference2;
			}
		}
		return offsetFromStart;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool TryFindMatch(ReadOnlySpan<char> span, ref char searchSpace, Vector128<byte> result, int matchStartOffset, out int offsetFromStart)
	{
		uint num = (~Vector128.Equals(result, Vector128<byte>.Zero)).ExtractMostSignificantBits();
		do
		{
			int num2 = BitOperations.TrailingZeroCount(num);
			ref char reference = ref Unsafe.Add(ref searchSpace, num2 - matchStartOffset);
			offsetFromStart = (int)((nuint)Unsafe.ByteOffset(in MemoryMarshal.GetReference(span), in reference) / (nuint)2u);
			int lengthRemaining = span.Length - offsetFromStart;
			uint num3 = Vector128.GetElementUnsafe(in result, num2);
			do
			{
				int num4 = BitOperations.TrailingZeroCount(num3);
				object o = _buckets[num4];
				if (TBucketized.Value ? StringSearchValuesHelper.StartsWith<TCaseSensitivity>(ref reference, lengthRemaining, Unsafe.As<string[]>(o)) : StringSearchValuesHelper.StartsWith<TCaseSensitivity>(ref reference, lengthRemaining, Unsafe.As<string>(o)))
				{
					return true;
				}
				num3 = BitOperations.ResetLowestSetBit(num3);
			}
			while (num3 != 0);
			num = BitOperations.ResetLowestSetBit(num);
		}
		while (num != 0);
		offsetFromStart = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool TryFindMatch(ReadOnlySpan<char> span, ref char searchSpace, Vector256<byte> result, int matchStartOffset, out int offsetFromStart)
	{
		uint num = (~Vector256.Equals(result, Vector256<byte>.Zero)).ExtractMostSignificantBits();
		do
		{
			int num2 = BitOperations.TrailingZeroCount(num);
			ref char reference = ref Unsafe.Add(ref searchSpace, num2 - matchStartOffset);
			offsetFromStart = (int)((nuint)Unsafe.ByteOffset(in MemoryMarshal.GetReference(span), in reference) / (nuint)2u);
			int lengthRemaining = span.Length - offsetFromStart;
			uint num3 = Vector256.GetElementUnsafe(in result, num2);
			do
			{
				int num4 = BitOperations.TrailingZeroCount(num3);
				object o = _buckets[num4];
				if (TBucketized.Value ? StringSearchValuesHelper.StartsWith<TCaseSensitivity>(ref reference, lengthRemaining, Unsafe.As<string[]>(o)) : StringSearchValuesHelper.StartsWith<TCaseSensitivity>(ref reference, lengthRemaining, Unsafe.As<string>(o)))
				{
					return true;
				}
				num3 = BitOperations.ResetLowestSetBit(num3);
			}
			while (num3 != 0);
			num = BitOperations.ResetLowestSetBit(num);
		}
		while (num != 0);
		offsetFromStart = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool TryFindMatch(ReadOnlySpan<char> span, ref char searchSpace, Vector512<byte> result, int matchStartOffset, out int offsetFromStart)
	{
		ulong num = (~Vector512.Equals(result, Vector512<byte>.Zero)).ExtractMostSignificantBits();
		do
		{
			int num2 = BitOperations.TrailingZeroCount(num);
			ref char reference = ref Unsafe.Add(ref searchSpace, num2 - matchStartOffset);
			offsetFromStart = (int)((nuint)Unsafe.ByteOffset(in MemoryMarshal.GetReference(span), in reference) / (nuint)2u);
			int lengthRemaining = span.Length - offsetFromStart;
			uint num3 = Vector512.GetElementUnsafe(in result, num2);
			do
			{
				int num4 = BitOperations.TrailingZeroCount(num3);
				object o = _buckets[num4];
				if (TBucketized.Value ? StringSearchValuesHelper.StartsWith<TCaseSensitivity>(ref reference, lengthRemaining, Unsafe.As<string[]>(o)) : StringSearchValuesHelper.StartsWith<TCaseSensitivity>(ref reference, lengthRemaining, Unsafe.As<string>(o)))
				{
					return true;
				}
				num3 = BitOperations.ResetLowestSetBit(num3);
			}
			while (num3 != 0);
			num = BitOperations.ResetLowestSetBit(num);
		}
		while (num != 0L);
		offsetFromStart = 0;
		return false;
	}
}
