using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>Provides HTTP content based on a stream.</summary>
public class StreamContent : HttpContent
{
	private sealed class ReadOnlyStream : DelegatingStream
	{
		public override bool CanWrite => false;

		public override int WriteTimeout
		{
			get
			{
				throw new InvalidOperationException(System.SR.net_http_content_readonly_stream);
			}
			set
			{
				throw new InvalidOperationException(System.SR.net_http_content_readonly_stream);
			}
		}

		public ReadOnlyStream(Stream innerStream)
			: base(innerStream)
		{
		}

		public override void Flush()
		{
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(System.SR.net_http_content_readonly_stream);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException(System.SR.net_http_content_readonly_stream);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotSupportedException(System.SR.net_http_content_readonly_stream);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException(System.SR.net_http_content_readonly_stream);
		}

		public override void Write(ReadOnlySpan<byte> buffer)
		{
			throw new NotSupportedException(System.SR.net_http_content_readonly_stream);
		}

		public override void WriteByte(byte value)
		{
			throw new NotSupportedException(System.SR.net_http_content_readonly_stream);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			throw new NotSupportedException(System.SR.net_http_content_readonly_stream);
		}

		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException(System.SR.net_http_content_readonly_stream);
		}
	}

	private Stream _content;

	private int _bufferSize;

	private bool _contentConsumed;

	private long _start;

	internal override bool AllowDuplex => false;

	/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.StreamContent" /> class.</summary>
	/// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.StreamContent" />.</param>
	public StreamContent(Stream content)
	{
		ArgumentNullException.ThrowIfNull(content, "content");
		InitializeContent(content, 0);
	}

	/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.StreamContent" /> class.</summary>
	/// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.StreamContent" />.</param>
	/// <param name="bufferSize">The size, in bytes, of the buffer for the <see cref="T:System.Net.Http.StreamContent" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> was <see langword="null" />.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="bufferSize" /> was less than or equal to zero.</exception>
	public StreamContent(Stream content, int bufferSize)
	{
		ArgumentNullException.ThrowIfNull(content, "content");
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bufferSize, "bufferSize");
		InitializeContent(content, bufferSize);
	}

	[MemberNotNull("_content")]
	private void InitializeContent(Stream content, int bufferSize)
	{
		_content = content;
		_bufferSize = bufferSize;
		if (content.CanSeek)
		{
			_start = content.Position;
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Associate(this, content, "InitializeContent");
		}
	}

	protected override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		PrepareContent();
		StreamToStreamCopy.Copy(_content, stream, _bufferSize, !_content.CanSeek);
	}

	/// <summary>Serialize the HTTP content to a stream as an asynchronous operation.</summary>
	/// <param name="stream">The target stream.</param>
	/// <param name="context">Information about the transport (channel binding token, for example). This parameter may be <see langword="null" />.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
	{
		return SerializeToStreamAsyncCore(stream, default(CancellationToken));
	}

	protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		if (!(GetType() == typeof(StreamContent)))
		{
			return base.SerializeToStreamAsync(stream, context, cancellationToken);
		}
		return SerializeToStreamAsyncCore(stream, cancellationToken);
	}

	private Task SerializeToStreamAsyncCore(Stream stream, CancellationToken cancellationToken)
	{
		PrepareContent();
		return StreamToStreamCopy.CopyAsync(_content, stream, _bufferSize, !_content.CanSeek, cancellationToken);
	}

	/// <summary>Determines whether the stream content has a valid length in bytes.</summary>
	/// <param name="length">The length in bytes of the stream content.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="length" /> is a valid length; otherwise, <see langword="false" />.</returns>
	protected internal override bool TryComputeLength(out long length)
	{
		if (_content.CanSeek)
		{
			length = _content.Length - _start;
			return true;
		}
		length = 0L;
		return false;
	}

	/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.StreamContent" /> and optionally disposes of the managed resources.</summary>
	/// <param name="disposing">
	///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_content.Dispose();
		}
		base.Dispose(disposing);
	}

	protected override Stream CreateContentReadStream(CancellationToken cancellationToken)
	{
		SeekToStartIfSeekable();
		return new ReadOnlyStream(_content);
	}

	/// <summary>Write the HTTP stream content to a memory stream as an asynchronous operation.</summary>
	/// <returns>The task object representing the asynchronous operation.</returns>
	protected override Task<Stream> CreateContentReadStreamAsync()
	{
		SeekToStartIfSeekable();
		return Task.FromResult((Stream)new ReadOnlyStream(_content));
	}

	internal override Stream TryCreateContentReadStream()
	{
		if (!(GetType() == typeof(StreamContent)))
		{
			return null;
		}
		return new ReadOnlyStream(_content);
	}

	private void PrepareContent()
	{
		if (_contentConsumed)
		{
			if (!_content.CanSeek)
			{
				throw new InvalidOperationException(System.SR.net_http_content_stream_already_read);
			}
			_content.Position = _start;
		}
		_contentConsumed = true;
	}

	private void SeekToStartIfSeekable()
	{
		if (_content.CanSeek)
		{
			_content.Position = _start;
		}
	}
}
