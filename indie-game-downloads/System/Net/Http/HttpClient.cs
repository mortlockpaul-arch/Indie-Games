using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>Provides a base class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI.</summary>
public class HttpClient : HttpMessageInvoker
{
	private static IWebProxy s_defaultProxy;

	private static readonly TimeSpan s_defaultTimeout = TimeSpan.FromSeconds(100L);

	private static readonly TimeSpan s_maxTimeout = TimeSpan.FromMilliseconds(2147483647L);

	private static readonly TimeSpan s_infiniteTimeout = System.Threading.Timeout.InfiniteTimeSpan;

	private volatile bool _operationStarted;

	private volatile bool _disposed;

	private CancellationTokenSource _pendingRequestsCts;

	private HttpRequestHeaders _defaultRequestHeaders;

	private Version _defaultRequestVersion = HttpRequestMessage.DefaultRequestVersion;

	private HttpVersionPolicy _defaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

	private Uri _baseAddress;

	private TimeSpan _timeout;

	private int _maxResponseContentBufferSize;

	public static IWebProxy DefaultProxy
	{
		get
		{
			return LazyInitializer.EnsureInitialized(ref s_defaultProxy, () => SystemProxyInfo.Proxy);
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			s_defaultProxy = value;
		}
	}

	/// <summary>Gets the headers which should be sent with each request.</summary>
	/// <returns>The headers which should be sent with each request.</returns>
	public HttpRequestHeaders DefaultRequestHeaders => _defaultRequestHeaders ?? (_defaultRequestHeaders = new HttpRequestHeaders());

	public Version DefaultRequestVersion
	{
		get
		{
			return _defaultRequestVersion;
		}
		set
		{
			CheckDisposedOrStarted();
			ArgumentNullException.ThrowIfNull(value, "value");
			_defaultRequestVersion = value;
		}
	}

	public HttpVersionPolicy DefaultVersionPolicy
	{
		get
		{
			return _defaultVersionPolicy;
		}
		set
		{
			CheckDisposedOrStarted();
			_defaultVersionPolicy = value;
		}
	}

