using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http;

/// <summary>Represents a HTTP response message including the status code and data.</summary>
public class HttpResponseMessage : IDisposable
{
	private HttpStatusCode _statusCode;

	private HttpResponseHeaders _headers;

	private HttpResponseHeaders _trailingHeaders;

	private string _reasonPhrase;

	private HttpRequestMessage _requestMessage;

	private Version _version;

	private HttpContent _content;

	private bool _disposed;

	private static Version DefaultResponseVersion => HttpVersion.Version11;

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

	/// <summary>Gets or sets the content of a HTTP response message.</summary>
	/// <returns>The content of the HTTP response message.</returns>
	public HttpContent Content
	{
		get
		{
			return _content ?? (_content = new EmptyContent());
		}
		[param: AllowNull]
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

	/// <summary>Gets or sets the status code of the HTTP response.</summary>
	/// <returns>The status code of the HTTP response.</returns>
	public HttpStatusCode StatusCode
	{
		get
		{
			return _statusCode;
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegative((int)value, "value");
			ArgumentOutOfRangeException.ThrowIfGreaterThan((int)value, 999, "value");
			CheckDisposed();
			_statusCode = value;
		}
	}

	/// <summary>Gets or sets the reason phrase which typically is sent by servers together with the status code.</summary>
	/// <returns>The reason phrase sent by the server.</returns>
	public string? ReasonPhrase
	{
		get
		{
			if (_reasonPhrase != null)
			{
				return _reasonPhrase;
			}
			return HttpStatusDescription.Get(StatusCode);
		}
		set
		{
			if (value != null && HttpRuleParser.ContainsNewLineOrNull(value))
			{
				throw new FormatException(System.SR.net_http_reasonphrase_format_error);
			}
			CheckDisposed();
			_reasonPhrase = value;
		}
	}

	/// <summary>Gets the collection of HTTP response headers.</summary>
	/// <returns>The collection of HTTP response headers.</returns>
	public HttpResponseHeaders Headers => _headers ?? (_headers = new HttpResponseHeaders());

	public HttpResponseHeaders TrailingHeaders => _trailingHeaders ?? (_trailingHeaders = new HttpResponseHeaders(containsTrailingHeaders: true));

	/// <summary>Gets or sets the request message which led to this response message.</summary>
	/// <returns>The request message which led to this response message.</returns>
	public HttpRequestMessage? RequestMessage
	{
		get
		{
			return _requestMessage;
		}
		set
		{
			CheckDisposed();
			if (value != null && System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Associate(this, value, "RequestMessage");
			}
			_requestMessage = value;
		}
	}

	/// <summary>Gets a value that indicates if the HTTP response was successful.</summary>
	/// <returns>
	///   <see langword="true" /> if <see cref="P:System.Net.Http.HttpResponseMessage.StatusCode" /> was in the range 200-299; otherwise, <see langword="false" />.</returns>
	public bool IsSuccessStatusCode
	{
		get
		{
			if (_statusCode >= HttpStatusCode.OK)
			{
				return _statusCode <= (HttpStatusCode)299;
			}
			return false;
		}
	}

	internal void SetVersionWithoutValidation(Version value)
	{
		_version = value;
	}

	internal void SetStatusCodeWithoutValidation(HttpStatusCode value)
	{
		_statusCode = value;
	}

	internal void SetReasonPhraseWithoutValidation(string value)
	{
		_reasonPhrase = value;
	}

	internal void StoreReceivedTrailingHeaders(HttpResponseHeaders headers)
	{
		if (_trailingHeaders == null)
		{
			_trailingHeaders = headers;
		}
		else
		{
			_trailingHeaders.AddHeaders(headers);
		}
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpResponseMessage" /> class.</summary>
	public HttpResponseMessage()
		: this(HttpStatusCode.OK)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpResponseMessage" /> class with a specific <see cref="P:System.Net.Http.HttpResponseMessage.StatusCode" />.</summary>
	/// <param name="statusCode">The status code of the HTTP response.</param>
	public HttpResponseMessage(HttpStatusCode statusCode)
	{
		ArgumentOutOfRangeException.ThrowIfNegative((int)statusCode, "statusCode");
		ArgumentOutOfRangeException.ThrowIfGreaterThan((int)statusCode, 999, "statusCode");
		_statusCode = statusCode;
		_version = DefaultResponseVersion;
	}

	/// <summary>Throws an exception if the <see cref="P:System.Net.Http.HttpResponseMessage.IsSuccessStatusCode" /> property for the HTTP response is <see langword="false" />.</summary>
	/// <returns>The HTTP response message if the call is successful.</returns>
	/// <exception cref="T:System.Net.Http.HttpRequestException">The HTTP response is unsuccessful.</exception>
	public HttpResponseMessage EnsureSuccessStatusCode()
	{
		if (!IsSuccessStatusCode)
		{
			throw new HttpRequestException(System.SR.Format(CultureInfo.InvariantCulture, string.IsNullOrWhiteSpace(ReasonPhrase) ? System.SR.net_http_message_not_success_statuscode : System.SR.net_http_message_not_success_statuscode_reason, (int)_statusCode, ReasonPhrase), null, _statusCode);
		}
		return this;
	}

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string representation of the current object.</returns>
	public override string ToString()
	{
		Span<char> initialBuffer = stackalloc char[512];
		System.Text.ValueStringBuilder sb = new System.Text.ValueStringBuilder(initialBuffer);
		sb.Append("StatusCode: ");
		sb.AppendSpanFormattable((int)_statusCode);
		sb.Append(", ReasonPhrase: '");
		sb.Append(ReasonPhrase ?? "<null>");
		sb.Append("', Version: ");
		sb.AppendSpanFormattable(_version);
		sb.Append(", Content: ");
		sb.Append((_content == null) ? "<null>" : _content.GetType().ToString());
		sb.Append(", Headers:");
		sb.Append(Environment.NewLine);
		HeaderUtilities.DumpHeaders(ref sb, _headers, _content?.Headers);
		if (_trailingHeaders != null)
		{
			sb.Append(", Trailing Headers:");
			sb.Append(Environment.NewLine);
			HeaderUtilities.DumpHeaders(ref sb, _trailingHeaders);
		}
		return sb.ToString();
	}

	/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpResponseMessage" /> and optionally disposes of the managed resources.</summary>
	/// <param name="disposing">
	///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing && !_disposed)
		{
			_disposed = true;
			_content?.Dispose();
		}
	}

	/// <summary>Releases the unmanaged resources and disposes of unmanaged resources used by the <see cref="T:System.Net.Http.HttpResponseMessage" />.</summary>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void CheckDisposed()
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
	}
}
