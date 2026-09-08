using System.Diagnostics.CodeAnalysis;

namespace System.Net.Http.Headers;

/// <summary>Represents an entity-tag header value.</summary>
public class EntityTagHeaderValue : ICloneable
{
	/// <summary>Gets the opaque quoted string.</summary>
	/// <returns>An opaque quoted string.</returns>
	public string Tag { get; private init; }

	/// <summary>Gets whether the entity-tag is prefaced by a weakness indicator.</summary>
	/// <returns>
	///   <see langword="true" /> if the entity-tag is prefaced by a weakness indicator; otherwise, <see langword="false" />.</returns>
	public bool IsWeak { get; private init; }

	/// <summary>Gets the entity-tag header value.</summary>
	/// <returns>Returns <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" />.</returns>
	public static EntityTagHeaderValue Any { get; } = new EntityTagHeaderValue("*", isWeak: false, _: false);

	private EntityTagHeaderValue(string tag, bool isWeak, bool _)
	{
		Tag = tag;
		IsWeak = isWeak;
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> class.</summary>
	/// <param name="tag">A string that contains an <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" />.</param>
	public EntityTagHeaderValue(string tag)
		: this(tag, isWeak: false)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> class.</summary>
	/// <param name="tag">A string that contains an  <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" />.</param>
	/// <param name="isWeak">A value that indicates if this entity-tag header is a weak validator. If the entity-tag header is weak validator, then <paramref name="isWeak" /> should be set to <see langword="true" />. If the entity-tag header is a strong validator, then <paramref name="isWeak" /> should be set to <see langword="false" />.</param>
	public EntityTagHeaderValue(string tag, bool isWeak)
	{
		HeaderUtilities.CheckValidQuotedString(tag, "tag");
		Tag = tag;
		IsWeak = isWeak;
	}

	private EntityTagHeaderValue(EntityTagHeaderValue source)
	{
		Tag = source.Tag;
		IsWeak = source.IsWeak;
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		if (!IsWeak)
		{
			return Tag;
		}
		return "W/" + Tag;
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is EntityTagHeaderValue entityTagHeaderValue && IsWeak == entityTagHeaderValue.IsWeak)
		{
			return string.Equals(Tag, entityTagHeaderValue.Tag, StringComparison.Ordinal);
		}
		return false;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return HashCode.Combine(Tag, IsWeak);
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents entity tag header value information.</param>
	/// <returns>An <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid entity tag header value information.</exception>
	public static EntityTagHeaderValue Parse(string input)
	{
		int index = 0;
		return (EntityTagHeaderValue)GenericHeaderParser.SingleValueEntityTagParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out EntityTagHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (GenericHeaderParser.SingleValueEntityTagParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (EntityTagHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetEntityTagLength(string input, int startIndex, out EntityTagHeaderValue parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		bool isWeak = false;
		int num = startIndex;
		char c = input[startIndex];
		if (c == '*')
		{
			parsedValue = Any;
			num++;
		}
		else
		{
			if (c == 'W' || c == 'w')
			{
				num++;
				if (num + 2 >= input.Length || input[num] != '/')
				{
					return 0;
				}
				isWeak = true;
				num++;
				num += HttpRuleParser.GetWhitespaceLength(input, num);
			}
			if (num == input.Length || HttpRuleParser.GetQuotedStringLength(input, num, out var length) != HttpParseResult.Parsed)
			{
				return 0;
			}
			parsedValue = new EntityTagHeaderValue(input.Substring(num, length), isWeak, _: false);
			num += length;
		}
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		return num - startIndex;
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		if (this != Any)
		{
			return new EntityTagHeaderValue(this);
		}
		return Any;
	}
}
