using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>The default message handler used by <see cref="T:System.Net.Http.HttpClient" />.</summary>
public class HttpClientHandler : HttpMessageHandler
{
	private readonly SocketsHttpHandler _underlyingHandler;

	private volatile bool _disposed;

	[CompilerGenerated]
	private static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> _003CDangerousAcceptAnyServerCertificateValidator_003Ek__BackingField;

	private SocketsHttpHandler Handler => _underlyingHandler;

	/// <summary>Gets a value that indicates whether the handler supports automatic response content decompression.</summary>
	/// <returns>
	///   <see langword="true" /> if the if the handler supports automatic response content decompression; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
	public virtual bool SupportsAutomaticDecompression => true;

	/// <summary>Gets a value that indicates whether the handler supports proxy settings.</summary>
	/// <returns>
	///   <see langword="true" /> if the if the handler supports proxy settings; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
	public virtual bool SupportsProxy => true;

	/// <summary>Gets a value that indicates whether the handler supports configuration settings for the <see cref="P:System.Net.Http.HttpClientHandler.AllowAutoRedirect" /> and <see cref="P:System.Net.Http.HttpClientHandler.MaxAutomaticRedirections" /> properties.</summary>
	/// <returns>
	///   <see langword="true" /> if the if the handler supports configuration settings for the <see cref="P:System.Net.Http.HttpClientHandler.AllowAutoRedirect" /> and <see cref="P:System.Net.Http.HttpClientHandler.MaxAutomaticRedirections" /> properties; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
	public virtual bool SupportsRedirectConfiguration => true;

	[CLSCompliant(false)]
	public IMeterFactory? MeterFactory
	{
		get
		{
			return _underlyingHandler.MeterFactory;
		}
		set
		{
			_underlyingHandler.MeterFactory = value;
		}
	}

	/// <summary>Gets or sets a value that indicates whether the handler uses the  <see cref="P:System.Net.Http.HttpClientHandler.CookieContainer" /> property  to store server cookies and uses these cookies when sending requests.</summary>
	/// <returns>
	///   <see langword="true" /> if the if the handler supports uses the  <see cref="P:System.Net.Http.HttpClientHandler.CookieContainer" /> property  to store server cookies and uses these cookies when sending requests; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
	[UnsupportedOSPlatform("browser")]
	public bool UseCookies
	{
		get
		{
			return _underlyingHandler.UseCookies;
		}
		set
		{
			_underlyingHandler.UseCookies = value;
		}
	}

	/// <summary>Gets or sets the cookie container used to store server cookies by the handler.</summary>
	/// <returns>The cookie container used to store server cookies by the handler.</returns>
	[UnsupportedOSPlatform("browser")]
	public CookieContainer CookieContainer
	{
		get
		{
			return _underlyingHandler.CookieContainer;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			_underlyingHandler.CookieContainer = value;
		}
	}

	/// <summary>Gets or sets the type of decompression method used by the handler for automatic decompression of the HTTP content response.</summary>
	/// <returns>The automatic decompression method used by the handler.</returns>
	[UnsupportedOSPlatform("browser")]
	public DecompressionMethods AutomaticDecompression
	{
		get
		{
			return _underlyingHandler.AutomaticDecompression;
		}
		set
		{
			_underlyingHandler.AutomaticDecompression = value;
		}
	}

	/// <summary>Gets or sets a value that indicates whether the handler uses a proxy for requests.</summary>
	/// <returns>
	///   <see langword="true" /> if the handler should use a proxy for requests; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
	[UnsupportedOSPlatform("browser")]
	public bool UseProxy
	{
		get
		{
			return _underlyingHandler.UseProxy;
		}
		set
		{
			_underlyingHandler.UseProxy = value;
		}
	}

