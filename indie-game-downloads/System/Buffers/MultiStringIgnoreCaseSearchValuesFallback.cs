using System.Collections.Generic;

namespace System.Buffers;

internal sealed class MultiStringIgnoreCaseSearchValuesFallback : StringSearchValuesBase
{
	private readonly string[] _values;

	public MultiStringIgnoreCaseSearchValuesFallback(HashSet<string> uniqueValues)
		: base(uniqueValues)
	{
		_values = new string[uniqueValues.Count];
		uniqueValues.CopyTo(_values, 0);
	}

	internal override int IndexOfAnyMultiString(ReadOnlySpan<char> span)
	{
		string[] values = _values;
		for (int i = 0; i < span.Length; i++)
		{
			ReadOnlySpan<char> span2 = span.Slice(i);
			string[] array = values;
			foreach (string text in array)
			{
				if (span2.StartsWith(text.AsSpan(), StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
		}
		return -1;
	}
}
