using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Net.Http.Headers;

/// <summary>Represents a Retry-After header value which can either be a date/time or a timespan value.</summary>
public class RetryConditionHeaderValue : ICloneable
{
	private readonly DateTimeOffset _date;

	private readonly TimeSpan _delta;

	/// <summary>Gets the date and time offset from the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
	/// <returns>The date and time offset from the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</returns>
	public DateTimeOffset? Date
	{
		get
		{
			if (_delta.Ticks != long.MaxValue)
			{
				return null;
			}
			return _date;
		}
	}

	/// <summary>Gets the delta in seconds from the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
	/// <returns>The delta in seconds from the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</returns>
	public TimeSpan? Delta
	{
		get
		{
			if (_delta.Ticks != long.MaxValue)
			{
				return _delta;
			}
			return null;
		}
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> class.</summary>
	/// <param name="date">The date and time offset used to initialize the new instance.</param>
	public RetryConditionHeaderValue(DateTimeOffset date)
	{
		_date = date;
		_delta = new TimeSpan(long.MaxValue);
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> class.</summary>
	/// <param name="delta">The delta, in seconds, used to initialize the new instance.</param>
	public RetryConditionHeaderValue(TimeSpan delta)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(delta.TotalSeconds, 2147483647.0, "delta.TotalSeconds");
		_delta = delta;
	}

	private RetryConditionHeaderValue(RetryConditionHeaderValue source)
	{
		_delta = source._delta;
		_date = source._date;
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		if (_delta.Ticks == long.MaxValue)
		{
			return _date.ToString("r");
		}
		return ((int)_delta.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is RetryConditionHeaderValue retryConditionHeaderValue && _delta == retryConditionHeaderValue._delta)
		{
			return _date == retryConditionHeaderValue._date;
		}
		return false;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return HashCode.Combine(_delta, _date);
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents retry condition header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid retry condition header value information.</exception>
	public static RetryConditionHeaderValue Parse(string input)
	{
		int index = 0;
		return (RetryConditionHeaderValue)GenericHeaderParser.RetryConditionParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out RetryConditionHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (GenericHeaderParser.RetryConditionParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (RetryConditionHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetRetryConditionLength(string input, int startIndex, out object parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int num = startIndex;
		DateTimeOffset result = DateTimeOffset.MinValue;
		int result2 = -1;
		if (char.IsAsciiDigit(input[num]))
		{
			int offset = num;
			int numberLength = HttpRuleParser.GetNumberLength(input, num, allowDecimal: false);
			if (numberLength == 0 || numberLength > 10)
			{
				return 0;
			}
			num += numberLength;
			num += HttpRuleParser.GetWhitespaceLength(input, num);
			if (num != input.Length)
			{
				return 0;
			}
			if (!HeaderUtilities.TryParseInt32(input, offset, numberLength, out result2))
			{
				return 0;
			}
		}
		else
		{
			if (!HttpDateParser.TryParse(input.AsSpan(num), out result))
			{
				return 0;
			}
			num = input.Length;
		}
		if (result2 == -1)
		{
			parsedValue = new RetryConditionHeaderValue(result);
		}
		else
		{
			parsedValue = new RetryConditionHeaderValue(new TimeSpan(0, 0, result2));
		}
		return num - startIndex;
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new RetryConditionHeaderValue(this);
	}
}
