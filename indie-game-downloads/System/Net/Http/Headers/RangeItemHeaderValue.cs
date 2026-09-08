using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Net.Http.Headers;

/// <summary>Represents a byte range in a Range header value.</summary>
public class RangeItemHeaderValue : ICloneable
{
	private readonly long _from;

	private readonly long _to;

	/// <summary>Gets the position at which to start sending data.</summary>
	/// <returns>The position at which to start sending data.</returns>
	public long? From
	{
		get
		{
			if (_from < 0)
			{
				return null;
			}
			return _from;
		}
	}

	/// <summary>Gets the position at which to stop sending data.</summary>
	/// <returns>The position at which to stop sending data.</returns>
	public long? To
	{
		get
		{
			if (_to < 0)
			{
				return null;
			}
			return _to;
		}
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RangeItemHeaderValue" /> class.</summary>
	/// <param name="from">The position at which to start sending data.</param>
	/// <param name="to">The position at which to stop sending data.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="from" /> is greater than <paramref name="to" />  
	/// -or-  
	/// <paramref name="from" /> or <paramref name="to" /> is less than 0.</exception>
	public RangeItemHeaderValue(long? from, long? to)
	{
		if (!from.HasValue && !to.HasValue)
		{
			throw new ArgumentException(System.SR.net_http_headers_invalid_range);
		}
		if (from.HasValue)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(from.GetValueOrDefault(), "from");
		}
		if (to.HasValue)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(to.GetValueOrDefault(), "to");
		}
		if (from.HasValue && to.HasValue)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan(from.GetValueOrDefault(), to.GetValueOrDefault(), "from");
		}
		_from = from ?? (-1);
		_to = to ?? (-1);
	}

	internal RangeItemHeaderValue(RangeItemHeaderValue source)
	{
		_from = source._from;
		_to = source._to;
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.RangeItemHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		Span<char> span = stackalloc char[128];
		IFormatProvider invariantCulture;
		Span<char> span2;
		if (_from < 0)
		{
			invariantCulture = CultureInfo.InvariantCulture;
			IFormatProvider provider = invariantCulture;
			span2 = span;
			Span<char> initialBuffer = span2;
			DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(1, 1, invariantCulture, span2);
			handler.AppendLiteral("-");
			handler.AppendFormatted(_to);
			return string.Create(provider, initialBuffer, ref handler);
		}
		if (_to < 0)
		{
			invariantCulture = CultureInfo.InvariantCulture;
			IFormatProvider provider2 = invariantCulture;
			span2 = span;
			Span<char> initialBuffer2 = span2;
			DefaultInterpolatedStringHandler handler2 = new DefaultInterpolatedStringHandler(1, 1, invariantCulture, span2);
			handler2.AppendFormatted(_from);
			handler2.AppendLiteral("-");
			return string.Create(provider2, initialBuffer2, ref handler2);
		}
		invariantCulture = CultureInfo.InvariantCulture;
		IFormatProvider provider3 = invariantCulture;
		span2 = span;
		Span<char> initialBuffer3 = span2;
		DefaultInterpolatedStringHandler handler3 = new DefaultInterpolatedStringHandler(1, 2, invariantCulture, span2);
		handler3.AppendFormatted(_from);
		handler3.AppendLiteral("-");
		handler3.AppendFormatted(_to);
		return string.Create(provider3, initialBuffer3, ref handler3);
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.RangeItemHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is RangeItemHeaderValue rangeItemHeaderValue && _from == rangeItemHeaderValue._from)
		{
			return _to == rangeItemHeaderValue._to;
		}
		return false;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.RangeItemHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return HashCode.Combine(_from, _to);
	}

	internal static int GetRangeItemListLength(string input, int startIndex, ICollection<RangeItemHeaderValue> rangeCollection)
	{
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int nextNonEmptyOrWhitespaceIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex, skipEmptyValues: true, out var _);
		if (nextNonEmptyOrWhitespaceIndex == input.Length)
		{
			return 0;
		}
		do
		{
			int rangeItemLength = GetRangeItemLength(input, nextNonEmptyOrWhitespaceIndex, out var parsedValue);
			if (rangeItemLength == 0)
			{
				return 0;
			}
			rangeCollection.Add(parsedValue);
			nextNonEmptyOrWhitespaceIndex += rangeItemLength;
			nextNonEmptyOrWhitespaceIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, nextNonEmptyOrWhitespaceIndex, skipEmptyValues: true, out var separatorFound2);
			if (nextNonEmptyOrWhitespaceIndex < input.Length && !separatorFound2)
			{
				return 0;
			}
		}
		while (nextNonEmptyOrWhitespaceIndex != input.Length);
		return nextNonEmptyOrWhitespaceIndex - startIndex;
	}

	internal static int GetRangeItemLength(string input, int startIndex, out RangeItemHeaderValue parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int num = startIndex;
		int offset = num;
		int numberLength = HttpRuleParser.GetNumberLength(input, num, allowDecimal: false);
		if (numberLength > 19)
		{
			return 0;
		}
		num += numberLength;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		if (num == input.Length || input[num] != '-')
		{
			return 0;
		}
		num++;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		int offset2 = num;
		int num2 = 0;
		if (num < input.Length)
		{
			num2 = HttpRuleParser.GetNumberLength(input, num, allowDecimal: false);
			if (num2 > 19)
			{
				return 0;
			}
			num += num2;
			num += HttpRuleParser.GetWhitespaceLength(input, num);
		}
		if (numberLength == 0 && num2 == 0)
		{
			return 0;
		}
		long result = 0L;
		if (numberLength > 0 && !HeaderUtilities.TryParseInt64(input, offset, numberLength, out result))
		{
			return 0;
		}
		long result2 = 0L;
		if (num2 > 0 && !HeaderUtilities.TryParseInt64(input, offset2, num2, out result2))
		{
			return 0;
		}
		if (numberLength > 0 && num2 > 0 && result > result2)
		{
			return 0;
		}
		parsedValue = new RangeItemHeaderValue((numberLength == 0) ? ((long?)null) : new long?(result), (num2 == 0) ? ((long?)null) : new long?(result2));
		return num - startIndex;
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.RangeItemHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new RangeItemHeaderValue(this);
	}
}
