using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal static class TeddyHelper
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	public static (Vector128<byte> Result, Vector128<byte> Prev0) ProcessInputN2(Vector128<byte> input, Vector128<byte> prev0, Vector128<byte> n0Low, Vector128<byte> n0High, Vector128<byte> n1Low, Vector128<byte> n1High)
	{
		(Vector128<byte> Low, Vector128<byte> High) nibbles = GetNibbles(input);
		Vector128<byte> item = nibbles.Low;
		Vector128<byte> item2 = nibbles.High;
		Vector128<byte> vector = Shuffle(n0Low, n0High, item, item2);
		Vector128<byte> vector2 = Shuffle(n1Low, n1High, item, item2);
		return (Result: RightShift1(prev0, vector) & vector2, Prev0: vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	public static (Vector256<byte> Result, Vector256<byte> Prev0) ProcessInputN2(Vector256<byte> input, Vector256<byte> prev0, Vector256<byte> n0Low, Vector256<byte> n0High, Vector256<byte> n1Low, Vector256<byte> n1High)
	{
		(Vector256<byte> Low, Vector256<byte> High) nibbles = GetNibbles(input);
		Vector256<byte> item = nibbles.Low;
		Vector256<byte> item2 = nibbles.High;
		Vector256<byte> vector = Shuffle(n0Low, n0High, item, item2);
		Vector256<byte> vector2 = Shuffle(n1Low, n1High, item, item2);
		return (Result: RightShift1(prev0, vector) & vector2, Prev0: vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512Vbmi))]
	public static (Vector512<byte> Result, Vector512<byte> Prev0) ProcessInputN2(Vector512<byte> input, Vector512<byte> prev0, Vector512<byte> n0Low, Vector512<byte> n0High, Vector512<byte> n1Low, Vector512<byte> n1High)
	{
		(Vector512<byte> Low, Vector512<byte> High) nibbles = GetNibbles(input);
		Vector512<byte> item = nibbles.Low;
		Vector512<byte> item2 = nibbles.High;
		Vector512<byte> vector = Shuffle(n0Low, n0High, item, item2);
		Vector512<byte> vector2 = Shuffle(n1Low, n1High, item, item2);
		return (Result: RightShift1(prev0, vector) & vector2, Prev0: vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	public static (Vector128<byte> Result, Vector128<byte> Prev0, Vector128<byte> Prev1) ProcessInputN3(Vector128<byte> input, Vector128<byte> prev0, Vector128<byte> prev1, Vector128<byte> n0Low, Vector128<byte> n0High, Vector128<byte> n1Low, Vector128<byte> n1High, Vector128<byte> n2Low, Vector128<byte> n2High)
	{
		(Vector128<byte> Low, Vector128<byte> High) nibbles = GetNibbles(input);
		Vector128<byte> item = nibbles.Low;
		Vector128<byte> item2 = nibbles.High;
		Vector128<byte> vector = Shuffle(n0Low, n0High, item, item2);
		Vector128<byte> vector2 = Shuffle(n1Low, n1High, item, item2);
		Vector128<byte> vector3 = Shuffle(n2Low, n2High, item, item2);
		Vector128<byte> vector4 = RightShift2(prev0, vector);
		Vector128<byte> vector5 = RightShift1(prev1, vector2);
		return (Result: vector4 & vector5 & vector3, Prev0: vector, Prev1: vector2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	public static (Vector256<byte> Result, Vector256<byte> Prev0, Vector256<byte> Prev1) ProcessInputN3(Vector256<byte> input, Vector256<byte> prev0, Vector256<byte> prev1, Vector256<byte> n0Low, Vector256<byte> n0High, Vector256<byte> n1Low, Vector256<byte> n1High, Vector256<byte> n2Low, Vector256<byte> n2High)
	{
		(Vector256<byte> Low, Vector256<byte> High) nibbles = GetNibbles(input);
		Vector256<byte> item = nibbles.Low;
		Vector256<byte> item2 = nibbles.High;
		Vector256<byte> vector = Shuffle(n0Low, n0High, item, item2);
		Vector256<byte> vector2 = Shuffle(n1Low, n1High, item, item2);
		Vector256<byte> vector3 = Shuffle(n2Low, n2High, item, item2);
		Vector256<byte> vector4 = RightShift2(prev0, vector);
		Vector256<byte> vector5 = RightShift1(prev1, vector2);
		return (Result: vector4 & vector5 & vector3, Prev0: vector, Prev1: vector2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512Vbmi))]
	public static (Vector512<byte> Result, Vector512<byte> Prev0, Vector512<byte> Prev1) ProcessInputN3(Vector512<byte> input, Vector512<byte> prev0, Vector512<byte> prev1, Vector512<byte> n0Low, Vector512<byte> n0High, Vector512<byte> n1Low, Vector512<byte> n1High, Vector512<byte> n2Low, Vector512<byte> n2High)
	{
		(Vector512<byte> Low, Vector512<byte> High) nibbles = GetNibbles(input);
		Vector512<byte> item = nibbles.Low;
		Vector512<byte> item2 = nibbles.High;
		Vector512<byte> vector = Shuffle(n0Low, n0High, item, item2);
		Vector512<byte> vector2 = Shuffle(n1Low, n1High, item, item2);
		Vector512<byte> vector3 = Shuffle(n2Low, n2High, item, item2);
		Vector512<byte> vector4 = RightShift2(prev0, vector);
		Vector512<byte> vector5 = RightShift1(prev1, vector2);
		return (Result: vector4 & vector5 & vector3, Prev0: vector, Prev1: vector2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Sse2))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	public static Vector128<byte> LoadAndPack16AsciiChars(ref char source)
	{
		Vector128<ushort> vector = Vector128.LoadUnsafe(ref source);
		Vector128<ushort> vector2 = Vector128.LoadUnsafe(ref source, (nuint)Vector128<ushort>.Count);
		if (Sse2.IsSupported)
		{
			return Sse2.PackUnsignedSaturate(vector.AsInt16(), vector2.AsInt16());
		}
		if (false)
		{
		}
		ThrowHelper.ThrowUnreachableException();
		return default(Vector128<byte>);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	public static Vector256<byte> LoadAndPack32AsciiChars(ref char source)
	{
		Vector256<ushort> vector = Vector256.LoadUnsafe(ref source);
		return PackedSpanHelpers.FixUpPackedVector256Result(Avx2.PackUnsignedSaturate(right: Vector256.LoadUnsafe(ref source, (nuint)Vector256<ushort>.Count).AsInt16(), left: vector.AsInt16()));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512BW))]
	public static Vector512<byte> LoadAndPack64AsciiChars(ref char source)
	{
		Vector512<ushort> vector = Vector512.LoadUnsafe(ref source);
		return PackedSpanHelpers.FixUpPackedVector512Result(Avx512BW.PackUnsignedSaturate(right: Vector512.LoadUnsafe(ref source, (nuint)Vector512<ushort>.Count).AsInt16(), left: vector.AsInt16()));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	private static (Vector128<byte> Low, Vector128<byte> High) GetNibbles(Vector128<byte> input)
	{
		Vector128<byte> item = (Ssse3.IsSupported ? input : (input & Vector128.Create((byte)15)));
		Vector128<byte> item2 = input >>> 4;
		return (Low: item, High: item2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static (Vector256<byte> Low, Vector256<byte> High) GetNibbles(Vector256<byte> input)
	{
		Vector256<byte> item = input >>> 4;
		return (Low: input, High: item);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static (Vector512<byte> Low, Vector512<byte> High) GetNibbles(Vector512<byte> input)
	{
		Vector512<byte> item = input >>> 4;
		return (Low: input, High: item);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	private static Vector128<byte> Shuffle(Vector128<byte> maskLow, Vector128<byte> maskHigh, Vector128<byte> low, Vector128<byte> high)
	{
		return SearchValues.ShuffleNativeModified(maskLow, low) & Vector128.ShuffleNative(maskHigh, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static Vector256<byte> Shuffle(Vector256<byte> maskLow, Vector256<byte> maskHigh, Vector256<byte> low, Vector256<byte> high)
	{
		return Avx2.Shuffle(maskLow, low) & Avx2.Shuffle(maskHigh, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512BW))]
	private static Vector512<byte> Shuffle(Vector512<byte> maskLow, Vector512<byte> maskHigh, Vector512<byte> low, Vector512<byte> high)
	{
		return Avx512BW.Shuffle(maskLow, low) & Avx512BW.Shuffle(maskHigh, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	private static Vector128<byte> RightShift1(Vector128<byte> left, Vector128<byte> right)
	{
		if (Ssse3.IsSupported)
		{
			return Ssse3.AlignRight(right, left, 15);
		}
		if (false)
		{
		}
		ThrowHelper.ThrowUnreachableException();
		return default(Vector128<byte>);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	private static Vector128<byte> RightShift2(Vector128<byte> left, Vector128<byte> right)
	{
		if (Ssse3.IsSupported)
		{
			return Ssse3.AlignRight(right, left, 14);
		}
		if (false)
		{
		}
		ThrowHelper.ThrowUnreachableException();
		return default(Vector128<byte>);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static Vector256<byte> RightShift1(Vector256<byte> left, Vector256<byte> right)
	{
		Vector256<byte> right2 = Avx2.Permute2x128(left, right, 33);
		return Avx2.AlignRight(right, right2, 15);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private static Vector256<byte> RightShift2(Vector256<byte> left, Vector256<byte> right)
	{
		Vector256<byte> right2 = Avx2.Permute2x128(left, right, 33);
		return Avx2.AlignRight(right, right2, 14);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512Vbmi))]
	private static Vector512<byte> RightShift1(Vector512<byte> left, Vector512<byte> right)
	{
		return Avx512Vbmi.PermuteVar64x8x2(left, Vector512.CreateSequence((byte)63, (byte)1), right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512Vbmi))]
	private static Vector512<byte> RightShift2(Vector512<byte> left, Vector512<byte> right)
	{
		return Avx512Vbmi.PermuteVar64x8x2(left, Vector512.CreateSequence((byte)62, (byte)1), right);
	}
}