	/// <summary>Gets or sets proxy information used by the handler.</summary>
	/// <returns>The proxy information used by the handler. The default value is <see langword="null" />.</returns>
	[UnsupportedOSPlatform("browser")]
	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("tvos")]
	public IWebProxy? Proxy
	{
		get
		{
			return _underlyingHandler.Proxy;
		}
		set
		{
			_underlyingHandler.Proxy = value;
		}
	}

	/// <summary>When the default (system) proxy is being used, gets or sets the credentials to submit to the default proxy server for authentication. The default proxy is used only when <see cref="P:System.Net.Http.HttpClientHandler.UseProxy" /> is set to <see langword="true" /> and <see cref="P:System.Net.Http.HttpClientHandler.Proxy" /> is set to <see langword="null" />.</summary>
	/// <returns>The credentials needed to authenticate a request to the default proxy server.</returns>
	[UnsupportedOSPlatform("browser")]
	public ICredentials? DefaultProxyCredentials
	{
		get
		{
			return _underlyingHandler.DefaultProxyCredentials;
		}
		set
		{
			_underlyingHandler.DefaultProxyCredentials = value;
		}
	}

	/// <summary>Gets or sets a value that indicates whether the handler sends an Authorization header with the request.</summary>
	/// <returns>
	///   <see langword="true" /> for the handler to send an HTTP Authorization header with requests after authentication has taken place; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
	[UnsupportedOSPlatform("browser")]
	public bool PreAuthenticate
	{
		get
		{
			return _underlyingHandler.PreAuthenticate;
		}
		set
		{
			_underlyingHandler.PreAuthenticate = value;
		}
	}

	/// <summary>Gets or sets a value that controls whether default credentials are sent with requests by the handler.</summary>
	/// <returns>
	///   <see langword="true" /> if the default credentials are used; otherwise <see langword="false" />. The default value is <see langword="false" />.</returns>
	[UnsupportedOSPlatform("browser")]
	public bool UseDefaultCredentials
	{
		get
		{
			return _underlyingHandler.Credentials == CredentialCache.DefaultCredentials;
		}
		set
		{
			if (value)
			{
				_underlyingHandler.Credentials = CredentialCache.DefaultCredentials;
			}
			else if (_underlyingHandler.Credentials == CredentialCache.DefaultCredentials)
			{
				_underlyingHandler.Credentials = null;
			}
		}
	}

	/// <summary>Gets or sets authentication information used by this handler.</summary>
	/// <returns>The authentication credentials associated with the handler. The default is <see langword="null" />.</returns>
	[UnsupportedOSPlatform("browser")]
	public ICredentials? Credentials
	{
		get
		{
			return _underlyingHandler.Credentials;
		}
		set
		{
			_underlyingHandler.Credentials = value;
		}
	}

	/// <summary>Gets or sets a value that indicates whether the handler should follow redirection responses.</summary>
	/// <returns>
	///   <see langword="true" /> if the handler should follow redirection responses; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
	public bool AllowAutoRedirect
	{
		get
		{
			return _underlyingHandler.AllowAutoRedirect;
		}
		set
		{
			_underlyingHandler.AllowAutoRedirect = value;
		}
	}

	/// <summary>Gets or sets the maximum number of redirects that the handler follows.</summary>
	/// <returns>The maximum number of redirection responses that the handler follows. The default value is 50.</returns>
	[UnsupportedOSPlatform("browser")]
	public int MaxAutomaticRedirections
	{
		get
		{
			return _underlyingHandler.MaxAutomaticRedirections;
		}
		set
		{
			_underlyingHandler.MaxAutomaticRedirections = value;
		}
	}

	/// <summary>Gets or sets the maximum number of concurrent connections (per server endpoint) allowed when making requests using an <see cref="T:System.Net.Http.HttpClient" /> object. Note that the limit is per server endpoint, so for example a value of 256 would permit 256 concurrent connections to http://www.adatum.com/ and another 256 to http://www.adventure-works.com/.</summary>
	/// <returns>The maximum number of concurrent connections (per server endpoint) allowed by an <see cref="T:System.Net.Http.HttpClient" /> object.</returns>
	[UnsupportedOSPlatform("browser")]
	public int MaxConnectionsPerServer
	{
		get
		{
			return _underlyingHandler.MaxConnectionsPerServer;
		}
		set
		{
			_underlyingHandler.MaxConnectionsPerServer = value;
		}
	}

	/// <summary>Gets or sets the maximum request content buffer size used by the handler.</summary>
	/// <returns>The maximum request content buffer size in bytes. The default value is 2 gigabytes.</returns>
	public long MaxRequestContentBufferSize
	{
		get
		{
			return 0L;
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegative(value, "value");
			if (value > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("value", value, System.SR.Format(CultureInfo.InvariantCulture, System.SR.net_http_content_buffersize_limit, int.MaxValue));
			}
			ObjectDisposedException.ThrowIf(_disposed, this);
		}
	}

	/// <summary>Gets or sets the maximum length, in kilobytes (1024 bytes), of the response headers. For example, if the value is 64, then 65536 bytes are allowed for the maximum response headers' length.</summary>
	/// <returns>The maximum length, in kilobytes (1024 bytes), of the response headers.</returns>
	[UnsupportedOSPlatform("browser")]
	public int MaxResponseHeadersLength
	{
		get
		{
			return _underlyingHandler.MaxResponseHeadersLength;
		}
		set
		{
			_underlyingHandler.MaxResponseHeadersLength = value;
		}
	}

	/// <summary>Gets or sets a value that indicates if the certificate is automatically picked from the certificate store or if the caller is allowed to pass in a specific client certificate.</summary>
	/// <returns>The collection of security certificates associated with this handler.</returns>
	public ClientCertificateOption ClientCertificateOptions
	{
		get
		{
			return _underlyingHandler.ClientCertificateOptions;
		}
		set
		{
			switch (value)
			{
			case ClientCertificateOption.Manual:
				ThrowForModifiedManagedSslOptionsIfStarted();
				_underlyingHandler.SslOptions.LocalCertificateSelectionCallback = (object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers) => CertificateHelper.GetEligibleClientCertificate(_underlyingHandler.SslOptions.ClientCertificates);
				break;
			case ClientCertificateOption.Automatic:
				ThrowForModifiedManagedSslOptionsIfStarted();
				_underlyingHandler.SslOptions.LocalCertificateSelectionCallback = (object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers) => CertificateHelper.GetEligibleClientCertificate();
				break;
			default:
				throw new ArgumentOutOfRangeException("value");
			}
			_underlyingHandler.ClientCertificateOptions = value;
		}
	}

	/// <summary>Gets the collection of security certificates that are associated requests to the server.</summary>
	/// <returns>The X509CertificateCollection that is presented to the server when performing certificate based client authentication.</returns>
	[UnsupportedOSPlatform("browser")]
	public X509CertificateCollection ClientCertificates
	{
		get
		{
			if (ClientCertificateOptions != ClientCertificateOption.Manual)
			{
				throw new InvalidOperationException(System.SR.Format(System.SR.net_http_invalid_enable_first, "ClientCertificateOptions", "Manual"));
			}
			return _underlyingHandler.SslOptions.ClientCertificates ?? (_underlyingHandler.SslOptions.ClientCertificates = new X509CertificateCollection());
		}
	}

	/// <summary>Gets or sets a callback method to validate the server certificate.</summary>
	/// <returns>A callback method to validate the server certificate.</returns>
	[UnsupportedOSPlatform("browser")]
	public Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool>? ServerCertificateCustomValidationCallback
	{
		get
		{
			return (_underlyingHandler.SslOptions.RemoteCertificateValidationCallback?.Target as ConnectHelper.CertificateCallbackMapper)?.FromHttpClientHandler;
		}
		set
		{
			ThrowForModifiedManagedSslOptionsIfStarted();
			_underlyingHandler.SslOptions.RemoteCertificateValidationCallback = ((value != null) ? new ConnectHelper.CertificateCallbackMapper(value).ForSocketsHttpHandler : null);
		}
	}

	/// <summary>Gets or sets a value that indicates whether the certificate is checked against the certificate authority revocation list.</summary>
	/// <returns>
	///   <see langword="true" /> if the certificate revocation list is checked; otherwise, <see langword="false" />.</returns>
	/// <exception cref="T:System.PlatformNotSupportedException">.NET Framework 4.7.1 only: This property is not implemented.</exception>
	[UnsupportedOSPlatform("browser")]
	public bool CheckCertificateRevocationList
	{
		get
		{
			return _underlyingHandler.SslOptions.CertificateRevocationCheckMode == X509RevocationMode.Online;
		}
		set
		{
			ThrowForModifiedManagedSslOptionsIfStarted();
			_underlyingHandler.SslOptions.CertificateRevocationCheckMode = (value ? X509RevocationMode.Online : X509RevocationMode.NoCheck);
		}
	}

	/// <summary>Gets or sets the TLS/SSL protocol used by the <see cref="T:System.Net.Http.HttpClient" /> objects managed by the HttpClientHandler object.</summary>
	/// <returns>One of the values defined in the <see cref="T:System.Security.Authentication.SslProtocols" /> enumeration.</returns>
	/// <exception cref="T:System.PlatformNotSupportedException">.NET Framework 4.7.1 only: This property is not implemented.</exception>
	[UnsupportedOSPlatform("browser")]
	public SslProtocols SslProtocols
	{
		get
		{
			return _underlyingHandler.SslOptions.EnabledSslProtocols;
		}
		set
		{
			ThrowForModifiedManagedSslOptionsIfStarted();
			_underlyingHandler.SslOptions.EnabledSslProtocols = value;
		}
	}

	/// <summary>Gets a writable dictionary (that is, a map) of custom properties for the <see cref="T:System.Net.Http.HttpClient" /> requests. The dictionary is initialized empty; you can insert and query key-value pairs for your custom handlers and special processing.</summary>
	/// <returns>a writable dictionary of custom properties.</returns>
	public IDictionary<string, object?> Properties => _underlyingHandler.Properties;

	/// <summary>Gets a cached delegate that always returns <see langword="true" />.</summary>
	/// <returns>A cached delegate that always returns <see langword="true" />.</returns>
	[UnsupportedOSPlatform("browser")]
	public static Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool> DangerousAcceptAnyServerCertificateValidator => _003CDangerousAcceptAnyServerCertificateValidator_003Ek__BackingField ?? Interlocked.CompareExchange(ref _003CDangerousAcceptAnyServerCertificateValidator_003Ek__BackingField, (HttpRequestMessage _003Cp0_003E, X509Certificate2 _003Cp1_003E, X509Chain _003Cp2_003E, SslPolicyErrors _003Cp3_003E) => true, null) ?? _003CDangerousAcceptAnyServerCertificateValidator_003Ek__BackingField;

	/// <summary>Creates an instance of a <see cref="T:System.Net.Http.HttpClientHandler" /> class.</summary>
	public HttpClientHandler()
	{
		_underlyingHandler = new SocketsHttpHandler();
		ClientCertificateOptions = ClientCertificateOption.Manual;
	}

	/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpClientHandler" /> and optionally disposes of the managed resources.</summary>
	/// <param name="disposing">
	///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && !_disposed)
		{
			_disposed = true;
			_underlyingHandler.Dispose();
		}
		base.Dispose(disposing);
	}

	[UnsupportedOSPlatform("browser")]
	protected internal override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request, "request");
		return Handler.Send(request, cancellationToken);
	}

	/// <summary>Creates an instance of  <see cref="T:System.Net.Http.HttpResponseMessage" /> based on the information provided in the <see cref="T:System.Net.Http.HttpRequestMessage" /> as an operation that will not block.</summary>
	/// <param name="request">The HTTP request message.</param>
	/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
	protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request, "request");
		return Handler.SendAsync(request, cancellationToken);
	}

	private void ThrowForModifiedManagedSslOptionsIfStarted()
	{
		_underlyingHandler.SslOptions = _underlyingHandler.SslOptions;
	}
}
