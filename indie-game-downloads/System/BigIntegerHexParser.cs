using System.Buffers;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal readonly struct BigIntegerHexParser<TChar> : IBigIntegerHexOrBinaryParser<BigIntegerHexParser<TChar>, TChar> where TChar : unmanaged, IBinaryInteger<TChar>
{
	public static int BitsPerDigit => 4;

	public static NumberStyles BlockNumberStyle => NumberStyles.AllowHexSpecifier;

	public static uint GetSignBitsIfValid(uint ch)
	{
		if ((ch & 0xF8) != 48)
		{
			return uint.MaxValue;
		}
		return 0u;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseWholeBlocks(ReadOnlySpan<TChar> input, Span<uint> destination)
	{
		if (typeof(TChar) == typeof(char))
		{
			if (Convert.FromHexString(MemoryMarshal.Cast<TChar, char>(input), MemoryMarshal.AsBytes(destination), out var _, out var _) != OperationStatus.Done)
			{
				return false;
			}
			if (BitConverter.IsLittleEndian)
			{
				MemoryMarshal.AsBytes(destination).Reverse();
			}
			else
			{
				destination.Reverse();
			}
			return true;
		}
		throw new NotSupportedException();
	}
}
