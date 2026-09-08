namespace System.Net.Http.Headers;

/// <summary>Represents the collection of Response Headers as defined in RFC 2616.</summary>
public sealed class HttpResponseHeaders : HttpHeaders
{
	private object[] _specialCollectionsSlots;

	private HttpGeneralHeaders _generalHeaders;

	private readonly bool _containsTrailingHeaders;

	/// <summary>Gets the value of the <see langword="Accept-Ranges" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Accept-Ranges" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<string> AcceptRanges => GetSpecializedCollection(0, (HttpResponseHeaders thisRef) => new HttpHeaderValueCollection<string>(KnownHeaders.AcceptRanges.Descriptor, thisRef));

	/// <summary>Gets or sets the value of the <see langword="Age" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Age" /> header for an HTTP response.</returns>
	public TimeSpan? Age
	{
		get
		{
			return HeaderUtilities.GetTimeSpanValue(KnownHeaders.Age.Descriptor, this);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.Age.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="ETag" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="ETag" /> header for an HTTP response.</returns>
	public EntityTagHeaderValue? ETag
	{
		get
		{
			return (EntityTagHeaderValue)GetSingleParsedValue(KnownHeaders.ETag.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.ETag.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Location" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Location" /> header for an HTTP response.</returns>
	public Uri? Location
	{
		get
		{
			return (Uri)GetSingleParsedValue(KnownHeaders.Location.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.Location.Descriptor, value);
		}
	}

	/// <summary>Gets the value of the <see langword="Proxy-Authenticate" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Proxy-Authenticate" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<AuthenticationHeaderValue> ProxyAuthenticate => GetSpecializedCollection(1, (HttpResponseHeaders thisRef) => new HttpHeaderValueCollection<AuthenticationHeaderValue>(KnownHeaders.ProxyAuthenticate.Descriptor, thisRef));

	/// <summary>Gets or sets the value of the <see langword="Retry-After" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Retry-After" /> header for an HTTP response.</returns>
	public RetryConditionHeaderValue? RetryAfter
	{
		get
		{
			return (RetryConditionHeaderValue)GetSingleParsedValue(KnownHeaders.RetryAfter.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.RetryAfter.Descriptor, value);
		}
	}

	/// <summary>Gets the value of the <see langword="Server" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Server" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<ProductInfoHeaderValue> Server => GetSpecializedCollection(2, (HttpResponseHeaders thisRef) => new HttpHeaderValueCollection<ProductInfoHeaderValue>(KnownHeaders.Server.Descriptor, thisRef));

	/// <summary>Gets the value of the <see langword="Vary" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Vary" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<string> Vary => GetSpecializedCollection(3, (HttpResponseHeaders thisRef) => new HttpHeaderValueCollection<string>(KnownHeaders.Vary.Descriptor, thisRef));

	/// <summary>Gets the value of the <see langword="WWW-Authenticate" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="WWW-Authenticate" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<AuthenticationHeaderValue> WwwAuthenticate => GetSpecializedCollection(4, (HttpResponseHeaders thisRef) => new HttpHeaderValueCollection<AuthenticationHeaderValue>(KnownHeaders.WWWAuthenticate.Descriptor, thisRef));

	/// <summary>Gets or sets the value of the <see langword="Cache-Control" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Cache-Control" /> header for an HTTP response.</returns>
	public CacheControlHeaderValue? CacheControl
	{
		get
		{
			return GeneralHeaders.CacheControl;
		}
		set
		{
			GeneralHeaders.CacheControl = value;
		}
	}

	/// <summary>Gets the value of the <see langword="Connection" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Connection" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<string> Connection => GeneralHeaders.Connection;

	/// <summary>Gets or sets a value that indicates if the <see langword="Connection" /> header for an HTTP response contains Close.</summary>
	/// <returns>
	///   <see langword="true" /> if the <see langword="Connection" /> header contains Close, otherwise <see langword="false" />.</returns>
	public bool? ConnectionClose
	{
		get
		{
			return HttpGeneralHeaders.GetConnectionClose(this, _generalHeaders);
		}
		set
		{
			GeneralHeaders.ConnectionClose = value;
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Date" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Date" /> header for an HTTP response.</returns>
	public DateTimeOffset? Date
	{
		get
		{
			return GeneralHeaders.Date;
		}
		set
		{
			GeneralHeaders.Date = value;
		}
	}

	/// <summary>Gets the value of the <see langword="Pragma" /> header for an HTTP response.</summary>
	/// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.  
	///  The value of the <see langword="Pragma" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<NameValueHeaderValue> Pragma => GeneralHeaders.Pragma;

	/// <summary>Gets the value of the <see langword="Trailer" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Trailer" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<string> Trailer => GeneralHeaders.Trailer;

	/// <summary>Gets the value of the <see langword="Transfer-Encoding" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Transfer-Encoding" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding => GeneralHeaders.TransferEncoding;

	/// <summary>Gets or sets a value that indicates if the <see langword="Transfer-Encoding" /> header for an HTTP response contains chunked.</summary>
	/// <returns>
	///   <see langword="true" /> if the <see langword="Transfer-Encoding" /> header contains chunked, otherwise <see langword="false" />.</returns>
	public bool? TransferEncodingChunked
	{
		get
		{
			return HttpGeneralHeaders.GetTransferEncodingChunked(this, _generalHeaders);
		}
		set
		{
			GeneralHeaders.TransferEncodingChunked = value;
		}
	}

	/// <summary>Gets the value of the <see langword="Upgrade" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Upgrade" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<ProductHeaderValue> Upgrade => GeneralHeaders.Upgrade;

	/// <summary>Gets the value of the <see langword="Via" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Via" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<ViaHeaderValue> Via => GeneralHeaders.Via;

	/// <summary>Gets the value of the <see langword="Warning" /> header for an HTTP response.</summary>
	/// <returns>The value of the <see langword="Warning" /> header for an HTTP response.</returns>
	public HttpHeaderValueCollection<WarningHeaderValue> Warning => GeneralHeaders.Warning;

	private HttpGeneralHeaders GeneralHeaders => _generalHeaders ?? (_generalHeaders = new HttpGeneralHeaders(this));

	private T GetSpecializedCollection<T>(int slot, Func<HttpResponseHeaders, T> creationFunc)
	{
		object[] array = _specialCollectionsSlots ?? (_specialCollectionsSlots = new object[5]);
		return (T)(array[slot] ?? (array[slot] = creationFunc(this)));
	}

	internal HttpResponseHeaders(bool containsTrailingHeaders = false)
		: base(containsTrailingHeaders ? (HttpHeaderType.General | HttpHeaderType.Response | HttpHeaderType.Content | HttpHeaderType.Custom | HttpHeaderType.NonTrailing) : (HttpHeaderType.General | HttpHeaderType.Response | HttpHeaderType.Custom), HttpHeaderType.Request)
	{
		_containsTrailingHeaders = containsTrailingHeaders;
	}

	internal override void AddHeaders(HttpHeaders sourceHeaders)
	{
		base.AddHeaders(sourceHeaders);
		HttpResponseHeaders httpResponseHeaders = sourceHeaders as HttpResponseHeaders;
		if (httpResponseHeaders._generalHeaders != null)
		{
			GeneralHeaders.AddSpecialsFrom(httpResponseHeaders._generalHeaders);
		}
	}

	internal override bool IsAllowedHeaderName(HeaderDescriptor descriptor)
	{
		if (!_containsTrailingHeaders)
		{
			return true;
		}
		KnownHeader knownHeader = KnownHeaders.TryGetKnownHeader(descriptor.Name);
		if (knownHeader == null)
		{
			return true;
		}
		return (knownHeader.HeaderType & HttpHeaderType.NonTrailing) == 0;
	}
}
