using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.Wasm;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal static class IndexOfAnyAsciiSearcher
{
	public struct AsciiState(Vector128<byte> bitmap, BitVector256 lookup)
	{
		public Vector256<byte> Bitmap = Vector256.Create(bitmap);

		public readonly BitVector256 Lookup = lookup;

		public readonly AsciiState CreateInverse()
		{
			return new AsciiState(~Bitmap._lower, Lookup.CreateInverse());
		}
	}

	public readonly struct AnyByteState(Vector128<byte> bitmap0, Vector128<byte> bitmap1, BitVector256 lookup)
	{
		public readonly Vector256<byte> Bitmap0 = Vector256.Create(bitmap0);

		public readonly Vector256<byte> Bitmap1 = Vector256.Create(bitmap1);

		public readonly BitVector256 Lookup = lookup;
	}

	internal interface INegator
	{
		static abstract bool NegateIfNeeded(bool result);

		static abstract Vector128<byte> NegateIfNeeded(Vector128<byte> result);

		static abstract Vector256<byte> NegateIfNeeded(Vector256<byte> result);

		static abstract uint ExtractMask(Vector128<byte> result);

		static abstract uint ExtractMask(Vector256<byte> result);
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal readonly struct DontNegate : INegator
	{
		public static bool NegateIfNeeded(bool result)
		{
			return result;
		}

		public static Vector128<byte> NegateIfNeeded(Vector128<byte> result)
		{
			return result;
		}

		public static Vector256<byte> NegateIfNeeded(Vector256<byte> result)
		{
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ExtractMask(Vector128<byte> result)
		{
			return ~Vector128.Equals(result, Vector128<byte>.Zero).ExtractMostSignificantBits();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ExtractMask(Vector256<byte> result)
		{
			return ~Vector256.Equals(result, Vector256<byte>.Zero).ExtractMostSignificantBits();
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal readonly struct Negate : INegator
	{
		public static bool NegateIfNeeded(bool result)
		{
			return !result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector128<byte> NegateIfNeeded(Vector128<byte> result)
		{
			return Vector128.Equals(result, Vector128<byte>.Zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<byte> NegateIfNeeded(Vector256<byte> result)
		{
			return Vector256.Equals(result, Vector256<byte>.Zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ExtractMask(Vector128<byte> result)
		{
			return result.ExtractMostSignificantBits();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ExtractMask(Vector256<byte> result)
		{
			return result.ExtractMostSignificantBits();
		}
	}

	internal interface IOptimizations
	{
		static abstract Vector128<byte> PackSources(Vector128<ushort> lower, Vector128<ushort> upper);

		static abstract Vector256<byte> PackSources(Vector256<ushort> lower, Vector256<ushort> upper);
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal readonly struct Ssse3AndWasmHandleZeroInNeedle : IOptimizations
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(Sse2))]
		[CompExactlyDependsOn(typeof(PackedSimd))]
		public static Vector128<byte> PackSources(Vector128<ushort> lower, Vector128<ushort> upper)
		{
			Vector128<short> left = Vector128.Min(lower, Vector128.Create((ushort)255)).AsInt16();
			Vector128<short> right = Vector128.Min(upper, Vector128.Create((ushort)255)).AsInt16();
			if (Sse2.IsSupported)
			{
				return Sse2.PackUnsignedSaturate(left, right);
			}
			if (false)
			{
			}
			ThrowHelper.ThrowUnreachableException();
			return default(Vector128<byte>);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(Avx2))]
		public static Vector256<byte> PackSources(Vector256<ushort> lower, Vector256<ushort> upper)
		{
			return Avx2.PackUnsignedSaturate(Vector256.Min(lower, Vector256.Create((ushort)255)).AsInt16(), Vector256.Min(upper, Vector256.Create((ushort)255)).AsInt16());
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal readonly struct Default : IOptimizations
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(Sse2))]
		[CompExactlyDependsOn(typeof(AdvSimd))]
		[CompExactlyDependsOn(typeof(PackedSimd))]
		public static Vector128<byte> PackSources(Vector128<ushort> lower, Vector128<ushort> upper)
		{
			if (Sse2.IsSupported)
			{
				return Sse2.PackUnsignedSaturate(lower.AsInt16(), upper.AsInt16());
			}
			if (false)
			{
			}
			if (false)
			{
			}
			ThrowHelper.ThrowUnreachableException();
			return default(Vector128<byte>);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(Avx2))]
		public static Vector256<byte> PackSources(Vector256<ushort> lower, Vector256<ushort> upper)
		{
			return Avx2.PackUnsignedSaturate(lower.AsInt16(), upper.AsInt16());
		}
	}

	private interface IResultMapper<T, TResult> where TResult : struct
	{
		static abstract TResult NotFound { get; }

		static abstract TResult ScalarResult(ref T searchSpace, ref T current);

		static abstract TResult FirstIndex<TNegator>(ref T searchSpace, ref T current, Vector128<byte> result) where TNegator : struct, INegator;

		static abstract TResult FirstIndex<TNegator>(ref T searchSpace, ref T current, Vector256<byte> result) where TNegator : struct, INegator;

		static abstract TResult FirstIndexOverlapped<TNegator>(ref T searchSpace, ref T current0, ref T current1, Vector128<byte> result) where TNegator : struct, INegator;

		static abstract TResult FirstIndexOverlapped<TNegator>(ref T searchSpace, ref T current0, ref T current1, Vector256<byte> result) where TNegator : struct, INegator;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct ContainsAnyResultMapper<T> : IResultMapper<T, bool>
	{
		public static bool NotFound => false;

		public static bool ScalarResult(ref T searchSpace, ref T current)
		{
			return true;
		}

		public static bool FirstIndex<TNegator>(ref T searchSpace, ref T current, Vector128<byte> result) where TNegator : struct, INegator
		{
			return true;
		}

		public static bool FirstIndex<TNegator>(ref T searchSpace, ref T current, Vector256<byte> result) where TNegator : struct, INegator
		{
			return true;
		}

		public static bool FirstIndexOverlapped<TNegator>(ref T searchSpace, ref T current0, ref T current1, Vector128<byte> result) where TNegator : struct, INegator
		{
			return true;
		}

		public static bool FirstIndexOverlapped<TNegator>(ref T searchSpace, ref T current0, ref T current1, Vector256<byte> result) where TNegator : struct, INegator
		{
			return true;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct IndexOfAnyResultMapper<T> : IResultMapper<T, int>
	{
		public static int NotFound => -1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ScalarResult(ref T searchSpace, ref T current)
		{
			return (int)((nuint)Unsafe.ByteOffset(in searchSpace, in current) / (nuint)Unsafe.SizeOf<T>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FirstIndex<TNegator>(ref T searchSpace, ref T current, Vector128<byte> result) where TNegator : struct, INegator
		{
			return BitOperations.TrailingZeroCount(TNegator.ExtractMask(result)) + (int)((nuint)Unsafe.ByteOffset(in searchSpace, in current) / (nuint)Unsafe.SizeOf<T>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(Avx2))]
		public static int FirstIndex<TNegator>(ref T searchSpace, ref T current, Vector256<byte> result) where TNegator : struct, INegator
		{
			if (typeof(T) == typeof(short))
			{
				result = PackedSpanHelpers.FixUpPackedVector256Result(result);
			}
			return BitOperations.TrailingZeroCount(TNegator.ExtractMask(result)) + (int)((nuint)Unsafe.ByteOffset(in searchSpace, in current) / (nuint)Unsafe.SizeOf<T>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FirstIndexOverlapped<TNegator>(ref T searchSpace, ref T current0, ref T current1, Vector128<byte> result) where TNegator : struct, INegator
		{
			int num = BitOperations.TrailingZeroCount(TNegator.ExtractMask(result));
			if (num >= Vector128<short>.Count)
			{
				current0 = ref current1;
				num -= Vector128<short>.Count;
			}
			return num + (int)((nuint)Unsafe.ByteOffset(in searchSpace, in current0) / (nuint)Unsafe.SizeOf<T>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(Avx2))]
		public static int FirstIndexOverlapped<TNegator>(ref T searchSpace, ref T current0, ref T current1, Vector256<byte> result) where TNegator : struct, INegator
		{
			if (typeof(T) == typeof(short))
			{
				result = PackedSpanHelpers.FixUpPackedVector256Result(result);
			}
			int num = BitOperations.TrailingZeroCount(TNegator.ExtractMask(result));
			if (num >= Vector256<short>.Count)
			{
				current0 = ref current1;
				num -= Vector256<short>.Count;
			}
			return num + (int)((nuint)Unsafe.ByteOffset(in searchSpace, in current0) / (nuint)Unsafe.SizeOf<T>());
		}
	}

	internal static bool IsVectorizationSupported
	{
		get
		{
			if (!Ssse3.IsSupported)
			{
				_ = 0;
				return false;
			}
			return true;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void SetBitmapBit(byte* bitmap, int value)
	{
		int num = value >> 4;
		int num2 = value & 0xF;
		byte* num3 = bitmap + (uint)num2;
		*num3 |= (byte)(1 << num);
	}

	internal unsafe static void ComputeAnyByteState(ReadOnlySpan<byte> values, out AnyByteState state)
	{
		Vector128<byte> bitmap = default(Vector128<byte>);
		Vector128<byte> bitmap2 = default(Vector128<byte>);
		byte* bitmap3 = (byte*)(&bitmap);
		byte* bitmap4 = (byte*)(&bitmap2);
		BitVector256 lookup = default(BitVector256);
		ReadOnlySpan<byte> readOnlySpan = values;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			byte b = readOnlySpan[i];
			lookup.Set(b);
			if (b < 128)
			{
				SetBitmapBit(bitmap3, b);
			}
			else
			{
				SetBitmapBit(bitmap4, b - 128);
			}
		}
		state = new AnyByteState(bitmap, bitmap2, lookup);
	}

	internal unsafe static void ComputeAsciiState<T>(ReadOnlySpan<T> values, out AsciiState state) where T : struct, IUnsignedNumber<T>
	{
		Vector128<byte> bitmap = default(Vector128<byte>);
		byte* bitmap2 = (byte*)(&bitmap);
		BitVector256 lookup = default(BitVector256);
		ReadOnlySpan<T> readOnlySpan = values;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			int num = int.CreateChecked(readOnlySpan[i]);
			if (num <= 127)
			{
				lookup.Set(num);
				SetBitmapBit(bitmap2, num);
			}
		}
		state = new AsciiState(bitmap, lookup);
	}

	public static bool CanUseUniqueLowNibbleSearch<T>(ReadOnlySpan<T> values, int maxInclusive) where T : struct, IUnsignedNumber<T>
	{
		if (!IsVectorizationSupported || values.Length > 16)
		{
			return false;
		}
		if (Ssse3.IsSupported && maxInclusive > 127)
		{
			return false;
		}
		if (typeof(T) == typeof(char) && maxInclusive >= 255)
		{
			return false;
		}
		int num = 0;
		ReadOnlySpan<T> readOnlySpan = values;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			T value = readOnlySpan[i];
			int num2 = 1 << (int.CreateChecked(value) & 0xF);
			if ((num & num2) != 0)
			{
				return false;
			}
			num |= num2;
		}
		return true;
	}

	public static void ComputeUniqueLowNibbleState<T>(ReadOnlySpan<T> values, out AsciiState state) where T : struct, IUnsignedNumber<T>
	{
		Vector128<byte> vector = default(Vector128<byte>);
		BitVector256 lookup = default(BitVector256);
		ReadOnlySpan<T> readOnlySpan = values;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			byte b = byte.CreateTruncating(readOnlySpan[i]);
			lookup.Set(b);
			vector.SetElementUnsafe(b & 0xF, b);
		}
		if (vector.GetElement(0) == 0 && !lookup.Contains(0))
		{
			vector.SetElementUnsafe<byte>(0, 1);
		}
		state = new AsciiState(vector, lookup);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static bool TryComputeBitmap(ReadOnlySpan<char> values, byte* bitmap, out bool needleContainsZero)
	{
		ReadOnlySpan<char> readOnlySpan = values;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = readOnlySpan[i];
			if (c > '\u007f')
			{
				needleContainsZero = false;
				return false;
			}
			SetBitmapBit(bitmap, c);
		}
		needleContainsZero = (*bitmap & 1) != 0;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryIndexOfAny(ref char searchSpace, int searchSpaceLength, ReadOnlySpan<char> asciiValues, out int index)
	{
		return TryIndexOfAny<DontNegate>(ref Unsafe.As<char, short>(ref searchSpace), searchSpaceLength, asciiValues, out index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryIndexOfAnyExcept(ref char searchSpace, int searchSpaceLength, ReadOnlySpan<char> asciiValues, out int index)
	{
		return TryIndexOfAny<Negate>(ref Unsafe.As<char, short>(ref searchSpace), searchSpaceLength, asciiValues, out index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryLastIndexOfAny(ref char searchSpace, int searchSpaceLength, ReadOnlySpan<char> asciiValues, out int index)
	{
		return TryLastIndexOfAny<DontNegate>(ref Unsafe.As<char, short>(ref searchSpace), searchSpaceLength, asciiValues, out index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryLastIndexOfAnyExcept(ref char searchSpace, int searchSpaceLength, ReadOnlySpan<char> asciiValues, out int index)
	{
		return TryLastIndexOfAny<Negate>(ref Unsafe.As<char, short>(ref searchSpace), searchSpaceLength, asciiValues, out index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static bool TryIndexOfAny<TNegator>(ref short searchSpace, int searchSpaceLength, ReadOnlySpan<char> asciiValues, out int index) where TNegator : struct, INegator
	{
		if (IsVectorizationSupported)
		{
			AsciiState state = default(AsciiState);
			if (TryComputeBitmap(asciiValues, (byte*)(&state.Bitmap._lower), out var needleContainsZero))
			{
				state.Bitmap = Vector256.Create(state.Bitmap.GetLower());
				index = ((((uint)(Ssse3.IsSupported ? 1 : 0) & (needleContainsZero ? 1u : 0u)) != 0) ? IndexOfAny<TNegator, Ssse3AndWasmHandleZeroInNeedle, SearchValues.FalseConst>(ref searchSpace, searchSpaceLength, ref state) : IndexOfAny<TNegator, Default, SearchValues.FalseConst>(ref searchSpace, searchSpaceLength, ref state));
				return true;
			}
		}
		index = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static bool TryLastIndexOfAny<TNegator>(ref short searchSpace, int searchSpaceLength, ReadOnlySpan<char> asciiValues, out int index) where TNegator : struct, INegator
	{
		if (IsVectorizationSupported)
		{
			AsciiState state = default(AsciiState);
			if (TryComputeBitmap(asciiValues, (byte*)(&state.Bitmap._lower), out var needleContainsZero))
			{
				state.Bitmap = Vector256.Create(state.Bitmap.GetLower());
				index = ((((uint)(Ssse3.IsSupported ? 1 : 0) & (needleContainsZero ? 1u : 0u)) != 0) ? LastIndexOfAny<TNegator, Ssse3AndWasmHandleZeroInNeedle, SearchValues.FalseConst>(ref searchSpace, searchSpaceLength, ref state) : LastIndexOfAny<TNegator, Default, SearchValues.FalseConst>(ref searchSpace, searchSpaceLength, ref state));
				return true;
			}
		}
		index = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	public static bool ContainsAny<TNegator, TOptimizations, TUniqueLowNibble>(ref short searchSpace, int searchSpaceLength, ref AsciiState state) where TNegator : struct, INegator where TOptimizations : struct, IOptimizations where TUniqueLowNibble : struct, SearchValues.IRuntimeConst
	{
		return IndexOfAnyCore<bool, TNegator, TOptimizations, TUniqueLowNibble, ContainsAnyResultMapper<short>>(ref searchSpace, searchSpaceLength, ref state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	public static int IndexOfAny<TNegator, TOptimizations, TUniqueLowNibble>(ref short searchSpace, int searchSpaceLength, ref AsciiState state) where TNegator : struct, INegator where TOptimizations : struct, IOptimizations where TUniqueLowNibble : struct, SearchValues.IRuntimeConst
	{
		return IndexOfAnyCore<int, TNegator, TOptimizations, TUniqueLowNibble, IndexOfAnyResultMapper<short>>(ref searchSpace, searchSpaceLength, ref state);
	}

	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	private static TResult IndexOfAnyCore<TResult, TNegator, TOptimizations, TUniqueLowNibble, TResultMapper>(ref short searchSpace, int searchSpaceLength, ref AsciiState state) where TResult : struct where TNegator : struct, INegator where TOptimizations : struct, IOptimizations where TUniqueLowNibble : struct, SearchValues.IRuntimeConst where TResultMapper : struct, IResultMapper<short, TResult>
	{
		ref short reference = ref searchSpace;
		if (searchSpaceLength < Vector128<ushort>.Count)
		{
			ref short right = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
			while (!Unsafe.AreSame(in reference, in right))
			{
				char c = (char)reference;
				if (TNegator.NegateIfNeeded(state.Lookup.Contains256(c)))
				{
					return TResultMapper.ScalarResult(ref searchSpace, ref reference);
				}
				reference = ref Unsafe.Add(ref reference, 1);
			}
			return TResultMapper.NotFound;
		}
		if (Avx2.IsSupported && searchSpaceLength > 2 * Vector128<short>.Count)
		{
			Vector256<byte> bitmap = state.Bitmap;
			if (searchSpaceLength > 2 * Vector256<short>.Count)
			{
				ref short right2 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - 2 * Vector256<short>.Count);
				do
				{
					Vector256<short> source = Vector256.LoadUnsafe(in reference);
					Vector256<short> source2 = Vector256.LoadUnsafe(in reference, (nuint)Vector256<short>.Count);
					Vector256<byte> vector = IndexOfAnyLookup<TNegator, TOptimizations, TUniqueLowNibble>(source, source2, bitmap);
					if (vector != Vector256<byte>.Zero)
					{
						return TResultMapper.FirstIndex<TNegator>(ref searchSpace, ref reference, vector);
					}
					reference = ref Unsafe.Add(ref reference, 2 * Vector256<short>.Count);
				}
				while (Unsafe.IsAddressLessThan(in reference, in right2));
			}
			ref short reference2 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - Vector256<short>.Count);
			ref short reference3 = ref Unsafe.IsAddressGreaterThan(in reference, in reference2) ? ref reference2 : ref reference;
			Vector256<short> source3 = Vector256.LoadUnsafe(in reference3);
			Vector256<short> source4 = Vector256.LoadUnsafe(in reference2);
			Vector256<byte> vector2 = IndexOfAnyLookup<TNegator, TOptimizations, TUniqueLowNibble>(source3, source4, bitmap);
			if (vector2 != Vector256<byte>.Zero)
			{
				return TResultMapper.FirstIndexOverlapped<TNegator>(ref searchSpace, ref reference3, ref reference2, vector2);
			}
			return TResultMapper.NotFound;
		}
		Vector128<byte> lower = state.Bitmap._lower;
		if (!Avx2.IsSupported && searchSpaceLength > 2 * Vector128<short>.Count)
		{
			ref short right3 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - 2 * Vector128<short>.Count);
			do
			{
				Vector128<short> source5 = Vector128.LoadUnsafe(in reference);
				Vector128<short> source6 = Vector128.LoadUnsafe(in reference, (nuint)Vector128<short>.Count);
				Vector128<byte> vector3 = IndexOfAnyLookup<TNegator, TOptimizations, TUniqueLowNibble>(source5, source6, lower);
				if (vector3 != Vector128<byte>.Zero)
				{
					return TResultMapper.FirstIndex<TNegator>(ref searchSpace, ref reference, vector3);
				}
				reference = ref Unsafe.Add(ref reference, 2 * Vector128<short>.Count);
			}
			while (Unsafe.IsAddressLessThan(in reference, in right3));
		}
		ref short reference4 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - Vector128<short>.Count);
		ref short reference5 = ref Unsafe.IsAddressGreaterThan(in reference, in reference4) ? ref reference4 : ref reference;
		Vector128<short> source7 = Vector128.LoadUnsafe(in reference5);
		Vector128<short> source8 = Vector128.LoadUnsafe(in reference4);
		Vector128<byte> vector4 = IndexOfAnyLookup<TNegator, TOptimizations, TUniqueLowNibble>(source7, source8, lower);
		if (vector4 != Vector128<byte>.Zero)
		{
			return TResultMapper.FirstIndexOverlapped<TNegator>(ref searchSpace, ref reference5, ref reference4, vector4);
		}
		return TResultMapper.NotFound;
	}

	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	public static int LastIndexOfAny<TNegator, TOptimizations, TUniqueLowNibble>(ref short searchSpace, int searchSpaceLength, ref AsciiState state) where TNegator : struct, INegator where TOptimizations : struct, IOptimizations where TUniqueLowNibble : struct, SearchValues.IRuntimeConst
	{
		if (searchSpaceLength < Vector128<ushort>.Count)
		{
			for (int num = searchSpaceLength - 1; num >= 0; num--)
			{
				char c = (char)Unsafe.Add(ref searchSpace, num);
				if (TNegator.NegateIfNeeded(state.Lookup.Contains256(c)))
				{
					return num;
				}
			}
			return -1;
		}
		ref short reference = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
		if (Avx2.IsSupported && searchSpaceLength > 2 * Vector128<short>.Count)
		{
			Vector256<byte> bitmap = state.Bitmap;
			if (searchSpaceLength > 2 * Vector256<short>.Count)
			{
				ref short right = ref Unsafe.Add(ref searchSpace, 2 * Vector256<short>.Count);
				do
				{
					reference = ref Unsafe.Subtract(ref reference, 2 * Vector256<short>.Count);
					Vector256<short> source = Vector256.LoadUnsafe(in reference);
					Vector256<short> source2 = Vector256.LoadUnsafe(in reference, (nuint)Vector256<short>.Count);
					Vector256<byte> vector = IndexOfAnyLookup<TNegator, TOptimizations, TUniqueLowNibble>(source, source2, bitmap);
					if (vector != Vector256<byte>.Zero)
					{
						return ComputeLastIndex<short, TNegator>(ref searchSpace, ref reference, vector);
					}
				}
				while (Unsafe.IsAddressGreaterThan(in reference, in right));
			}
			ref short reference2 = ref Unsafe.IsAddressGreaterThan(in reference, in Unsafe.Add(ref searchSpace, Vector256<short>.Count)) ? ref Unsafe.Subtract(ref reference, Vector256<short>.Count) : ref searchSpace;
			Vector256<short> source3 = Vector256.LoadUnsafe(in searchSpace);
			Vector256<short> source4 = Vector256.LoadUnsafe(in reference2);
			Vector256<byte> vector2 = IndexOfAnyLookup<TNegator, TOptimizations, TUniqueLowNibble>(source3, source4, bitmap);
			if (vector2 != Vector256<byte>.Zero)
			{
				return ComputeLastIndexOverlapped<short, TNegator>(ref searchSpace, ref reference2, vector2);
			}
			return -1;
		}
		Vector128<byte> lower = state.Bitmap._lower;
		if (!Avx2.IsSupported && searchSpaceLength > 2 * Vector128<short>.Count)
		{
			ref short right2 = ref Unsafe.Add(ref searchSpace, 2 * Vector128<short>.Count);
			do
			{
				reference = ref Unsafe.Subtract(ref reference, 2 * Vector128<short>.Count);
				Vector128<short> source5 = Vector128.LoadUnsafe(in reference);
				Vector128<short> source6 = Vector128.LoadUnsafe(in reference, (nuint)Vector128<short>.Count);
				Vector128<byte> vector3 = IndexOfAnyLookup<TNegator, TOptimizations, TUniqueLowNibble>(source5, source6, lower);
				if (vector3 != Vector128<byte>.Zero)
				{
					return ComputeLastIndex<short, TNegator>(ref searchSpace, ref reference, vector3);
				}
			}
			while (Unsafe.IsAddressGreaterThan(in reference, in right2));
		}
		ref short reference3 = ref Unsafe.IsAddressGreaterThan(in reference, in Unsafe.Add(ref searchSpace, Vector128<short>.Count)) ? ref Unsafe.Subtract(ref reference, Vector128<short>.Count) : ref searchSpace;
		Vector128<short> source7 = Vector128.LoadUnsafe(in searchSpace);
		Vector128<short> source8 = Vector128.LoadUnsafe(in reference3);
		Vector128<byte> vector4 = IndexOfAnyLookup<TNegator, TOptimizations, TUniqueLowNibble>(source7, source8, lower);
		if (vector4 != Vector128<byte>.Zero)
		{
			return ComputeLastIndexOverlapped<short, TNegator>(ref searchSpace, ref reference3, vector4);
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	public static bool ContainsAny<TNegator, TUniqueLowNibble>(ref byte searchSpace, int searchSpaceLength, ref AsciiState state) where TNegator : struct, INegator where TUniqueLowNibble : struct, SearchValues.IRuntimeConst
	{
		return IndexOfAnyCore<bool, TNegator, TUniqueLowNibble, ContainsAnyResultMapper<byte>>(ref searchSpace, searchSpaceLength, ref state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	public static int IndexOfAny<TNegator, TUniqueLowNibble>(ref byte searchSpace, int searchSpaceLength, ref AsciiState state) where TNegator : struct, INegator where TUniqueLowNibble : struct, SearchValues.IRuntimeConst
	{
		return IndexOfAnyCore<int, TNegator, TUniqueLowNibble, IndexOfAnyResultMapper<byte>>(ref searchSpace, searchSpaceLength, ref state);
	}

	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	private static TResult IndexOfAnyCore<TResult, TNegator, TUniqueLowNibble, TResultMapper>(ref byte searchSpace, int searchSpaceLength, ref AsciiState state) where TResult : struct where TNegator : struct, INegator where TUniqueLowNibble : struct, SearchValues.IRuntimeConst where TResultMapper : struct, IResultMapper<byte, TResult>
	{
		ref byte reference = ref searchSpace;
		if (searchSpaceLength < 8)
		{
			ref byte right = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
			while (!Unsafe.AreSame(in reference, in right))
			{
				byte b = reference;
				if (TNegator.NegateIfNeeded(state.Lookup.Contains(b)))
				{
					return TResultMapper.ScalarResult(ref searchSpace, ref reference);
				}
				reference = ref Unsafe.Add(ref reference, 1);
			}
			return TResultMapper.NotFound;
		}
		if (Avx2.IsSupported && searchSpaceLength > Vector128<byte>.Count)
		{
			Vector256<byte> bitmap = state.Bitmap;
			if (searchSpaceLength > Vector256<byte>.Count)
			{
				ref byte right2 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - Vector256<byte>.Count);
				do
				{
					Vector256<byte> vector = TNegator.NegateIfNeeded(IndexOfAnyLookupCore<TUniqueLowNibble>(Vector256.LoadUnsafe(in reference), bitmap));
					if (vector != Vector256<byte>.Zero)
					{
						return TResultMapper.FirstIndex<TNegator>(ref searchSpace, ref reference, vector);
					}
					reference = ref Unsafe.Add(ref reference, Vector256<byte>.Count);
				}
				while (Unsafe.IsAddressLessThan(in reference, in right2));
			}
			ref byte reference2 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - Vector128<byte>.Count);
			ref byte reference3 = ref Unsafe.IsAddressGreaterThan(in reference, in reference2) ? ref reference2 : ref reference;
			Vector128<byte> lower = Vector128.LoadUnsafe(in reference3);
			Vector128<byte> upper = Vector128.LoadUnsafe(in reference2);
			Vector256<byte> vector2 = TNegator.NegateIfNeeded(IndexOfAnyLookupCore<TUniqueLowNibble>(Vector256.Create(lower, upper), bitmap));
			if (vector2 != Vector256<byte>.Zero)
			{
				return TResultMapper.FirstIndexOverlapped<TNegator>(ref searchSpace, ref reference3, ref reference2, vector2);
			}
			return TResultMapper.NotFound;
		}
		Vector128<byte> lower2 = state.Bitmap._lower;
		if (!Avx2.IsSupported && searchSpaceLength > Vector128<byte>.Count)
		{
			ref byte right3 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - Vector128<byte>.Count);
			do
			{
				Vector128<byte> vector3 = TNegator.NegateIfNeeded(IndexOfAnyLookupCore<TUniqueLowNibble>(Vector128.LoadUnsafe(in reference), lower2));
				if (vector3 != Vector128<byte>.Zero)
				{
					return TResultMapper.FirstIndex<TNegator>(ref searchSpace, ref reference, vector3);
				}
				reference = ref Unsafe.Add(ref reference, Vector128<byte>.Count);
			}
			while (Unsafe.IsAddressLessThan(in reference, in right3));
		}
		ref byte reference4 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - 8);
		ref byte reference5 = ref Unsafe.IsAddressGreaterThan(in reference, in reference4) ? ref reference4 : ref reference;
		ulong e = Unsafe.ReadUnaligned<ulong>(in reference5);
		ulong e2 = Unsafe.ReadUnaligned<ulong>(in reference4);
		Vector128<byte> vector4 = TNegator.NegateIfNeeded(IndexOfAnyLookupCore<TUniqueLowNibble>(Vector128.Create(e, e2).AsByte(), lower2));
		if (vector4 != Vector128<byte>.Zero)
		{
			return TResultMapper.FirstIndexOverlapped<TNegator>(ref searchSpace, ref reference5, ref reference4, vector4);
		}
		return TResultMapper.NotFound;
	}

	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	public static int LastIndexOfAny<TNegator, TUniqueLowNibble>(ref byte searchSpace, int searchSpaceLength, ref AsciiState state) where TNegator : struct, INegator where TUniqueLowNibble : struct, SearchValues.IRuntimeConst
	{
		if (searchSpaceLength < 8)
		{
			for (int num = searchSpaceLength - 1; num >= 0; num--)
			{
				byte b = Unsafe.Add(ref searchSpace, num);
				if (TNegator.NegateIfNeeded(state.Lookup.Contains(b)))
				{
					return num;
				}
			}
			return -1;
		}
		ref byte reference = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
		if (Avx2.IsSupported && searchSpaceLength > Vector128<byte>.Count)
		{
			Vector256<byte> bitmap = state.Bitmap;
			if (searchSpaceLength > Vector256<byte>.Count)
			{
				ref byte right = ref Unsafe.Add(ref searchSpace, Vector256<byte>.Count);
				do
				{
					reference = ref Unsafe.Subtract(ref reference, Vector256<byte>.Count);
					Vector256<byte> vector = TNegator.NegateIfNeeded(IndexOfAnyLookupCore<TUniqueLowNibble>(Vector256.LoadUnsafe(in reference), bitmap));
					if (vector != Vector256<byte>.Zero)
					{
						return ComputeLastIndex<byte, TNegator>(ref searchSpace, ref reference, vector);
					}
				}
				while (Unsafe.IsAddressGreaterThan(in reference, in right));
			}
			ref byte reference2 = ref Unsafe.IsAddressGreaterThan(in reference, in Unsafe.Add(ref searchSpace, Vector128<byte>.Count)) ? ref Unsafe.Subtract(ref reference, Vector128<byte>.Count) : ref searchSpace;
			Vector128<byte> lower = Vector128.LoadUnsafe(in searchSpace);
			Vector128<byte> upper = Vector128.LoadUnsafe(in reference2);
			Vector256<byte> vector2 = TNegator.NegateIfNeeded(IndexOfAnyLookupCore<TUniqueLowNibble>(Vector256.Create(lower, upper), bitmap));
			if (vector2 != Vector256<byte>.Zero)
			{
				return ComputeLastIndexOverlapped<byte, TNegator>(ref searchSpace, ref reference2, vector2);
			}
			return -1;
		}
		Vector128<byte> lower2 = state.Bitmap._lower;
		if (!Avx2.IsSupported && searchSpaceLength > Vector128<byte>.Count)
		{
			ref byte right2 = ref Unsafe.Add(ref searchSpace, Vector128<byte>.Count);
			do
			{
				reference = ref Unsafe.Subtract(ref reference, Vector128<byte>.Count);
				Vector128<byte> vector3 = TNegator.NegateIfNeeded(IndexOfAnyLookupCore<TUniqueLowNibble>(Vector128.LoadUnsafe(in reference), lower2));
				if (vector3 != Vector128<byte>.Zero)
				{
					return ComputeLastIndex<byte, TNegator>(ref searchSpace, ref reference, vector3);
				}
			}
			while (Unsafe.IsAddressGreaterThan(in reference, in right2));
		}
		ref byte reference3 = ref Unsafe.IsAddressGreaterThan(in reference, in Unsafe.Add(ref searchSpace, 8)) ? ref Unsafe.Subtract(ref reference, 8) : ref searchSpace;
		ulong e = Unsafe.ReadUnaligned<ulong>(in searchSpace);
		ulong e2 = Unsafe.ReadUnaligned<ulong>(in reference3);
		Vector128<byte> vector4 = TNegator.NegateIfNeeded(IndexOfAnyLookupCore<TUniqueLowNibble>(Vector128.Create(e, e2).AsByte(), lower2));
		if (vector4 != Vector128<byte>.Zero)
		{
			return ComputeLastIndexOverlapped<byte, TNegator>(ref searchSpace, ref reference3, vector4);
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	public static bool ContainsAny<TNegator>(ref byte searchSpace, int searchSpaceLength, ref AnyByteState state) where TNegator : struct, INegator
	{
		return IndexOfAnyCore<bool, TNegator, ContainsAnyResultMapper<byte>>(ref searchSpace, searchSpaceLength, ref state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	public static int IndexOfAny<TNegator>(ref byte searchSpace, int searchSpaceLength, ref AnyByteState state) where TNegator : struct, INegator
	{
		return IndexOfAnyCore<int, TNegator, IndexOfAnyResultMapper<byte>>(ref searchSpace, searchSpaceLength, ref state);
	}

	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	private static TResult IndexOfAnyCore<TResult, TNegator, TResultMapper>(ref byte searchSpace, int searchSpaceLength, ref AnyByteState state) where TResult : struct where TNegator : struct, INegator where TResultMapper : struct, IResultMapper<byte, TResult>
	{
		ref byte reference = ref searchSpace;
		if (!IsVectorizationSupported || searchSpaceLength < 8)
		{
			ref byte right = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
			while (!Unsafe.AreSame(in reference, in right))
			{
				byte b = reference;
				if (TNegator.NegateIfNeeded(state.Lookup.Contains(b)))
				{
					return TResultMapper.ScalarResult(ref searchSpace, ref reference);
				}
				reference = ref Unsafe.Add(ref reference, 1);
			}
			return TResultMapper.NotFound;
		}
		if (Avx2.IsSupported && searchSpaceLength > Vector128<byte>.Count)
		{
			Vector256<byte> bitmap = state.Bitmap0;
			Vector256<byte> bitmap2 = state.Bitmap1;
			if (searchSpaceLength > Vector256<byte>.Count)
			{
				ref byte right2 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - Vector256<byte>.Count);
				do
				{
					Vector256<byte> vector = IndexOfAnyLookup<TNegator>(Vector256.LoadUnsafe(in reference), bitmap, bitmap2);
					if (vector != Vector256<byte>.Zero)
					{
						return TResultMapper.FirstIndex<TNegator>(ref searchSpace, ref reference, vector);
					}
					reference = ref Unsafe.Add(ref reference, Vector256<byte>.Count);
				}
				while (Unsafe.IsAddressLessThan(in reference, in right2));
			}
			ref byte reference2 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - Vector128<byte>.Count);
			ref byte reference3 = ref Unsafe.IsAddressGreaterThan(in reference, in reference2) ? ref reference2 : ref reference;
			Vector128<byte> lower = Vector128.LoadUnsafe(in reference3);
			Vector128<byte> upper = Vector128.LoadUnsafe(in reference2);
			Vector256<byte> vector2 = IndexOfAnyLookup<TNegator>(Vector256.Create(lower, upper), bitmap, bitmap2);
			if (vector2 != Vector256<byte>.Zero)
			{
				return TResultMapper.FirstIndexOverlapped<TNegator>(ref searchSpace, ref reference3, ref reference2, vector2);
			}
			return TResultMapper.NotFound;
		}
		Vector128<byte> lower2 = state.Bitmap0._lower;
		Vector128<byte> lower3 = state.Bitmap1._lower;
		if (!Avx2.IsSupported && searchSpaceLength > Vector128<byte>.Count)
		{
			ref byte right3 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - Vector128<byte>.Count);
			do
			{
				Vector128<byte> vector3 = IndexOfAnyLookup<TNegator>(Vector128.LoadUnsafe(in reference), lower2, lower3);
				if (vector3 != Vector128<byte>.Zero)
				{
					return TResultMapper.FirstIndex<TNegator>(ref searchSpace, ref reference, vector3);
				}
				reference = ref Unsafe.Add(ref reference, Vector128<byte>.Count);
			}
			while (Unsafe.IsAddressLessThan(in reference, in right3));
		}
		ref byte reference4 = ref Unsafe.Add(ref searchSpace, searchSpaceLength - 8);
		ref byte reference5 = ref Unsafe.IsAddressGreaterThan(in reference, in reference4) ? ref reference4 : ref reference;
		ulong e = Unsafe.ReadUnaligned<ulong>(in reference5);
		ulong e2 = Unsafe.ReadUnaligned<ulong>(in reference4);
		Vector128<byte> vector4 = IndexOfAnyLookup<TNegator>(Vector128.Create(e, e2).AsByte(), lower2, lower3);
		if (vector4 != Vector128<byte>.Zero)
		{
			return TResultMapper.FirstIndexOverlapped<TNegator>(ref searchSpace, ref reference5, ref reference4, vector4);
		}
		return TResultMapper.NotFound;
	}

	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	public static int LastIndexOfAny<TNegator>(ref byte searchSpace, int searchSpaceLength, ref AnyByteState state) where TNegator : struct, INegator
	{
		if (!IsVectorizationSupported || searchSpaceLength < 8)
		{
			for (int num = searchSpaceLength - 1; num >= 0; num--)
			{
				byte b = Unsafe.Add(ref searchSpace, num);
				if (TNegator.NegateIfNeeded(state.Lookup.Contains(b)))
				{
					return num;
				}
			}
			return -1;
		}
		ref byte reference = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
		if (Avx2.IsSupported && searchSpaceLength > Vector128<byte>.Count)
		{
			Vector256<byte> bitmap = state.Bitmap0;
			Vector256<byte> bitmap2 = state.Bitmap1;
			if (searchSpaceLength > Vector256<byte>.Count)
			{
				ref byte right = ref Unsafe.Add(ref searchSpace, Vector256<byte>.Count);
				do
				{
					reference = ref Unsafe.Subtract(ref reference, Vector256<byte>.Count);
					Vector256<byte> vector = IndexOfAnyLookup<TNegator>(Vector256.LoadUnsafe(in reference), bitmap, bitmap2);
					if (vector != Vector256<byte>.Zero)
					{
						return ComputeLastIndex<byte, TNegator>(ref searchSpace, ref reference, vector);
					}
				}
				while (Unsafe.IsAddressGreaterThan(in reference, in right));
			}
			ref byte reference2 = ref Unsafe.IsAddressGreaterThan(in reference, in Unsafe.Add(ref searchSpace, Vector128<byte>.Count)) ? ref Unsafe.Subtract(ref reference, Vector128<byte>.Count) : ref searchSpace;
			Vector128<byte> lower = Vector128.LoadUnsafe(in searchSpace);
			Vector128<byte> upper = Vector128.LoadUnsafe(in reference2);
			Vector256<byte> vector2 = IndexOfAnyLookup<TNegator>(Vector256.Create(lower, upper), bitmap, bitmap2);
			if (vector2 != Vector256<byte>.Zero)
			{
				return ComputeLastIndexOverlapped<byte, TNegator>(ref searchSpace, ref reference2, vector2);
			}
			return -1;
		}
		Vector128<byte> lower2 = state.Bitmap0._lower;
		Vector128<byte> lower3 = state.Bitmap1._lower;
		if (!Avx2.IsSupported && searchSpaceLength > Vector128<byte>.Count)
		{
			ref byte right2 = ref Unsafe.Add(ref searchSpace, Vector128<byte>.Count);
			do
			{
				reference = ref Unsafe.Subtract(ref reference, Vector128<byte>.Count);
				Vector128<byte> vector3 = IndexOfAnyLookup<TNegator>(Vector128.LoadUnsafe(in reference), lower2, lower3);
				if (vector3 != Vector128<byte>.Zero)
				{
					return ComputeLastIndex<byte, TNegator>(ref searchSpace, ref reference, vector3);
				}
			}
			while (Unsafe.IsAddressGreaterThan(in reference, in right2));
		}
		ref byte reference3 = ref Unsafe.IsAddressGreaterThan(in reference, in Unsafe.Add(ref searchSpace, 8)) ? ref Unsafe.Subtract(ref reference, 8) : ref searchSpace;
		ulong e = Unsafe.ReadUnaligned<ulong>(in searchSpace);
		ulong e2 = Unsafe.ReadUnaligned<ulong>(in reference3);
		Vector128<byte> vector4 = IndexOfAnyLookup<TNegator>(Vector128.Create(e, e2).AsByte(), lower2, lower3);
		if (vector4 != Vector128<byte>.Zero)
		{
			return ComputeLastIndexOverlapped<byte, TNegator>(ref searchSpace, ref reference3, vector4);
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Sse2))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	private static Vector128<byte> IndexOfAnyLookup<TNegator, TOptimizations, TUniqueLowNibble>(Vector128<short> source0, Vector128<short> source1, Vector128<byte> bitmapLookup) where TNegator : struct, INegator where TOptimizations : struct, IOptimizations where TUniqueLowNibble : struct, SearchValues.IRuntimeConst
	{
		return TNegator.NegateIfNeeded(IndexOfAnyLookupCore<TUniqueLowNibble>(TOptimizations.PackSources(source0.AsUInt16(), source1.AsUInt16()), bitmapLookup));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	private static Vector128<byte> IndexOfAnyLookupCore<TUniqueLowNibble>(Vector128<byte> source, Vector128<byte> bitmapLookup) where TUniqueLowNibble : struct, SearchValues.IRuntimeConst
	{
		if (TUniqueLowNibble.Value)
		{
			Vector128<byte> indices = (Ssse3.IsSupported ? source : (source & Vector128.Create((byte)15)));
			Vector128<byte> right = SearchValues.ShuffleNativeModified(bitmapLookup, indices);
			return Vector128.Equals(source, right);
		}
		Vector128<byte> indices2 = (Ssse3.IsSupported ? source : (source & Vector128.Create((byte)15)));
		_ = 0;
		Vector128<byte> indices3 = source >>> 4;
		Vector128<byte> vector = SearchValues.ShuffleNativeModified(bitmapLookup, indices2);
		Vector128<byte> vector2 = SearchValues.ShuffleNativeModified(Vector128.Create(9241421688590303745uL, 0uL).AsByte(), indices3);
		return vector & vector2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static Vector256<byte> IndexOfAnyLookup<TNegator, TOptimizations, TUniqueLowNibble>(Vector256<short> source0, Vector256<short> source1, Vector256<byte> bitmapLookup) where TNegator : struct, INegator where TOptimizations : struct, IOptimizations where TUniqueLowNibble : struct, SearchValues.IRuntimeConst
	{
		return TNegator.NegateIfNeeded(IndexOfAnyLookupCore<TUniqueLowNibble>(TOptimizations.PackSources(source0.AsUInt16(), source1.AsUInt16()), bitmapLookup));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static Vector256<byte> IndexOfAnyLookupCore<TUniqueLowNibble>(Vector256<byte> source, Vector256<byte> bitmapLookup) where TUniqueLowNibble : struct, SearchValues.IRuntimeConst
	{
		if (TUniqueLowNibble.Value)
		{
			Vector256<byte> right = Avx2.Shuffle(bitmapLookup, source);
			return Vector256.Equals(source, right);
		}
		Vector256<byte> mask = source >>> 4;
		Vector256<byte> vector = Avx2.Shuffle(bitmapLookup, source);
		Vector256<byte> vector2 = Avx2.Shuffle(Vector256.Create(9241421688590303745uL).AsByte(), mask);
		return vector & vector2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	private static Vector128<byte> IndexOfAnyLookup<TNegator>(Vector128<byte> source, Vector128<byte> bitmapLookup0, Vector128<byte> bitmapLookup1) where TNegator : struct, INegator
	{
		Vector128<byte> indices = source & Vector128.Create((byte)15);
		Vector128<byte> vector = source >>> 4;
		Vector128<byte> right = Vector128.ShuffleNative(bitmapLookup0, indices);
		Vector128<byte> left = Vector128.ShuffleNative(bitmapLookup1, indices);
		Vector128<byte> vector2 = Vector128.ShuffleNative(Vector128.Create(9241421688590303745uL).AsByte(), vector);
		return TNegator.NegateIfNeeded(Vector128.Equals(Vector128.ConditionalSelect(Vector128.GreaterThan(vector.AsSByte(), Vector128.Create((sbyte)7)).AsByte(), left, right) & vector2, vector2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static Vector256<byte> IndexOfAnyLookup<TNegator>(Vector256<byte> source, Vector256<byte> bitmapLookup0, Vector256<byte> bitmapLookup1) where TNegator : struct, INegator
	{
		Vector256<byte> mask = source & Vector256.Create((byte)15);
		Vector256<byte> vector = source >>> 4;
		Vector256<byte> right = Avx2.Shuffle(bitmapLookup0, mask);
		Vector256<byte> left = Avx2.Shuffle(bitmapLookup1, mask);
		Vector256<byte> vector2 = Avx2.Shuffle(Vector256.Create(9241421688590303745uL).AsByte(), vector);
		return TNegator.NegateIfNeeded(Vector256.Equals(Vector256.ConditionalSelect(Vector256.GreaterThan(vector.AsSByte(), Vector256.Create((sbyte)7)).AsByte(), left, right) & vector2, vector2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ComputeLastIndex<T, TNegator>(ref T searchSpace, ref T current, Vector128<byte> result) where TNegator : struct, INegator
	{
		uint value = TNegator.ExtractMask(result) & 0xFFFF;
		return 31 - BitOperations.LeadingZeroCount(value) + (int)((nuint)Unsafe.ByteOffset(in searchSpace, in current) / (nuint)Unsafe.SizeOf<T>());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ComputeLastIndexOverlapped<T, TNegator>(ref T searchSpace, ref T secondVector, Vector128<byte> result) where TNegator : struct, INegator
	{
		uint value = TNegator.ExtractMask(result) & 0xFFFF;
		int num = 31 - BitOperations.LeadingZeroCount(value);
		if (num < Vector128<short>.Count)
		{
			return num;
		}
		return num - Vector128<short>.Count + (int)((nuint)Unsafe.ByteOffset(in searchSpace, in secondVector) / (nuint)Unsafe.SizeOf<T>());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static int ComputeLastIndex<T, TNegator>(ref T searchSpace, ref T current, Vector256<byte> result) where TNegator : struct, INegator
	{
		if (typeof(T) == typeof(short))
		{
			result = PackedSpanHelpers.FixUpPackedVector256Result(result);
		}
		uint value = TNegator.ExtractMask(result);
		return 31 - BitOperations.LeadingZeroCount(value) + (int)((nuint)Unsafe.ByteOffset(in searchSpace, in current) / (nuint)Unsafe.SizeOf<T>());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static int ComputeLastIndexOverlapped<T, TNegator>(ref T searchSpace, ref T secondVector, Vector256<byte> result) where TNegator : struct, INegator
	{
		if (typeof(T) == typeof(short))
		{
			result = PackedSpanHelpers.FixUpPackedVector256Result(result);
		}
		uint value = TNegator.ExtractMask(result);
		int num = 31 - BitOperations.LeadingZeroCount(value);
		if (num < Vector256<short>.Count)
		{
			return num;
		}
		return num - Vector256<short>.Count + (int)((nuint)Unsafe.ByteOffset(in searchSpace, in secondVector) / (nuint)Unsafe.SizeOf<T>());
	}
}
