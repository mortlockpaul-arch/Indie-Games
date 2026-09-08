using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Net.Http.Headers;

/// <summary>Represents a name/value pair with parameters used in various headers as defined in RFC 2616.</summary>
public class NameValueWithParametersHeaderValue : NameValueHeaderValue, ICloneable
{
	private static readonly Func<NameValueHeaderValue> s_nameValueCreator = CreateNameValue;

	private UnvalidatedObjectCollection<NameValueHeaderValue> _parameters;

	/// <summary>Gets the parameters from the <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object.</summary>
	/// <returns>A collection containing the parameters.</returns>
	public ICollection<NameValueHeaderValue> Parameters => _parameters ?? (_parameters = new UnvalidatedObjectCollection<NameValueHeaderValue>());

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> class.</summary>
	/// <param name="name">The header name.</param>
	public NameValueWithParametersHeaderValue(string name)
		: base(name)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> class.</summary>
	/// <param name="name">The header name.</param>
	/// <param name="value">The header value.</param>
	public NameValueWithParametersHeaderValue(string name, string? value)
		: base(name, value)
	{
	}

	internal NameValueWithParametersHeaderValue()
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> class.</summary>
	/// <param name="source">A <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object used to initialize the new instance.</param>
	protected NameValueWithParametersHeaderValue(NameValueWithParametersHeaderValue source)
		: base(source)
	{
		_parameters = source._parameters.Clone();
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (base.Equals(obj))
		{
			if (!(obj is NameValueWithParametersHeaderValue nameValueWithParametersHeaderValue))
			{
				return false;
			}
			return HeaderUtilities.AreEqualCollections(_parameters, nameValueWithParametersHeaderValue._parameters);
		}
		return false;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return base.GetHashCode() ^ NameValueHeaderValue.GetHashCode(_parameters);
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		string value = base.ToString();
		StringBuilder stringBuilder = System.Text.StringBuilderCache.Acquire();
		stringBuilder.Append(value);
		NameValueHeaderValue.ToString(_parameters, ';', leadingSeparator: true, stringBuilder);
		return System.Text.StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents name value with parameter header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid name value with parameter header value information.</exception>
	public new static NameValueWithParametersHeaderValue Parse(string input)
	{
		int index = 0;
		return (NameValueWithParametersHeaderValue)GenericHeaderParser.SingleValueNameValueWithParametersParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out NameValueWithParametersHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (GenericHeaderParser.SingleValueNameValueWithParametersParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (NameValueWithParametersHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetNameValueWithParametersLength(string input, int startIndex, out object parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int nameValueLength = NameValueHeaderValue.GetNameValueLength(input, startIndex, s_nameValueCreator, out var parsedValue2);
		if (nameValueLength == 0)
		{
			return 0;
		}
		int num = startIndex + nameValueLength;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		NameValueWithParametersHeaderValue nameValueWithParametersHeaderValue = parsedValue2 as NameValueWithParametersHeaderValue;
		if (num < input.Length && input[num] == ';')
		{
			num++;
			int nameValueListLength = NameValueHeaderValue.GetNameValueListLength(input, num, ';', (UnvalidatedObjectCollection<NameValueHeaderValue>)nameValueWithParametersHeaderValue.Parameters);
			if (nameValueListLength == 0)
			{
				return 0;
			}
			parsedValue = nameValueWithParametersHeaderValue;
			return num + nameValueListLength - startIndex;
		}
		parsedValue = nameValueWithParametersHeaderValue;
		return num - startIndex;
	}

	private static NameValueHeaderValue CreateNameValue()
	{
		return new NameValueWithParametersHeaderValue();
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new NameValueWithParametersHeaderValue(this);
	}
}
