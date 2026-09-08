using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Buffers;

internal sealed class SingleStringSearchValuesFallback<TIgnoreCase> : StringSearchValuesBase where TIgnoreCase : struct, SearchValues.IRuntimeConst
{
	private readonly string _value;

	public SingleStringSearchValuesFallback(string value, HashSet<string> uniqueValues)
		: base(uniqueValues)
	{
		_value = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int IndexOfAnyMultiString(ReadOnlySpan<char> span)
	{
		if (!TIgnoreCase.Value)
		{
			return span.IndexOf(_value.AsSpan());
		}
		return Ordinal.IndexOfOrdinalIgnoreCase(span, _value.AsSpan());
	}
}
