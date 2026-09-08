using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Net.Http.Headers;

/// <summary>Represents an accept-encoding header value.</summary>
public class TransferCodingHeaderValue : ICloneable
{
	private UnvalidatedObjectCollection<NameValueHeaderValue> _parameters;

	private string _value;

	/// <summary>Gets the transfer-coding value.</summary>
	/// <returns>The transfer-coding value.</returns>
	public string Value => _value;

	/// <summary>Gets the transfer-coding parameters.</summary>
	/// <returns>The transfer-coding parameters.</returns>
	public ICollection<NameValueHeaderValue> Parameters => _parameters ?? (_parameters = new UnvalidatedObjectCollection<NameValueHeaderValue>());

	internal TransferCodingHeaderValue()
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> class.</summary>
	/// <param name="source">A <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> object used to initialize the new instance.</param>
	protected TransferCodingHeaderValue(TransferCodingHeaderValue source)
	{
		_value = source._value;
		_parameters = source._parameters.Clone();
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> class.</summary>
	/// <param name="value">A string used to initialize the new instance.</param>
	public TransferCodingHeaderValue(string value)
	{
		HeaderUtilities.CheckValidToken(value, "value");
		_value = value;
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents transfer-coding header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid transfer-coding header value information.</exception>
	public static TransferCodingHeaderValue Parse(string input)
	{
		int index = 0;
		return (TransferCodingHeaderValue)TransferCodingHeaderParser.SingleValueParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out TransferCodingHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (TransferCodingHeaderParser.SingleValueParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (TransferCodingHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetTransferCodingLength(string input, int startIndex, Func<TransferCodingHeaderValue> transferCodingCreator, out TransferCodingHeaderValue parsedValue)
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
		string value = input.Substring(startIndex, tokenLength);
		int num = startIndex + tokenLength;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		TransferCodingHeaderValue transferCodingHeaderValue;
		if (num < input.Length && input[num] == ';')
		{
			transferCodingHeaderValue = transferCodingCreator();
			transferCodingHeaderValue._value = value;
			num++;
			int nameValueListLength = NameValueHeaderValue.GetNameValueListLength(input, num, ';', (UnvalidatedObjectCollection<NameValueHeaderValue>)transferCodingHeaderValue.Parameters);
			if (nameValueListLength == 0)
			{
				return 0;
			}
			parsedValue = transferCodingHeaderValue;
			return num + nameValueListLength - startIndex;
		}
		transferCodingHeaderValue = transferCodingCreator();
		transferCodingHeaderValue._value = value;
		parsedValue = transferCodingHeaderValue;
		return num - startIndex;
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		StringBuilder stringBuilder = System.Text.StringBuilderCache.Acquire();
		stringBuilder.Append(_value);
		NameValueHeaderValue.ToString(_parameters, ';', leadingSeparator: true, stringBuilder);
		return System.Text.StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	/// <summary>Determines whether the specified Object is equal to the current <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (!(obj is TransferCodingHeaderValue transferCodingHeaderValue))
		{
			return false;
		}
		if (string.Equals(_value, transferCodingHeaderValue._value, StringComparison.OrdinalIgnoreCase))
		{
			return HeaderUtilities.AreEqualCollections(_parameters, transferCodingHeaderValue._parameters);
		}
		return false;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return StringComparer.OrdinalIgnoreCase.GetHashCode(_value) ^ NameValueHeaderValue.GetHashCode(_parameters);
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.TransferCodingHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new TransferCodingHeaderValue(this);
	}
}
