using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Net.Http.Headers;

/// <summary>Represents a media type used in a Content-Type header as defined in the RFC 2616.</summary>
public class MediaTypeHeaderValue : ICloneable
{
	private UnvalidatedObjectCollection<NameValueHeaderValue> _parameters;

	private string _mediaType;

	/// <summary>Gets or sets the character set.</summary>
	/// <returns>The character set.</returns>
	public string? CharSet
	{
		get
		{
			return NameValueHeaderValue.Find(_parameters, "charset")?.Value;
		}
		set
		{
			NameValueHeaderValue nameValueHeaderValue = NameValueHeaderValue.Find(_parameters, "charset");
			if (string.IsNullOrEmpty(value))
			{
				if (nameValueHeaderValue != null)
				{
					_parameters.Remove(nameValueHeaderValue);
				}
			}
			else if (nameValueHeaderValue != null)
			{
				nameValueHeaderValue.Value = value;
			}
			else
			{
				Parameters.Add(new NameValueHeaderValue("charset", value));
			}
		}
	}

	/// <summary>Gets or sets the media-type header value parameters.</summary>
	/// <returns>The media-type header value parameters.</returns>
	public ICollection<NameValueHeaderValue> Parameters => _parameters ?? (_parameters = new UnvalidatedObjectCollection<NameValueHeaderValue>());

	/// <summary>Gets or sets the media-type header value.</summary>
	/// <returns>The media-type header value.</returns>
	public string? MediaType
	{
		get
		{
			return _mediaType;
		}
		[param: DisallowNull]
		set
		{
			CheckMediaTypeFormat(value, "value");
			_mediaType = value;
		}
	}

	internal MediaTypeHeaderValue()
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> class.</summary>
	/// <param name="source">A <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> object used to initialize the new instance.</param>
	protected MediaTypeHeaderValue(MediaTypeHeaderValue source)
	{
		_mediaType = source._mediaType;
		_parameters = source._parameters.Clone();
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> class.</summary>
	/// <param name="mediaType">The source represented as a string to initialize the new instance.</param>
	public MediaTypeHeaderValue(string mediaType)
		: this(mediaType, null)
	{
	}

	public MediaTypeHeaderValue(string mediaType, string? charSet)
	{
		CheckMediaTypeFormat(mediaType, "mediaType");
		_mediaType = mediaType;
		if (!string.IsNullOrEmpty(charSet))
		{
			CharSet = charSet;
		}
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		if (_parameters == null || _parameters.Count == 0)
		{
			return _mediaType ?? string.Empty;
		}
		StringBuilder stringBuilder = System.Text.StringBuilderCache.Acquire();
		stringBuilder.Append(_mediaType);
		NameValueHeaderValue.ToString(_parameters, ';', leadingSeparator: true, stringBuilder);
		return System.Text.StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is MediaTypeHeaderValue mediaTypeHeaderValue && string.Equals(_mediaType, mediaTypeHeaderValue._mediaType, StringComparison.OrdinalIgnoreCase))
		{
			return HeaderUtilities.AreEqualCollections(_parameters, mediaTypeHeaderValue._parameters);
		}
		return false;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return StringComparer.OrdinalIgnoreCase.GetHashCode(_mediaType) ^ NameValueHeaderValue.GetHashCode(_parameters);
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents media type header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid media type header value information.</exception>
	public static MediaTypeHeaderValue Parse(string input)
	{
		int index = 0;
		return (MediaTypeHeaderValue)MediaTypeHeaderParser.SingleValueParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out MediaTypeHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (MediaTypeHeaderParser.SingleValueParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (MediaTypeHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetMediaTypeLength(string input, int startIndex, Func<MediaTypeHeaderValue> mediaTypeCreator, out MediaTypeHeaderValue parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int mediaTypeExpressionLength = GetMediaTypeExpressionLength(input, startIndex, out var mediaType);
		if (mediaTypeExpressionLength == 0)
		{
			return 0;
		}
		int num = startIndex + mediaTypeExpressionLength;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		MediaTypeHeaderValue mediaTypeHeaderValue;
		if (num < input.Length && input[num] == ';')
		{
			mediaTypeHeaderValue = mediaTypeCreator();
			mediaTypeHeaderValue._mediaType = mediaType;
			num++;
			int nameValueListLength = NameValueHeaderValue.GetNameValueListLength(input, num, ';', (UnvalidatedObjectCollection<NameValueHeaderValue>)mediaTypeHeaderValue.Parameters);
			if (nameValueListLength == 0)
			{
				return 0;
			}
			parsedValue = mediaTypeHeaderValue;
			return num + nameValueListLength - startIndex;
		}
		mediaTypeHeaderValue = mediaTypeCreator();
		mediaTypeHeaderValue._mediaType = mediaType;
		parsedValue = mediaTypeHeaderValue;
		return num - startIndex;
	}

	private static int GetMediaTypeExpressionLength(string input, int startIndex, out string mediaType)
	{
		mediaType = null;
		int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
		if (tokenLength == 0)
		{
			return 0;
		}
		int num = startIndex + tokenLength;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		if (num >= input.Length || input[num] != '/')
		{
			return 0;
		}
		num++;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		int tokenLength2 = HttpRuleParser.GetTokenLength(input, num);
		if (tokenLength2 == 0)
		{
			return 0;
		}
		int num2 = num + tokenLength2 - startIndex;
		if (tokenLength + tokenLength2 + 1 == num2)
		{
			mediaType = input.Substring(startIndex, num2);
		}
		else
		{
			mediaType = string.Concat(input.AsSpan(startIndex, tokenLength), "/".AsSpan(), input.AsSpan(num, tokenLength2));
		}
		return num2;
	}

	private static void CheckMediaTypeFormat(string mediaType, [CallerArgumentExpression("mediaType")] string parameterName = null)
	{
		ArgumentException.ThrowIfNullOrEmpty(mediaType, parameterName);
		if (GetMediaTypeExpressionLength(mediaType, 0, out var mediaType2) == 0 || mediaType2.Length != mediaType.Length)
		{
			throw new FormatException(System.SR.Format(CultureInfo.InvariantCulture, System.SR.net_http_headers_invalid_value, mediaType));
		}
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new MediaTypeHeaderValue(this);
	}
}
