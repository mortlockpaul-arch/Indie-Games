using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace System.Net.Http;

/// <summary>Represents a HTTP request message.</summary>
public class HttpRequestMessage : IDisposable
{
	private int _sendStatus;

	private HttpMethod _method;

	private Uri _requestUri;

	private HttpRequestHeaders _headers;

	private Version _version;

	private HttpVersionPolicy _versionPolicy;

	private HttpContent _content;

	internal HttpRequestOptions _options;

	internal static Version DefaultRequestVersion => HttpVersion.Version11;

	/// <summary>Gets or sets the HTTP message version.</summary>
	/// <returns>The HTTP message version. The default is 1.1.</returns>
	public Version Version
	{
		get
		{
			return _version;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			CheckDisposed();
			_version = value;
		}
	}

	public HttpVersionPolicy VersionPolicy
	{
		get
		{
			return _versionPolicy;
		}
		set
		{
			CheckDisposed();
			_versionPolicy = value;
		}
	}

	/// <summary>Gets or sets the contents of the HTTP message.</summary>
	/// <returns>The content of a message</returns>
	public HttpContent? Content
	{
		get
		{
			return _content;
		}
		set
		{
			CheckDisposed();
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				if (value == null)
				{
					System.Net.NetEventSource.ContentNull(this);
				}
				else
				{
					System.Net.NetEventSource.Associate(this, value, "Content");
				}
			}
			_content = value;
		}
	}

	/// <summary>Gets or sets the HTTP method used by the HTTP request message.</summary>
	/// <returns>The HTTP method used by the request message. The default is the GET method.</returns>
	public HttpMethod Method
	{
		get
		{
			return _method;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			CheckDisposed();
			_method = value;
		}
	}

	/// <summary>Gets or sets the <see cref="T:System.Uri" /> used for the HTTP request.</summary>
	/// <returns>The <see cref="T:System.Uri" /> used for the HTTP request.</returns>
	public Uri? RequestUri
	{
		get
		{
			return _requestUri;
		}
		set
		{
			CheckDisposed();
			_requestUri = value;
		}
	}

	/// <summary>Gets the collection of HTTP request headers.</summary>
	/// <returns>The collection of HTTP request headers.</returns>
	public HttpRequestHeaders Headers => _headers ?? (_headers = new HttpRequestHeaders());

	internal bool HasHeaders => _headers != null;

	/// <summary>Gets a set of properties for the HTTP request.</summary>
	/// <returns>Returns <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
	[Obsolete("HttpRequestMessage.Properties has been deprecated. Use Options instead.")]
	public IDictionary<string, object?> Properties => Options;

	public HttpRequestOptions Options => _options ?? (_options = new HttpRequestOptions());

	private bool Disposed
	{
		get
		{
			return (_sendStatus & 4) != 0;
		}
		set
		{
			_sendStatus |= 4;
		}
	}

	internal bool IsExtendedConnectRequest
	{
		get
		{
			if (Method == HttpMethod.Connect)
			{
				return _headers?.Protocol != null;
			}
			return false;
		}
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestMessage" /> class.</summary>
	public HttpRequestMessage()
		: this(HttpMethod.Get, (Uri?)null)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestMessage" /> class with an HTTP method and a request <see cref="T:System.Uri" />.</summary>
	/// <param name="method">The HTTP method.</param>
	/// <param name="requestUri">The <see cref="T:System.Uri" /> to request.</param>
	public HttpRequestMessage(HttpMethod method, Uri? requestUri)
	{
		ArgumentNullException.ThrowIfNull(method, "method");
		_method = method;
		_requestUri = requestUri;
		_version = DefaultRequestVersion;
		_versionPolicy = HttpVersionPolicy.RequestVersionOrLower;
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestMessage" /> class with an HTTP method and a request <see cref="T:System.Uri" />.</summary>
	/// <param name="method">The HTTP method.</param>
	/// <param name="requestUri">A string that represents the request  <see cref="T:System.Uri" />.</param>
	public HttpRequestMessage(HttpMethod method, [StringSyntax("Uri")] string? requestUri)
		: this(method, string.IsNullOrEmpty(requestUri) ? null : new Uri(requestUri, UriKind.RelativeOrAbsolute))
	{
	}

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string representation of the current object.</returns>
	public override string ToString()
	{
		Span<char> initialBuffer = stackalloc char[512];
		System.Text.ValueStringBuilder sb = new System.Text.ValueStringBuilder(initialBuffer);
		sb.Append("Method: ");
		sb.Append(_method.ToString());
		sb.Append(", RequestUri: '");
		if ((object)_requestUri == null)
		{
			sb.Append("<null>");
		}
		else
		{
			sb.AppendSpanFormattable(_requestUri);
		}
		sb.Append("', Version: ");
		sb.AppendSpanFormattable(_version);
		sb.Append(", Content: ");
		sb.Append((_content == null) ? "<null>" : _content.GetType().ToString());
		sb.Append(", Headers:");
		sb.Append(Environment.NewLine);
		HeaderUtilities.DumpHeaders(ref sb, _headers, _content?.Headers);
		return sb.ToString();
	}

	internal bool MarkAsSent()
	{
		return Interlocked.CompareExchange(ref _sendStatus, 1, 0) == 0;
	}

	internal bool WasSentByHttpClient()
	{
		return (_sendStatus & 1) != 0;
	}

	internal void MarkPropagatorStateInjectedByDiagnosticsHandler()
	{
		_sendStatus |= 2;
	}

	internal bool WasPropagatorStateInjectedByDiagnosticsHandler()
	{
		return (_sendStatus & 2) != 0;
	}

	internal void DisableAuth()
	{
		_sendStatus |= 8;
	}

	internal bool IsAuthDisabled()
	{
		return (_sendStatus & 8) != 0;
	}

	/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpRequestMessage" /> and optionally disposes of the managed resources.</summary>
	/// <param name="disposing">
	///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing && !Disposed)
		{
			Disposed = true;
			_content?.Dispose();
		}
	}

	/// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:System.Net.Http.HttpRequestMessage" />.</summary>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void CheckDisposed()
	{
		ObjectDisposedException.ThrowIf(Disposed, this);
	}
}
