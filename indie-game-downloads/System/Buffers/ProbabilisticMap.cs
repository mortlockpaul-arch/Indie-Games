using System.Numerics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.Wasm;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal readonly struct ProbabilisticMap
{
	private readonly uint _e0 = 0u;

	private readonly uint _e1 = 0u;

	private readonly uint _e2 = 0u;

	private readonly uint _e3 = 0u;

	private readonly uint _e4 = 0u;

	private readonly uint _e5 = 0u;

	private readonly uint _e6 = 0u;

	private readonly uint _e7 = 0u;

	public ProbabilisticMap(ReadOnlySpan<char> values)
	{
		bool flag = false;
		ref readonly uint e = ref _e0;
		for (int i = 0; i < values.Length; i++)
		{
			int num = values[i];
			SetCharBit(ref e, (byte)num);
			num >>= 8;
			if (num == 0)
			{
				flag = true;
			}
			else
			{
				SetCharBit(ref e, (byte)num);
			}
		}
		if (flag)
		{
			SetCharBit(ref e, 0);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[BypassReadyToRun]
	private static void SetCharBit(ref uint charMap, byte value)
	{
		if (Sse41.IsSupported ? true : false)
		{
			Unsafe.Add(ref Unsafe.As<uint, byte>(ref charMap), (uint)(value & 0x1F)) |= (byte)(1 << (value >> 5));
		}
		else
		{
			Unsafe.Add(ref charMap, (uint)(value & 7)) |= (uint)(1 << (value >> 3));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[BypassReadyToRun]
	private static bool IsCharBitSet(ref uint charMap, byte value)
	{
		if (!Sse41.IsSupported)
		{
			_ = 0;
			return (Unsafe.Add(ref charMap, (uint)(value & 7)) & (uint)(1 << (value >> 3))) != 0;
		}
		return (Unsafe.Add(ref Unsafe.As<uint, byte>(ref charMap), (uint)(value & 0x1F)) & (1 << (value >> 5))) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool Contains(ref uint charMap, ReadOnlySpan<char> values, int ch)
	{
		if (IsCharBitSet(ref charMap, (byte)ch) && IsCharBitSet(ref charMap, (byte)(ch >> 8)))
		{
			return Contains(values, (char)ch);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool Contains(ReadOnlySpan<char> values, char ch)
	{
		return SpanHelpers.NonPackedContainsValueType(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(values)), (short)ch, values.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512Vbmi))]
	private static Vector512<byte> ContainsMask64CharsAvx512(Vector512<byte> charMap, ref char searchSpace0, ref char searchSpace1)
	{
		Vector512<ushort> vector = Vector512.LoadUnsafe(ref searchSpace0);
		Vector512<ushort> vector2 = Vector512.LoadUnsafe(ref searchSpace1);
		Vector512<byte> values = Avx512Vbmi.PermuteVar64x8x2(vector.AsByte(), Vector512.CreateSequence((byte)0, (byte)2), vector2.AsByte());
		Vector512<byte> values2 = Avx512Vbmi.PermuteVar64x8x2(vector.AsByte(), Vector512.CreateSequence((byte)1, (byte)2), vector2.AsByte());
		Vector512<byte> vector3 = IsCharBitNotSetAvx512(charMap, values);
		Vector512<byte> vector4 = IsCharBitNotSetAvx512(charMap, values2);
		return ~(vector3 | vector4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512Vbmi))]
	private static Vector512<byte> IsCharBitNotSetAvx512(Vector512<byte> charMap, Vector512<byte> values)
	{
		Vector512<byte> control = (values.AsInt32() >>> 5).AsByte();
		Vector512<byte> vector = Avx512Vbmi.PermuteVar64x8(Vector512.Create(9241421688590303745uL).AsByte(), control);
		return Vector512.Equals(Avx512Vbmi.PermuteVar64x8(charMap, values) & vector, Vector512<byte>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512Vbmi.VL))]
	private static Vector256<byte> ContainsMask32CharsAvx512(Vector256<byte> charMap, ref char searchSpace0, ref char searchSpace1)
	{
		Vector256<ushort> vector = Vector256.LoadUnsafe(ref searchSpace0);
		Vector256<ushort> vector2 = Vector256.LoadUnsafe(ref searchSpace1);
		Vector256<byte> values = Avx512Vbmi.VL.PermuteVar32x8x2(vector.AsByte(), Vector256.CreateSequence((byte)0, (byte)2), vector2.AsByte());
		Vector256<byte> values2 = Avx512Vbmi.VL.PermuteVar32x8x2(vector.AsByte(), Vector256.CreateSequence((byte)1, (byte)2), vector2.AsByte());
		Vector256<byte> vector3 = IsCharBitNotSetAvx512(charMap, values);
		Vector256<byte> vector4 = IsCharBitNotSetAvx512(charMap, values2);
		return ~(vector3 | vector4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512Vbmi.VL))]
	private static Vector256<byte> IsCharBitNotSetAvx512(Vector256<byte> charMap, Vector256<byte> values)
	{
		Vector256<byte> control = (values.AsInt32() >>> 5).AsByte();
		Vector256<byte> vector = Avx512Vbmi.VL.PermuteVar32x8(Vector256.Create(9241421688590303745uL).AsByte(), control);
		return Vector256.Equals(Avx512Vbmi.VL.PermuteVar32x8(charMap, values) & vector, Vector256<byte>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static Vector256<byte> ContainsMask32CharsAvx2(Vector256<byte> charMapLower, Vector256<byte> charMapUpper, ref char searchSpace)
	{
		Vector256<ushort> vector = Vector256.LoadUnsafe(ref searchSpace);
		Vector256<ushort> vector2 = Vector256.LoadUnsafe(ref searchSpace, (nuint)Vector256<ushort>.Count);
		Vector256<byte> values = Avx2.PackUnsignedSaturate((vector & Vector256.Create((ushort)255)).AsInt16(), (vector2 & Vector256.Create((ushort)255)).AsInt16());
		Vector256<byte> values2 = Avx2.PackUnsignedSaturate((vector >>> 8).AsInt16(), (vector2 >>> 8).AsInt16());
		Vector256<byte> vector3 = IsCharBitNotSetAvx2(charMapLower, charMapUpper, values);
		Vector256<byte> vector4 = IsCharBitNotSetAvx2(charMapLower, charMapUpper, values2);
		return ~(vector3 | vector4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static Vector256<byte> IsCharBitNotSetAvx2(Vector256<byte> charMapLower, Vector256<byte> charMapUpper, Vector256<byte> values)
	{
		Vector256<byte> mask = values >>> 5;
		Vector256<byte> vector = Avx2.Shuffle(Vector256.Create(9241421688590303745uL).AsByte(), mask);
		Vector256<byte> vector2 = values & Vector256.Create((byte)31);
		Vector256<byte> right = Avx2.Shuffle(charMapLower, vector2);
		Vector256<byte> left = Avx2.Shuffle(charMapUpper, vector2 - Vector256.Create((byte)16));
		return Vector256.Equals(Vector256.ConditionalSelect(Vector256.GreaterThan(vector2, Vector256.Create((byte)15)), left, right) & vector, Vector256<byte>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	[CompExactlyDependsOn(typeof(Sse2))]
	private static Vector128<byte> ContainsMask16Chars(Vector128<byte> charMapLower, Vector128<byte> charMapUpper, ref char searchSpace)
	{
		Vector128<ushort> vector = Vector128.LoadUnsafe(ref searchSpace);
		Vector128<ushort> vector2 = Vector128.LoadUnsafe(ref searchSpace, (nuint)Vector128<ushort>.Count);
		Vector128<byte> values;
		Vector128<byte> values2;
		if (Sse2.IsSupported)
		{
			values = Sse2.PackUnsignedSaturate((vector & Vector128.Create((ushort)255)).AsInt16(), (vector2 & Vector128.Create((ushort)255)).AsInt16());
			values2 = Sse2.PackUnsignedSaturate((vector >>> 8).AsInt16(), (vector2 >>> 8).AsInt16());
		}
		else
		{
			if (false)
			{
			}
			ThrowHelper.ThrowUnreachableException();
			values = default(Vector128<byte>);
			values2 = default(Vector128<byte>);
		}
		Vector128<byte> vector3 = IsCharBitNotSet(charMapLower, charMapUpper, values);
		Vector128<byte> vector4 = IsCharBitNotSet(charMapLower, charMapUpper, values2);
		return ~(vector3 | vector4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Sse2))]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	private static Vector128<byte> IsCharBitNotSet(Vector128<byte> charMapLower, Vector128<byte> charMapUpper, Vector128<byte> values)
	{
		Vector128<byte> indices = values >>> 5;
		Vector128<byte> vector = Vector128.ShuffleNative(Vector128.Create(9241421688590303745uL).AsByte(), indices);
		Vector128<byte> vector2 = values & Vector128.Create((byte)31);
		if (false)
		{
		}
		Vector128<byte> right = Vector128.ShuffleNative(charMapLower, vector2);
		Vector128<byte> left = Vector128.ShuffleNative(charMapUpper, vector2 - Vector128.Create((byte)16));
		Vector128<byte> vector3 = Vector128.ConditionalSelect(Vector128.GreaterThan(vector2, Vector128.Create((byte)15)), left, right);
		return Vector128.Equals(vector3 & vector, Vector128<byte>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool ShouldUseSimpleLoop(int searchSpaceLength, int valuesLength)
	{
		if (searchSpaceLength >= Vector128<short>.Count)
		{
			if (searchSpaceLength < 20)
			{
				return searchSpaceLength < valuesLength >> 1;
			}
			return false;
		}
		return true;
	}

	public static int IndexOfAny(ref char searchSpace, int searchSpaceLength, ref char values, int valuesLength)
	{
		ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(ref values, valuesLength);
		if (ShouldUseSimpleLoop(searchSpaceLength, valuesLength))
		{
			return IndexOfAnySimpleLoop<IndexOfAnyAsciiSearcher.DontNegate>(ref searchSpace, searchSpaceLength, readOnlySpan);
		}
		if (IndexOfAnyAsciiSearcher.TryIndexOfAny(ref searchSpace, searchSpaceLength, readOnlySpan, out var index))
		{
			return index;
		}
		return ProbabilisticIndexOfAny(ref searchSpace, searchSpaceLength, ref values, valuesLength);
	}

	public static int IndexOfAnyExcept(ref char searchSpace, int searchSpaceLength, ref char values, int valuesLength)
	{
		ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(ref values, valuesLength);
		if (IndexOfAnyAsciiSearcher.IsVectorizationSupported && !ShouldUseSimpleLoop(searchSpaceLength, valuesLength) && IndexOfAnyAsciiSearcher.TryIndexOfAnyExcept(ref searchSpace, searchSpaceLength, readOnlySpan, out var index))
		{
			return index;
		}
		return IndexOfAnySimpleLoop<IndexOfAnyAsciiSearcher.Negate>(ref searchSpace, searchSpaceLength, readOnlySpan);
	}

	public static int LastIndexOfAny(ref char searchSpace, int searchSpaceLength, ref char values, int valuesLength)
	{
		ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(ref values, valuesLength);
		if (ShouldUseSimpleLoop(searchSpaceLength, valuesLength))
		{
			return LastIndexOfAnySimpleLoop<IndexOfAnyAsciiSearcher.DontNegate>(ref searchSpace, searchSpaceLength, readOnlySpan);
		}
		if (IndexOfAnyAsciiSearcher.TryLastIndexOfAny(ref searchSpace, searchSpaceLength, readOnlySpan, out var index))
		{
			return index;
		}
		return ProbabilisticLastIndexOfAny(ref searchSpace, searchSpaceLength, ref values, valuesLength);
	}

	public static int LastIndexOfAnyExcept(ref char searchSpace, int searchSpaceLength, ref char values, int valuesLength)
	{
		ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(ref values, valuesLength);
		if (IndexOfAnyAsciiSearcher.IsVectorizationSupported && !ShouldUseSimpleLoop(searchSpaceLength, valuesLength) && IndexOfAnyAsciiSearcher.TryLastIndexOfAnyExcept(ref searchSpace, searchSpaceLength, readOnlySpan, out var index))
		{
			return index;
		}
		return LastIndexOfAnySimpleLoop<IndexOfAnyAsciiSearcher.Negate>(ref searchSpace, searchSpaceLength, readOnlySpan);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe static int ProbabilisticIndexOfAny(ref char searchSpace, int searchSpaceLength, ref char values, int valuesLength)
	{
		ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(ref values, valuesLength);
		ProbabilisticMapState state = new ProbabilisticMapState(&readOnlySpan);
		return IndexOfAny<SearchValues.FalseConst>(ref searchSpace, searchSpaceLength, ref state);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe static int ProbabilisticLastIndexOfAny(ref char searchSpace, int searchSpaceLength, ref char values, int valuesLength)
	{
		ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(ref values, valuesLength);
		ProbabilisticMapState state = new ProbabilisticMapState(&readOnlySpan);
		return LastIndexOfAny<SearchValues.FalseConst>(ref searchSpace, searchSpaceLength, ref state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int IndexOfAny<TUseFastContains>(ref char searchSpace, int searchSpaceLength, ref ProbabilisticMapState state) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		if ((Sse41.IsSupported ? true : false) && searchSpaceLength >= 16)
		{
			if (!Vector512.IsHardwareAccelerated || !Avx512Vbmi.VL.IsSupported)
			{
				return IndexOfAnyVectorized<TUseFastContains>(ref searchSpace, searchSpaceLength, ref state);
			}
			return IndexOfAnyVectorizedAvx512<TUseFastContains>(ref searchSpace, searchSpaceLength, ref state);
		}
		return ProbabilisticMapState.IndexOfAnySimpleLoop<TUseFastContains, IndexOfAnyAsciiSearcher.DontNegate>(ref searchSpace, searchSpaceLength, ref state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int LastIndexOfAny<TUseFastContains>(ref char searchSpace, int searchSpaceLength, ref ProbabilisticMapState state) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		if ((Sse41.IsSupported ? true : false) && searchSpaceLength >= 16)
		{
			if (!Vector512.IsHardwareAccelerated || !Avx512Vbmi.VL.IsSupported)
			{
				return LastIndexOfAnyVectorized<TUseFastContains>(ref searchSpace, searchSpaceLength, ref state);
			}
			return LastIndexOfAnyVectorizedAvx512<TUseFastContains>(ref searchSpace, searchSpaceLength, ref state);
		}
		return ProbabilisticMapState.LastIndexOfAnySimpleLoop<TUseFastContains, IndexOfAnyAsciiSearcher.DontNegate>(ref searchSpace, searchSpaceLength, ref state);
	}

	[CompExactlyDependsOn(typeof(Avx512Vbmi.VL))]
	private static int IndexOfAnyVectorizedAvx512<TUseFastContains>(ref char searchSpace, int searchSpaceLength, ref ProbabilisticMapState state) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		ref char reference = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
		Vector256<byte> vector = Vector256.LoadUnsafe(in Unsafe.As<ProbabilisticMap, byte>(ref state.Map));
		if (searchSpaceLength > 32)
		{
			Vector512<byte> charMap = Vector512.Create(vector);
			if (searchSpaceLength > 64)
			{
				ref char reference2 = ref searchSpace;
				ref char reference3 = ref Unsafe.Subtract(ref reference, 64);
				while (true)
				{
					Vector512<byte> vector2 = ContainsMask64CharsAvx512(charMap, ref reference2, ref Unsafe.Add(ref reference2, Vector512<ushort>.Count));
					if (vector2 != Vector512<byte>.Zero && TryFindMatchAvx512<TUseFastContains>(ref reference2, vector2.ExtractMostSignificantBits(), ref state, out var index))
					{
						return MatchOffset(ref searchSpace, ref reference2) + index;
					}
					reference2 = ref Unsafe.Add(ref reference2, 64);
					if (Unsafe.IsAddressGreaterThan(in reference2, in reference3))
					{
						if (Unsafe.AreSame(in reference2, in reference))
						{
							break;
						}
						reference2 = ref reference3;
					}
				}
			}
			else
			{
				Vector512<byte> vector3 = ContainsMask64CharsAvx512(charMap, ref searchSpace, ref Unsafe.Subtract(ref reference, Vector512<ushort>.Count));
				if (vector3 != Vector512<byte>.Zero && TryFindMatchOverlappedAvx512<TUseFastContains>(ref searchSpace, searchSpaceLength, vector3.ExtractMostSignificantBits(), ref state, out var index2))
				{
					return index2;
				}
			}
		}
		else
		{
			Vector256<byte> vector4 = ContainsMask32CharsAvx512(vector, ref searchSpace, ref Unsafe.Subtract(ref reference, Vector256<ushort>.Count));
			if (vector4 != Vector256<byte>.Zero && TryFindMatchOverlappedAvx512<TUseFastContains>(ref searchSpace, searchSpaceLength, vector4.ExtractMostSignificantBits(), ref state, out var index3))
			{
				return index3;
			}
		}
		return -1;
	}

	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	[CompExactlyDependsOn(typeof(Sse41))]
	private static int IndexOfAnyVectorized<TUseFastContains>(ref char searchSpace, int searchSpaceLength, ref ProbabilisticMapState state) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		ref char reference = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
		ref char reference2 = ref searchSpace;
		Vector128<byte> vector = Vector128.LoadUnsafe(in Unsafe.As<ProbabilisticMap, byte>(ref state.Map));
		Vector128<byte> vector2 = Vector128.LoadUnsafe(in Unsafe.As<ProbabilisticMap, byte>(ref state.Map), (nuint)Vector128<byte>.Count);
		if (Avx2.IsSupported && searchSpaceLength >= 32)
		{
			Vector256<byte> charMapLower = Vector256.Create(vector);
			Vector256<byte> charMapUpper = Vector256.Create(vector2);
			ref char reference3 = ref Unsafe.Subtract(ref reference, 32);
			while (true)
			{
				Vector256<byte> vector3 = ContainsMask32CharsAvx2(charMapLower, charMapUpper, ref reference2);
				if (vector3 != Vector256<byte>.Zero && TryFindMatch<TUseFastContains>(ref reference2, PackedSpanHelpers.FixUpPackedVector256Result(vector3).ExtractMostSignificantBits(), ref state, out var index))
				{
					return MatchOffset(ref searchSpace, ref reference2) + index;
				}
				reference2 = ref Unsafe.Add(ref reference2, 32);
				if (Unsafe.IsAddressGreaterThan(in reference2, in reference3))
				{
					if (Unsafe.AreSame(in reference2, in reference))
					{
						return -1;
					}
					if (Unsafe.ByteOffset(in reference2, in reference) <= 32)
					{
						break;
					}
					reference2 = ref reference3;
				}
			}
			reference2 = ref Unsafe.Subtract(ref reference, 16);
		}
		ref char reference4 = ref Unsafe.Subtract(ref reference, 16);
		while (true)
		{
			Vector128<byte> vector4 = ContainsMask16Chars(vector, vector2, ref reference2);
			if (vector4 != Vector128<byte>.Zero && TryFindMatch<TUseFastContains>(ref reference2, vector4.ExtractMostSignificantBits(), ref state, out var index2))
			{
				return MatchOffset(ref searchSpace, ref reference2) + index2;
			}
			reference2 = ref Unsafe.Add(ref reference2, 16);
			if (Unsafe.IsAddressGreaterThan(in reference2, in reference4))
			{
				if (Unsafe.AreSame(in reference2, in reference))
				{
					break;
				}
				reference2 = ref reference4;
			}
		}
		return -1;
	}

	[CompExactlyDependsOn(typeof(Avx512Vbmi.VL))]
	private static int LastIndexOfAnyVectorizedAvx512<TUseFastContains>(ref char searchSpace, int searchSpaceLength, ref ProbabilisticMapState state) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		ref char reference = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
		Vector256<byte> vector = Vector256.LoadUnsafe(in Unsafe.As<ProbabilisticMap, byte>(ref state.Map));
		if (searchSpaceLength > 32)
		{
			Vector512<byte> charMap = Vector512.Create(vector);
			if (searchSpaceLength > 64)
			{
				ref char reference2 = ref Unsafe.Add(ref searchSpace, 64);
				while (true)
				{
					reference = ref Unsafe.Subtract(ref reference, 64);
					Vector512<byte> vector2 = ContainsMask64CharsAvx512(charMap, ref reference, ref Unsafe.Add(ref reference, Vector512<ushort>.Count));
					if (vector2 != Vector512<byte>.Zero && TryFindLastMatchAvx512<TUseFastContains>(ref reference, vector2.ExtractMostSignificantBits(), ref state, out var index))
					{
						return MatchOffset(ref searchSpace, ref reference) + index;
					}
					if (Unsafe.IsAddressLessThanOrEqualTo(in reference, in reference2))
					{
						if (Unsafe.AreSame(in reference, in searchSpace))
						{
							break;
						}
						reference = ref reference2;
					}
				}
			}
			else
			{
				Vector512<byte> vector3 = ContainsMask64CharsAvx512(charMap, ref searchSpace, ref Unsafe.Subtract(ref reference, Vector512<ushort>.Count));
				if (vector3 != Vector512<byte>.Zero && TryFindLastMatchOverlappedAvx512<TUseFastContains>(ref searchSpace, searchSpaceLength, vector3.ExtractMostSignificantBits(), ref state, out var index2))
				{
					return index2;
				}
			}
		}
		else
		{
			Vector256<byte> vector4 = ContainsMask32CharsAvx512(vector, ref searchSpace, ref Unsafe.Subtract(ref reference, Vector256<ushort>.Count));
			if (vector4 != Vector256<byte>.Zero && TryFindLastMatchOverlappedAvx512<TUseFastContains>(ref searchSpace, searchSpaceLength, vector4.ExtractMostSignificantBits(), ref state, out var index3))
			{
				return index3;
			}
		}
		return -1;
	}

	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	[CompExactlyDependsOn(typeof(Sse41))]
	private static int LastIndexOfAnyVectorized<TUseFastContains>(ref char searchSpace, int searchSpaceLength, ref ProbabilisticMapState state) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		ref char reference = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
		Vector128<byte> vector = Vector128.LoadUnsafe(in Unsafe.As<ProbabilisticMap, byte>(ref state.Map));
		Vector128<byte> vector2 = Vector128.LoadUnsafe(in Unsafe.As<ProbabilisticMap, byte>(ref state.Map), (nuint)Vector128<byte>.Count);
		if (Avx2.IsSupported && searchSpaceLength >= 32)
		{
			Vector256<byte> charMapLower = Vector256.Create(vector);
			Vector256<byte> charMapUpper = Vector256.Create(vector2);
			ref char reference2 = ref Unsafe.Add(ref searchSpace, 32);
			while (true)
			{
				reference = ref Unsafe.Subtract(ref reference, 32);
				Vector256<byte> vector3 = ContainsMask32CharsAvx2(charMapLower, charMapUpper, ref reference);
				if (vector3 != Vector256<byte>.Zero && TryFindLastMatch<TUseFastContains>(ref reference, PackedSpanHelpers.FixUpPackedVector256Result(vector3).ExtractMostSignificantBits(), ref state, out var index))
				{
					return MatchOffset(ref searchSpace, ref reference) + index;
				}
				if (Unsafe.IsAddressLessThanOrEqualTo(in reference, in reference2))
				{
					if (Unsafe.AreSame(in reference, in searchSpace))
					{
						return -1;
					}
					if (Unsafe.ByteOffset(in searchSpace, in reference) <= 32)
					{
						break;
					}
					reference = ref reference2;
				}
			}
			reference = ref Unsafe.Add(ref searchSpace, 16);
		}
		ref char reference3 = ref Unsafe.Add(ref searchSpace, 16);
		while (true)
		{
			reference = ref Unsafe.Subtract(ref reference, 16);
			Vector128<byte> vector4 = ContainsMask16Chars(vector, vector2, ref reference);
			if (vector4 != Vector128<byte>.Zero && TryFindLastMatch<TUseFastContains>(ref reference, vector4.ExtractMostSignificantBits(), ref state, out var index2))
			{
				return MatchOffset(ref searchSpace, ref reference) + index2;
			}
			if (Unsafe.IsAddressLessThanOrEqualTo(in reference, in reference3))
			{
				if (Unsafe.AreSame(in reference, in searchSpace))
				{
					break;
				}
				reference = ref reference3;
			}
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int MatchOffset(ref char searchSpace, ref char cur)
	{
		return (int)((nuint)Unsafe.ByteOffset(in searchSpace, in cur) / (nuint)2u);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryFindMatch<TUseFastContains>(ref char cur, uint mask, ref ProbabilisticMapState state, out int index) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		do
		{
			index = BitOperations.TrailingZeroCount(mask);
			if (state.ConfirmProbabilisticMatch<TUseFastContains>(Unsafe.Add(ref cur, index)))
			{
				return true;
			}
			mask = BitOperations.ResetLowestSetBit(mask);
		}
		while (mask != 0);
		index = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryFindMatchOverlappedAvx512<TUseFastContains>(ref char cur, int searchSpaceLength, uint mask, ref ProbabilisticMapState state, out int index) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		do
		{
			index = BitOperations.TrailingZeroCount(mask);
			if (index >= Vector256<ushort>.Count)
			{
				index += searchSpaceLength - 2 * Vector256<ushort>.Count;
			}
			if (state.ConfirmProbabilisticMatch<TUseFastContains>(Unsafe.Add(ref cur, index)))
			{
				return true;
			}
			mask = BitOperations.ResetLowestSetBit(mask);
		}
		while (mask != 0);
		index = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryFindMatchAvx512<TUseFastContains>(ref char cur, ulong mask, ref ProbabilisticMapState state, out int index) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		do
		{
			index = BitOperations.TrailingZeroCount(mask);
			if (state.ConfirmProbabilisticMatch<TUseFastContains>(Unsafe.Add(ref cur, index)))
			{
				return true;
			}
			mask = BitOperations.ResetLowestSetBit(mask);
		}
		while (mask != 0L);
		index = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryFindMatchOverlappedAvx512<TUseFastContains>(ref char cur, int searchSpaceLength, ulong mask, ref ProbabilisticMapState state, out int index) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		do
		{
			index = BitOperations.TrailingZeroCount(mask);
			if (index >= Vector512<ushort>.Count)
			{
				index += searchSpaceLength - 2 * Vector512<ushort>.Count;
			}
			if (state.ConfirmProbabilisticMatch<TUseFastContains>(Unsafe.Add(ref cur, index)))
			{
				return true;
			}
			mask = BitOperations.ResetLowestSetBit(mask);
		}
		while (mask != 0L);
		index = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryFindLastMatch<TUseFastContains>(ref char cur, uint mask, ref ProbabilisticMapState state, out int index) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		do
		{
			index = 31 - BitOperations.LeadingZeroCount(mask);
			if (state.ConfirmProbabilisticMatch<TUseFastContains>(Unsafe.Add(ref cur, index)))
			{
				return true;
			}
			mask = BitOperations.FlipBit(mask, index);
		}
		while (mask != 0);
		index = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryFindLastMatchOverlappedAvx512<TUseFastContains>(ref char cur, int searchSpaceLength, uint mask, ref ProbabilisticMapState state, out int index) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		do
		{
			index = 31 - BitOperations.LeadingZeroCount(mask);
			mask = BitOperations.FlipBit(mask, index);
			if (index >= Vector256<ushort>.Count)
			{
				index += searchSpaceLength - 2 * Vector256<ushort>.Count;
			}
			if (state.ConfirmProbabilisticMatch<TUseFastContains>(Unsafe.Add(ref cur, index)))
			{
				return true;
			}
		}
		while (mask != 0);
		index = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryFindLastMatchAvx512<TUseFastContains>(ref char cur, ulong mask, ref ProbabilisticMapState state, out int index) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		do
		{
			index = 63 - BitOperations.LeadingZeroCount(mask);
			if (state.ConfirmProbabilisticMatch<TUseFastContains>(Unsafe.Add(ref cur, index)))
			{
				return true;
			}
			mask = BitOperations.FlipBit(mask, index);
		}
		while (mask != 0L);
		index = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryFindLastMatchOverlappedAvx512<TUseFastContains>(ref char cur, int searchSpaceLength, ulong mask, ref ProbabilisticMapState state, out int index) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		do
		{
			index = 63 - BitOperations.LeadingZeroCount(mask);
			mask = BitOperations.FlipBit(mask, index);
			if (index >= Vector512<ushort>.Count)
			{
				index += searchSpaceLength - 2 * Vector512<ushort>.Count;
			}
			if (state.ConfirmProbabilisticMatch<TUseFastContains>(Unsafe.Add(ref cur, index)))
			{
				return true;
			}
		}
		while (mask != 0L);
		index = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int IndexOfAnySimpleLoop<TNegator>(ref char searchSpace, int searchSpaceLength, ReadOnlySpan<char> values) where TNegator : struct, IndexOfAnyAsciiSearcher.INegator
	{
		ref char right = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
		ref char reference = ref searchSpace;
		while (!Unsafe.AreSame(in reference, in right))
		{
			char ch = reference;
			if (TNegator.NegateIfNeeded(Contains(values, ch)))
			{
				return MatchOffset(ref searchSpace, ref reference);
			}
			reference = ref Unsafe.Add(ref reference, 1);
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int LastIndexOfAnySimpleLoop<TNegator>(ref char searchSpace, int searchSpaceLength, ReadOnlySpan<char> values) where TNegator : struct, IndexOfAnyAsciiSearcher.INegator
	{
		for (int num = searchSpaceLength - 1; num >= 0; num--)
		{
			char ch = Unsafe.Add(ref searchSpace, num);
			if (TNegator.NegateIfNeeded(Contains(values, ch)))
			{
				return num;
			}
		}
		return -1;
	}
}
