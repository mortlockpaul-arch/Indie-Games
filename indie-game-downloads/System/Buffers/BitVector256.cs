using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Buffers;

internal struct BitVector256
{
	private unsafe fixed uint _values[8];

	public unsafe readonly BitVector256 CreateInverse()
	{
		BitVector256 result = default(BitVector256);
		for (int i = 0; i < 8; i++)
		{
			result._values[i] = ~_values[i];
		}
		return result;
	}

	public unsafe void Set(int c)
	{
		uint num = (uint)(c >> 5);
		uint num2 = (uint)(1 << c);
		ref uint reference = ref _values[num];
		reference |= num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains256(char c)
	{
		if (c < 'Ā')
		{
			return ContainsUnchecked(c);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(byte b)
	{
		return ContainsUnchecked(b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe readonly bool ContainsUnchecked(int b)
	{
		uint num = (uint)(b >> 5);
		uint num2 = (uint)(1 << b);
		return (_values[num] & num2) != 0;
	}

	public readonly char[] GetCharValues()
	{
		List<char> list = new List<char>();
		for (int i = 0; i < 256; i++)
		{
			if (ContainsUnchecked(i))
			{
				list.Add((char)i);
			}
		}
		return list.ToArray();
	}

	public readonly byte[] GetByteValues()
	{
		List<byte> list = new List<byte>();
		for (int i = 0; i < 256; i++)
		{
			if (ContainsUnchecked(i))
			{
				list.Add((byte)i);
			}
		}
		return list.ToArray();
	}
}
