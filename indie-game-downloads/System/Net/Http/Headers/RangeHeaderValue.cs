using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Net.Http.Headers;

/// <summary>Represents a Range header value.</summary>
public class RangeHeaderValue : ICloneable
{
	private string _unit;

	private UnvalidatedObjectCollection<RangeItemHeaderValue> _ranges;

	/// <summary>Gets the unit from the <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> object.</summary>
	/// <returns>The unit from the <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> object.</returns>
	public string Unit
	{
		get
		{
			return _unit;
		}
		set
		{
			HeaderUtilities.CheckValidToken(value, "value");
			_unit = value;
		}
	}

	/// <summary>Gets the ranges specified from the <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> object.</summary>
	/// <returns>The ranges from the <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> object.</returns>
	public ICollection<RangeItemHeaderValue> Ranges => _ranges ?? (_ranges = new UnvalidatedObjectCollection<RangeItemHeaderValue>());

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> class.</summary>
	public RangeHeaderValue()
	{
		_unit = "bytes";
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> class with a byte range.</summary>
	/// <param name="from">The position at which to start sending data.</param>
	/// <param name="to">The position at which to stop sending data.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="from" /> is greater than <paramref name="to" />  
	/// -or-  
	/// <paramref name="from" /> or <paramref name="to" /> is less than 0.</exception>
	public RangeHeaderValue(long? from, long? to)
	{
		_unit = "bytes";
		Ranges.Add(new RangeItemHeaderValue(from, to));
	}

	private RangeHeaderValue(RangeHeaderValue source)
	{
		_unit = source._unit;
		if (source._ranges == null)
		{
			return;
		}
		foreach (RangeItemHeaderValue range in source._ranges)
		{
			Ranges.Add(new RangeItemHeaderValue(range));
		}
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		Span<char> initialBuffer = stackalloc char[256];
		System.Text.ValueStringBuilder valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
		valueStringBuilder.Append(_unit);
		valueStringBuilder.Append('=');
		if (_ranges != null)
		{
			bool flag = true;
			foreach (RangeItemHeaderValue range in _ranges)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					valueStringBuilder.Append(", ");
				}
				if (range.From.HasValue)
				{
					valueStringBuilder.AppendSpanFormattable(range.From.GetValueOrDefault());
				}
				valueStringBuilder.Append('-');
				if (range.To.HasValue)
				{
					valueStringBuilder.AppendSpanFormattable(range.To.GetValueOrDefault());
				}
			}
		}
		return valueStringBuilder.ToString();
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (!(obj is RangeHeaderValue rangeHeaderValue))
		{
			return false;
		}
		if (string.Equals(_unit, rangeHeaderValue._unit, StringComparison.OrdinalIgnoreCase))
		{
			return HeaderUtilities.AreEqualCollections(_ranges, rangeHeaderValue._ranges);
		}
		return false;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		int num = StringComparer.OrdinalIgnoreCase.GetHashCode(_unit);
		if (_ranges != null)
		{
			foreach (RangeItemHeaderValue range in _ranges)
			{
				num ^= range.GetHashCode();
			}
		}
		return num;
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents range header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid range header value information.</exception>
	public static RangeHeaderValue Parse(string input)
	{
		int index = 0;
		return (RangeHeaderValue)GenericHeaderParser.RangeParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> information.</summary>
	/// <param name="input">he string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out RangeHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (GenericHeaderParser.RangeParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (RangeHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetRangeLength(string input, int startIndex, out object parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
		if (tokenLength == 0)
		{
			return 0;
		}
		RangeHeaderValue rangeHeaderValue = new RangeHeaderValue();
		rangeHeaderValue._unit = input.Substring(startIndex, tokenLength);
		int num = startIndex + tokenLength;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		if (num == input.Length || input[num] != '=')
		{
			return 0;
		}
		num++;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		int rangeItemListLength = RangeItemHeaderValue.GetRangeItemListLength(input, num, rangeHeaderValue.Ranges);
		if (rangeItemListLength == 0)
		{
			return 0;
		}
		num += rangeItemListLength;
		parsedValue = rangeHeaderValue;
		return num - startIndex;
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.RangeHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new RangeHeaderValue(this);
	}
}
