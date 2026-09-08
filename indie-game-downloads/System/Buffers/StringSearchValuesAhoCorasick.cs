using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Buffers;

internal sealed class StringSearchValuesAhoCorasick<TCaseSensitivity, TFastScanVariant> : StringSearchValuesBase where TCaseSensitivity : struct, StringSearchValuesHelper.ICaseSensitivity where TFastScanVariant : struct, AhoCorasick.IFastScan
{
	private readonly AhoCorasick _ahoCorasick;

	public StringSearchValuesAhoCorasick(AhoCorasick ahoCorasick, HashSet<string> uniqueValues)
		: base(uniqueValues)
	{
		_ahoCorasick = ahoCorasick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int IndexOfAnyMultiString(ReadOnlySpan<char> span)
	{
		return _ahoCorasick.IndexOfAny<TCaseSensitivity, TFastScanVariant>(span);
	}
}
