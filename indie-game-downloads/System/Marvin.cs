using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace System;

internal static class Marvin
{
	public static ulong DefaultSeed { get; } = GenerateSeed();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeHash32(ReadOnlySpan<byte> data, ulong seed)
	{
		return ComputeHash32(ref MemoryMarshal.GetReference(data), (uint)data.Length, (uint)seed, (uint)(seed >> 32));
	}

	public static int ComputeHash32(ref byte data, uint count, uint p0, uint p1)
	{
		uint num;
		if (count < 8)
		{
			if (count < 4)
			{
				num = ((!BitConverter.IsLittleEndian) ? 2147483648u : 128u);
				if ((count & 1) != 0)
				{
					num = Unsafe.AddByteOffset(ref data, (nuint)count & (nuint)2u);
					if (BitConverter.IsLittleEndian)
					{
						num |= 0x8000;
					}
					else
					{
						num <<= 24;
						num |= 0x800000;
					}
				}
				if ((count & 2) != 0)
				{
					if (BitConverter.IsLittleEndian)
					{
						num <<= 16;
						num |= Unsafe.ReadUnaligned<ushort>(in data);
					}
					else
					{
						num |= Unsafe.ReadUnaligned<ushort>(in data);
						num = RotateLeft(num, 16);
					}
				}
				goto IL_00bd;
			}
		}
		else
		{
			uint num2 = count / 8;
			do
			{
				p0 += Unsafe.ReadUnaligned<uint>(in data);
				uint num3 = Unsafe.ReadUnaligned<uint>(in Unsafe.AddByteOffset(ref data, (nint)4));
				Block(ref p0, ref p1);
				p0 += num3;
				Block(ref p0, ref p1);
				data = ref Unsafe.AddByteOffset(ref data, (nint)8);
			}
			while (--num2 != 0);
			if ((count & 4) == 0)
			{
				goto IL_006a;
			}
		}
		p0 += Unsafe.ReadUnaligned<uint>(in data);
		Block(ref p0, ref p1);
		goto IL_006a;
		IL_00bd:
		p0 += num;
		Block(ref p0, ref p1);
		Block(ref p0, ref p1);
		return (int)(p1 ^ p0);
		IL_006a:
		num = Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref Unsafe.AddByteOffset(ref data, (nuint)count & (nuint)7u), -4));
		count = ~count << 3;
		if (BitConverter.IsLittleEndian)
		{
			num >>= 8;
			num |= 0x80000000u;
			num >>= (int)(count & 0x1F);
		}
		else
		{
			num <<= 8;
			num |= 0x80;
			num <<= (int)(count & 0x1F);
		}
		goto IL_00bd;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Block(ref uint rp0, ref uint rp1)
	{
		uint num = rp0;
		uint num2 = rp1;
		num2 ^= num;
		num = RotateLeft(num, 20);
		num += num2;
		num2 = RotateLeft(num2, 9);
		num2 ^= num;
		num = RotateLeft(num, 27);
		num += num2;
		num2 = RotateLeft(num2, 19);
		rp0 = num;
		rp1 = num2;
	}

	private unsafe static ulong GenerateSeed()
	{
		byte[] array = new byte[8];
		using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
		randomNumberGenerator.GetBytes(array);
		fixed (byte* ptr = array)
		{
			return *(ulong*)ptr;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint RotateLeft(uint value, int shift)
	{
		return (value << shift) | (value >> 32 - shift);
	}
}
