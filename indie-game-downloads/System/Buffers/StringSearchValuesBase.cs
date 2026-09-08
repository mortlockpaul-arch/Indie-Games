using System.Collections.Generic;

namespace System.Buffers;

internal abstract class StringSearchValuesBase : SearchValues<string>
{
	private readonly HashSet<string> _uniqueValues;

	protected bool HasUniqueValues => _uniqueValues != null;

	public StringSearchValuesBase(HashSet<string> uniqueValues)
	{
		_uniqueValues = uniqueValues;
	}

	internal override bool ContainsCore(string value)
	{
		return _uniqueValues.Contains(value);
	}

	internal override string[] GetValues()
	{
		string[] array = new string[_uniqueValues.Count];
		_uniqueValues.CopyTo(array);
		return array;
	}

	internal sealed override int IndexOfAny(ReadOnlySpan<string> span)
	{
		return IndexOfAny<IndexOfAnyAsciiSearcher.DontNegate>(span);
	}

	internal sealed override int IndexOfAnyExcept(ReadOnlySpan<string> span)
	{
		return IndexOfAny<IndexOfAnyAsciiSearcher.Negate>(span);
	}

	internal sealed override int LastIndexOfAny(ReadOnlySpan<string> span)
	{
		return LastIndexOfAny<IndexOfAnyAsciiSearcher.DontNegate>(span);
	}

	internal sealed override int LastIndexOfAnyExcept(ReadOnlySpan<string> span)
	{
		return LastIndexOfAny<IndexOfAnyAsciiSearcher.Negate>(span);
	}

	private int IndexOfAny<TNegator>(ReadOnlySpan<string> span) where TNegator : struct, IndexOfAnyAsciiSearcher.INegator
	{
		for (int i = 0; i < span.Length; i++)
		{
			if (TNegator.NegateIfNeeded(ContainsCore(span[i])))
			{
				return i;
			}
		}
		return -1;
	}

	private int LastIndexOfAny<TNegator>(ReadOnlySpan<string> span) where TNegator : struct, IndexOfAnyAsciiSearcher.INegator
	{
		for (int num = span.Length - 1; num >= 0; num--)
		{
			if (TNegator.NegateIfNeeded(ContainsCore(span[num])))
			{
				return num;
			}
		}
		return -1;
	}
}
