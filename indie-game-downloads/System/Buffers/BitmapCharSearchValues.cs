using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers;

internal sealed class BitmapCharSearchValues : SearchValues<char>
{
	private readonly uint[] _bitmap;

	public BitmapCharSearchValues(ReadOnlySpan<char> values, int maxInclusive)
	{
		_bitmap = new uint[maxInclusive / 32 + 1];
		ReadOnlySpan<char> readOnlySpan = values;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = readOnlySpan[i];
			_bitmap[(int)c >> 5] |= (uint)(1 << (int)c);
		}
	}

	internal override char[] GetValues()
	{
		List<char> list = new List<char>();
		uint[] bitmap = _bitmap;
		for (int i = 0; i < _bitmap.Length * 32; i++)
		{
			if (Contains(bitmap, i))
			{
				list.Add((char)i);
			}
		}
		return list.ToArray();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override bool ContainsCore(char value)
	{
		return Contains(_bitmap, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool Contains(uint[] bitmap, int value)
	{
		uint num = (uint)(value >> 5);
		if (num < (uint)bitmap.Length)
		{
			return (bitmap[num] & (uint)(1 << value)) != 0;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int IndexOfAny(ReadOnlySpan<char> span)
	{
		return IndexOfAny<IndexOfAnyAsciiSearcher.DontNegate>(ref MemoryMarshal.GetReference(span), span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int IndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		return IndexOfAny<IndexOfAnyAsciiSearcher.Negate>(ref MemoryMarshal.GetReference(span), span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int LastIndexOfAny(ReadOnlySpan<char> span)
	{
		return LastIndexOfAny<IndexOfAnyAsciiSearcher.DontNegate>(ref MemoryMarshal.GetReference(span), span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int LastIndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		return LastIndexOfAny<IndexOfAnyAsciiSearcher.Negate>(ref MemoryMarshal.GetReference(span), span.Length);
	}

	private int IndexOfAny<TNegator>(ref char searchSpace, int searchSpaceLength) where TNegator : struct, IndexOfAnyAsciiSearcher.INegator
	{
		ref char right = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
		ref char reference = ref searchSpace;
		uint[] bitmap = _bitmap;
		while (!Unsafe.AreSame(in reference, in right))
		{
			char value = reference;
			if (TNegator.NegateIfNeeded(Contains(bitmap, value)))
			{
				return (int)((nuint)Unsafe.ByteOffset(in searchSpace, in reference) / (nuint)2u);
			}
			reference = ref Unsafe.Add(ref reference, 1);
		}
		return -1;
	}

	private int LastIndexOfAny<TNegator>(ref char searchSpace, int searchSpaceLength) where TNegator : struct, IndexOfAnyAsciiSearcher.INegator
	{
		uint[] bitmap = _bitmap;
		while (--searchSpaceLength >= 0)
		{
			char value = Unsafe.Add(ref searchSpace, searchSpaceLength);
			if (TNegator.NegateIfNeeded(Contains(bitmap, value)))
			{
				break;
			}
		}
		return searchSpaceLength;
	}
}
