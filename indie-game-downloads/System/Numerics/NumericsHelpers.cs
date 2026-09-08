using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace System.Numerics;

internal static class NumericsHelpers
{
	public static void GetDoubleParts(double dbl, out int sign, out int exp, out ulong man, out bool fFinite)
	{
		ulong num = BitConverter.DoubleToUInt64Bits(dbl);
		sign = 1 - ((int)(num >> 62) & 2);
		man = num & 0xFFFFFFFFFFFFFL;
		exp = (int)(num >> 52) & 0x7FF;
		if (exp == 0)
		{
			fFinite = true;
			if (man != 0L)
			{
				exp = -1074;
			}
		}
		else if (exp == 2047)
		{
			fFinite = false;
			exp = int.MaxValue;
		}
		else
		{
			fFinite = true;
			man |= 4503599627370496uL;
			exp -= 1075;
		}
	}

	public static double GetDoubleFromParts(int sign, int exp, ulong man)
	{
		ulong num;
		if (man == 0L)
		{
			num = 0uL;
		}
		else
		{
			int num2 = BitOperations.LeadingZeroCount(man) - 11;
			man = ((num2 >= 0) ? (man << num2) : (man >> -num2));
			exp -= num2;
			exp += 1075;
			if (exp >= 2047)
			{
				num = 9218868437227405312uL;
			}
			else if (exp <= 0)
			{
				exp--;
				num = ((exp >= -52) ? (man >> -exp) : 0);
			}
			else
			{
				num = (man & 0xFFFFFFFFFFFFFL) | (ulong)((long)exp << 52);
			}
		}
		if (sign < 0)
		{
			num |= 0x8000000000000000uL;
		}
		return BitConverter.UInt64BitsToDouble(num);
	}

	public static void DangerousMakeTwosComplement(Span<uint> d)
	{
		d = d.TrimStart(0u);
		if (d.Length > 0)
		{
			d[0] = 0 - d[0];
			d = d.Slice(1);
		}
		if (!d.IsEmpty)
		{
			int i = 0;
			ref uint reference = ref MemoryMarshal.GetReference(d);
			while (Vector512.IsHardwareAccelerated && d.Length - i >= Vector512<uint>.Count)
			{
				(~Vector512.LoadUnsafe(in reference, (nuint)i)).StoreUnsafe(ref reference, (nuint)i);
				i += Vector512<uint>.Count;
			}
			while (Vector256.IsHardwareAccelerated && d.Length - i >= Vector256<uint>.Count)
			{
				(~Vector256.LoadUnsafe(in reference, (nuint)i)).StoreUnsafe(ref reference, (nuint)i);
				i += Vector256<uint>.Count;
			}
			while (Vector128.IsHardwareAccelerated && d.Length - i >= Vector128<uint>.Count)
			{
				(~Vector128.LoadUnsafe(in reference, (nuint)i)).StoreUnsafe(ref reference, (nuint)i);
				i += Vector128<uint>.Count;
			}
			for (; i < d.Length; i++)
			{
				d[i] = ~d[i];
			}
		}
	}

	public static ulong MakeUInt64(uint uHi, uint uLo)
	{
		return ((ulong)uHi << 32) | uLo;
	}

	public static uint Abs(int a)
	{
		uint num = (uint)(a >> 31);
		return ((uint)a ^ num) - num;
	}
}
