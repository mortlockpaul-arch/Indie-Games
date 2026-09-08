using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Http.HPack;
using System.Net.Http.Headers;
using System.Net.Http.QPack;
using System.Net.Quic;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

internal sealed class HttpConnectionPool : IDisposable
{
	private static readonly List<SslApplicationProtocol> s_http3ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 };

	private static readonly List<SslApplicationProtocol> s_http2ApplicationProtocols = new List<SslApplicationProtocol>
	{
		SslApplicationProtocol.Http2,
		SslApplicationProtocol.Http11
	};

	private static readonly List<SslApplicationProtocol> s_http2OnlyApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http2 };

	private readonly HttpConnectionPoolManager _poolManager;

	private readonly HttpConnectionKind _kind;

	private readonly Uri _proxyUri;

	private readonly string _telemetryServerAddress;

	private readonly HttpAuthority _originAuthority;

	private string _connectTunnelUserAgent;

	internal uint _lastSeenHttp2MaxHeaderListSize;

	internal uint _lastSeenHttp3MaxHeaderListSize;

	private readonly SslClientAuthenticationOptions _sslOptionsHttp11;

	private readonly SslClientAuthenticationOptions _sslOptionsHttp2;

	private readonly SslClientAuthenticationOptions _sslOptionsHttp2Only;

	private SslClientAuthenticationOptions _sslOptionsHttp3;

	private readonly SslClientAuthenticationOptions _sslOptionsProxy;

	private readonly PreAuthCredentialCache _preAuthCredentials;

	private bool _usedSinceLastCleanup = true;

	private bool _disposed;

	private readonly ConcurrentStack<HttpConnection> _http11Connections = new ConcurrentStack<HttpConnection>();

	private bool _http11RequestQueueIsEmptyAndNotDisposed;

	private readonly int _maxHttp11Connections;

	private int _associatedHttp11ConnectionCount;

	private int _pendingHttp11ConnectionCount;

	private RequestQueue<HttpConnection> _http11RequestQueue;

	private readonly byte[] _hostHeaderLineBytes;

	private List<Http2Connection> _availableHttp2Connections;

	private int _associatedHttp2ConnectionCount;

	private bool _pendingHttp2Connection;

	private RequestQueue<Http2Connection> _http2RequestQueue;

	private bool _http2Enabled;

	private byte[] _http2AltSvcOriginUri;

	internal readonly byte[] _http2EncodedAuthorityHostHeader;

	private List<Http3Connection> _availableHttp3Connections;

	private int _associatedHttp3ConnectionCount;

	private bool _pendingHttp3Connection;

	private RequestQueue<Http3Connection> _http3RequestQueue;

	private bool _http3Enabled;

	internal readonly byte[] _http3EncodedAuthorityHostHeader;

	private volatile HttpAuthority _http3Authority;

	private Timer _authorityExpireTimer;

	private bool _persistAuthority;

	private volatile Dictionary<HttpAuthority, Exception> _altSvcBlocklist;

	private CancellationTokenSource _altSvcBlocklistTimerCancellation;

	private volatile bool _altSvcEnabled = true;

	public string TelemetryServerAddress => _telemetryServerAddress;

	public HttpAuthority OriginAuthority => _originAuthority;

	public HttpConnectionSettings Settings => _poolManager.Settings;

	public HttpConnectionKind Kind => _kind;

	public bool IsSecure
	{
		get
		{
			if (_kind != HttpConnectionKind.Https && _kind != HttpConnectionKind.SslProxyTunnel)
			{
				return _kind == HttpConnectionKind.SslSocksTunnel;
			}
			return true;
		}
	}

	public Uri ProxyUri => _proxyUri;

	public ICredentials ProxyCredentials => _poolManager.ProxyCredentials;

	public PreAuthCredentialCache PreAuthCredentials => _preAuthCredentials;

	public bool IsDefaultPort => OriginAuthority.Port == (IsSecure ? 443 : 80);

	private bool DoProxyAuth
	{
		get
		{
			if (_kind != HttpConnectionKind.Proxy)
			{
				return _kind == HttpConnectionKind.ProxyConnect;
			}
			return true;
		}
	}

	private object SyncObj => _http11Connections;

	public byte[] HostHeaderLineBytes => _hostHeaderLineBytes;

	public byte[] Http2AltSvcOriginUri
	{
		get
		{
			if (_http2AltSvcOriginUri == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(IsSecure ? "https://" : "http://").Append(_originAuthority.IdnHost);
				if (!IsDefaultPort)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder2, invariantCulture);
					handler.AppendLiteral(":");
					handler.AppendFormatted(_originAuthority.Port);
					stringBuilder2.Append(invariantCulture, ref handler);
				}
				_http2AltSvcOriginUri = Encoding.ASCII.GetBytes(stringBuilder.ToString());
			}
			return _http2AltSvcOriginUri;
		}
	}

	private bool EnableMultipleHttp2Connections => _poolManager.Settings.EnableMultipleHttp2Connections;

	private bool EnableMultipleHttp3Connections => _poolManager.Settings.EnableMultipleHttp3Connections;

	public HttpConnectionPool(HttpConnectionPoolManager poolManager, HttpConnectionKind kind, string host, int port, string sslHostName, Uri proxyUri, string telemetryServerAddress)
	{
		_poolManager = poolManager;
		_kind = kind;
		_proxyUri = proxyUri;
		_maxHttp11Connections = Settings._maxConnectionsPerServer;
		_telemetryServerAddress = telemetryServerAddress;
		_originAuthority = new HttpAuthority(host ?? proxyUri.IdnHost, port);
		_http2Enabled = _poolManager.Settings._maxHttpVersion >= HttpVersion.Version20;
		if (GlobalHttpSettings.SocketsHttpHandler.AllowHttp3)
		{
			_http3Enabled = _poolManager.Settings._maxHttpVersion >= HttpVersion.Version30;
		}
		switch (kind)
		{
		case HttpConnectionKind.Http:
			_http3Enabled = false;
			break;
		case HttpConnectionKind.Proxy:
			_http2Enabled = false;
			_http3Enabled = false;
			break;
		case HttpConnectionKind.ProxyTunnel:
			_http2Enabled = false;
			_http3Enabled = false;
			break;
		case HttpConnectionKind.SslProxyTunnel:
			_http3Enabled = false;
			break;
		case HttpConnectionKind.ProxyConnect:
			_maxHttp11Connections = int.MaxValue;
			_http2Enabled = false;
			_http3Enabled = false;
			break;
		case HttpConnectionKind.SocksTunnel:
		case HttpConnectionKind.SslSocksTunnel:
			_http3Enabled = false;
			break;
		}
		if (!_http3Enabled)
		{
			_altSvcEnabled = false;
		}
		string text = null;
		if (host != null)
		{
			text = (IsDefaultPort ? _originAuthority.HostValue : $"{_originAuthority.HostValue}:{_originAuthority.Port}");
			byte[] array = new byte[6 + text.Length + 2];
			"Host: "u8.CopyTo(array);
			Encoding.ASCII.GetBytes(text.AsSpan(), array.AsSpan(6));
			array[^2] = 13;
			array[^1] = 10;
			_hostHeaderLineBytes = array;
		}
		if (sslHostName != null)
		{
			_sslOptionsHttp11 = ConstructSslOptions(poolManager, sslHostName);
			_sslOptionsHttp11.ApplicationProtocols = null;
			if (_http2Enabled)
			{
				_sslOptionsHttp2 = ConstructSslOptions(poolManager, sslHostName);
				_sslOptionsHttp2.ApplicationProtocols = s_http2ApplicationProtocols;
				_sslOptionsHttp2Only = ConstructSslOptions(poolManager, sslHostName);
				_sslOptionsHttp2Only.ApplicationProtocols = s_http2OnlyApplicationProtocols;
			}
		}
		if (text != null)
		{
			if (_http2Enabled)
			{
				_http2EncodedAuthorityHostHeader = HPackEncoder.EncodeLiteralHeaderFieldWithoutIndexingToAllocatedArray(1, text);
			}
			if (GlobalHttpSettings.SocketsHttpHandler.AllowHttp3 && _http3Enabled)
			{
				_http3EncodedAuthorityHostHeader = QPackEncoder.EncodeLiteralHeaderFieldWithStaticNameReferenceToArray(0, text);
			}
		}
		if (_poolManager.Settings._preAuthenticate)
		{
			_preAuthCredentials = new PreAuthCredentialCache();
		}
		_http11RequestQueue = new RequestQueue<HttpConnection>();
		if (_http2Enabled)
		{
			_http2RequestQueue = new RequestQueue<Http2Connection>();
		}
		if (GlobalHttpSettings.SocketsHttpHandler.AllowHttp3 && _http3Enabled)
		{
			_http3RequestQueue = new RequestQueue<Http3Connection>();
		}
		if (_proxyUri != null && HttpUtilities.IsSupportedSecureScheme(_proxyUri.Scheme))
		{
			_sslOptionsProxy = ConstructSslOptions(poolManager, _proxyUri.IdnHost);
			_sslOptionsProxy.ApplicationProtocols = null;
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace($"{this}", ".ctor");
		}
	}

	private static SslClientAuthenticationOptions ConstructSslOptions(HttpConnectionPoolManager poolManager, string sslHostName)
	{
		SslClientAuthenticationOptions sslClientAuthenticationOptions = poolManager.Settings._sslOptions?.ShallowClone() ?? new SslClientAuthenticationOptions();
		if (poolManager.Settings._clientCertificateOptions == ClientCertificateOption.Manual && sslClientAuthenticationOptions.LocalCertificateSelectionCallback != null && (sslClientAuthenticationOptions.ClientCertificates == null || sslClientAuthenticationOptions.ClientCertificates.Count == 0))
		{
			sslClientAuthenticationOptions.LocalCertificateSelectionCallback = null;
		}
		sslClientAuthenticationOptions.TargetHost = sslHostName;
		return sslClientAuthenticationOptions;
	}

	public ValueTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, bool async, bool doRequestAuth, CancellationToken cancellationToken)
	{
		HttpConnectionKind kind = Kind;
		bool flag = kind - 3 <= HttpConnectionKind.Https;
		if (flag && request.HasHeaders && request.Headers.NonValidated.TryGetValues("User-Agent", out var values))
		{
			_connectTunnelUserAgent = values.ToString();
		}
		if (doRequestAuth && Settings._credentials != null)
		{
			return AuthenticationHelper.SendWithRequestAuthAsync(request, async, Settings._credentials, Settings._preAuthenticate, this, cancellationToken);
		}
		return SendWithProxyAuthAsync(request, async, doRequestAuth, cancellationToken);
	}

	public ValueTask<HttpResponseMessage> SendWithProxyAuthAsync(HttpRequestMessage request, bool async, bool doRequestAuth, CancellationToken cancellationToken)
	{
		if (DoProxyAuth && ProxyCredentials != null)
		{
			return AuthenticationHelper.SendWithProxyAuthAsync(request, _proxyUri, async, ProxyCredentials, doRequestAuth, this, cancellationToken);
		}
		return SendWithVersionDetectionAndRetryAsync(request, async, doRequestAuth, cancellationToken);
	}

	private Task<HttpResponseMessage> SendWithNtConnectionAuthAsync(HttpConnection connection, HttpRequestMessage request, bool async, bool doRequestAuth, CancellationToken cancellationToken)
	{
		if (doRequestAuth && Settings._credentials != null)
		{
			return AuthenticationHelper.SendWithNtConnectionAuthAsync(request, async, Settings._credentials, Settings._impersonationLevel, connection, this, cancellationToken);
		}
		return SendWithNtProxyAuthAsync(connection, request, async, cancellationToken);
	}

	public Task<HttpResponseMessage> SendWithNtProxyAuthAsync(HttpConnection connection, HttpRequestMessage request, bool async, CancellationToken cancellationToken)
	{
		if (DoProxyAuth && ProxyCredentials != null)
		{
			return AuthenticationHelper.SendWithNtProxyAuthAsync(request, ProxyUri, async, ProxyCredentials, TokenImpersonationLevel.None, connection, this, cancellationToken);
		}
		return connection.SendAsync(request, async, cancellationToken);
	}

	public async ValueTask<HttpResponseMessage> SendWithVersionDetectionAndRetryAsync(HttpRequestMessage request, bool async, bool doRequestAuth, CancellationToken cancellationToken)
	{
		_usedSinceLastCleanup = true;
		int retryCount = 0;
		while (true)
		{
			HttpConnectionWaiter<HttpConnection> http11ConnectionWaiter = null;
			HttpConnectionWaiter<Http2Connection> http2ConnectionWaiter = null;
			try
			{
				HttpResponseMessage response = null;
				if (GlobalHttpSettings.SocketsHttpHandler.AllowHttp3 && _http3Enabled && (request.Version.Major >= 3 || (request.VersionPolicy == HttpVersionPolicy.RequestVersionOrHigher && IsSecure)) && !request.IsExtendedConnectRequest)
				{
					if (QuicConnection.IsSupported)
					{
						if (_sslOptionsHttp3 == null)
						{
							SslClientAuthenticationOptions sslClientAuthenticationOptions = ConstructSslOptions(_poolManager, _sslOptionsHttp11.TargetHost);
							sslClientAuthenticationOptions.ApplicationProtocols = s_http3ApplicationProtocols;
							Interlocked.CompareExchange(ref _sslOptionsHttp3, sslClientAuthenticationOptions, null);
						}
						response = await TrySendUsingHttp3Async(request, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
					else
					{
						_altSvcEnabled = false;
						_http3Enabled = false;
					}
				}
				if (response == null)
				{
					if (request.Version.Major >= 3 && request.VersionPolicy != HttpVersionPolicy.RequestVersionOrLower)
					{
						ThrowGetVersionException(request, 3);
					}
					if (_http2Enabled && (request.Version.Major >= 2 || (request.VersionPolicy == HttpVersionPolicy.RequestVersionOrHigher && IsSecure)) && (request.VersionPolicy != HttpVersionPolicy.RequestVersionOrLower || IsSecure))
					{
						if (!TryGetPooledHttp2Connection(request, out var connection, out http2ConnectionWaiter) && http2ConnectionWaiter != null)
						{
							connection = await http2ConnectionWaiter.WaitForConnectionAsync(request, this, async, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						}
						if (connection != null)
						{
							if (request.IsExtendedConnectRequest)
							{
								await connection.InitialSettingsReceived.WaitWithCancellationAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
								if (!connection.IsConnectEnabled)
								{
									HttpRequestException ex = new HttpRequestException(HttpRequestError.ExtendedConnectNotSupported, System.SR.net_unsupported_extended_connect);
									ex.Data["SETTINGS_ENABLE_CONNECT_PROTOCOL"] = false;
									throw ex;
								}
							}
							response = await connection.SendAsync(request, async, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						}
						connection = null;
					}
					if (response == null)
					{
						if (request.Version.Major >= 2 && request.VersionPolicy != HttpVersionPolicy.RequestVersionOrLower)
						{
							ThrowGetVersionException(request, 2);
						}
						if (!TryGetPooledHttp11Connection(request, async, out var connection2, out http11ConnectionWaiter))
						{
							connection2 = await http11ConnectionWaiter.WaitForConnectionAsync(request, this, async, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						}
						connection2.Acquire();
						try
						{
							response = await SendWithNtConnectionAuthAsync(connection2, request, async, doRequestAuth, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						}
						finally
						{
							connection2.Release();
						}
						connection2 = null;
					}
				}
				ProcessAltSvc(response);
				return response;
			}
			catch (HttpRequestException ex2) when (ex2.AllowRetry == RequestRetryType.RetryOnConnectionFailure)
			{
				if (retryCount == 3)
				{
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						Trace($"MaxConnectionFailureRetries limit of {3} hit. Retryable request will not be retried. Exception: {ex2}", "SendWithVersionDetectionAndRetryAsync");
					}
					throw;
				}
				retryCount++;
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					Trace($"Retry attempt {retryCount} after connection failure. Connection exception: {ex2}", "SendWithVersionDetectionAndRetryAsync");
				}
			}
			catch (HttpRequestException ex3) when (ex3.AllowRetry == RequestRetryType.RetryOnLowerHttpVersion)
			{
				if (request.VersionPolicy != HttpVersionPolicy.RequestVersionOrLower)
				{
					throw new HttpRequestException(HttpRequestError.VersionNegotiationError, System.SR.Format(System.SR.net_http_requested_version_server_refused, request.Version, request.VersionPolicy), ex3);
				}
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					Trace($"Retrying request because server requested version fallback: {ex3}", "SendWithVersionDetectionAndRetryAsync");
				}
				request.Version = HttpVersion.Version11;
			}
			catch (HttpRequestException ex4) when (ex4.AllowRetry == RequestRetryType.RetryOnStreamLimitReached)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					Trace($"Retrying request on another HTTP/2 connection after active streams limit is reached on existing one: {ex4}", "SendWithVersionDetectionAndRetryAsync");
				}
			}
			finally
			{
				http11ConnectionWaiter?.SetTimeoutToPendingConnectionAttempt(this, cancellationToken.IsCancellationRequested);
				http2ConnectionWaiter?.SetTimeoutToPendingConnectionAttempt(this, cancellationToken.IsCancellationRequested);
			}
		}
	}

	private async ValueTask<(Stream, TransportContext, Activity, IPEndPoint)> ConnectAsync(HttpRequestMessage request, bool async, CancellationToken cancellationToken)
	{
		Stream stream = null;
		IPEndPoint remoteEndPoint = null;
		Exception exception = null;
		TransportContext transportContext = null;
		Activity activity = ConnectionSetupDistributedTracing.StartConnectionSetupActivity(IsSecure, _telemetryServerAddress, OriginAuthority.Port);
		try
		{
			switch (_kind)
			{
			case HttpConnectionKind.Http:
			case HttpConnectionKind.Https:
			case HttpConnectionKind.ProxyConnect:
				stream = await ConnectToTcpHostAsync(_originAuthority.IdnHost, _originAuthority.Port, request, async, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				remoteEndPoint = GetRemoteEndPoint(stream);
				if (_kind == HttpConnectionKind.ProxyConnect && _sslOptionsProxy != null)
				{
					stream = await ConnectHelper.EstablishSslConnectionAsync(_sslOptionsProxy, request, async, stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				break;
			case HttpConnectionKind.Proxy:
				stream = await ConnectToTcpHostAsync(_proxyUri.IdnHost, _proxyUri.Port, request, async, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				remoteEndPoint = GetRemoteEndPoint(stream);
				if (_sslOptionsProxy != null)
				{
					stream = await ConnectHelper.EstablishSslConnectionAsync(_sslOptionsProxy, request, async, stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				break;
			case HttpConnectionKind.ProxyTunnel:
			case HttpConnectionKind.SslProxyTunnel:
				stream = await EstablishProxyTunnelAsync(async, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				if (stream is HttpContentStream httpContentStream)
				{
					Stream stream2 = httpContentStream._connection?._stream;
					if (stream2 != null)
					{
						remoteEndPoint = GetRemoteEndPoint(stream2);
					}
				}
				break;
			case HttpConnectionKind.SocksTunnel:
			case HttpConnectionKind.SslSocksTunnel:
				stream = await EstablishSocksTunnel(request, async, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				remoteEndPoint = GetRemoteEndPoint(stream);
				break;
			}
			if (IsSecure)
			{
				SslStream sslStream = stream as SslStream;
				if (sslStream == null)
				{
					sslStream = await ConnectHelper.EstablishSslConnectionAsync(GetSslOptionsForRequest(request), request, async, stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				else if (System.Net.NetEventSource.Log.IsEnabled())
				{
					Trace($"Connected with custom SslStream: alpn='${sslStream.NegotiatedApplicationProtocol}'", "ConnectAsync");
				}
				transportContext = sslStream.TransportContext;
				stream = sslStream;
			}
		}
		catch (Exception ex) when (activity != null)
		{
			exception = ex;
			throw;
		}
		finally
		{
			if (activity != null)
			{
				ConnectionSetupDistributedTracing.StopConnectionSetupActivity(activity, exception, remoteEndPoint);
			}
		}
		return (stream, transportContext, activity, remoteEndPoint);
		static IPEndPoint GetRemoteEndPoint(Stream stream3)
		{
			return (stream3 as NetworkStream)?.Socket?.RemoteEndPoint as IPEndPoint;
		}
	}

	private async ValueTask<Stream> ConnectToTcpHostAsync(string host, int port, HttpRequestMessage initialRequest, bool async, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		DnsEndPoint dnsEndPoint = new DnsEndPoint(host, port);
		try
		{
			Stream result;
			if (Settings._connectCallback != null)
			{
				ValueTask<Stream> valueTask = Settings._connectCallback(new SocketsHttpConnectionContext(dnsEndPoint, initialRequest), cancellationToken);
				if (!async && !valueTask.IsCompleted)
				{
					Trace("ConnectCallback completing asynchronously for a synchronous request.", "ConnectToTcpHostAsync");
				}
				result = (await valueTask.ConfigureAwait(continueOnCapturedContext: false)) ?? throw new HttpRequestException(System.SR.net_http_null_from_connect_callback);
			}
			else
			{
				Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
				{
					NoDelay = true
				};
				try
				{
					if (async)
					{
						await socket.ConnectAsync(dnsEndPoint, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
					else
					{
						using (cancellationToken.UnsafeRegister(delegate(object s)
						{
							((Socket)s).Dispose();
						}, socket))
						{
							socket.Connect(dnsEndPoint);
						}
					}
					result = new NetworkStream(socket, ownsSocket: true);
				}
				catch
				{
					socket.Dispose();
					throw;
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			throw (ex is OperationCanceledException ex2 && ex2.CancellationToken == cancellationToken) ? CancellationHelper.CreateOperationCanceledException(null, cancellationToken) : ConnectHelper.CreateWrappedException(ex, host, port, cancellationToken);
		}
	}

	private SslClientAuthenticationOptions GetSslOptionsForRequest(HttpRequestMessage request)
	{
		if (_http2Enabled)
		{
			if (request.Version.Major >= 2 && request.VersionPolicy != HttpVersionPolicy.RequestVersionOrLower)
			{
				return _sslOptionsHttp2Only;
			}
			if (request.Version.Major >= 2 || request.VersionPolicy == HttpVersionPolicy.RequestVersionOrHigher)
			{
				return _sslOptionsHttp2;
			}
		}
		return _sslOptionsHttp11;
	}

	private async ValueTask<Stream> ApplyPlaintextFilterAsync(bool async, Stream stream, Version httpVersion, HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (Settings._plaintextStreamFilter == null)
		{
			return stream;
		}
		Stream stream2;
		try
		{
			ValueTask<Stream> valueTask = Settings._plaintextStreamFilter(new SocketsHttpPlaintextStreamFilterContext(stream, httpVersion, request), cancellationToken);
			if (!async && !valueTask.IsCompleted)
			{
				Trace("PlaintextStreamFilter completing asynchronously for a synchronous request.", "ApplyPlaintextFilterAsync");
			}
			stream2 = await valueTask.ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
		{
			stream.Dispose();
			throw;
		}
		catch (Exception inner)
		{
			stream.Dispose();
			throw new HttpRequestException(System.SR.net_http_exception_during_plaintext_filter, inner);
		}
		if (stream2 == null)
		{
			stream.Dispose();
			throw new HttpRequestException(System.SR.net_http_null_from_plaintext_filter);
		}
		return stream2;
	}

	private async ValueTask<Stream> EstablishProxyTunnelAsync(bool async, CancellationToken cancellationToken)
	{
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Connect, _proxyUri);
		httpRequestMessage.Headers.Host = $"{_originAuthority.IdnHost}:{_originAuthority.Port}";
		if (_connectTunnelUserAgent != null)
		{
			httpRequestMessage.Headers.TryAddWithoutValidation(KnownHeaders.UserAgent.Descriptor, _connectTunnelUserAgent);
		}
		HttpResponseMessage httpResponseMessage = await _poolManager.SendProxyConnectAsync(httpRequestMessage, _proxyUri, async, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
		{
			httpResponseMessage.Dispose();
			throw new HttpRequestException(HttpRequestError.ProxyTunnelError, System.SR.Format(System.SR.net_http_proxy_tunnel_returned_failure_status_code, _proxyUri, (int)httpResponseMessage.StatusCode), null, httpResponseMessage.StatusCode);
		}
		try
		{
			return httpResponseMessage.Content.ReadAsStream(cancellationToken);
		}
		catch
		{
			httpResponseMessage.Dispose();
			throw;
		}
	}

	private async ValueTask<Stream> EstablishSocksTunnel(HttpRequestMessage request, bool async, CancellationToken cancellationToken)
	{
		Stream stream = await ConnectToTcpHostAsync(_proxyUri.IdnHost, _proxyUri.Port, request, async, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		try
		{
			await SocksHelper.EstablishSocksTunnelAsync(stream, _originAuthority.IdnHost, _originAuthority.Port, _proxyUri, ProxyCredentials, async, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return stream;
		}
		catch (Exception ex) when (!(ex is OperationCanceledException))
		{
			throw new HttpRequestException(HttpRequestError.ProxyTunnelError, System.SR.net_http_proxy_tunnel_error, ex);
		}
	}

	private CancellationTokenSource GetConnectTimeoutCancellationTokenSource<T>(HttpConnectionWaiter<T> waiter) where T : HttpConnectionBase
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(Settings._connectTimeout);
		lock (waiter)
		{
			waiter.ConnectionCancellationTokenSource = cancellationTokenSource;
			if (waiter.Task.IsCompleted)
			{
				waiter.SetTimeoutToPendingConnectionAttempt(this, waiter.Task.IsCanceled);
				waiter.ConnectionCancellationTokenSource = null;
			}
		}
		return cancellationTokenSource;
	}

	private static Exception CreateConnectTimeoutException(OperationCanceledException oce)
	{
		Exception ex = CancellationHelper.CreateOperationCanceledException(new TimeoutException(System.SR.net_http_connect_timedout, oce.InnerException), oce.CancellationToken);
		ExceptionDispatchInfo.SetCurrentStackTrace(ex);
		return ex;
	}

	[DoesNotReturn]
	private static void ThrowGetVersionException(HttpRequestMessage request, int desiredVersion, Exception inner = null)
	{
		HttpRequestException ex = new HttpRequestException(HttpRequestError.VersionNegotiationError, System.SR.Format(System.SR.net_http_requested_version_cannot_establish, request.Version, request.VersionPolicy, desiredVersion), inner);
		if (request.IsExtendedConnectRequest && desiredVersion == 2)
		{
			ex.Data["HTTP2_ENABLED"] = false;
		}
		throw ex;
	}

	private bool CheckExpirationOnGet(HttpConnectionBase connection)
	{
		TimeSpan pooledConnectionLifetime = _poolManager.Settings._pooledConnectionLifetime;
		if (pooledConnectionLifetime != Timeout.InfiniteTimeSpan)
		{
			return (double)connection.GetLifetimeTicks(Environment.TickCount64) > pooledConnectionLifetime.TotalMilliseconds;
		}
		return false;
	}

	private bool CheckExpirationOnReturn(HttpConnectionBase connection)
	{
		TimeSpan pooledConnectionLifetime = _poolManager.Settings._pooledConnectionLifetime;
		if (pooledConnectionLifetime != Timeout.InfiniteTimeSpan)
		{
			if (!(pooledConnectionLifetime == TimeSpan.Zero))
			{
				return (double)connection.GetLifetimeTicks(Environment.TickCount64) > pooledConnectionLifetime.TotalMilliseconds;
			}
			return true;
		}
		return false;
	}

	public void Dispose()
	{
		List<HttpConnectionBase> list = null;
		lock (SyncObj)
		{
			if (_disposed)
			{
				return;
			}
			_disposed = true;
			_http11RequestQueueIsEmptyAndNotDisposed = false;
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				Trace("Disposing the pool.", "Dispose");
			}
			if (_availableHttp2Connections != null)
			{
				List<Http2Connection> availableHttp2Connections = _availableHttp2Connections;
				int count = availableHttp2Connections.Count;
				List<HttpConnectionBase> list2 = new List<HttpConnectionBase>(count);
				CollectionsMarshal.SetCount(list2, count);
				Span<HttpConnectionBase> span = CollectionsMarshal.AsSpan(list2);
				int num = 0;
				foreach (Http2Connection item in availableHttp2Connections)
				{
					span[num] = item;
					num++;
				}
				list = list2;
				_associatedHttp2ConnectionCount -= _availableHttp2Connections.Count;
				_availableHttp2Connections.Clear();
			}
			if (GlobalHttpSettings.SocketsHttpHandler.AllowHttp3 && _availableHttp3Connections != null)
			{
				if (list == null)
				{
					list = new List<HttpConnectionBase>();
				}
				list.AddRange(_availableHttp3Connections);
				_associatedHttp3ConnectionCount -= _availableHttp3Connections.Count;
				_availableHttp3Connections.Clear();
			}
			if (_authorityExpireTimer != null)
			{
				_authorityExpireTimer.Dispose();
				_authorityExpireTimer = null;
			}
			if (_altSvcBlocklistTimerCancellation != null)
			{
				_altSvcBlocklistTimerCancellation.Cancel();
				_altSvcBlocklistTimerCancellation.Dispose();
				_altSvcBlocklistTimerCancellation = null;
			}
		}
		ProcessHttp11RequestQueue(null);
		list?.ForEach(delegate(HttpConnectionBase c)
		{
			c.Dispose();
		});
	}

	public bool CleanCacheAndDisposeIfUnused()
	{
		TimeSpan pooledConnectionLifetime = _poolManager.Settings._pooledConnectionLifetime;
		TimeSpan pooledConnectionIdleTimeout = _poolManager.Settings._pooledConnectionIdleTimeout;
		long tickCount = Environment.TickCount64;
		List<HttpConnectionBase> toDispose = null;
		lock (SyncObj)
		{
			if (!_usedSinceLastCleanup && _associatedHttp11ConnectionCount == 0 && _associatedHttp2ConnectionCount == 0)
			{
				_disposed = true;
				return true;
			}
			_usedSinceLastCleanup = false;
			ScavengeHttp11ConnectionStack(this, _http11Connections, ref toDispose, tickCount, pooledConnectionLifetime, pooledConnectionIdleTimeout);
			if (_availableHttp2Connections != null)
			{
				int num = ScavengeHttp2ConnectionList(_availableHttp2Connections, ref toDispose, tickCount, pooledConnectionLifetime, pooledConnectionIdleTimeout);
				_associatedHttp2ConnectionCount -= num;
			}
			if (GlobalHttpSettings.SocketsHttpHandler.AllowHttp3 && _availableHttp3Connections != null)
			{
				int num2 = ScavengeHttp3ConnectionList(_availableHttp3Connections, ref toDispose, tickCount, pooledConnectionLifetime, pooledConnectionIdleTimeout);
				_associatedHttp3ConnectionCount -= num2;
			}
		}
		if (toDispose != null)
		{
			Task.Factory.StartNew(delegate(object s)
			{
				((List<HttpConnectionBase>)s).ForEach(delegate(HttpConnectionBase c)
				{
					c.Dispose();
				});
			}, toDispose, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}
		return false;
	}

	public override string ToString()
	{
		return "HttpConnectionPool " + ((!(_proxyUri == null)) ? ((_sslOptionsHttp11 == null) ? $"Proxy {_proxyUri}" : ($"https://{_originAuthority}/ tunnelled via Proxy {_proxyUri}" + ((_sslOptionsHttp11.TargetHost != _originAuthority.IdnHost) ? (", SSL TargetHost=" + _sslOptionsHttp11.TargetHost) : null))) : ((_sslOptionsHttp11 == null) ? $"http://{_originAuthority}" : ($"https://{_originAuthority}" + ((_sslOptionsHttp11.TargetHost != _originAuthority.IdnHost) ? (", SSL TargetHost=" + _sslOptionsHttp11.TargetHost) : null))));
	}

	public void Trace(string message, [CallerMemberName] string memberName = null)
	{
		System.Net.NetEventSource.Log.HandlerMessage(GetHashCode(), 0, 0, memberName, message);
	}

	private bool TryGetPooledHttp11Connection(HttpRequestMessage request, bool async, [NotNullWhen(true)] out HttpConnection connection, [NotNullWhen(false)] out HttpConnectionWaiter<HttpConnection> waiter)
	{
		while (_http11Connections.TryPop(out connection))
		{
			if (CheckExpirationOnGet(connection))
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					connection.Trace("Found expired HTTP/1.1 connection in pool.", "TryGetPooledHttp11Connection");
				}
				connection.Dispose();
				continue;
			}
			if (!connection.PrepareForReuse(async))
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					connection.Trace("Found invalid HTTP/1.1 connection in pool.", "TryGetPooledHttp11Connection");
				}
				connection.Dispose();
				continue;
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				connection.Trace("Found usable HTTP/1.1 connection in pool.", "TryGetPooledHttp11Connection");
			}
			waiter = null;
			return true;
		}
		waiter = new HttpConnectionWaiter<HttpConnection>();
		lock (SyncObj)
		{
			_http11RequestQueue.EnqueueRequest(request, waiter);
			_http11RequestQueueIsEmptyAndNotDisposed = false;
		}
		ProcessHttp11RequestQueue(null);
		return false;
	}

	private void ProcessHttp11RequestQueue(HttpConnection connection)
	{
		while (true)
		{
			HttpConnectionWaiter<HttpConnection> waiter = null;
			lock (SyncObj)
			{
				if (_http11RequestQueue.Count == 0 || (connection == null && !_http11Connections.TryPop(out connection)))
				{
					goto IL_004b;
				}
				if (connection.TryOwnScavengingTaskCompletion())
				{
					_http11RequestQueue.TryDequeueWaiter(this, out waiter);
					goto IL_004b;
				}
				goto end_IL_000b;
				IL_004b:
				_http11RequestQueueIsEmptyAndNotDisposed = _http11RequestQueue.Count == 0 && !_disposed;
				if (waiter != null)
				{
					goto IL_009c;
				}
				if (connection == null)
				{
					CheckForHttp11ConnectionInjection();
					break;
				}
				if (connection.TryReturnScavengingTaskCompletionOwnership())
				{
					_http11Connections.Push(connection);
					break;
				}
				end_IL_000b:;
			}
			goto IL_00b1;
			IL_009c:
			if (waiter.TrySignal(connection))
			{
				return;
			}
			if (connection.TryReturnScavengingTaskCompletionOwnership())
			{
				continue;
			}
			goto IL_00b1;
			IL_00b1:
			connection.Dispose();
			connection = null;
		}
		if (_disposed)
		{
			while (_http11Connections.TryPop(out connection))
			{
				connection.Dispose();
			}
		}
	}

	private void CheckForHttp11ConnectionInjection()
	{
		_http11RequestQueue.PruneCompletedRequestsFromHeadOfQueue(this);
		bool flag = _http11RequestQueue.Count > _pendingHttp11ConnectionCount && _associatedHttp11ConnectionCount < _maxHttp11Connections && _http11RequestQueue.RequestsWithoutAConnectionAttempt > 0;
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace($"Available HTTP/1.1 connections: {_http11Connections.Count}, Requests in the queue: {_http11RequestQueue.Count}, Requests without a connection attempt: {_http11RequestQueue.RequestsWithoutAConnectionAttempt}, Pending HTTP/1.1 connections: {_pendingHttp11ConnectionCount}, Total associated HTTP/1.1 connections: {_associatedHttp11ConnectionCount}, Max HTTP/1.1 connection limit: {_maxHttp11Connections}, Will inject connection: {flag}.", "CheckForHttp11ConnectionInjection");
		}
		if (flag)
		{
			_associatedHttp11ConnectionCount++;
			_pendingHttp11ConnectionCount++;
			RequestQueue<HttpConnection>.QueueItem queueItem = _http11RequestQueue.PeekNextRequestForConnectionAttempt();
			InjectNewHttp11ConnectionAsync(queueItem);
		}
	}

	private async Task InjectNewHttp11ConnectionAsync(RequestQueue<HttpConnection>.QueueItem queueItem)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace("Creating new HTTP/1.1 connection for pool.", "InjectNewHttp11ConnectionAsync");
		}
		await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
		HttpConnectionWaiter<HttpConnection> waiter = queueItem.Waiter;
		HttpConnection connection = null;
		Exception connectionException = null;
		CancellationTokenSource cts = GetConnectTimeoutCancellationTokenSource(waiter);
		try
		{
			connection = await CreateHttp11ConnectionAsync(queueItem.Request, async: true, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception ex)
		{
			connectionException = ((ex is OperationCanceledException ex2 && ex2.CancellationToken == cts.Token && !waiter.CancelledByOriginatingRequestCompletion) ? CreateConnectTimeoutException(ex2) : ex);
		}
		finally
		{
			lock (waiter)
			{
				waiter.ConnectionCancellationTokenSource = null;
				cts.Dispose();
			}
		}
		if (connection != null)
		{
			AddNewHttp11Connection(connection, queueItem.Waiter);
		}
		else
		{
			HandleHttp11ConnectionFailure(waiter, connectionException);
		}
	}

	internal async ValueTask<HttpConnection> CreateHttp11ConnectionAsync(HttpRequestMessage request, bool async, CancellationToken cancellationToken)
	{
		var (stream, transportContext, activity, remoteEndPoint) = await ConnectAsync(request, async, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		return await ConstructHttp11ConnectionAsync(async, stream, transportContext, request, activity, remoteEndPoint, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	private async ValueTask<HttpConnection> ConstructHttp11ConnectionAsync(bool async, Stream stream, TransportContext transportContext, HttpRequestMessage request, Activity activity, IPEndPoint remoteEndPoint, CancellationToken cancellationToken)
	{
		return new HttpConnection(this, await ApplyPlaintextFilterAsync(async, stream, HttpVersion.Version11, request, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), transportContext, activity, remoteEndPoint);
	}

	private void HandleHttp11ConnectionFailure(HttpConnectionWaiter<HttpConnection> requestWaiter, Exception e)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace($"HTTP/1.1 connection failed: {e}", "HandleHttp11ConnectionFailure");
		}
		requestWaiter?.TrySetException(e);
		lock (SyncObj)
		{
			_associatedHttp11ConnectionCount--;
			_pendingHttp11ConnectionCount--;
			CheckForHttp11ConnectionInjection();
		}
	}

	public void RecycleHttp11Connection(HttpConnection connection)
	{
		if (CheckExpirationOnReturn(connection))
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				connection.Trace("Disposing HTTP/1.1 connection when returning to pool. Connection lifetime expired.", "RecycleHttp11Connection");
			}
			connection.Dispose();
		}
		else
		{
			ReturnHttp11Connection(connection);
		}
	}

	private void AddNewHttp11Connection(HttpConnection connection, HttpConnectionWaiter<HttpConnection> initialRequestWaiter)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace("", "AddNewHttp11Connection");
		}
		lock (SyncObj)
		{
			_pendingHttp11ConnectionCount--;
			if (initialRequestWaiter != null)
			{
				_http11RequestQueue.TryDequeueSpecificWaiter(initialRequestWaiter);
			}
		}
		if (initialRequestWaiter == null || !initialRequestWaiter.TrySignal(connection))
		{
			ReturnHttp11Connection(connection);
		}
	}

	private void ReturnHttp11Connection(HttpConnection connection)
	{
		connection.MarkConnectionAsIdle();
		if (Volatile.Read(in _http11RequestQueueIsEmptyAndNotDisposed))
		{
			_http11Connections.Push(connection);
			if (!Volatile.Read(in _http11RequestQueueIsEmptyAndNotDisposed))
			{
				ProcessHttp11RequestQueue(null);
			}
		}
		else
		{
			ProcessHttp11RequestQueue(connection);
		}
	}

	public void InvalidateHttp11Connection(HttpConnection connection, bool disposing = true)
	{
		lock (SyncObj)
		{
			_associatedHttp11ConnectionCount--;
			CheckForHttp11ConnectionInjection();
		}
	}

	private static void ScavengeHttp11ConnectionStack(HttpConnectionPool pool, ConcurrentStack<HttpConnection> connections, ref List<HttpConnectionBase> toDispose, long nowTicks, TimeSpan pooledConnectionLifetime, TimeSpan pooledConnectionIdleTimeout)
	{
		HttpConnection[] array = ArrayPool<HttpConnection>.Shared.Rent(pool._associatedHttp11ConnectionCount);
		int num = 0;
		HttpConnection result;
		while (connections.TryPop(out result))
		{
			if (result.IsUsable(nowTicks, pooledConnectionLifetime, pooledConnectionIdleTimeout))
			{
				array[num++] = result;
				continue;
			}
			if (toDispose == null)
			{
				toDispose = new List<HttpConnectionBase>();
			}
			toDispose.Add(result);
		}
		if (num > 0)
		{
			Span<HttpConnection> span = array.AsSpan(0, num);
			span.Reverse();
			connections.PushRange(array, 0, num);
			span.Clear();
		}
		ArrayPool<HttpConnection>.Shared.Return(array);
	}

	private bool TryGetPooledHttp2Connection(HttpRequestMessage request, [NotNullWhen(true)] out Http2Connection connection, out HttpConnectionWaiter<Http2Connection> waiter)
	{
		while (true)
		{
			lock (SyncObj)
			{
				if (!_http2Enabled)
				{
					waiter = null;
					connection = null;
					return false;
				}
				int num = _availableHttp2Connections?.Count ?? 0;
				if (num <= 0)
				{
					waiter = _http2RequestQueue.EnqueueRequest(request);
					CheckForHttp2ConnectionInjection();
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						Trace("No available HTTP/2 connections; request queued.", "TryGetPooledHttp2Connection");
					}
					connection = null;
					return false;
				}
				connection = _availableHttp2Connections[num - 1];
			}
			if (CheckExpirationOnGet(connection))
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					connection.Trace("Found expired HTTP/2 connection in pool.", "TryGetPooledHttp2Connection");
				}
				InvalidateHttp2Connection(connection);
				continue;
			}
			if (connection.TryReserveStream())
			{
				break;
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				connection.Trace("Found HTTP/2 connection in pool without available streams.", "TryGetPooledHttp2Connection");
			}
			bool flag = false;
			lock (SyncObj)
			{
				int num2 = _availableHttp2Connections.IndexOf(connection);
				if (num2 != -1)
				{
					flag = true;
					_availableHttp2Connections.RemoveAt(num2);
				}
			}
			if (flag)
			{
				DisableHttp2Connection(connection);
			}
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			connection.Trace("Found usable HTTP/2 connection in pool.", "TryGetPooledHttp2Connection");
		}
		waiter = null;
		return true;
	}

	private void CheckForHttp2ConnectionInjection()
	{
		_http2RequestQueue.PruneCompletedRequestsFromHeadOfQueue(this);
		int num = _availableHttp2Connections?.Count ?? 0;
		bool flag = num == 0 && !_pendingHttp2Connection && _http2RequestQueue.Count > 0 && (_associatedHttp2ConnectionCount == 0 || EnableMultipleHttp2Connections) && _http2RequestQueue.RequestsWithoutAConnectionAttempt > 0;
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace($"Available HTTP/2.0 connections: {num}, Pending HTTP/2.0 connection: {_pendingHttp2Connection}, Requests in the queue: {_http2RequestQueue.Count}, Requests without a connection attempt: {_http2RequestQueue.RequestsWithoutAConnectionAttempt}, Total associated HTTP/2.0 connections: {_associatedHttp2ConnectionCount}, Will inject connection: {flag}.", "CheckForHttp2ConnectionInjection");
		}
		if (flag)
		{
			_associatedHttp2ConnectionCount++;
			_pendingHttp2Connection = true;
			RequestQueue<Http2Connection>.QueueItem queueItem = _http2RequestQueue.PeekNextRequestForConnectionAttempt();
			InjectNewHttp2ConnectionAsync(queueItem);
		}
	}

	private async Task InjectNewHttp2ConnectionAsync(RequestQueue<Http2Connection>.QueueItem queueItem)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace("Creating new HTTP/2 connection for pool.", "InjectNewHttp2ConnectionAsync");
		}
		await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
		Http2Connection connection = null;
		Exception connectionException = null;
		HttpConnectionWaiter<Http2Connection> waiter = queueItem.Waiter;
		CancellationTokenSource cts = GetConnectTimeoutCancellationTokenSource(waiter);
		try
		{
			var (stream, transportContext, activity, remoteEndPoint) = await ConnectAsync(queueItem.Request, async: true, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
			if (IsSecure)
			{
				SslStream sslStream = (SslStream)stream;
				if (!(sslStream.NegotiatedApplicationProtocol == SslApplicationProtocol.Http2))
				{
					await HandleHttp11Downgrade(queueItem.Request, stream, transportContext, activity, remoteEndPoint, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
					return;
				}
				if (sslStream.SslProtocol < SslProtocols.Tls12)
				{
					stream.Dispose();
					connectionException = new HttpRequestException(System.SR.Format(System.SR.net_ssl_http2_requires_tls12, sslStream.SslProtocol));
				}
				else
				{
					connection = await ConstructHttp2ConnectionAsync(stream, queueItem.Request, activity, remoteEndPoint, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			else
			{
				connection = await ConstructHttp2ConnectionAsync(stream, queueItem.Request, activity, remoteEndPoint, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch (Exception ex)
		{
			connectionException = ((ex is OperationCanceledException ex2 && ex2.CancellationToken == cts.Token && !waiter.CancelledByOriginatingRequestCompletion) ? CreateConnectTimeoutException(ex2) : ex);
		}
		finally
		{
			lock (waiter)
			{
				waiter.ConnectionCancellationTokenSource = null;
				cts.Dispose();
			}
		}
		if (connection != null)
		{
			ReturnHttp2Connection(connection, isNewConnection: true, queueItem.Waiter);
		}
		else
		{
			HandleHttp2ConnectionFailure(waiter, connectionException);
		}
	}

	private async ValueTask<Http2Connection> ConstructHttp2ConnectionAsync(Stream stream, HttpRequestMessage request, Activity activity, IPEndPoint remoteEndPoint, CancellationToken cancellationToken)
	{
		stream = await ApplyPlaintextFilterAsync(async: true, stream, HttpVersion.Version20, request, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		Http2Connection http2Connection = new Http2Connection(this, stream, activity, remoteEndPoint);
		try
		{
			await http2Connection.SetupAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return http2Connection;
		}
		catch (Exception ex)
		{
			if (ex is OperationCanceledException ex2 && ex2.CancellationToken == cancellationToken)
			{
				throw;
			}
			throw new HttpRequestException(System.SR.net_http_client_execution_error, ex);
		}
	}

	private void HandleHttp2ConnectionFailure(HttpConnectionWaiter<Http2Connection> requestWaiter, Exception e)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace($"HTTP2 connection failed: {e}", "HandleHttp2ConnectionFailure");
		}
		requestWaiter.TrySetException(e);
		lock (SyncObj)
		{
			_associatedHttp2ConnectionCount--;
			_pendingHttp2Connection = false;
			CheckForHttp2ConnectionInjection();
		}
	}

	private async Task HandleHttp11Downgrade(HttpRequestMessage request, Stream stream, TransportContext transportContext, Activity activity, IPEndPoint remoteEndPoint, CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace("Server does not support HTTP2; disabling HTTP2 use and proceeding with HTTP/1.1 connection", "HandleHttp11Downgrade");
		}
		bool flag = true;
		HttpConnectionWaiter<Http2Connection> waiter = null;
		lock (SyncObj)
		{
			_http2Enabled = false;
			_associatedHttp2ConnectionCount--;
			_pendingHttp2Connection = false;
			if (_associatedHttp11ConnectionCount < _maxHttp11Connections)
			{
				_associatedHttp11ConnectionCount++;
				_pendingHttp11ConnectionCount++;
			}
			else
			{
				flag = false;
			}
			_http2RequestQueue.TryDequeueWaiter(this, out waiter);
		}
		while (waiter != null)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				Trace("Downgrading queued HTTP2 request to HTTP/1.1", "HandleHttp11Downgrade");
			}
			Volatile.Write(ref waiter.ConnectionCancellationTokenSource, null);
			waiter.TrySetResult(null);
			lock (SyncObj)
			{
				_http2RequestQueue.TryDequeueWaiter(this, out waiter);
			}
		}
		if (!flag)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				Trace("Discarding downgraded HTTP/1.1 connection because HTTP/1.1 connection limit is exceeded", "HandleHttp11Downgrade");
			}
			stream.Dispose();
			return;
		}
		HttpConnection connection;
		try
		{
			connection = await ConstructHttp11ConnectionAsync(async: true, stream, transportContext, request, activity, remoteEndPoint, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
		{
			HandleHttp11ConnectionFailure(null, CreateConnectTimeoutException(ex));
			return;
		}
		catch (Exception e)
		{
			HandleHttp11ConnectionFailure(null, e);
			return;
		}
		AddNewHttp11Connection(connection, null);
	}

	private void ReturnHttp2Connection(Http2Connection connection, bool isNewConnection, HttpConnectionWaiter<Http2Connection> initialRequestWaiter = null)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			connection.Trace($"{"isNewConnection"}={isNewConnection}", "ReturnHttp2Connection");
		}
		if (!isNewConnection && CheckExpirationOnReturn(connection))
		{
			lock (SyncObj)
			{
				_associatedHttp2ConnectionCount--;
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				connection.Trace("Disposing HTTP2 connection return to pool. Connection lifetime expired.", "ReturnHttp2Connection");
			}
			connection.Dispose();
			return;
		}
		while (connection.TryReserveStream())
		{
			HttpConnectionWaiter<Http2Connection> waiter;
			do
			{
				waiter = null;
				bool flag = false;
				lock (SyncObj)
				{
					if (isNewConnection)
					{
						_pendingHttp2Connection = false;
						isNewConnection = false;
					}
					if (initialRequestWaiter != null)
					{
						waiter = initialRequestWaiter;
						initialRequestWaiter = null;
						_http2RequestQueue.TryDequeueSpecificWaiter(waiter);
					}
					else if (!_http2RequestQueue.TryDequeueWaiter(this, out waiter))
					{
						if (_disposed)
						{
							_associatedHttp2ConnectionCount--;
						}
						else
						{
							flag = true;
							if (_availableHttp2Connections == null)
							{
								_availableHttp2Connections = new List<Http2Connection>();
							}
							_availableHttp2Connections.Add(connection);
						}
					}
				}
				if (waiter != null)
				{
					continue;
				}
				connection.ReleaseStream();
				if (flag)
				{
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						connection.Trace("Put HTTP2 connection in pool.", "ReturnHttp2Connection");
					}
					return;
				}
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					connection.Trace("Disposing HTTP2 connection returned to pool. Pool was disposed.", "ReturnHttp2Connection");
				}
				connection.Dispose();
				return;
			}
			while (!waiter.TrySignal(connection));
		}
		if (isNewConnection)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				connection.Trace("New HTTP2 connection is unusable due to no available streams.", "ReturnHttp2Connection");
			}
			connection.Dispose();
			HttpRequestException ex = new HttpRequestException(System.SR.net_http_http2_connection_not_established);
			ExceptionDispatchInfo.SetCurrentStackTrace(ex);
			HandleHttp2ConnectionFailure(initialRequestWaiter, ex);
		}
		else
		{
			lock (SyncObj)
			{
				CheckForHttp2ConnectionInjection();
			}
			DisableHttp2Connection(connection);
		}
	}

	private void DisableHttp2Connection(Http2Connection connection)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			connection.Trace("", "DisableHttp2Connection");
		}
		DisableHttp2ConnectionAsync(connection);
		async Task DisableHttp2ConnectionAsync(Http2Connection http2Connection)
		{
			bool flag = await http2Connection.WaitForAvailableStreamsAsync().ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				http2Connection.Trace($"{"WaitForAvailableStreamsAsync"} completed, {"usable"}={flag}", "DisableHttp2Connection");
			}
			if (flag)
			{
				ReturnHttp2Connection(http2Connection, isNewConnection: false);
			}
			else
			{
				lock (SyncObj)
				{
					_associatedHttp2ConnectionCount--;
					CheckForHttp2ConnectionInjection();
				}
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					http2Connection.Trace("HTTP2 connection no longer usable", "DisableHttp2Connection");
				}
				http2Connection.Dispose();
			}
		}
	}

	public void InvalidateHttp2Connection(Http2Connection connection)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			connection.Trace("", "InvalidateHttp2Connection");
		}
		bool flag = false;
		lock (SyncObj)
		{
			if (_availableHttp2Connections != null)
			{
				int num = _availableHttp2Connections.IndexOf(connection);
				if (num != -1)
				{
					flag = true;
					_availableHttp2Connections.RemoveAt(num);
					_associatedHttp2ConnectionCount--;
				}
			}
			CheckForHttp2ConnectionInjection();
		}
		if (flag)
		{
			connection.Dispose();
		}
	}

	public void HeartBeat()
	{
		Http2Connection[] array;
		lock (SyncObj)
		{
			array = _availableHttp2Connections?.ToArray();
		}
		if (array != null)
		{
			Http2Connection[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].HeartBeat();
			}
		}
	}

	private static int ScavengeHttp2ConnectionList(List<Http2Connection> list, ref List<HttpConnectionBase> toDispose, long nowTicks, TimeSpan pooledConnectionLifetime, TimeSpan pooledConnectionIdleTimeout)
	{
		int i;
		for (i = 0; i < list.Count && list[i].IsUsable(nowTicks, pooledConnectionLifetime, pooledConnectionIdleTimeout); i++)
		{
		}
		int num = 0;
		if (i < list.Count)
		{
			if (toDispose == null)
			{
				toDispose = new List<HttpConnectionBase>();
			}
			toDispose.Add(list[i]);
			int j = i + 1;
			while (j < list.Count)
			{
				for (; j < list.Count && !list[j].IsUsable(nowTicks, pooledConnectionLifetime, pooledConnectionIdleTimeout); j++)
				{
					toDispose.Add(list[j]);
				}
				if (j < list.Count)
				{
					list[i++] = list[j++];
				}
			}
			num = list.Count - i;
			list.RemoveRange(i, num);
		}
		return num;
	}

	[SupportedOSPlatform("windows")]
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	private async ValueTask<HttpResponseMessage> TrySendUsingHttp3Async(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		while (true)
		{
			HttpConnectionWaiter<Http3Connection> http3ConnectionWaiter = null;
			try
			{
				if (!TryGetHttp3Authority(request, out var authority, out var reasonException))
				{
					if (reasonException == null)
					{
						return null;
					}
					ThrowGetVersionException(request, 3, reasonException);
				}
				WaitForHttp3ConnectionActivity waitForConnectionActivity = new WaitForHttp3ConnectionActivity(Settings, authority);
				if (!TryGetPooledHttp3Connection(request, out var connection, out http3ConnectionWaiter, out var streamAvailable))
				{
					waitForConnectionActivity.Start();
					try
					{
						connection = await http3ConnectionWaiter.WaitWithCancellationAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (Exception exception)
					{
						waitForConnectionActivity.Stop(request, this, exception);
						throw;
					}
				}
				if (connection == null)
				{
					return null;
				}
				HttpResponseMessage httpResponseMessage = await connection.SendAsync(request, waitForConnectionActivity, streamAvailable, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				if (httpResponseMessage.StatusCode == HttpStatusCode.MisdirectedRequest && connection.Authority != _originAuthority)
				{
					httpResponseMessage.Dispose();
					BlocklistAuthority(connection.Authority);
					continue;
				}
				return httpResponseMessage;
			}
			finally
			{
				http3ConnectionWaiter?.SetTimeoutToPendingConnectionAttempt(this, cancellationToken.IsCancellationRequested);
			}
		}
	}

	[SupportedOSPlatform("windows")]
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	private bool TryGetPooledHttp3Connection(HttpRequestMessage request, [NotNullWhen(true)] out Http3Connection connection, [NotNullWhen(false)] out HttpConnectionWaiter<Http3Connection> waiter, out bool streamAvailable)
	{
		while (true)
		{
			lock (SyncObj)
			{
				int num = _availableHttp3Connections?.Count ?? 0;
				if (num <= 0)
				{
					waiter = _http3RequestQueue.EnqueueRequest(request);
					CheckForHttp3ConnectionInjection();
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						Trace("No available HTTP/3 connections; request queued.", "TryGetPooledHttp3Connection");
					}
					connection = null;
					streamAvailable = false;
					return false;
				}
				connection = _availableHttp3Connections[num - 1];
			}
			if (CheckExpirationOnGet(connection))
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					connection.Trace("Found expired HTTP/3 connection in pool.", "TryGetPooledHttp3Connection");
				}
				InvalidateHttp3Connection(connection);
				continue;
			}
			streamAvailable = connection.TryReserveStream();
			if (streamAvailable || !EnableMultipleHttp3Connections)
			{
				break;
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				connection.Trace("Found HTTP/3 connection in pool without available streams.", "TryGetPooledHttp3Connection");
			}
			bool flag = false;
			lock (SyncObj)
			{
				int num2 = _availableHttp3Connections.IndexOf(connection);
				if (num2 != -1)
				{
					flag = true;
					_availableHttp3Connections.RemoveAt(num2);
				}
			}
			if (flag)
			{
				DisableHttp3Connection(connection);
			}
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			connection.Trace("Found usable HTTP/3 connection in pool.", "TryGetPooledHttp3Connection");
		}
		waiter = null;
		return true;
	}

	[SupportedOSPlatform("windows")]
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	private void CheckForHttp3ConnectionInjection()
	{
		_http3RequestQueue.PruneCompletedRequestsFromHeadOfQueue(this);
		int num = _availableHttp3Connections?.Count ?? 0;
		bool flag = num == 0 && !_pendingHttp3Connection && _http3RequestQueue.Count > 0 && (_associatedHttp3ConnectionCount == 0 || EnableMultipleHttp3Connections) && _http3RequestQueue.RequestsWithoutAConnectionAttempt > 0;
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace($"Available HTTP/3.0 connections: {num}, Pending HTTP/3.0 connection: {_pendingHttp3Connection}, Requests in the queue: {_http3RequestQueue.Count}, Requests without a connection attempt: {_http3RequestQueue.RequestsWithoutAConnectionAttempt}, Total associated HTTP/3.0 connections: {_associatedHttp3ConnectionCount}, Will inject connection: {flag}.", "CheckForHttp3ConnectionInjection");
		}
		if (flag)
		{
			_associatedHttp3ConnectionCount++;
			_pendingHttp3Connection = true;
			RequestQueue<Http3Connection>.QueueItem queueItem = _http3RequestQueue.PeekNextRequestForConnectionAttempt();
			InjectNewHttp3ConnectionAsync(queueItem);
		}
	}

	[SupportedOSPlatform("windows")]
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	private async Task InjectNewHttp3ConnectionAsync(RequestQueue<Http3Connection>.QueueItem queueItem)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace("Creating new HTTP/3 connection for pool.", "InjectNewHttp3ConnectionAsync");
		}
		await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
		Http3Connection connection = null;
		Exception connectionException = null;
		HttpAuthority authority = null;
		HttpConnectionWaiter<Http3Connection> waiter = queueItem.Waiter;
		CancellationTokenSource cts = GetConnectTimeoutCancellationTokenSource(waiter);
		Activity connectionSetupActivity = null;
		try
		{
			if (TryGetHttp3Authority(queueItem.Request, out authority, out var reasonException))
			{
				connectionSetupActivity = ConnectionSetupDistributedTracing.StartConnectionSetupActivity(isSecure: true, _telemetryServerAddress, authority.Port);
				connection = new Http3Connection(this, authority, _http3Authority == authority);
				QuicConnection quicConnection = await ConnectHelper.ConnectQuicAsync(queueItem.Request, new DnsEndPoint(authority.IdnHost, authority.Port), _poolManager.Settings._pooledConnectionIdleTimeout, _sslOptionsHttp3, connection.StreamCapacityCallback, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
				if (quicConnection.NegotiatedApplicationProtocol != SslApplicationProtocol.Http3)
				{
					await quicConnection.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
					throw new HttpRequestException(HttpRequestError.ConnectionError, "QUIC connected but no HTTP/3 indicated via ALPN.", null, RequestRetryType.RetryOnConnectionFailure);
				}
				if (connectionSetupActivity != null)
				{
					ConnectionSetupDistributedTracing.StopConnectionSetupActivity(connectionSetupActivity, null, quicConnection.RemoteEndPoint);
				}
				connection.InitQuicConnection(quicConnection, connectionSetupActivity);
			}
			else if (reasonException != null)
			{
				ThrowGetVersionException(queueItem.Request, 3, reasonException);
			}
		}
		catch (Exception ex)
		{
			connectionException = ((ex is OperationCanceledException ex2 && ex2.CancellationToken == cts.Token && !waiter.CancelledByOriginatingRequestCompletion) ? CreateConnectTimeoutException(ex2) : ex);
			if (connectionSetupActivity != null)
			{
				ConnectionSetupDistributedTracing.StopConnectionSetupActivity(connectionSetupActivity, connectionException, null);
			}
			connection?.Dispose();
			connection = null;
		}
		finally
		{
			lock (waiter)
			{
				waiter.ConnectionCancellationTokenSource = null;
				cts.Dispose();
			}
		}
		if (connection != null)
		{
			ReturnHttp3Connection(connection, isNewConnection: true, waiter);
			return;
		}
		if (connectionException != null && !(connectionException is OperationCanceledException) && (object)authority != null)
		{
			BlocklistAuthority(authority, connectionException);
		}
		HandleHttp3ConnectionFailure(waiter, connectionException);
	}

	[SupportedOSPlatform("windows")]
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	private void HandleHttp3ConnectionFailure(HttpConnectionWaiter<Http3Connection> requestWaiter, Exception e)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace($"HTTP3 connection failed: {e}", "HandleHttp3ConnectionFailure");
		}
		if (e == null)
		{
			requestWaiter.TrySetResult(null);
		}
		else
		{
			requestWaiter.TrySetException(e);
		}
		lock (SyncObj)
		{
			_associatedHttp3ConnectionCount--;
			_pendingHttp3Connection = false;
			CheckForHttp3ConnectionInjection();
		}
	}

	[SupportedOSPlatform("windows")]
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	private void ReturnHttp3Connection(Http3Connection connection, bool isNewConnection, HttpConnectionWaiter<Http3Connection> initialRequestWaiter = null)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			connection.Trace($"{"isNewConnection"}={isNewConnection}", "ReturnHttp3Connection");
		}
		if (!isNewConnection && CheckExpirationOnReturn(connection))
		{
			lock (SyncObj)
			{
				_associatedHttp3ConnectionCount--;
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				connection.Trace("Disposing HTTP3 connection return to pool. Connection lifetime expired.", "ReturnHttp3Connection");
			}
			connection.Dispose();
			return;
		}
		while (connection.TryReserveStream() || !EnableMultipleHttp3Connections)
		{
			HttpConnectionWaiter<Http3Connection> waiter;
			do
			{
				waiter = null;
				bool flag = false;
				lock (SyncObj)
				{
					if (isNewConnection)
					{
						_pendingHttp3Connection = false;
						isNewConnection = false;
					}
					if (initialRequestWaiter != null)
					{
						waiter = initialRequestWaiter;
						initialRequestWaiter = null;
						_http3RequestQueue.TryDequeueSpecificWaiter(waiter);
					}
					else if (!_http3RequestQueue.TryDequeueWaiter(this, out waiter))
					{
						if (_disposed)
						{
							_associatedHttp3ConnectionCount--;
						}
						else
						{
							flag = true;
							if (_availableHttp3Connections == null)
							{
								_availableHttp3Connections = new List<Http3Connection>();
							}
							_availableHttp3Connections.Add(connection);
						}
					}
				}
				if (waiter != null)
				{
					continue;
				}
				connection.ReleaseStream();
				if (flag)
				{
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						connection.Trace("Put HTTP3 connection in pool.", "ReturnHttp3Connection");
					}
					return;
				}
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					connection.Trace("Disposing HTTP3 connection returned to pool. Pool was disposed.", "ReturnHttp3Connection");
				}
				connection.Dispose();
				return;
			}
			while (!waiter.TrySignal(connection));
		}
		lock (SyncObj)
		{
			CheckForHttp3ConnectionInjection();
		}
		DisableHttp3Connection(connection);
	}

	[SupportedOSPlatform("windows")]
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	private void DisableHttp3Connection(Http3Connection connection)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			connection.Trace("", "DisableHttp3Connection");
		}
		DisableHttp3ConnectionAsync(connection);
		async Task DisableHttp3ConnectionAsync(Http3Connection http3Connection)
		{
			bool flag = await http3Connection.WaitForAvailableStreamsAsync().ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				http3Connection.Trace($"{"WaitForAvailableStreamsAsync"} completed, {"usable"}={flag}", "DisableHttp3Connection");
			}
			if (flag)
			{
				ReturnHttp3Connection(http3Connection, isNewConnection: false);
			}
			else
			{
				lock (SyncObj)
				{
					_associatedHttp3ConnectionCount--;
					CheckForHttp3ConnectionInjection();
				}
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					http3Connection.Trace("HTTP3 connection no longer usable", "DisableHttp3Connection");
				}
				http3Connection.Dispose();
			}
		}
	}

	[SupportedOSPlatform("windows")]
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	public void InvalidateHttp3Connection(Http3Connection connection, bool dispose = true)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			connection.Trace("", "InvalidateHttp3Connection");
		}
		bool flag = false;
		lock (SyncObj)
		{
			if (_availableHttp3Connections != null)
			{
				int num = _availableHttp3Connections.IndexOf(connection);
				if (num != -1)
				{
					flag = true;
					_availableHttp3Connections.RemoveAt(num);
					_associatedHttp3ConnectionCount--;
				}
			}
			CheckForHttp3ConnectionInjection();
		}
		if (flag & dispose)
		{
			connection.Dispose();
		}
	}

	[SupportedOSPlatform("windows")]
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	private static int ScavengeHttp3ConnectionList(List<Http3Connection> list, ref List<HttpConnectionBase> toDispose, long nowTicks, TimeSpan pooledConnectionLifetime, TimeSpan pooledConnectionIdleTimeout)
	{
		int i;
		for (i = 0; i < list.Count && list[i].IsUsable(nowTicks, pooledConnectionLifetime, pooledConnectionIdleTimeout); i++)
		{
		}
		int num = 0;
		if (i < list.Count)
		{
			if (toDispose == null)
			{
				toDispose = new List<HttpConnectionBase>();
			}
			toDispose.Add(list[i]);
			int j = i + 1;
			while (j < list.Count)
			{
				for (; j < list.Count && !list[j].IsUsable(nowTicks, pooledConnectionLifetime, pooledConnectionIdleTimeout); j++)
				{
					toDispose.Add(list[j]);
				}
				if (j < list.Count)
				{
					list[i++] = list[j++];
				}
			}
			num = list.Count - i;
			list.RemoveRange(i, num);
		}
		return num;
	}

	private bool TryGetHttp3Authority(HttpRequestMessage request, [NotNullWhen(true)] out HttpAuthority authority, out Exception reasonException)
	{
		authority = _http3Authority;
		if (request.Version.Major >= 3 && request.VersionPolicy != HttpVersionPolicy.RequestVersionOrLower && (object)authority == null)
		{
			authority = _originAuthority;
		}
		if ((object)authority == null)
		{
			reasonException = null;
			return false;
		}
		if (IsAltSvcBlocked(authority, out reasonException))
		{
			return false;
		}
		return true;
	}

	private void ProcessAltSvc(HttpResponseMessage response)
	{
		if (_altSvcEnabled && response.Headers.TryGetValues(KnownHeaders.AltSvc.Descriptor, out var values))
		{
			HandleAltSvc(values, response.Headers.Age);
		}
	}

	internal void HandleAltSvc(IEnumerable<string> altSvcHeaderValues, TimeSpan? responseAge)
	{
		HttpAuthority httpAuthority = null;
		TimeSpan dueTime = default(TimeSpan);
		bool flag = false;
		foreach (string altSvcHeaderValue2 in altSvcHeaderValues)
		{
			int index = 0;
			if (!AltSvcHeaderParser.Parser.TryParseValue(altSvcHeaderValue2, null, ref index, out var parsedValue))
			{
				continue;
			}
			AltSvcHeaderValue altSvcHeaderValue = (AltSvcHeaderValue)parsedValue;
			if (AltSvcHeaderValue.Clear == altSvcHeaderValue)
			{
				lock (SyncObj)
				{
					ExpireAltSvcAuthority();
					_authorityExpireTimer?.Change(-1, -1);
					return;
				}
			}
			if ((object)httpAuthority != null || altSvcHeaderValue.AlpnProtocolName != "h3")
			{
				continue;
			}
			HttpAuthority httpAuthority2 = new HttpAuthority(altSvcHeaderValue.Host ?? _originAuthority.IdnHost, altSvcHeaderValue.Port);
			if (!IsAltSvcBlocked(httpAuthority2, out var _))
			{
				TimeSpan maxAge = altSvcHeaderValue.MaxAge;
				if (responseAge.HasValue)
				{
					maxAge -= responseAge.GetValueOrDefault();
				}
				if (!(maxAge <= TimeSpan.Zero))
				{
					httpAuthority = httpAuthority2;
					dueTime = maxAge;
					flag = altSvcHeaderValue.Persist;
				}
			}
		}
		if (!(httpAuthority != null) || httpAuthority.Equals(_http3Authority))
		{
			return;
		}
		if (dueTime.Ticks > 25920000000000L)
		{
			dueTime = TimeSpan.FromTicks(25920000000000L);
		}
		lock (SyncObj)
		{
			if (_disposed)
			{
				return;
			}
			if (_authorityExpireTimer == null)
			{
				WeakReference<HttpConnectionPool> state = new WeakReference<HttpConnectionPool>(this);
				using (ExecutionContext.SuppressFlow())
				{
					_authorityExpireTimer = new Timer(delegate(object o)
					{
						if (((WeakReference<HttpConnectionPool>)o).TryGetTarget(out var target))
						{
							lock (target.SyncObj)
							{
								target.ExpireAltSvcAuthority();
							}
						}
					}, state, dueTime, Timeout.InfiniteTimeSpan);
				}
			}
			else
			{
				_authorityExpireTimer.Change(dueTime, Timeout.InfiniteTimeSpan);
			}
			_http3Authority = httpAuthority;
			_persistAuthority = flag;
		}
		if (!flag)
		{
			_poolManager.StartMonitoringNetworkChanges();
		}
	}

	private void ExpireAltSvcAuthority()
	{
		_http3Authority = null;
	}

	private bool IsAltSvcBlocked(HttpAuthority authority, out Exception reasonException)
	{
		if (_altSvcBlocklist != null)
		{
			lock (_altSvcBlocklist)
			{
				return _altSvcBlocklist.TryGetValue(authority, out reasonException);
			}
		}
		reasonException = null;
		return false;
	}

	internal void BlocklistAuthority(HttpAuthority badAuthority, Exception exception = null)
	{
		Dictionary<HttpAuthority, Exception> altSvcBlocklist = _altSvcBlocklist;
		if (altSvcBlocklist == null)
		{
			lock (SyncObj)
			{
				if (_disposed)
				{
					return;
				}
				altSvcBlocklist = _altSvcBlocklist;
				if (altSvcBlocklist == null)
				{
					altSvcBlocklist = new Dictionary<HttpAuthority, Exception>();
					_altSvcBlocklistTimerCancellation = new CancellationTokenSource();
					_altSvcBlocklist = altSvcBlocklist;
				}
			}
		}
		bool flag = false;
		bool flag2;
		lock (altSvcBlocklist)
		{
			flag2 = altSvcBlocklist.TryAdd(badAuthority, exception);
			if (flag2 && altSvcBlocklist.Count >= 8 && _altSvcEnabled)
			{
				_altSvcEnabled = false;
				flag = true;
			}
		}
		CancellationToken token;
		lock (SyncObj)
		{
			if (_disposed)
			{
				return;
			}
			if (_http3Authority == badAuthority)
			{
				ExpireAltSvcAuthority();
				_authorityExpireTimer.Change(-1, -1);
			}
			token = _altSvcBlocklistTimerCancellation.Token;
		}
		if (flag2)
		{
			Task.Delay(600000, token).ContinueWith(delegate
			{
				lock (altSvcBlocklist)
				{
					altSvcBlocklist.Remove(badAuthority);
				}
			}, token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}
		if (flag)
		{
			Task.Delay(600000, token).ContinueWith(delegate
			{
				_altSvcEnabled = true;
			}, token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}
	}

	public void OnNetworkChanged()
	{
		lock (SyncObj)
		{
			if (_http3Authority != null && !_persistAuthority)
			{
				ExpireAltSvcAuthority();
				_authorityExpireTimer?.Change(-1, -1);
			}
		}
	}
}
