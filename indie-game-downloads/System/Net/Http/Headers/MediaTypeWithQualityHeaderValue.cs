using System.Diagnostics.CodeAnalysis;

namespace System.Net.Http.Headers;

/// <summary>Represents a media type with an additional quality factor used in a Content-Type header.</summary>
public sealed class MediaTypeWithQualityHeaderValue : MediaTypeHeaderValue, ICloneable
{
	/// <summary>Gets or sets the quality value for the <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" />.</summary>
	/// <returns>The quality value for the <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> object.</returns>
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

	internal MediaTypeWithQualityHeaderValue()
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> class.</summary>
	/// <param name="mediaType">A <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> represented as string to initialize the new instance.</param>
	public MediaTypeWithQualityHeaderValue(string mediaType)
		: base(mediaType)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> class.</summary>
	/// <param name="mediaType">A <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> represented as string to initialize the new instance.</param>
	/// <param name="quality">The quality associated with this header value.</param>
	public MediaTypeWithQualityHeaderValue(string mediaType, double quality)
		: base(mediaType)
	{
		Quality = quality;
	}

	private MediaTypeWithQualityHeaderValue(MediaTypeWithQualityHeaderValue source)
		: base(source)
	{
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new MediaTypeWithQualityHeaderValue(this);
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents media type with quality header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid media type with quality header value information.</exception>
	public new static MediaTypeWithQualityHeaderValue Parse(string input)
	{
		int index = 0;
		return (MediaTypeWithQualityHeaderValue)MediaTypeHeaderParser.SingleValueWithQualityParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out MediaTypeWithQualityHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (MediaTypeHeaderParser.SingleValueWithQualityParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (MediaTypeWithQualityHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}
}
