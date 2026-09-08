using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers;

/// <summary>Represents a name/value pair used in various headers as defined in RFC 2616.</summary>
public class NameValueHeaderValue : ICloneable
{
	private static readonly Func<NameValueHeaderValue> s_defaultNameValueCreator = CreateNameValue;

	private string _name;

	private string _value;

	/// <summary>Gets the header name.</summary>
	/// <returns>The header name.</returns>
	public string Name => _name;

	/// <summary>Gets the header value.</summary>
	/// <returns>The header value.</returns>
	public string? Value
	{
		get
		{
			return _value;
		}
		set
		{
			CheckValueFormat(value);
			_value = value;
		}
	}

	internal NameValueHeaderValue()
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> class.</summary>
	/// <param name="name">The header name.</param>
	public NameValueHeaderValue(string name)
		: this(name, null)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> class.</summary>
	/// <param name="name">The header name.</param>
	/// <param name="value">The header value.</param>
	public NameValueHeaderValue(string name, string? value)
	{
		CheckNameValueFormat(name, value);
		_name = name;
		_value = value;
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> class.</summary>
	/// <param name="source">A <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> object used to initialize the new instance.</param>
	protected internal NameValueHeaderValue(NameValueHeaderValue source)
	{
		_name = source._name;
		_value = source._value;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		int hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(_name);
		if (!string.IsNullOrEmpty(_value))
		{
			if (_value[0] == '"')
			{
				return hashCode ^ _value.GetHashCode();
			}
			return hashCode ^ StringComparer.OrdinalIgnoreCase.GetHashCode(_value);
		}
		return hashCode;
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (!(obj is NameValueHeaderValue nameValueHeaderValue))
		{
			return false;
		}
		if (!string.Equals(_name, nameValueHeaderValue._name, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		if (string.IsNullOrEmpty(_value))
		{
			return string.IsNullOrEmpty(nameValueHeaderValue._value);
		}
		if (_value[0] == '"')
		{
			return string.Equals(_value, nameValueHeaderValue._value, StringComparison.Ordinal);
		}
		return string.Equals(_value, nameValueHeaderValue._value, StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents name value header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid name value header value information.</exception>
	public static NameValueHeaderValue Parse(string input)
	{
		int index = 0;
		return (NameValueHeaderValue)GenericHeaderParser.SingleValueNameValueParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out NameValueHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (GenericHeaderParser.SingleValueNameValueParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (NameValueHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		if (!string.IsNullOrEmpty(_value))
		{
			return _name + "=" + _value;
		}
		return _name;
	}

	private void AddToStringBuilder(StringBuilder sb)
	{
		if (GetType() != typeof(NameValueHeaderValue))
		{
			sb.Append(ToString());
			return;
		}
		sb.Append(_name);
		if (!string.IsNullOrEmpty(_value))
		{
			sb.Append('=');
			sb.Append(_value);
		}
	}

	internal static void ToString(UnvalidatedObjectCollection<NameValueHeaderValue> values, char separator, bool leadingSeparator, StringBuilder destination)
	{
		if (values == null || values.Count == 0)
		{
			return;
		}
		foreach (NameValueHeaderValue value in values)
		{
			if (leadingSeparator || destination.Length > 0)
			{
				destination.Append(separator);
				destination.Append(' ');
			}
			value.AddToStringBuilder(destination);
		}
	}

	internal static int GetHashCode(UnvalidatedObjectCollection<NameValueHeaderValue> values)
	{
		if (values == null || values.Count == 0)
		{
			return 0;
		}
		int num = 0;
		foreach (NameValueHeaderValue value in values)
		{
			num ^= value.GetHashCode();
		}
		return num;
	}

	internal static int GetNameValueLength(string input, int startIndex, out NameValueHeaderValue parsedValue)
	{
		return GetNameValueLength(input, startIndex, s_defaultNameValueCreator, out parsedValue);
	}

	internal static int GetNameValueLength(string input, int startIndex, Func<NameValueHeaderValue> nameValueCreator, out NameValueHeaderValue parsedValue)
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
		string name = input.Substring(startIndex, tokenLength);
		int num = startIndex + tokenLength;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		if (num == input.Length || input[num] != '=')
		{
			parsedValue = nameValueCreator();
			parsedValue._name = name;
			num += HttpRuleParser.GetWhitespaceLength(input, num);
			return num - startIndex;
		}
		num++;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		int valueLength = GetValueLength(input, num);
		if (valueLength == 0)
		{
			return 0;
		}
		parsedValue = nameValueCreator();
		parsedValue._name = name;
		parsedValue._value = input.Substring(num, valueLength);
		num += valueLength;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		return num - startIndex;
	}

	internal static int GetNameValueListLength(string input, int startIndex, char delimiter, UnvalidatedObjectCollection<NameValueHeaderValue> nameValueCollection)
	{
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int num = startIndex + HttpRuleParser.GetWhitespaceLength(input, startIndex);
		while (true)
		{
			int nameValueLength = GetNameValueLength(input, num, s_defaultNameValueCreator, out var parsedValue);
			if (nameValueLength == 0)
			{
				return 0;
			}
			nameValueCollection.Add(parsedValue);
			num += nameValueLength;
			num += HttpRuleParser.GetWhitespaceLength(input, num);
			if (num == input.Length || input[num] != delimiter)
			{
				break;
			}
			num++;
			num += HttpRuleParser.GetWhitespaceLength(input, num);
		}
		return num - startIndex;
	}

	internal static NameValueHeaderValue Find(UnvalidatedObjectCollection<NameValueHeaderValue> values, string name)
	{
		if (values == null || values.Count == 0)
		{
			return null;
		}
		foreach (NameValueHeaderValue value in values)
		{
			if (string.Equals(value.Name, name, StringComparison.OrdinalIgnoreCase))
			{
				return value;
			}
		}
		return null;
	}

	internal static int GetValueLength(string input, int startIndex)
	{
		if (startIndex >= input.Length)
		{
			return 0;
		}
		int length = HttpRuleParser.GetTokenLength(input, startIndex);
		if (length == 0 && HttpRuleParser.GetQuotedStringLength(input, startIndex, out length) != HttpParseResult.Parsed)
		{
			return 0;
		}
		return length;
	}

	private static void CheckNameValueFormat(string name, string value)
	{
		HeaderUtilities.CheckValidToken(name, "name");
		CheckValueFormat(value);
	}

	private static void CheckValueFormat(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		if (value.StartsWith(' ') || value.StartsWith('\t') || value.EndsWith(' ') || value.EndsWith('\t'))
		{
			ThrowFormatException(value);
		}
		if (value[0] == '"')
		{
			if (HttpRuleParser.GetQuotedStringLength(value, 0, out var length) != HttpParseResult.Parsed || length != value.Length)
			{
				ThrowFormatException(value);
			}
		}
		else if (HttpRuleParser.ContainsNewLineOrNull(value))
		{
			ThrowFormatException(value);
		}
		static void ThrowFormatException(string p)
		{
			throw new FormatException(System.SR.Format(CultureInfo.InvariantCulture, System.SR.net_http_headers_invalid_value, p));
		}
	}

	private static NameValueHeaderValue CreateNameValue()
	{
		return new NameValueHeaderValue();
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new NameValueHeaderValue(this);
	}
}
