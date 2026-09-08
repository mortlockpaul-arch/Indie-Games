using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;

namespace System;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal readonly struct BigIntegerBinaryParser<TChar> : IBigIntegerHexOrBinaryParser<BigIntegerBinaryParser<TChar>, TChar> where TChar : unmanaged, IBinaryInteger<TChar>
{
	public static int BitsPerDigit => 1;

	public static NumberStyles BlockNumberStyle => NumberStyles.AllowBinarySpecifier;

	public static uint GetSignBitsIfValid(uint ch)
	{
		return (uint)((int)(ch << 31) >> 31);
	}
}
