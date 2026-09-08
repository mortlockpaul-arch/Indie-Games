using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers.Text;

internal static class FormattingHelpers
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CountDigits(ulong value)
	{
		nint num = ((ReadOnlySpan<byte>)new byte[64]
		{
			1, 1, 1, 2, 2, 2, 3, 3, 3, 4,
			4, 4, 4, 5, 5, 5, 6, 6, 6, 7,
			7, 7, 7, 8, 8, 8, 9, 9, 9, 10,
			10, 10, 10, 11, 11, 11, 12, 12, 12, 13,
			13, 13, 13, 14, 14, 14, 15, 15, 15, 16,
			16, 16, 16, 17, 17, 17, 18, 18, 18, 19,
			19, 19, 19, 20
		})[(int)ulong.Log2(value)];
		ulong num2 = Unsafe.Add(ref MemoryMarshal.GetReference(new ulong[21]
		{
			0uL, 0uL, 10uL, 100uL, 1000uL, 10000uL, 100000uL, 1000000uL, 10000000uL, 100000000uL,
			1000000000uL, 10000000000uL, 100000000000uL, 1000000000000uL, 10000000000000uL, 100000000000000uL, 1000000000000000uL, 10000000000000000uL, 100000000000000000uL, 1000000000000000000uL,
			10000000000000000000uL
		}), num);
		return (int)num - ((value < num2) ? 1 : 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CountDigits(uint value)
	{
		long num = ((ReadOnlySpan<long>)new long[32]
		{
			4294967296L, 8589934582L, 8589934582L, 8589934582L, 12884901788L, 12884901788L, 12884901788L, 17179868184L, 17179868184L, 17179868184L,
			21474826480L, 21474826480L, 21474826480L, 21474826480L, 25769703776L, 25769703776L, 25769703776L, 30063771072L, 30063771072L, 30063771072L,
			34349738368L, 34349738368L, 34349738368L, 34349738368L, 38554705664L, 38554705664L, 38554705664L, 41949672960L, 41949672960L, 41949672960L,
			42949672960L, 42949672960L
		})[(int)uint.Log2(value)];
		return (int)(value + num >> 32);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CountHexDigits(ulong value)
	{
		return (BitOperations.Log2(value) >> 2) + 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CountDecimalTrailingZeros(uint value, out uint valueWithoutTrailingZeros)
	{
		int num = 0;
		if (value != 0)
		{
			while (true)
			{
				uint num2 = value / 10;
				if (value != num2 * 10)
				{
					break;
				}
				value = num2;
				num++;
			}
		}
		valueWithoutTrailingZeros = value;
		return num;
	}
}
