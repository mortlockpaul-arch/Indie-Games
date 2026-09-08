namespace System.Net.Http.Headers;

/// <summary>Represents the collection of Request Headers as defined in RFC 2616.</summary>
public sealed class HttpRequestHeaders : HttpHeaders
{
	private object[] _specialCollectionsSlots;

	private HttpGeneralHeaders _generalHeaders;

	private bool _expectContinueSet;

	/// <summary>Gets the value of the <see langword="Accept" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Accept" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> Accept => GetSpecializedCollection(0, (HttpRequestHeaders thisRef) => new HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue>(KnownHeaders.Accept.Descriptor, thisRef));

	/// <summary>Gets the value of the <see langword="Accept-Charset" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Accept-Charset" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharset => GetSpecializedCollection(1, (HttpRequestHeaders thisRef) => new HttpHeaderValueCollection<StringWithQualityHeaderValue>(KnownHeaders.AcceptCharset.Descriptor, thisRef));

	/// <summary>Gets the value of the <see langword="Accept-Encoding" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Accept-Encoding" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptEncoding => GetSpecializedCollection(2, (HttpRequestHeaders thisRef) => new HttpHeaderValueCollection<StringWithQualityHeaderValue>(KnownHeaders.AcceptEncoding.Descriptor, thisRef));

	/// <summary>Gets the value of the <see langword="Accept-Language" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Accept-Language" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptLanguage => GetSpecializedCollection(3, (HttpRequestHeaders thisRef) => new HttpHeaderValueCollection<StringWithQualityHeaderValue>(KnownHeaders.AcceptLanguage.Descriptor, thisRef));

	/// <summary>Gets or sets the value of the <see langword="Authorization" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Authorization" /> header for an HTTP request.</returns>
	public AuthenticationHeaderValue? Authorization
	{
		get
		{
			return (AuthenticationHeaderValue)GetSingleParsedValue(KnownHeaders.Authorization.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.Authorization.Descriptor, value);
		}
	}

	/// <summary>Gets or sets a value that indicates if the <see langword="Expect" /> header for an HTTP request contains Continue.</summary>
	/// <returns>
	///   <see langword="true" /> if the <see langword="Expect" /> header contains Continue, otherwise <see langword="false" />.</returns>
	public bool? ExpectContinue
	{
		get
		{
			if (ContainsParsedValue(KnownHeaders.Expect.Descriptor, HeaderUtilities.ExpectContinue))
			{
				return true;
			}
			if (_expectContinueSet)
			{
				return false;
			}
			return null;
		}
		set
		{
			if (value == true)
			{
				_expectContinueSet = true;
				if (!ContainsParsedValue(KnownHeaders.Expect.Descriptor, HeaderUtilities.ExpectContinue))
				{
					AddParsedValue(KnownHeaders.Expect.Descriptor, HeaderUtilities.ExpectContinue);
				}
			}
			else
			{
				_expectContinueSet = value.HasValue;
				RemoveParsedValue(KnownHeaders.Expect.Descriptor, HeaderUtilities.ExpectContinue);
			}
		}
	}

