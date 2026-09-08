using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Net.Http.Headers;

/// <summary>Represents the value of the Content-Range header.</summary>
public class ContentRangeHeaderValue : ICloneable
{
	private string _unit;

	private long _from;

	private long _to;

	private long _length;

	/// <summary>The range units used.</summary>
	/// <returns>A <see cref="T:System.String" /> that contains range units.</returns>
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

	/// <summary>Gets the position at which to start sending data.</summary>
	/// <returns>The position, in bytes, at which to start sending data.</returns>
	public long? From
	{
		get
		{
			if (!HasRange)
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
			if (!HasRange)
			{
				return null;
			}
			return _to;
		}
	}

	/// <summary>Gets the length of the full entity-body.</summary>
	/// <returns>The length of the full entity-body.</returns>
	public long? Length
	{
		get
		{
			if (!HasLength)
			{
				return null;
			}
			return _length;
		}
	}

	/// <summary>Gets whether the Content-Range header has a length specified.</summary>
	/// <returns>
	///   <see langword="true" /> if the Content-Range has a length specified; otherwise, <see langword="false" />.</returns>
	public bool HasLength => _length >= 0;

	/// <summary>Gets whether the Content-Range has a range specified.</summary>
	/// <returns>
	///   <see langword="true" /> if the Content-Range has a range specified; otherwise, <see langword="false" />.</returns>
	public bool HasRange => _from >= 0;

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> class.</summary>
	/// <param name="from">The position, in bytes, at which to start sending data.</param>
	/// <param name="to">The position, in bytes, at which to stop sending data.</param>
	/// <param name="length">The starting or ending point of the range, in bytes.</param>
	public ContentRangeHeaderValue(long from, long to, long length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length, "length");
		ArgumentOutOfRangeException.ThrowIfNegative(to, "to");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(to, length, "to");
		ArgumentOutOfRangeException.ThrowIfNegative(from, "from");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(from, to, "from");
		_from = from;
		_to = to;
		_length = length;
		_unit = "bytes";
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> class.</summary>
	/// <param name="length">The starting or ending point of the range, in bytes.</param>
	public ContentRangeHeaderValue(long length)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length, "length");
		_length = length;
		_unit = "bytes";
		_from = -1L;
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> class.</summary>
	/// <param name="from">The position, in bytes, at which to start sending data.</param>
	/// <param name="to">The position, in bytes, at which to stop sending data.</param>
	public ContentRangeHeaderValue(long from, long to)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(to, "to");
		ArgumentOutOfRangeException.ThrowIfNegative(from, "from");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(from, to, "from");
		_from = from;
		_to = to;
		_unit = "bytes";
		_length = -1L;
	}

	private ContentRangeHeaderValue()
	{
		_from = -1L;
		_length = -1L;
	}

	private ContentRangeHeaderValue(ContentRangeHeaderValue source)
	{
		_from = source._from;
		_to = source._to;
		_length = source._length;
		_unit = source._unit;
	}

	/// <summary>Determines whether the specified Object is equal to the current <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is ContentRangeHeaderValue contentRangeHeaderValue && _from == contentRangeHeaderValue._from && _to == contentRangeHeaderValue._to && _length == contentRangeHeaderValue._length)
		{
			return string.Equals(_unit, contentRangeHeaderValue._unit, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return HashCode.Combine(StringComparer.OrdinalIgnoreCase.GetHashCode(_unit), _from, _to, _length);
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		Span<char> initialBuffer = stackalloc char[256];
		System.Text.ValueStringBuilder valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
		valueStringBuilder.Append(_unit);
		valueStringBuilder.Append(' ');
		if (HasRange)
		{
			valueStringBuilder.AppendSpanFormattable(_from);
			valueStringBuilder.Append('-');
			valueStringBuilder.AppendSpanFormattable(_to);
		}
		else
		{
			valueStringBuilder.Append('*');
		}
		valueStringBuilder.Append('/');
		if (HasLength)
		{
			valueStringBuilder.AppendSpanFormattable(_length);
		}
		else
		{
			valueStringBuilder.Append('*');
		}
		return valueStringBuilder.ToString();
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents content range header value information.</param>
	/// <returns>An <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid content range header value information.</exception>
	public static ContentRangeHeaderValue Parse(string input)
	{
		int index = 0;
		return (ContentRangeHeaderValue)GenericHeaderParser.ContentRangeParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out ContentRangeHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (GenericHeaderParser.ContentRangeParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (ContentRangeHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetContentRangeLength(string input, int startIndex, out object parsedValue)
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
		string unit = input.Substring(startIndex, tokenLength);
		int num = startIndex + tokenLength;
		int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, num);
		if (whitespaceLength == 0)
		{
			return 0;
		}
		num += whitespaceLength;
		if (num == input.Length)
		{
			return 0;
		}
		int fromStartIndex = num;
		if (!TryGetRangeLength(input, ref num, out var fromLength, out var toStartIndex, out var toLength))
		{
			return 0;
		}
		if (num == input.Length || input[num] != '/')
		{
			return 0;
		}
		num++;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		if (num == input.Length)
		{
			return 0;
		}
		int lengthStartIndex = num;
		if (!TryGetLengthLength(input, ref num, out var lengthLength))
		{
			return 0;
		}
		if (!TryCreateContentRange(input, unit, fromStartIndex, fromLength, toStartIndex, toLength, lengthStartIndex, lengthLength, out parsedValue))
		{
			return 0;
		}
		return num - startIndex;
	}

	private static bool TryGetLengthLength(string input, ref int current, out int lengthLength)
	{
		lengthLength = 0;
		if (input[current] == '*')
		{
			current++;
		}
		else
		{
			lengthLength = HttpRuleParser.GetNumberLength(input, current, allowDecimal: false);
			if (lengthLength == 0 || lengthLength > 19)
			{
				return false;
			}
			current += lengthLength;
		}
		current += HttpRuleParser.GetWhitespaceLength(input, current);
		return true;
	}

	private static bool TryGetRangeLength(string input, ref int current, out int fromLength, out int toStartIndex, out int toLength)
	{
		fromLength = 0;
		toStartIndex = 0;
		toLength = 0;
		if (input[current] == '*')
		{
			current++;
		}
		else
		{
			fromLength = HttpRuleParser.GetNumberLength(input, current, allowDecimal: false);
			if (fromLength == 0 || fromLength > 19)
			{
				return false;
			}
			current += fromLength;
			current += HttpRuleParser.GetWhitespaceLength(input, current);
			if (current == input.Length || input[current] != '-')
			{
				return false;
			}
			current++;
			current += HttpRuleParser.GetWhitespaceLength(input, current);
			if (current == input.Length)
			{
				return false;
			}
			toStartIndex = current;
			toLength = HttpRuleParser.GetNumberLength(input, current, allowDecimal: false);
			if (toLength == 0 || toLength > 19)
			{
				return false;
			}
			current += toLength;
		}
		current += HttpRuleParser.GetWhitespaceLength(input, current);
		return true;
	}

	private static bool TryCreateContentRange(string input, string unit, int fromStartIndex, int fromLength, int toStartIndex, int toLength, int lengthStartIndex, int lengthLength, [NotNullWhen(true)] out object parsedValue)
	{
		parsedValue = null;
		long result = 0L;
		if (fromLength > 0 && !HeaderUtilities.TryParseInt64(input, fromStartIndex, fromLength, out result))
		{
			return false;
		}
		long result2 = 0L;
		if (toLength > 0 && !HeaderUtilities.TryParseInt64(input, toStartIndex, toLength, out result2))
		{
			return false;
		}
		if (fromLength > 0 && toLength > 0 && result > result2)
		{
			return false;
		}
		long result3 = 0L;
		if (lengthLength > 0 && !HeaderUtilities.TryParseInt64(input, lengthStartIndex, lengthLength, out result3))
		{
			return false;
		}
		if (toLength > 0 && lengthLength > 0 && result2 >= result3)
		{
			return false;
		}
		ContentRangeHeaderValue contentRangeHeaderValue = new ContentRangeHeaderValue();
		contentRangeHeaderValue._unit = unit;
		if (fromLength > 0)
		{
			contentRangeHeaderValue._from = result;
			contentRangeHeaderValue._to = result2;
		}
		if (lengthLength > 0)
		{
			contentRangeHeaderValue._length = result3;
		}
		parsedValue = contentRangeHeaderValue;
		return true;
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new ContentRangeHeaderValue(this);
	}
}
