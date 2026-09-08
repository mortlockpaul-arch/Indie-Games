using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

internal interface IBigIntegerHexOrBinaryParser<TParser, TChar> where TParser : struct, IBigIntegerHexOrBinaryParser<TParser, TChar> where TChar : unmanaged, IBinaryInteger<TChar>
{
	static abstract int BitsPerDigit { get; }

	static virtual int DigitsPerBlock => 32 / BitsPerDigit;

	static abstract NumberStyles BlockNumberStyle { get; }

	static abstract uint GetSignBitsIfValid(uint ch);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static virtual bool TryParseUnalignedBlock(ReadOnlySpan<TChar> input, out uint result)
	{
		if (typeof(TChar) == typeof(char))
		{
			return uint.TryParse(MemoryMarshal.Cast<TChar, char>(input), BlockNumberStyle, null, out result);
		}
		throw new NotSupportedException();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static virtual bool TryParseSingleBlock(ReadOnlySpan<TChar> input, out uint result)
	{
		return TryParseUnalignedBlock(input, out result);
	}

	static virtual bool TryParseWholeBlocks(ReadOnlySpan<TChar> input, Span<uint> destination)
	{
		ref TChar source = ref Unsafe.Add(ref MemoryMarshal.GetReference(input), input.Length - DigitsPerBlock);
		for (int i = 0; i < destination.Length; i++)
		{
			if (!TryParseSingleBlock(MemoryMarshal.CreateReadOnlySpan(in Unsafe.Subtract(ref source, i * DigitsPerBlock), DigitsPerBlock), out destination[i]))
			{
				return false;
			}
		}
		return true;
	}
}
