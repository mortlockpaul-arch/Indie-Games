using System.Diagnostics.CodeAnalysis;

namespace System.Net.Http.Headers;

/// <summary>Represents an If-Range header value which can either be a date/time or an entity-tag value.</summary>
public class RangeConditionHeaderValue : ICloneable
{
	private readonly DateTimeOffset _date;

	private readonly EntityTagHeaderValue _entityTag;

	/// <summary>Gets the date from the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
	/// <returns>The date from the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</returns>
	public DateTimeOffset? Date
	{
		get
		{
			if (_entityTag != null)
			{
				return null;
			}
			return _date;
		}
	}

	/// <summary>Gets the entity tag from the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
	/// <returns>The entity tag from the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</returns>
	public EntityTagHeaderValue? EntityTag => _entityTag;

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> class.</summary>
	/// <param name="date">A date value used to initialize the new instance.</param>
	public RangeConditionHeaderValue(DateTimeOffset date)
	{
		_date = date;
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> class.</summary>
	/// <param name="entityTag">An <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> object used to initialize the new instance.</param>
	public RangeConditionHeaderValue(EntityTagHeaderValue entityTag)
	{
		ArgumentNullException.ThrowIfNull(entityTag, "entityTag");
		_entityTag = entityTag;
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> class.</summary>
	/// <param name="entityTag">An entity tag represented as a string used to initialize the new instance.</param>
	public RangeConditionHeaderValue(string entityTag)
		: this(new EntityTagHeaderValue(entityTag))
	{
	}

	private RangeConditionHeaderValue(RangeConditionHeaderValue source)
	{
		_entityTag = source._entityTag;
		_date = source._date;
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		return _entityTag?.ToString() ?? _date.ToString("r");
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is RangeConditionHeaderValue rangeConditionHeaderValue && ((_entityTag == null) ? (rangeConditionHeaderValue._entityTag == null) : _entityTag.Equals(rangeConditionHeaderValue._entityTag)))
		{
			return _date == rangeConditionHeaderValue._date;
		}
		return false;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return _entityTag?.GetHashCode() ?? _date.GetHashCode();
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents range condition header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid range Condition header value information.</exception>
	public static RangeConditionHeaderValue Parse(string input)
	{
		int index = 0;
		return (RangeConditionHeaderValue)GenericHeaderParser.RangeConditionParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out RangeConditionHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (GenericHeaderParser.RangeConditionParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (RangeConditionHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetRangeConditionLength(string input, int startIndex, out object parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex + 1 >= input.Length)
		{
			return 0;
		}
		int num = startIndex;
		DateTimeOffset result = DateTimeOffset.MinValue;
		EntityTagHeaderValue parsedValue2 = null;
		char c = input[num];
		char c2 = input[num + 1];
		if (c == '"' || ((c == 'w' || c == 'W') && c2 == '/'))
		{
			int entityTagLength = EntityTagHeaderValue.GetEntityTagLength(input, num, out parsedValue2);
			if (entityTagLength == 0)
			{
				return 0;
			}
			num += entityTagLength;
			if (num != input.Length)
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
		if (parsedValue2 == null)
		{
			parsedValue = new RangeConditionHeaderValue(result);
		}
		else
		{
			parsedValue = new RangeConditionHeaderValue(parsedValue2);
		}
		return num - startIndex;
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new RangeConditionHeaderValue(this);
	}
}