	/// <summary>Gets or sets the value of the <see langword="From" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="From" /> header for an HTTP request.</returns>
	public string? From
	{
		get
		{
			return (string)GetSingleParsedValue(KnownHeaders.From.Descriptor);
		}
		set
		{
			if (value == string.Empty)
			{
				value = null;
			}
			HttpHeaders.CheckContainsNewLineOrNull(value);
			SetOrRemoveParsedValue(KnownHeaders.From.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Host" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Host" /> header for an HTTP request.</returns>
	public string? Host
	{
		get
		{
			return (string)GetSingleParsedValue(KnownHeaders.Host.Descriptor);
		}
		set
		{
			if (value == string.Empty)
			{
				value = null;
			}
			if (value != null && HttpRuleParser.GetHostLength(value, 0, allowToken: false) != value.Length)
			{
				throw new FormatException(System.SR.net_http_headers_invalid_host_header);
			}
			SetOrRemoveParsedValue(KnownHeaders.Host.Descriptor, value);
		}
	}

	/// <summary>Gets the value of the <see langword="If-Match" /> header for an HTTP request.</summary>
	/// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.  
	///  The value of the <see langword="If-Match" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<EntityTagHeaderValue> IfMatch => GetSpecializedCollection(4, (HttpRequestHeaders thisRef) => new HttpHeaderValueCollection<EntityTagHeaderValue>(KnownHeaders.IfMatch.Descriptor, thisRef));

	/// <summary>Gets or sets the value of the <see langword="If-Modified-Since" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="If-Modified-Since" /> header for an HTTP request.</returns>
	public DateTimeOffset? IfModifiedSince
	{
		get
		{
			return HeaderUtilities.GetDateTimeOffsetValue(KnownHeaders.IfModifiedSince.Descriptor, this);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.IfModifiedSince.Descriptor, value);
		}
	}

	/// <summary>Gets the value of the <see langword="If-None-Match" /> header for an HTTP request.</summary>
	/// <returns>Gets the value of the <see langword="If-None-Match" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<EntityTagHeaderValue> IfNoneMatch => GetSpecializedCollection(5, (HttpRequestHeaders thisRef) => new HttpHeaderValueCollection<EntityTagHeaderValue>(KnownHeaders.IfNoneMatch.Descriptor, thisRef));

	/// <summary>Gets or sets the value of the <see langword="If-Range" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="If-Range" /> header for an HTTP request.</returns>
	public RangeConditionHeaderValue? IfRange
	{
		get
		{
			return (RangeConditionHeaderValue)GetSingleParsedValue(KnownHeaders.IfRange.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.IfRange.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="If-Unmodified-Since" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="If-Unmodified-Since" /> header for an HTTP request.</returns>
	public DateTimeOffset? IfUnmodifiedSince
	{
		get
		{
			return HeaderUtilities.GetDateTimeOffsetValue(KnownHeaders.IfUnmodifiedSince.Descriptor, this);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.IfUnmodifiedSince.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Max-Forwards" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Max-Forwards" /> header for an HTTP request.</returns>
	public int? MaxForwards
	{
		get
		{
			object singleParsedValue = GetSingleParsedValue(KnownHeaders.MaxForwards.Descriptor);
			if (singleParsedValue != null)
			{
				return (int)singleParsedValue;
			}
			return null;
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.MaxForwards.Descriptor, value);
		}
	}

	public string? Protocol
	{
		get
		{
			if (_specialCollectionsSlots != null)
			{
				return (string)_specialCollectionsSlots[9];
			}
			return null;
		}
		set
		{
			HttpHeaders.CheckContainsNewLineOrNull(value);
			if (_specialCollectionsSlots == null)
			{
				_specialCollectionsSlots = new object[10];
			}
			_specialCollectionsSlots[9] = value;
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Proxy-Authorization" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Proxy-Authorization" /> header for an HTTP request.</returns>
	public AuthenticationHeaderValue? ProxyAuthorization
	{
		get
		{
			return (AuthenticationHeaderValue)GetSingleParsedValue(KnownHeaders.ProxyAuthorization.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.ProxyAuthorization.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Range" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Range" /> header for an HTTP request.</returns>
	public RangeHeaderValue? Range
	{
		get
		{
			return (RangeHeaderValue)GetSingleParsedValue(KnownHeaders.Range.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.Range.Descriptor, value);
		}
	}

	/// <summary>Gets or sets the value of the <see langword="Referer" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Referer" /> header for an HTTP request.</returns>
	public Uri? Referrer
	{
		get
		{
			return (Uri)GetSingleParsedValue(KnownHeaders.Referer.Descriptor);
		}
		set
		{
			SetOrRemoveParsedValue(KnownHeaders.Referer.Descriptor, value);
		}
	}

	/// <summary>Gets the value of the <see langword="TE" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="TE" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue> TE => GetSpecializedCollection(6, (HttpRequestHeaders thisRef) => new HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue>(KnownHeaders.TE.Descriptor, thisRef));

	/// <summary>Gets the value of the <see langword="User-Agent" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="User-Agent" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<ProductInfoHeaderValue> UserAgent => GetSpecializedCollection(7, (HttpRequestHeaders thisRef) => new HttpHeaderValueCollection<ProductInfoHeaderValue>(KnownHeaders.UserAgent.Descriptor, thisRef));

	/// <summary>Gets the value of the <see langword="Expect" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Expect" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<NameValueWithParametersHeaderValue> Expect => GetSpecializedCollection(8, (HttpRequestHeaders thisRef) => new HttpHeaderValueCollection<NameValueWithParametersHeaderValue>(KnownHeaders.Expect.Descriptor, thisRef));

	/// <summary>Gets or sets the value of the <see langword="Cache-Control" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Cache-Control" /> header for an HTTP request.</returns>
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

	/// <summary>Gets the value of the <see langword="Connection" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Connection" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<string> Connection => GeneralHeaders.Connection;

	/// <summary>Gets or sets a value that indicates if the <see langword="Connection" /> header for an HTTP request contains Close.</summary>
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

	/// <summary>Gets or sets the value of the <see langword="Date" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Date" /> header for an HTTP request.</returns>
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

	/// <summary>Gets the value of the <see langword="Pragma" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Pragma" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<NameValueHeaderValue> Pragma => GeneralHeaders.Pragma;

	/// <summary>Gets the value of the <see langword="Trailer" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Trailer" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<string> Trailer => GeneralHeaders.Trailer;

	/// <summary>Gets the value of the <see langword="Transfer-Encoding" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Transfer-Encoding" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding => GeneralHeaders.TransferEncoding;

	/// <summary>Gets or sets a value that indicates if the <see langword="Transfer-Encoding" /> header for an HTTP request contains chunked.</summary>
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

	/// <summary>Gets the value of the <see langword="Upgrade" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Upgrade" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<ProductHeaderValue> Upgrade => GeneralHeaders.Upgrade;

	/// <summary>Gets the value of the <see langword="Via" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Via" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<ViaHeaderValue> Via => GeneralHeaders.Via;

	/// <summary>Gets the value of the <see langword="Warning" /> header for an HTTP request.</summary>
	/// <returns>The value of the <see langword="Warning" /> header for an HTTP request.</returns>
	public HttpHeaderValueCollection<WarningHeaderValue> Warning => GeneralHeaders.Warning;

	private HttpGeneralHeaders GeneralHeaders => _generalHeaders ?? (_generalHeaders = new HttpGeneralHeaders(this));

	private T GetSpecializedCollection<T>(int slot, Func<HttpRequestHeaders, T> creationFunc)
	{
		if (_specialCollectionsSlots == null)
		{
			_specialCollectionsSlots = new object[10];
		}
		object[] specialCollectionsSlots = _specialCollectionsSlots;
		return (T)(specialCollectionsSlots[slot] ?? (specialCollectionsSlots[slot] = creationFunc(this)));
	}

	internal HttpRequestHeaders()
		: base(HttpHeaderType.General | HttpHeaderType.Request | HttpHeaderType.Custom, HttpHeaderType.Response)
	{
	}

	internal override void AddHeaders(HttpHeaders sourceHeaders)
	{
		base.AddHeaders(sourceHeaders);
		HttpRequestHeaders httpRequestHeaders = sourceHeaders as HttpRequestHeaders;
		if (httpRequestHeaders._generalHeaders != null)
		{
			GeneralHeaders.AddSpecialsFrom(httpRequestHeaders._generalHeaders);
		}
		if (!ExpectContinue.HasValue)
		{
			ExpectContinue = httpRequestHeaders.ExpectContinue;
		}
	}
}