	/// <summary>Gets or sets the base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.</summary>
	/// <returns>The base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.</returns>
	public Uri? BaseAddress
	{
		get
		{
			return _baseAddress;
		}
		set
		{
			if ((object)value != null && !value.IsAbsoluteUri)
			{
				throw new ArgumentException(System.SR.net_http_client_absolute_baseaddress_required, "value");
			}
			CheckDisposedOrStarted();
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.UriBaseAddress(this, value);
			}
			_baseAddress = value;
		}
	}

	/// <summary>Gets or sets the timespan to wait before the request times out.</summary>
	/// <returns>The timespan to wait before the request times out.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">The timeout specified is less than or equal to zero and is not <see cref="F:System.Threading.Timeout.InfiniteTimeSpan" />.</exception>
	/// <exception cref="T:System.InvalidOperationException">An operation has already been started on the current instance.</exception>
	/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
	public TimeSpan Timeout
	{
		get
		{
			return _timeout;
		}
		set
		{
			if (value != s_infiniteTimeout)
			{
				ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, TimeSpan.Zero, "value");
				ArgumentOutOfRangeException.ThrowIfGreaterThan(value, s_maxTimeout, "value");
			}
			CheckDisposedOrStarted();
			_timeout = value;
		}
	}

	/// <summary>Gets or sets the maximum number of bytes to buffer when reading the response content.</summary>
	/// <returns>The maximum number of bytes to buffer when reading the response content. The default value for this property is 2 gigabytes.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">The size specified is less than or equal to zero.</exception>
	/// <exception cref="T:System.InvalidOperationException">An operation has already been started on the current instance.</exception>
	/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
	public long MaxResponseContentBufferSize
	{
		get
		{
			return _maxResponseContentBufferSize;
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, "value");
			if (value > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("value", value, System.SR.Format(CultureInfo.InvariantCulture, System.SR.net_http_content_buffersize_limit, int.MaxValue));
			}
			CheckDisposedOrStarted();
			_maxResponseContentBufferSize = (int)value;
		}
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpClient" /> class.</summary>
	public HttpClient()
		: this(new HttpClientHandler())
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpClient" /> class with a specific handler.</summary>
	/// <param name="handler">The HTTP handler stack to use for sending requests.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="handler" /> is <see langword="null" />.</exception>
	public HttpClient(HttpMessageHandler handler)
		: this(handler, disposeHandler: true)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpClient" /> class with a specific handler.</summary>
	/// <param name="handler">The <see cref="T:System.Net.Http.HttpMessageHandler" /> responsible for processing the HTTP response messages.</param>
	/// <param name="disposeHandler">
	///   <see langword="true" /> if the inner handler should be disposed of by HttpClient.Dispose, <see langword="false" /> if you intend to reuse the inner handler.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="handler" /> is <see langword="null" />.</exception>
	public HttpClient(HttpMessageHandler handler, bool disposeHandler)
		: base(handler, disposeHandler)
	{
		_timeout = s_defaultTimeout;
		_maxResponseContentBufferSize = int.MaxValue;
		_pendingRequestsCts = new CancellationTokenSource();
	}

	/// <summary>Send a GET request to the specified Uri and return the response body as a string in an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<string> GetStringAsync([StringSyntax("Uri")] string? requestUri)
	{
		return GetStringAsync(CreateUri(requestUri));
	}

	/// <summary>Send a GET request to the specified Uri and return the response body as a string in an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<string> GetStringAsync(Uri? requestUri)
	{
		return GetStringAsync(requestUri, CancellationToken.None);
	}

	public Task<string> GetStringAsync([StringSyntax("Uri")] string? requestUri, CancellationToken cancellationToken)
	{
		return GetStringAsync(CreateUri(requestUri), cancellationToken);
	}

	public Task<string> GetStringAsync(Uri? requestUri, CancellationToken cancellationToken)
	{
		HttpRequestMessage request = CreateRequestMessage(HttpMethod.Get, requestUri);
		CheckRequestBeforeSend(request);
		return GetStringAsyncCore(request, cancellationToken);
	}

	private async Task<string> GetStringAsyncCore(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		bool telemetryStarted = StartSend(request);
		bool responseContentTelemetryStarted = false;
		(CancellationTokenSource, bool, CancellationTokenSource) tuple = PrepareCancellationTokenSource(cancellationToken);
		CancellationTokenSource cts = tuple.Item1;
		bool disposeCts = tuple.Item2;
		CancellationTokenSource pendingRequestsCts = tuple.Item3;
		HttpResponseMessage response = null;
		try
		{
			response = await base.SendAsync(request, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
			ThrowForNullResponse(response);
			response.EnsureSuccessStatusCode();
			HttpContent c = response.Content;
			if (HttpTelemetry.Log.IsEnabled() & telemetryStarted)
			{
				HttpTelemetry.Log.ResponseContentStart();
				responseContentTelemetryStarted = true;
			}
			using HttpContent.LimitArrayPoolWriteStream buffer = new HttpContent.LimitArrayPoolWriteStream(_maxResponseContentBufferSize, c.Headers.ContentLength.GetValueOrDefault(), getFinalSizeFromPool: true);
			Stream stream = c.TryReadAsStream();
			if (stream == null)
			{
				stream = await c.ReadAsStreamAsync(cts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
			using Stream responseStream = stream;
			_ = 2;
			try
			{
				await responseStream.CopyToAsync(buffer, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception e) when (HttpContent.StreamCopyExceptionNeedsWrapping(e))
			{
				throw HttpContent.WrapStreamCopyException(e);
			}
			return HttpContent.ReadBufferAsString(buffer, c.Headers);
		}
		catch (Exception e2)
		{
			HandleFailure(e2, telemetryStarted, response, cts, cancellationToken, pendingRequestsCts);
			throw;
		}
		finally
		{
			FinishSend(response, cts, disposeCts, telemetryStarted, responseContentTelemetryStarted);
		}
	}

	/// <summary>Sends a GET request to the specified Uri and return the response body as a byte array in an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<byte[]> GetByteArrayAsync([StringSyntax("Uri")] string? requestUri)
	{
		return GetByteArrayAsync(CreateUri(requestUri));
	}

	/// <summary>Send a GET request to the specified Uri and return the response body as a byte array in an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<byte[]> GetByteArrayAsync(Uri? requestUri)
	{
		return GetByteArrayAsync(requestUri, CancellationToken.None);
	}

	public Task<byte[]> GetByteArrayAsync([StringSyntax("Uri")] string? requestUri, CancellationToken cancellationToken)
	{
		return GetByteArrayAsync(CreateUri(requestUri), cancellationToken);
	}

	public Task<byte[]> GetByteArrayAsync(Uri? requestUri, CancellationToken cancellationToken)
	{
		HttpRequestMessage request = CreateRequestMessage(HttpMethod.Get, requestUri);
		CheckRequestBeforeSend(request);
		return GetByteArrayAsyncCore(request, cancellationToken);
	}

	private async Task<byte[]> GetByteArrayAsyncCore(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		bool telemetryStarted = StartSend(request);
		bool responseContentTelemetryStarted = false;
		(CancellationTokenSource, bool, CancellationTokenSource) tuple = PrepareCancellationTokenSource(cancellationToken);
		CancellationTokenSource cts = tuple.Item1;
		bool disposeCts = tuple.Item2;
		CancellationTokenSource pendingRequestsCts = tuple.Item3;
		HttpResponseMessage response = null;
		try
		{
			response = await base.SendAsync(request, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
			ThrowForNullResponse(response);
			response.EnsureSuccessStatusCode();
			HttpContent content = response.Content;
			if (HttpTelemetry.Log.IsEnabled() & telemetryStarted)
			{
				HttpTelemetry.Log.ResponseContentStart();
				responseContentTelemetryStarted = true;
			}
			using HttpContent.LimitArrayPoolWriteStream buffer = new HttpContent.LimitArrayPoolWriteStream(_maxResponseContentBufferSize, content.Headers.ContentLength.GetValueOrDefault(), getFinalSizeFromPool: false);
			Stream stream = content.TryReadAsStream();
			if (stream == null)
			{
				stream = await content.ReadAsStreamAsync(cts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
			using Stream responseStream = stream;
			_ = 2;
			try
			{
				await responseStream.CopyToAsync(buffer, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception e) when (HttpContent.StreamCopyExceptionNeedsWrapping(e))
			{
				throw HttpContent.WrapStreamCopyException(e);
			}
			return buffer.ToArray();
		}
		catch (Exception e2)
		{
			HandleFailure(e2, telemetryStarted, response, cts, cancellationToken, pendingRequestsCts);
			throw;
		}
		finally
		{
			FinishSend(response, cts, disposeCts, telemetryStarted, responseContentTelemetryStarted);
		}
	}

	/// <summary>Send a GET request to the specified Uri and return the response body as a stream in an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<Stream> GetStreamAsync([StringSyntax("Uri")] string? requestUri)
	{
		return GetStreamAsync(CreateUri(requestUri));
	}

	public Task<Stream> GetStreamAsync([StringSyntax("Uri")] string? requestUri, CancellationToken cancellationToken)
	{
		return GetStreamAsync(CreateUri(requestUri), cancellationToken);
	}

	/// <summary>Send a GET request to the specified Uri and return the response body as a stream in an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<Stream> GetStreamAsync(Uri? requestUri)
	{
		return GetStreamAsync(requestUri, CancellationToken.None);
	}

	public Task<Stream> GetStreamAsync(Uri? requestUri, CancellationToken cancellationToken)
	{
		HttpRequestMessage request = CreateRequestMessage(HttpMethod.Get, requestUri);
		CheckRequestBeforeSend(request);
		return GetStreamAsyncCore(request, cancellationToken);
	}

	private async Task<Stream> GetStreamAsyncCore(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		bool telemetryStarted = StartSend(request);
		(CancellationTokenSource, bool, CancellationTokenSource) tuple = PrepareCancellationTokenSource(cancellationToken);
		CancellationTokenSource cts = tuple.Item1;
		bool disposeCts = tuple.Item2;
		CancellationTokenSource pendingRequestsCts = tuple.Item3;
		HttpResponseMessage response = null;
		try
		{
			response = await base.SendAsync(request, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
			ThrowForNullResponse(response);
			response.EnsureSuccessStatusCode();
			HttpContent content = response.Content;
			Stream stream = content.TryReadAsStream();
			if (stream == null)
			{
				stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			return stream;
		}
		catch (Exception e)
		{
			HandleFailure(e, telemetryStarted, response, cts, cancellationToken, pendingRequestsCts);
			throw;
		}
		finally
		{
			FinishSend(response, cts, disposeCts, telemetryStarted, responseContentTelemetryStarted: false);
		}
	}

	/// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> GetAsync([StringSyntax("Uri")] string? requestUri)
	{
		return GetAsync(CreateUri(requestUri));
	}

	/// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> GetAsync(Uri? requestUri)
	{
		return GetAsync(requestUri, HttpCompletionOption.ResponseContentRead);
	}

	/// <summary>Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="completionOption">An HTTP completion option value that indicates when the operation should be considered completed.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> GetAsync([StringSyntax("Uri")] string? requestUri, HttpCompletionOption completionOption)
	{
		return GetAsync(CreateUri(requestUri), completionOption);
	}

	/// <summary>Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="completionOption">An HTTP completion option value that indicates when the operation should be considered completed.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> GetAsync(Uri? requestUri, HttpCompletionOption completionOption)
	{
		return GetAsync(requestUri, completionOption, CancellationToken.None);
	}

	/// <summary>Send a GET request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> GetAsync([StringSyntax("Uri")] string? requestUri, CancellationToken cancellationToken)
	{
		return GetAsync(CreateUri(requestUri), cancellationToken);
	}

	/// <summary>Send a GET request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> GetAsync(Uri? requestUri, CancellationToken cancellationToken)
	{
		return GetAsync(requestUri, HttpCompletionOption.ResponseContentRead, cancellationToken);
	}

	/// <summary>Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> GetAsync([StringSyntax("Uri")] string? requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
	{
		return GetAsync(CreateUri(requestUri), completionOption, cancellationToken);
	}

	/// <summary>Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> GetAsync(Uri? requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
	{
		return SendAsync(CreateRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
	}

	/// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="content">The HTTP request content sent to the server.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> PostAsync([StringSyntax("Uri")] string? requestUri, HttpContent? content)
	{
		return PostAsync(CreateUri(requestUri), content);
	}

	/// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="content">The HTTP request content sent to the server.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> PostAsync(Uri? requestUri, HttpContent? content)
	{
		return PostAsync(requestUri, content, CancellationToken.None);
	}

	/// <summary>Send a POST request with a cancellation token as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="content">The HTTP request content sent to the server.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> PostAsync([StringSyntax("Uri")] string? requestUri, HttpContent? content, CancellationToken cancellationToken)
	{
		return PostAsync(CreateUri(requestUri), content, cancellationToken);
	}

	/// <summary>Send a POST request with a cancellation token as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="content">The HTTP request content sent to the server.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> PostAsync(Uri? requestUri, HttpContent? content, CancellationToken cancellationToken)
	{
		HttpRequestMessage httpRequestMessage = CreateRequestMessage(HttpMethod.Post, requestUri);
		httpRequestMessage.Content = content;
		return SendAsync(httpRequestMessage, cancellationToken);
	}

	/// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="content">The HTTP request content sent to the server.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> PutAsync([StringSyntax("Uri")] string? requestUri, HttpContent? content)
	{
		return PutAsync(CreateUri(requestUri), content);
	}

	/// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="content">The HTTP request content sent to the server.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> PutAsync(Uri? requestUri, HttpContent? content)
	{
		return PutAsync(requestUri, content, CancellationToken.None);
	}

	/// <summary>Send a PUT request with a cancellation token as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="content">The HTTP request content sent to the server.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> PutAsync([StringSyntax("Uri")] string? requestUri, HttpContent? content, CancellationToken cancellationToken)
	{
		return PutAsync(CreateUri(requestUri), content, cancellationToken);
	}

	/// <summary>Send a PUT request with a cancellation token as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="content">The HTTP request content sent to the server.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> PutAsync(Uri? requestUri, HttpContent? content, CancellationToken cancellationToken)
	{
		HttpRequestMessage httpRequestMessage = CreateRequestMessage(HttpMethod.Put, requestUri);
		httpRequestMessage.Content = content;
		return SendAsync(httpRequestMessage, cancellationToken);
	}

	public Task<HttpResponseMessage> PatchAsync([StringSyntax("Uri")] string? requestUri, HttpContent? content)
	{
		return PatchAsync(CreateUri(requestUri), content);
	}

	public Task<HttpResponseMessage> PatchAsync(Uri? requestUri, HttpContent? content)
	{
		return PatchAsync(requestUri, content, CancellationToken.None);
	}

	public Task<HttpResponseMessage> PatchAsync([StringSyntax("Uri")] string? requestUri, HttpContent? content, CancellationToken cancellationToken)
	{
		return PatchAsync(CreateUri(requestUri), content, cancellationToken);
	}

	public Task<HttpResponseMessage> PatchAsync(Uri? requestUri, HttpContent? content, CancellationToken cancellationToken)
	{
		HttpRequestMessage httpRequestMessage = CreateRequestMessage(HttpMethod.Patch, requestUri);
		httpRequestMessage.Content = content;
		return SendAsync(httpRequestMessage, cancellationToken);
	}

	/// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> DeleteAsync([StringSyntax("Uri")] string? requestUri)
	{
		return DeleteAsync(CreateUri(requestUri));
	}

	/// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> DeleteAsync(Uri? requestUri)
	{
		return DeleteAsync(requestUri, CancellationToken.None);
	}

	/// <summary>Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> DeleteAsync([StringSyntax("Uri")] string? requestUri, CancellationToken cancellationToken)
	{
		return DeleteAsync(CreateUri(requestUri), cancellationToken);
	}

	/// <summary>Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
	/// <param name="requestUri">The Uri the request is sent to.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> DeleteAsync(Uri? requestUri, CancellationToken cancellationToken)
	{
		return SendAsync(CreateRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
	}

	[UnsupportedOSPlatform("browser")]
	public HttpResponseMessage Send(HttpRequestMessage request)
	{
		return Send(request, HttpCompletionOption.ResponseContentRead, default(CancellationToken));
	}

	[UnsupportedOSPlatform("browser")]
	public HttpResponseMessage Send(HttpRequestMessage request, HttpCompletionOption completionOption)
	{
		return Send(request, completionOption, default(CancellationToken));
	}

	[UnsupportedOSPlatform("browser")]
	public override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		return Send(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
	}

	[UnsupportedOSPlatform("browser")]
	public HttpResponseMessage Send(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
	{
		CheckRequestBeforeSend(request);
		(CancellationTokenSource TokenSource, bool DisposeTokenSource, CancellationTokenSource PendingRequestsCts) tuple = PrepareCancellationTokenSource(cancellationToken);
		CancellationTokenSource item = tuple.TokenSource;
		bool item2 = tuple.DisposeTokenSource;
		CancellationTokenSource item3 = tuple.PendingRequestsCts;
		bool flag = StartSend(request);
		bool responseContentTelemetryStarted = false;
		HttpResponseMessage httpResponseMessage = null;
		try
		{
			httpResponseMessage = base.Send(request, item.Token);
			ThrowForNullResponse(httpResponseMessage);
			if (ShouldBufferResponse(completionOption, request))
			{
				if (HttpTelemetry.Log.IsEnabled() & flag)
				{
					HttpTelemetry.Log.ResponseContentStart();
					responseContentTelemetryStarted = true;
				}
				httpResponseMessage.Content.LoadIntoBuffer(_maxResponseContentBufferSize, item.Token);
			}
			return httpResponseMessage;
		}
		catch (Exception e)
		{
			HandleFailure(e, flag, httpResponseMessage, item, cancellationToken, item3);
			throw;
		}
		finally
		{
			FinishSend(httpResponseMessage, item, item2, flag, responseContentTelemetryStarted);
		}
	}

	/// <summary>Send an HTTP request as an asynchronous operation.</summary>
	/// <param name="request">The HTTP request message to send.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
	{
		return SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
	}

	/// <summary>Send an HTTP request as an asynchronous operation.</summary>
	/// <param name="request">The HTTP request message to send.</param>
	/// <param name="cancellationToken">The cancellation token to cancel operation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		return SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
	}

	/// <summary>Send an HTTP request as an asynchronous operation.</summary>
	/// <param name="request">The HTTP request message to send.</param>
	/// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
	{
		return SendAsync(request, completionOption, CancellationToken.None);
	}

	/// <summary>Send an HTTP request as an asynchronous operation.</summary>
	/// <param name="request">The HTTP request message to send.</param>
	/// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
	/// <param name="cancellationToken">The cancellation token to cancel operation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
	{
		CheckRequestBeforeSend(request);
		var (cts, disposeCts, pendingRequestsCts) = PrepareCancellationTokenSource(cancellationToken);
		return Core(request, completionOption, cts, disposeCts, pendingRequestsCts, cancellationToken);
		async Task<HttpResponseMessage> Core(HttpRequestMessage request2, HttpCompletionOption completionOption2, CancellationTokenSource cancellationTokenSource, bool disposeCts2, CancellationTokenSource pendingRequestsCts2, CancellationToken originalCancellationToken)
		{
			bool telemetryStarted = StartSend(request2);
			bool responseContentTelemetryStarted = false;
			HttpResponseMessage response = null;
			try
			{
				response = await base.SendAsync(request2, cancellationTokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
				ThrowForNullResponse(response);
				if (ShouldBufferResponse(completionOption2, request2))
				{
					if (HttpTelemetry.Log.IsEnabled() & telemetryStarted)
					{
						HttpTelemetry.Log.ResponseContentStart();
						responseContentTelemetryStarted = true;
					}
					await response.Content.LoadIntoBufferAsync(_maxResponseContentBufferSize, cancellationTokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
				}
				return response;
			}
			catch (Exception e)
			{
				HandleFailure(e, telemetryStarted, response, cancellationTokenSource, originalCancellationToken, pendingRequestsCts2);
				throw;
			}
			finally
			{
				FinishSend(response, cancellationTokenSource, disposeCts2, telemetryStarted, responseContentTelemetryStarted);
			}
		}
	}

	private void CheckRequestBeforeSend(HttpRequestMessage request)
	{
		ArgumentNullException.ThrowIfNull(request, "request");
		ObjectDisposedException.ThrowIf(_disposed, this);
		CheckRequestMessage(request);
		SetOperationStarted();
		PrepareRequestMessage(request);
	}

	private static void ThrowForNullResponse([NotNull] HttpResponseMessage response)
	{
		if (response == null)
		{
			throw new InvalidOperationException(System.SR.net_http_handler_noresponse);
		}
	}

	private static bool ShouldBufferResponse(HttpCompletionOption completionOption, HttpRequestMessage request)
	{
		if (completionOption == HttpCompletionOption.ResponseContentRead)
		{
			return !string.Equals(request.Method.Method, "HEAD", StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	private void HandleFailure(Exception e, bool telemetryStarted, HttpResponseMessage response, CancellationTokenSource cts, CancellationToken cancellationToken, CancellationTokenSource pendingRequestsCts)
	{
		response?.Dispose();
		Exception ex = null;
		if (e is OperationCanceledException ex2)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				if (ex2.CancellationToken != cancellationToken)
				{
					e = (ex = new TaskCanceledException(ex2.Message, ex2, cancellationToken));
				}
			}
			else if (cts.IsCancellationRequested && !pendingRequestsCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
			{
				e = (ex = new TaskCanceledException(System.SR.Format(System.SR.net_http_request_timedout, _timeout.TotalSeconds), new TimeoutException(e.Message, e), ex2.CancellationToken));
			}
		}
		else if (e is HttpRequestException && cts.IsCancellationRequested)
		{
			e = (ex = CancellationHelper.CreateOperationCanceledException(e, cancellationToken.IsCancellationRequested ? cancellationToken : cts.Token));
		}
		HttpMessageInvoker.LogRequestFailed(e, telemetryStarted);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Error(this, e, "HandleFailure");
		}
		if (ex != null)
		{
			throw ex;
		}
	}

	private static bool StartSend(HttpRequestMessage request)
	{
		if (HttpTelemetry.Log.IsEnabled())
		{
			HttpTelemetry.Log.RequestStart(request);
			return true;
		}
		return false;
	}

	private static void FinishSend(HttpResponseMessage response, CancellationTokenSource cts, bool disposeCts, bool telemetryStarted, bool responseContentTelemetryStarted)
	{
		if (HttpTelemetry.Log.IsEnabled() & telemetryStarted)
		{
			if (responseContentTelemetryStarted)
			{
				HttpTelemetry.Log.ResponseContentStop();
			}
			HttpTelemetry.Log.RequestStop(response);
		}
		if (disposeCts)
		{
			cts.Dispose();
		}
	}

	/// <summary>Cancel all pending requests on this instance.</summary>
	public void CancelPendingRequests()
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		CancellationTokenSource cancellationTokenSource = Interlocked.Exchange(ref _pendingRequestsCts, new CancellationTokenSource());
		cancellationTokenSource.Cancel();
		cancellationTokenSource.Dispose();
	}

	/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpClient" /> and optionally disposes of the managed resources.</summary>
	/// <param name="disposing">
	///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && !_disposed)
		{
			_disposed = true;
			_pendingRequestsCts.Cancel();
			_pendingRequestsCts.Dispose();
		}
		base.Dispose(disposing);
	}

	private void SetOperationStarted()
	{
		if (!_operationStarted)
		{
			_operationStarted = true;
		}
	}

	private void CheckDisposedOrStarted()
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		if (_operationStarted)
		{
			throw new InvalidOperationException(System.SR.net_http_operation_started);
		}
	}

	private static void CheckRequestMessage(HttpRequestMessage request)
	{
		if (!request.MarkAsSent())
		{
			throw new InvalidOperationException(System.SR.net_http_client_request_already_sent);
		}
	}

	private void PrepareRequestMessage(HttpRequestMessage request)
	{
		Uri uri = null;
		if (request.RequestUri == null && _baseAddress == null)
		{
			throw new InvalidOperationException(System.SR.net_http_client_invalid_requesturi);
		}
		if (request.RequestUri == null)
		{
			uri = _baseAddress;
		}
		else if (!request.RequestUri.IsAbsoluteUri)
		{
			if (_baseAddress == null)
			{
				throw new InvalidOperationException(System.SR.net_http_client_invalid_requesturi);
			}
			uri = new Uri(_baseAddress, request.RequestUri);
		}
		if (uri != null)
		{
			request.RequestUri = uri;
		}
		if (_defaultRequestHeaders != null)
		{
			request.Headers.AddHeaders(_defaultRequestHeaders);
		}
	}

	private (CancellationTokenSource TokenSource, bool DisposeTokenSource, CancellationTokenSource PendingRequestsCts) PrepareCancellationTokenSource(CancellationToken cancellationToken)
	{
		CancellationTokenSource pendingRequestsCts = _pendingRequestsCts;
		bool flag = _timeout != s_infiniteTimeout;
		if (flag || cancellationToken.CanBeCanceled)
		{
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, pendingRequestsCts.Token);
			if (flag)
			{
				cancellationTokenSource.CancelAfter(_timeout);
			}
			return (TokenSource: cancellationTokenSource, DisposeTokenSource: true, PendingRequestsCts: pendingRequestsCts);
		}
		return (TokenSource: pendingRequestsCts, DisposeTokenSource: false, PendingRequestsCts: pendingRequestsCts);
	}

	private static Uri CreateUri(string uri)
	{
		if (!string.IsNullOrEmpty(uri))
		{
			return new Uri(uri, UriKind.RelativeOrAbsolute);
		}
		return null;
	}

	private HttpRequestMessage CreateRequestMessage(HttpMethod method, Uri uri)
	{
		return new HttpRequestMessage(method, uri)
		{
			Version = _defaultRequestVersion,
			VersionPolicy = _defaultVersionPolicy
		};
	}
}
