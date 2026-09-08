using System.Diagnostics.CodeAnalysis;

namespace System.Net.Http.Headers;

/// <summary>Represents authentication information in Authorization, ProxyAuthorization, WWW-Authenticate, and Proxy-Authenticate header values.</summary>
public class AuthenticationHeaderValue : ICloneable
{
	private readonly string _scheme;

	private readonly string _parameter;

	/// <summary>Gets the scheme to use for authorization.</summary>
	/// <returns>The scheme to use for authorization.</returns>
	public string Scheme => _scheme;

	/// <summary>Gets the credentials containing the authentication information of the user agent for the resource being requested.</summary>
	/// <returns>The credentials containing the authentication information.</returns>
	public string? Parameter => _parameter;

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> class.</summary>
	/// <param name="scheme">The scheme to use for authorization.</param>
	public AuthenticationHeaderValue(string scheme)
		: this(scheme, null)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> class.</summary>
	/// <param name="scheme">The scheme to use for authorization.</param>
	/// <param name="parameter">The credentials containing the authentication information of the user agent for the resource being requested.</param>
	public AuthenticationHeaderValue(string scheme, string? parameter)
	{
		HeaderUtilities.CheckValidToken(scheme, "scheme");
		HttpHeaders.CheckContainsNewLineOrNull(parameter);
		_scheme = scheme;
		_parameter = parameter;
	}

	private AuthenticationHeaderValue(AuthenticationHeaderValue source)
	{
		_scheme = source._scheme;
		_parameter = source._parameter;
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		if (string.IsNullOrEmpty(_parameter))
		{
			return _scheme;
		}
		return _scheme + " " + _parameter;
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (!(obj is AuthenticationHeaderValue authenticationHeaderValue))
		{
			return false;
		}
		if (string.IsNullOrEmpty(_parameter) && string.IsNullOrEmpty(authenticationHeaderValue._parameter))
		{
			return string.Equals(_scheme, authenticationHeaderValue._scheme, StringComparison.OrdinalIgnoreCase);
		}
		if (string.Equals(_scheme, authenticationHeaderValue._scheme, StringComparison.OrdinalIgnoreCase))
		{
			return string.Equals(_parameter, authenticationHeaderValue._parameter, StringComparison.Ordinal);
		}
		return false;
	}

	/// <summary>Serves as a hash function for an  <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		int num = StringComparer.OrdinalIgnoreCase.GetHashCode(_scheme);
		if (!string.IsNullOrEmpty(_parameter))
		{
			num ^= _parameter.GetHashCode();
		}
		return num;
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents authentication header value information.</param>
	/// <returns>An <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid authentication header value information.</exception>
	public static AuthenticationHeaderValue Parse(string input)
	{
		int index = 0;
		return (AuthenticationHeaderValue)GenericHeaderParser.SingleValueAuthenticationParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out AuthenticationHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (GenericHeaderParser.SingleValueAuthenticationParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (AuthenticationHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetAuthenticationLength(string input, int startIndex, out object parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length || HttpRuleParser.ContainsNewLineOrNull(input, startIndex))
		{
			return 0;
		}
		int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
		if (tokenLength == 0)
		{
			return 0;
		}
		string text = null;
		switch (tokenLength)
		{
		case 5:
			text = "Basic";
			break;
		case 6:
			text = "Digest";
			break;
		case 4:
			text = "NTLM";
			break;
		case 9:
			text = "Negotiate";
			break;
		}
		string scheme = ((text != null && string.CompareOrdinal(input, startIndex, text, 0, tokenLength) == 0) ? text : input.Substring(startIndex, tokenLength));
		int num = startIndex + tokenLength;
		int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, num);
		num += whitespaceLength;
		if (num == input.Length || input[num] == ',')
		{
			parsedValue = new AuthenticationHeaderValue(scheme);
			return num - startIndex;
		}
		if (whitespaceLength == 0)
		{
			return 0;
		}
		int num2 = num;
		int parameterEndIndex = num;
		if (!TrySkipFirstBlob(input, ref num, ref parameterEndIndex))
		{
			return 0;
		}
		if (num < input.Length && !TryGetParametersEndIndex(input, ref num, ref parameterEndIndex))
		{
			return 0;
		}
		string parameter = input.Substring(num2, parameterEndIndex - num2 + 1);
		parsedValue = new AuthenticationHeaderValue(scheme, parameter);
		return num - startIndex;
	}

	private static bool TrySkipFirstBlob(string input, ref int current, ref int parameterEndIndex)
	{
		while (current < input.Length && input[current] != ',')
		{
			if (input[current] == '"')
			{
				if (HttpRuleParser.GetQuotedStringLength(input, current, out var length) != HttpParseResult.Parsed)
				{
					return false;
				}
				current += length;
				parameterEndIndex = current - 1;
			}
			else
			{
				int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
				if (whitespaceLength == 0)
				{
					parameterEndIndex = current;
					current++;
				}
				else
				{
					current += whitespaceLength;
				}
			}
		}
		return true;
	}

	private static bool TryGetParametersEndIndex(string input, ref int parseEndIndex, ref int parameterEndIndex)
	{
		int num = parseEndIndex;
		do
		{
			num++;
			num = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, num, skipEmptyValues: true, out var _);
			if (num == input.Length)
			{
				return true;
			}
			int tokenLength = HttpRuleParser.GetTokenLength(input, num);
			if (tokenLength == 0)
			{
				return false;
			}
			num += tokenLength;
			num += HttpRuleParser.GetWhitespaceLength(input, num);
			if (num == input.Length || input[num] != '=')
			{
				return true;
			}
			num++;
			num += HttpRuleParser.GetWhitespaceLength(input, num);
			int valueLength = NameValueHeaderValue.GetValueLength(input, num);
			if (valueLength == 0)
			{
				return false;
			}
			num += valueLength;
			parameterEndIndex = num - 1;
			num = (parseEndIndex = num + HttpRuleParser.GetWhitespaceLength(input, num));
		}
		while (num < input.Length && input[num] == ',');
		return true;
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new AuthenticationHeaderValue(this);
	}
}
