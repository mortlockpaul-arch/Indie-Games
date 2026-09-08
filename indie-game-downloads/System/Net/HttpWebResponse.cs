using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net;

public class HttpWebResponse : WebResponse, ISerializable
{
	internal sealed class TruncatedReadStream(Stream innerStream, long maxSize) : Stream
	{
		private long _maxRemainingLength = maxSize;

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return Read(new Span<byte>(buffer, offset, count));
		}

		public override int Read(Span<byte> buffer)
		{
			int num = innerStream.Read(buffer.Slice(0, (int)Math.Min(buffer.Length, _maxRemainingLength)));
			_maxRemainingLength -= num;
			return num;
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			return ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
		}

		public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			int num = await innerStream.ReadAsync(buffer.Slice(0, (int)Math.Min(buffer.Length, _maxRemainingLength)), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			_maxRemainingLength -= num;
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override ValueTask DisposeAsync()
		{
			return innerStream.DisposeAsync();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				innerStream.Dispose();
			}
		}
	}

	private HttpResponseMessage _httpResponseMessage;

	private readonly Uri _requestUri;

	private CookieCollection _cookies;

	private WebHeaderCollection _webHeaderCollection;

	private string _characterSet;

	private readonly bool _isVersionHttp11 = true;

	public override bool IsMutuallyAuthenticated => base.IsMutuallyAuthenticated;

	public override long ContentLength
	{
		get
		{
			CheckDisposed();
			return _httpResponseMessage.Content?.Headers.ContentLength ?? (-1);
		}
	}

	public override string ContentType
	{
		get
		{
			CheckDisposed();
			if (_httpResponseMessage.Content != null && _httpResponseMessage.Content.Headers.TryGetValues("Content-Type", out IEnumerable<string> values))
			{
				return string.Join(',', values);
			}
			return string.Empty;
		}
	}

	public string ContentEncoding
	{
		get
		{
			CheckDisposed();
			if (_httpResponseMessage.Content != null)
			{
				return GetHeaderValueAsString(_httpResponseMessage.Content.Headers.ContentEncoding);
			}
			return string.Empty;
		}
	}

	public virtual CookieCollection Cookies
	{
		get
		{
			CheckDisposed();
			return _cookies;
		}
		set
		{
			CheckDisposed();
			_cookies = value;
		}
	}

	public DateTime LastModified
	{
		get
		{
			CheckDisposed();
			string text = Headers["Last-Modified"];
			if (string.IsNullOrEmpty(text))
			{
				return DateTime.Now;
			}
			if (System.Net.HttpDateParser.TryParse(text.AsSpan(), out var result))
			{
				return result.LocalDateTime;
			}
			throw new ProtocolViolationException(System.SR.net_baddate);
		}
	}

	public string Server
	{
		get
		{
			CheckDisposed();
			string text = Headers["Server"];
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return string.Empty;
		}
	}

	public Version ProtocolVersion
	{
		get
		{
			CheckDisposed();
			if (!_isVersionHttp11)
			{
				return HttpVersion.Version10;
			}
			return HttpVersion.Version11;
		}
	}

	public override WebHeaderCollection Headers
	{
		get
		{
			CheckDisposed();
			if (_webHeaderCollection == null)
			{
				_webHeaderCollection = new WebHeaderCollection();
				foreach (KeyValuePair<string, IEnumerable<string>> header in _httpResponseMessage.Headers)
				{
					_webHeaderCollection[header.Key] = GetHeaderValueAsString(header.Value);
				}
				if (_httpResponseMessage.Content != null)
				{
					foreach (KeyValuePair<string, IEnumerable<string>> header2 in _httpResponseMessage.Content.Headers)
					{
						_webHeaderCollection[header2.Key] = GetHeaderValueAsString(header2.Value);
					}
				}
			}
			return _webHeaderCollection;
		}
	}

	public virtual string Method
	{
		get
		{
			CheckDisposed();
			return _httpResponseMessage.RequestMessage.Method.Method;
		}
	}

	public override Uri ResponseUri
	{
		get
		{
			CheckDisposed();
			return _httpResponseMessage.RequestMessage.RequestUri;
		}
	}

	public virtual HttpStatusCode StatusCode
	{
		get
		{
			CheckDisposed();
			return _httpResponseMessage.StatusCode;
		}
	}

	public virtual string StatusDescription
	{
		get
		{
			CheckDisposed();
			return _httpResponseMessage.ReasonPhrase ?? string.Empty;
		}
	}

	public string? CharacterSet
	{
		get
		{
			CheckDisposed();
			string text = Headers["Content-Type"];
			if (_characterSet == null && !string.IsNullOrWhiteSpace(text))
			{
				_characterSet = string.Empty;
				string text2 = text.ToLowerInvariant();
				if (text2.AsSpan().Trim().StartsWith("text/".AsSpan(), StringComparison.Ordinal))
				{
					_characterSet = "ISO-8859-1";
				}
				int i = text2.IndexOf(';');
				if (i > 0)
				{
					while ((i = text2.IndexOf("charset", i, StringComparison.Ordinal)) >= 0)
					{
						i += 7;
						if (text2[i - 8] != ';' && text2[i - 8] != ' ')
						{
							continue;
						}
						for (; i < text2.Length && text2[i] == ' '; i++)
						{
						}
						if (i < text2.Length - 1 && text2[i] == '=')
						{
							i++;
							int num = text2.IndexOf(';', i);
							if (num > i)
							{
								_characterSet = text.AsSpan(i, num - i).Trim().ToString();
							}
							else
							{
								_characterSet = text.AsSpan(i).Trim().ToString();
							}
							break;
						}
					}
				}
			}
			return _characterSet;
		}
	}

	public override bool SupportsHeaders => true;

	[Obsolete("This API supports the .NET infrastructure and is not intended to be used directly from your code.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public HttpWebResponse()
	{
		_requestUri = null;
		_cookies = null;
	}

	[Obsolete("Serialization has been deprecated for HttpWebResponse.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected HttpWebResponse(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
	{
		throw new PlatformNotSupportedException();
	}

	[Obsolete("Serialization has been deprecated for HttpWebResponse.")]
	void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		throw new PlatformNotSupportedException();
	}

	[Obsolete("Serialization has been deprecated for HttpWebResponse.")]
	protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
	{
		throw new PlatformNotSupportedException();
	}

	internal HttpWebResponse(HttpResponseMessage _message, Uri requestUri, CookieContainer cookieContainer)
	{
		_httpResponseMessage = _message;
		_requestUri = requestUri;
		if (cookieContainer != null)
		{
			_cookies = cookieContainer.GetCookies(requestUri);
		}
		else
		{
			_cookies = new CookieCollection();
		}
	}

	public override Stream GetResponseStream()
	{
		CheckDisposed();
		if (_httpResponseMessage.Content != null)
		{
			Stream stream = _httpResponseMessage.Content.ReadAsStream();
			int defaultMaximumErrorResponseLength = HttpWebRequest.DefaultMaximumErrorResponseLength;
			if (defaultMaximumErrorResponseLength < 0 || StatusCode < HttpStatusCode.BadRequest)
			{
				return stream;
			}
			return new TruncatedReadStream(stream, (long)defaultMaximumErrorResponseLength * 1024L);
		}
		return Stream.Null;
	}

	public string GetResponseHeader(string headerName)
	{
		CheckDisposed();
		return Headers[headerName] ?? string.Empty;
	}

	public override void Close()
	{
		Dispose(disposing: true);
	}

	protected override void Dispose(bool disposing)
	{
		HttpResponseMessage httpResponseMessage = _httpResponseMessage;
		if (httpResponseMessage != null)
		{
			httpResponseMessage.Dispose();
			_httpResponseMessage = null;
		}
	}

	private void CheckDisposed()
	{
		ObjectDisposedException.ThrowIf(_httpResponseMessage == null, this);
	}

	private static string GetHeaderValueAsString(IEnumerable<string> values)
	{
		return string.Join(", ", values);
	}
}
