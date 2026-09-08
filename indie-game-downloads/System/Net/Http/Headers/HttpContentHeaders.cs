using System.Collections.Generic;

namespace System.Net.Http.Headers;

/// <summary>Represents the collection of Content Headers as defined in RFC 2616.</summary>
public sealed class HttpContentHeaders : HttpHeaders
{
	private readonly HttpContent _parent;

	private bool _contentLengthSet;

	private HttpHeaderValueCollection<string> _allow;

	private HttpHeaderValueCollection<string> _contentEncoding;

	private HttpHeaderValueCollection<string> _contentLanguage;

	/// <summary>Gets the value of the <see langword="Allow" /> content header on an HTTP response.</summary>
	/// <returns>The value of the <see langword="Allow" /> header on an HTTP response.</returns>
	public ICollection<string> Allow => _allow ?? (_allow = new HttpHeaderValueCollection<string>(KnownHeaders.Allow.Descriptor, this));

	/// <summary>Gets the value of the <see langword="Content-Disposition" /> content header on an HTTP response.</summary>
	/// <returns>The value of the <see langword="Content-Disposition" /> content header on an HTTP response.</returns>
	public ContentDispositionHeaderValue? ContentDisposition
	{
		get
		{
			return (ContentDispositionHeaderValue)GetSingleParsedValue(KnownHeaders.ContentDisposition.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.ContentDisposition.Descriptor, value);
		}
	}

	/// <summary>Gets the value of the <see langword="Content-Encoding" /> content header on an HTTP response.</summary>
	/// <returns>The value of the <see langword="Content-Encoding" /> content header on an HTTP response.</returns>
	public ICollection<string> ContentEncoding => _contentEncoding ?? (_contentEncoding = new HttpHeaderValueCollection<string>(KnownHeaders.ContentEncoding.Descriptor, this));

	/// <summary>Gets the value of the <see langword="Content-Language" /> content header on an HTTP response.</summary>
	/// <returns>The value of the <see langword="Content-Language" /> content header on an HTTP response.</returns>
	public ICollection<string> ContentLanguage => _contentLanguage ?? (_contentLanguage = new HttpHeaderValueCollection<string>(KnownHeaders.ContentLanguage.Descriptor, this));

	/// <summary>Gets or sets the value of the <see langword="Content-Length" /> content header on an HTTP response.</summary>
	/// <returns>The value of the <see langword="Content-Length" /> content header on an HTTP response.</returns>
	public long? ContentLength
	{
		get
		{
			object singleParsedValue = GetSingleParsedValue(KnownHeaders.ContentLength.Descriptor);
			if (!_contentLengthSet && singleParsedValue == null)
			{
				long? computedOrBufferLength = _parent.GetComputedOrBufferLength();
				if (computedOrBufferLength.HasValue)
				{
					SetParsedValue(KnownHeaders.ContentLength.Descriptor, computedOrBufferLength.Value);
				}
				return computedOrBufferLength;
			}
			if (singleParsedValue == null)
			{
				return null;
			}
			return (long)singleParsedValue;
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.ContentLength.Descriptor, value);
			_contentLengthSet = true;
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Content-Location" /> content header on an HTTP response.</summary>
	/// <returns>The value of the <see langword="Content-Location" /> content header on an HTTP response.</returns>
	public Uri? ContentLocation
	{
		get
		{
			return (Uri)GetSingleParsedValue(KnownHeaders.ContentLocation.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.ContentLocation.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Content-MD5" /> content header on an HTTP response.</summary>
	/// <returns>The value of the <see langword="Content-MD5" /> content header on an HTTP response.</returns>
	public byte[]? ContentMD5
	{
		get
		{
			return (byte[])GetSingleParsedValue(KnownHeaders.ContentMD5.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.ContentMD5.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Content-Range" /> content header on an HTTP response.</summary>
	/// <returns>The value of the <see langword="Content-Range" /> content header on an HTTP response.</returns>
	public ContentRangeHeaderValue? ContentRange
	{
		get
		{
			return (ContentRangeHeaderValue)GetSingleParsedValue(KnownHeaders.ContentRange.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.ContentRange.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Content-Type" /> content header on an HTTP response.</summary>
	/// <returns>The value of the <see langword="Content-Type" /> content header on an HTTP response.</returns>
	public MediaTypeHeaderValue? ContentType
	{
		get
		{
			return (MediaTypeHeaderValue)GetSingleParsedValue(KnownHeaders.ContentType.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.ContentType.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Expires" /> content header on an HTTP response.</summary>
	/// <returns>The value of the <see langword="Expires" /> content header on an HTTP response.</returns>
	public DateTimeOffset? Expires
	{
		get
		{
			return HeaderUtilities.GetDateTimeOffsetValue(KnownHeaders.Expires.Descriptor, this, DateTimeOffset.MinValue);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.Expires.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Last-Modified" /> content header on an HTTP response.</summary>
	/// <returns>The value of the <see langword="Last-Modified" /> content header on an HTTP response.</returns>
	public DateTimeOffset? LastModified
	{
		get
		{
			return HeaderUtilities.GetDateTimeOffsetValue(KnownHeaders.LastModified.Descriptor, this);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.LastModified.Descriptor, value);
		}
	}

	internal HttpContentHeaders(HttpContent parent)
		: base(HttpHeaderType.Content | HttpHeaderType.Custom, HttpHeaderType.None)
	{
		_parent = parent;
	}
}
