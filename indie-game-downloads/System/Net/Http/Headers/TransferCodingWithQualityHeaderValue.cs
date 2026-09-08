using System.Diagnostics.CodeAnalysis;

namespace System.Net.Http.Headers;

/// <summary>Represents an Accept-Encoding header value.with optional quality factor.</summary>
public sealed class TransferCodingWithQualityHeaderValue : TransferCodingHeaderValue, ICloneable
{
	/// <summary>Gets the quality factor from the <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" />.</summary>
	/// <returns>The quality factor from the <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" />.</returns>
	public double? Quality
	{
		get
		{
			return HeaderUtilities.GetQuality((UnvalidatedObjectCollection<NameValueHeaderValue>)base.Parameters);
		}
		set
		{
			HeaderUtilities.SetQuality((UnvalidatedObjectCollection<NameValueHeaderValue>)base.Parameters, value);
		}
	}

	internal TransferCodingWithQualityHeaderValue()
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> class.</summary>
	/// <param name="value">A string used to initialize the new instance.</param>
	public TransferCodingWithQualityHeaderValue(string value)
		: base(value)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> class.</summary>
	/// <param name="value">A string used to initialize the new instance.</param>
	/// <param name="quality">A value for the quality factor.</param>
	public TransferCodingWithQualityHeaderValue(string value, double quality)
		: base(value)
	{
		Quality = quality;
	}

	private TransferCodingWithQualityHeaderValue(TransferCodingWithQualityHeaderValue source)
		: base(source)
	{
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new TransferCodingWithQualityHeaderValue(this);
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents transfer-coding value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid transfer-coding with quality header value information.</exception>
	public new static TransferCodingWithQualityHeaderValue Parse(string input)
	{
		int index = 0;
		return (TransferCodingWithQualityHeaderValue)TransferCodingHeaderParser.SingleValueWithQualityParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out TransferCodingWithQualityHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (TransferCodingHeaderParser.SingleValueWithQualityParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (TransferCodingWithQualityHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}
}
