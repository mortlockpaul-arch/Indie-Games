using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers;

/// <summary>Represents the value of a Via header.</summary>
public class ViaHeaderValue : ICloneable
{
	private readonly string _protocolName;

	private readonly string _protocolVersion;

	private readonly string _receivedBy;

	private readonly string _comment;

	/// <summary>Gets the protocol name of the received protocol.</summary>
	/// <returns>The protocol name.</returns>
	public string? ProtocolName => _protocolName;

	/// <summary>Gets the protocol version of the received protocol.</summary>
	/// <returns>The protocol version.</returns>
	public string ProtocolVersion => _protocolVersion;

	/// <summary>Gets the host and port that the request or response was received by.</summary>
	/// <returns>The host and port that the request or response was received by.</returns>
	public string ReceivedBy => _receivedBy;

	/// <summary>Gets the comment field used to identify the software of the recipient proxy or gateway.</summary>
	/// <returns>The comment field used to identify the software of the recipient proxy or gateway.</returns>
	public string? Comment => _comment;

	private ViaHeaderValue(string protocolVersion, string receivedBy, string protocolName, string comment, bool _)
	{
		_protocolVersion = protocolVersion;
		_receivedBy = receivedBy;
		_protocolName = protocolName;
		_comment = comment;
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> class.</summary>
	/// <param name="protocolVersion">The protocol version of the received protocol.</param>
	/// <param name="receivedBy">The host and port that the request or response was received by.</param>
	public ViaHeaderValue(string protocolVersion, string receivedBy)
		: this(protocolVersion, receivedBy, null, null)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> class.</summary>
	/// <param name="protocolVersion">The protocol version of the received protocol.</param>
	/// <param name="receivedBy">The host and port that the request or response was received by.</param>
	/// <param name="protocolName">The protocol name of the received protocol.</param>
	public ViaHeaderValue(string protocolVersion, string receivedBy, string? protocolName)
		: this(protocolVersion, receivedBy, protocolName, null)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> class.</summary>
	/// <param name="protocolVersion">The protocol version of the received protocol.</param>
	/// <param name="receivedBy">The host and port that the request or response was received by.</param>
	/// <param name="protocolName">The protocol name of the received protocol.</param>
	/// <param name="comment">The comment field used to identify the software of the recipient proxy or gateway.</param>
	public ViaHeaderValue(string protocolVersion, string receivedBy, string? protocolName, string? comment)
	{
		HeaderUtilities.CheckValidToken(protocolVersion, "protocolVersion");
		CheckReceivedBy(receivedBy);
		if (!string.IsNullOrEmpty(protocolName))
		{
			HeaderUtilities.CheckValidToken(protocolName, "protocolName");
			_protocolName = protocolName;
		}
		if (!string.IsNullOrEmpty(comment))
		{
			HeaderUtilities.CheckValidComment(comment, "comment");
			_comment = comment;
		}
		_protocolVersion = protocolVersion;
		_receivedBy = receivedBy;
	}

	private ViaHeaderValue(ViaHeaderValue source)
	{
		_protocolName = source._protocolName;
		_protocolVersion = source._protocolVersion;
		_receivedBy = source._receivedBy;
		_comment = source._comment;
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		Span<char> initialBuffer = stackalloc char[256];
		System.Text.ValueStringBuilder valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
		if (!string.IsNullOrEmpty(_protocolName))
		{
			valueStringBuilder.Append(_protocolName);
			valueStringBuilder.Append('/');
		}
		valueStringBuilder.Append(_protocolVersion);
		valueStringBuilder.Append(' ');
		valueStringBuilder.Append(_receivedBy);
		if (!string.IsNullOrEmpty(_comment))
		{
			valueStringBuilder.Append(' ');
			valueStringBuilder.Append(_comment);
		}
		return valueStringBuilder.ToString();
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is ViaHeaderValue viaHeaderValue && string.Equals(_protocolVersion, viaHeaderValue._protocolVersion, StringComparison.OrdinalIgnoreCase) && string.Equals(_receivedBy, viaHeaderValue._receivedBy, StringComparison.OrdinalIgnoreCase) && string.Equals(_protocolName, viaHeaderValue._protocolName, StringComparison.OrdinalIgnoreCase))
		{
			return string.Equals(_comment, viaHeaderValue._comment, StringComparison.Ordinal);
		}
		return false;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return HashCode.Combine(StringComparer.OrdinalIgnoreCase.GetHashCode(_protocolVersion), StringComparer.OrdinalIgnoreCase.GetHashCode(_receivedBy), (_protocolName != null) ? StringComparer.OrdinalIgnoreCase.GetHashCode(_protocolName) : 0, _comment);
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents via header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid via header value information.</exception>
	public static ViaHeaderValue Parse(string input)
	{
		int index = 0;
		return (ViaHeaderValue)GenericHeaderParser.SingleValueViaParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out ViaHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (GenericHeaderParser.SingleValueViaParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (ViaHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetViaLength(string input, int startIndex, out object parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int protocolEndIndex = GetProtocolEndIndex(input, startIndex, out var protocolName, out var protocolVersion);
		if (protocolEndIndex == 0 || protocolEndIndex == input.Length)
		{
			return 0;
		}
		int hostLength = HttpRuleParser.GetHostLength(input, protocolEndIndex, allowToken: true);
		if (hostLength == 0)
		{
			return 0;
		}
		string receivedBy = input.Substring(protocolEndIndex, hostLength);
		protocolEndIndex += hostLength;
		protocolEndIndex += HttpRuleParser.GetWhitespaceLength(input, protocolEndIndex);
		string comment = null;
		if (protocolEndIndex < input.Length && input[protocolEndIndex] == '(')
		{
			if (HttpRuleParser.GetCommentLength(input, protocolEndIndex, out var length) != HttpParseResult.Parsed)
			{
				return 0;
			}
			comment = input.Substring(protocolEndIndex, length);
			protocolEndIndex += length;
			protocolEndIndex += HttpRuleParser.GetWhitespaceLength(input, protocolEndIndex);
		}
		parsedValue = new ViaHeaderValue(protocolVersion, receivedBy, protocolName, comment, _: false);
		return protocolEndIndex - startIndex;
	}

	private static int GetProtocolEndIndex(string input, int startIndex, out string protocolName, out string protocolVersion)
	{
		protocolName = null;
		protocolVersion = null;
		int startIndex2 = startIndex;
		int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex2);
		if (tokenLength == 0)
		{
			return 0;
		}
		startIndex2 = startIndex + tokenLength;
		int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, startIndex2);
		startIndex2 += whitespaceLength;
		if (startIndex2 == input.Length)
		{
			return 0;
		}
		if (input[startIndex2] == '/')
		{
			protocolName = input.Substring(startIndex, tokenLength);
			startIndex2++;
			startIndex2 += HttpRuleParser.GetWhitespaceLength(input, startIndex2);
			tokenLength = HttpRuleParser.GetTokenLength(input, startIndex2);
			if (tokenLength == 0)
			{
				return 0;
			}
			protocolVersion = input.Substring(startIndex2, tokenLength);
			startIndex2 += tokenLength;
			whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, startIndex2);
			startIndex2 += whitespaceLength;
		}
		else
		{
			protocolVersion = input.Substring(startIndex, tokenLength);
		}
		if (whitespaceLength == 0)
		{
			return 0;
		}
		return startIndex2;
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new ViaHeaderValue(this);
	}

	private static void CheckReceivedBy(string receivedBy)
	{
		ArgumentException.ThrowIfNullOrEmpty(receivedBy, "receivedBy");
		if (HttpRuleParser.GetHostLength(receivedBy, 0, allowToken: true) != receivedBy.Length)
		{
			throw new FormatException(System.SR.Format(CultureInfo.InvariantCulture, System.SR.net_http_headers_invalid_value, receivedBy));
		}
	}
}
