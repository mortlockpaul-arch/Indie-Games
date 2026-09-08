using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Buffers;

internal abstract class StringSearchValuesRabinKarp<TCaseSensitivity> : StringSearchValuesBase where TCaseSensitivity : struct, StringSearchValuesHelper.ICaseSensitivity
{
	private readonly RabinKarp _rabinKarp;

	public StringSearchValuesRabinKarp(ReadOnlySpan<string> values, HashSet<string> uniqueValues)
		: base(uniqueValues)
	{
		_rabinKarp = new RabinKarp(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected int ShortInputFallback(ReadOnlySpan<char> span)
	{
		return _rabinKarp.IndexOfAny<TCaseSensitivity>(span);
	}
}
