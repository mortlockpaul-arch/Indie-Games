using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>A base class representing an HTTP entity body and content headers.</summary>
public abstract class HttpContent : IDisposable
{
	internal sealed class LimitArrayPoolWriteStream : Stream
	{
		private readonly int _maxBufferSize;

		private readonly int _expectedFinalSize;

		private readonly bool _shouldPoolFinalSize;

		private bool _lastBufferIsPooled;

		private byte[] _lastBuffer;

		private byte[][] _pooledBuffers;

		private int _lastBufferOffset;

		private int _totalLength;

		public override long Length => _totalLength;

		public override bool CanWrite => true;

		public override bool CanRead => false;

		public override bool CanSeek => false;

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

		public LimitArrayPoolWriteStream(int maxBufferSize, long expectedFinalSize, bool getFinalSizeFromPool)
		{
			if (expectedFinalSize > maxBufferSize)
			{
				throw CreateOverCapacityException(maxBufferSize);
			}
			_maxBufferSize = maxBufferSize;
			_expectedFinalSize = (int)expectedFinalSize;
			_shouldPoolFinalSize = getFinalSizeFromPool || expectedFinalSize == 0;
			_lastBufferIsPooled = false;
			_lastBuffer = Array.Empty<byte>();
		}

		protected override void Dispose(bool disposing)
		{
			ReturnAllPooledBuffers();
			base.Dispose(disposing);
		}

		public byte[] ToArray()
		{
			if (!_lastBufferIsPooled && _totalLength == _lastBuffer.Length)
			{
				return _lastBuffer;
			}
			if (_totalLength == 0)
			{
				return Array.Empty<byte>();
			}
			byte[] array = new byte[_totalLength];
			CopyToCore(array);
			return array;
		}

		public byte[] GetSingleBuffer()
		{
			return _lastBuffer;
		}

		public ReadOnlySpan<byte> GetFirstBuffer()
		{
			byte[][] pooledBuffers = _pooledBuffers;
			return (pooledBuffers != null) ? ((Span<byte>)pooledBuffers[0]) : _lastBuffer.AsSpan(0, _totalLength);
		}

		public byte[] CreateCopy()
		{
			return _lastBuffer.AsSpan(0, _totalLength).ToArray();
		}

		public void ReallocateIfPooled()
		{
			if (_lastBufferIsPooled)
			{
				byte[] array = new byte[_totalLength];
				CopyToCore(array);
				ReturnAllPooledBuffers();
				_lastBuffer = array;
				_lastBufferOffset = array.Length;
			}
		}

		public override void Write(ReadOnlySpan<byte> buffer)
		{
			if (_maxBufferSize - _totalLength < buffer.Length)
			{
				throw CreateOverCapacityException(_maxBufferSize);
			}
			byte[] lastBuffer = _lastBuffer;
			int lastBufferOffset = _lastBufferOffset;
			if (lastBuffer.Length - lastBufferOffset >= buffer.Length)
			{
				buffer.CopyTo(lastBuffer.AsSpan(lastBufferOffset));
				_lastBufferOffset = lastBufferOffset + buffer.Length;
				_totalLength += buffer.Length;
			}
			else
			{
				GrowAndWrite(buffer);
			}
		}

		private void GrowAndWrite(ReadOnlySpan<byte> buffer)
		{
			int num = _lastBuffer.Length;
			int val = (int)Math.Min((uint)(num * 2), Array.MaxLength);
			val = Math.Max(val, _totalLength + buffer.Length);
			if (num == 0)
			{
				int val2 = ((_expectedFinalSize == 0) ? 16384 : Math.Min(_expectedFinalSize, 4194304));
				val = Math.Max(val, val2);
			}
			int num2 = _totalLength - _lastBufferOffset + num;
			int val3 = _maxBufferSize - num2;
			val = Math.Min(val, val3);
			int num3 = num2 + val;
			byte[] array;
			if (!_shouldPoolFinalSize && num3 >= _expectedFinalSize / 4)
			{
				array = new byte[(_totalLength + buffer.Length <= _expectedFinalSize) ? _expectedFinalSize : num3];
				CopyToCore(array);
				ReturnAllPooledBuffers();
				buffer.CopyTo(array.AsSpan(_totalLength));
				_totalLength += buffer.Length;
				_lastBufferOffset = _totalLength;
				_lastBufferIsPooled = false;
			}
			else if (num == 0)
			{
				array = ArrayPool<byte>.Shared.Rent(val);
				buffer.CopyTo(array);
				_totalLength = (_lastBufferOffset = buffer.Length);
				_lastBufferIsPooled = true;
			}
			else
			{
				_totalLength += buffer.Length;
				Span<byte> destination = _lastBuffer.AsSpan(_lastBufferOffset);
				buffer.Slice(0, destination.Length).CopyTo(destination);
				buffer = buffer.Slice(destination.Length);
				array = ArrayPool<byte>.Shared.Rent(val);
				buffer.CopyTo(array);
				_lastBufferOffset = buffer.Length;
				int i = 0;
				if (_pooledBuffers == null)
				{
					_pooledBuffers = new byte[4][];
				}
				else
				{
					byte[][] pooledBuffers;
					for (pooledBuffers = _pooledBuffers; i < pooledBuffers.Length && pooledBuffers[i] != null; i++)
					{
					}
					if (i == pooledBuffers.Length)
					{
						Array.Resize(ref _pooledBuffers, i + 4);
					}
				}
				_pooledBuffers[i] = _lastBuffer;
			}
			_lastBuffer = array;
		}

		public void CopyToCore(Span<byte> destination)
		{
			byte[][] pooledBuffers = _pooledBuffers;
			if (pooledBuffers != null)
			{
				byte[][] array = pooledBuffers;
				foreach (byte[] array2 in array)
				{
					if (array2 == null)
					{
						break;
					}
					array2.CopyTo(destination);
					destination = destination.Slice(array2.Length);
				}
			}
			_lastBuffer.AsSpan(0, _lastBufferOffset).CopyTo(destination);
		}

		private void ReturnAllPooledBuffers()
		{
			byte[][] pooledBuffers = _pooledBuffers;
			if (pooledBuffers != null)
			{
				_pooledBuffers = null;
				byte[][] array = pooledBuffers;
				foreach (byte[] array2 in array)
				{
					if (array2 == null)
					{
						break;
					}
					ArrayPool<byte>.Shared.Return(array2);
				}
			}
			byte[] lastBuffer = _lastBuffer;
			_lastBuffer = null;
			if (_lastBufferIsPooled)
			{
				_lastBufferIsPooled = false;
				ArrayPool<byte>.Shared.Return(lastBuffer);
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			Stream.ValidateBufferArguments(buffer, offset, count);
			Write(buffer.AsSpan(offset, count));
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			Write(buffer, offset, count);
			return Task.CompletedTask;
		}

		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Write(buffer.Span);
			return default(ValueTask);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return TaskToAsyncResult.Begin(WriteAsync(buffer, offset, count, CancellationToken.None), asyncCallback, asyncState);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			TaskToAsyncResult.End(asyncResult);
		}

		public override void WriteByte(byte value)
		{
			Write(new ReadOnlySpan<byte>(in value));
		}

		public override void Flush()
		{
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
	}

	private HttpContentHeaders _headers;

	private LimitArrayPoolWriteStream _bufferedContent;

	private object _contentReadStream;

	private bool _disposed;

	private bool _canCalculateLength;

	internal static readonly Encoding DefaultStringEncoding = Encoding.UTF8;

	private static ReadOnlySpan<byte> UTF8Preamble => "\ufeff"u8;

	private static ReadOnlySpan<byte> UTF32Preamble => new byte[4] { 255, 254, 0, 0 };

	private static ReadOnlySpan<byte> UnicodePreamble => new byte[2] { 255, 254 };

	private static ReadOnlySpan<byte> BigEndianUnicodePreamble => new byte[2] { 254, 255 };

	/// <summary>Gets the HTTP content headers as defined in RFC 2616.</summary>
	/// <returns>The content headers as defined in RFC 2616.</returns>
	public HttpContentHeaders Headers => _headers ?? (_headers = new HttpContentHeaders(this));

	[MemberNotNullWhen(true, "_bufferedContent")]
	private bool IsBuffered
	{
		[MemberNotNullWhen(true, "_bufferedContent")]
		get
		{
			return _bufferedContent != null;
		}
	}

	internal virtual bool AllowDuplex => true;

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpContent" /> class.</summary>
	protected HttpContent()
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, null, ".ctor");
		}
		_canCalculateLength = true;
	}

	private MemoryStream CreateMemoryStreamFromBufferedContent()
	{
		return new MemoryStream(_bufferedContent.GetSingleBuffer(), 0, (int)_bufferedContent.Length, writable: false);
	}

	/// <summary>Serialize the HTTP content to a string as an asynchronous operation.</summary>
	/// <returns>The task object representing the asynchronous operation.</returns>
	public Task<string> ReadAsStringAsync()
	{
		return ReadAsStringAsync(CancellationToken.None);
	}

	public Task<string> ReadAsStringAsync(CancellationToken cancellationToken)
	{
		CheckDisposed();
		return WaitAndReturnAsync(LoadIntoBufferAsync(cancellationToken), this, (HttpContent s) => s.ReadBufferedContentAsString());
	}

	private string ReadBufferedContentAsString()
	{
		return ReadBufferAsString(_bufferedContent, Headers);
	}

	internal static string ReadBufferAsString(LimitArrayPoolWriteStream stream, HttpContentHeaders headers)
	{
		if (stream.Length == 0L)
		{
			return string.Empty;
		}
		ReadOnlySpan<byte> firstBuffer = stream.GetFirstBuffer();
		Encoding encoding = null;
		int preambleLength = -1;
		string text = headers.ContentType?.CharSet;
		if (text != null)
		{
			try
			{
				encoding = ((text.Length <= 2 || !text.StartsWith('"') || !text.EndsWith('"')) ? Encoding.GetEncoding(text) : Encoding.GetEncoding(text.Substring(1, text.Length - 2)));
				preambleLength = GetPreambleLength(firstBuffer, encoding);
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidOperationException(System.SR.net_http_content_invalid_charset, innerException);
			}
		}
		if (encoding == null && !TryDetectEncoding(firstBuffer, out encoding, out preambleLength))
		{
			encoding = DefaultStringEncoding;
			preambleLength = 0;
		}
		int num;
		if (firstBuffer.Length == stream.Length)
		{
			Encoding encoding2 = encoding;
			num = preambleLength;
			return encoding2.GetString(firstBuffer.Slice(num, firstBuffer.Length - num));
		}
		byte[] array = ArrayPool<byte>.Shared.Rent((int)stream.Length);
		stream.CopyToCore(array);
		Encoding encoding3 = encoding;
		Span<byte> span = array.AsSpan(0, (int)stream.Length);
		num = preambleLength;
		string result = encoding3.GetString(span.Slice(num, span.Length - num));
		ArrayPool<byte>.Shared.Return(array);
		return result;
	}

	/// <summary>Serialize the HTTP content to a byte array as an asynchronous operation.</summary>
	/// <returns>The task object representing the asynchronous operation.</returns>
	public Task<byte[]> ReadAsByteArrayAsync()
	{
		return ReadAsByteArrayAsync(CancellationToken.None);
	}

	public Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken)
	{
		CheckDisposed();
		return WaitAndReturnAsync(LoadIntoBufferAsync(cancellationToken), this, (HttpContent s) => s.ReadBufferedContentAsByteArray());
	}

	internal byte[] ReadBufferedContentAsByteArray()
	{
		return _bufferedContent.CreateCopy();
	}

	public Stream ReadAsStream()
	{
		return ReadAsStream(CancellationToken.None);
	}

	public Stream ReadAsStream(CancellationToken cancellationToken)
	{
		CheckDisposed();
		if (_contentReadStream == null)
		{
			return (Stream)(_contentReadStream = (IsBuffered ? CreateMemoryStreamFromBufferedContent() : CreateContentReadStream(cancellationToken)));
		}
		if (_contentReadStream is Stream result)
		{
			return result;
		}
		throw new HttpRequestException(System.SR.net_http_content_read_as_stream_has_task);
	}

	/// <summary>Serialize the HTTP content and return a stream that represents the content as an asynchronous operation.</summary>
	/// <returns>The task object representing the asynchronous operation.</returns>
	public Task<Stream> ReadAsStreamAsync()
	{
		return ReadAsStreamAsync(CancellationToken.None);
	}

	public Task<Stream> ReadAsStreamAsync(CancellationToken cancellationToken)
	{
		CheckDisposed();
		if (_contentReadStream == null)
		{
			return (Task<Stream>)(_contentReadStream = (IsBuffered ? ((Task)Task.FromResult((Stream)CreateMemoryStreamFromBufferedContent())) : ((Task)CreateContentReadStreamAsync(cancellationToken))));
		}
		if (_contentReadStream is Task<Stream> result)
		{
			return result;
		}
		return (Task<Stream>)(_contentReadStream = Task.FromResult((Stream)_contentReadStream));
	}

	internal Stream TryReadAsStream()
	{
		CheckDisposed();
		if (_contentReadStream == null)
		{
			return (Stream)(_contentReadStream = (IsBuffered ? CreateMemoryStreamFromBufferedContent() : TryCreateContentReadStream()));
		}
		if (_contentReadStream is Stream result)
		{
			return result;
		}
		Task<Stream> task = (Task<Stream>)_contentReadStream;
		if (task.Status != TaskStatus.RanToCompletion)
		{
			return null;
		}
		return task.Result;
	}

	/// <summary>Serialize the HTTP content to a stream as an asynchronous operation.</summary>
	/// <param name="stream">The target stream.</param>
	/// <param name="context">Information about the transport (channel binding token, for example). This parameter may be <see langword="null" />.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	protected abstract Task SerializeToStreamAsync(Stream stream, TransportContext? context);

	protected virtual void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		throw new NotSupportedException(System.SR.Format(System.SR.net_http_missing_sync_implementation, GetType(), "HttpContent", "SerializeToStream"));
	}

	protected virtual Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		return SerializeToStreamAsync(stream, context);
	}

	public void CopyTo(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		CheckDisposed();
		ArgumentNullException.ThrowIfNull(stream, "stream");
		try
		{
			if (IsBuffered)
			{
				stream.Write(_bufferedContent.GetSingleBuffer(), 0, (int)_bufferedContent.Length);
			}
			else
			{
				SerializeToStream(stream, context, cancellationToken);
			}
		}
		catch (Exception ex) when (StreamCopyExceptionNeedsWrapping(ex))
		{
			throw GetStreamCopyException(ex);
		}
	}

	/// <summary>Serialize the HTTP content into a stream of bytes and copies it to the stream object provided as the <paramref name="stream" /> parameter.</summary>
	/// <param name="stream">The target stream.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	public Task CopyToAsync(Stream stream)
	{
		return CopyToAsync(stream, CancellationToken.None);
	}

	public Task CopyToAsync(Stream stream, CancellationToken cancellationToken)
	{
		return CopyToAsync(stream, null, cancellationToken);
	}

	/// <summary>Serialize the HTTP content into a stream of bytes and copies it to the stream object provided as the <paramref name="stream" /> parameter.</summary>
	/// <param name="stream">The target stream.</param>
	/// <param name="context">Information about the transport (channel binding token, for example). This parameter may be <see langword="null" />.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	public Task CopyToAsync(Stream stream, TransportContext? context)
	{
		return CopyToAsync(stream, context, CancellationToken.None);
	}

	public Task CopyToAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		CheckDisposed();
		ArgumentNullException.ThrowIfNull(stream, "stream");
		try
		{
			return WaitAsync(InternalCopyToAsync(stream, context, cancellationToken));
		}
		catch (Exception ex) when (StreamCopyExceptionNeedsWrapping(ex))
		{
			return Task.FromException(GetStreamCopyException(ex));
		}
		static async Task WaitAsync(ValueTask copyTask)
		{
			try
			{
				await copyTask.ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception e) when (StreamCopyExceptionNeedsWrapping(e))
			{
				throw WrapStreamCopyException(e);
			}
		}
	}

	internal ValueTask InternalCopyToAsync(Stream stream, TransportContext context, CancellationToken cancellationToken)
	{
		if (IsBuffered)
		{
			return stream.WriteAsync(_bufferedContent.GetSingleBuffer().AsMemory(0, (int)_bufferedContent.Length), cancellationToken);
		}
		Task task = SerializeToStreamAsync(stream, context, cancellationToken);
		CheckTaskNotNull(task);
		return new ValueTask(task);
	}

	internal void LoadIntoBuffer(long maxBufferSize, CancellationToken cancellationToken)
	{
		CheckDisposed();
		if (!CreateTemporaryBuffer(maxBufferSize, out var tempBuffer, out var error))
		{
			return;
		}
		if (tempBuffer == null)
		{
			throw error;
		}
		CancellationTokenRegistration cancellationTokenRegistration = cancellationToken.Register(delegate(object s)
		{
			((HttpContent)s).Dispose();
		}, this);
		try
		{
			SerializeToStream(tempBuffer, null, cancellationToken);
		}
		catch (Exception ex)
		{
			tempBuffer.Dispose();
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(this, ex, "LoadIntoBuffer");
			}
			if (CancellationHelper.ShouldWrapInOperationCanceledException(ex, cancellationToken))
			{
				throw CancellationHelper.CreateOperationCanceledException(ex, cancellationToken);
			}
			if (StreamCopyExceptionNeedsWrapping(ex))
			{
				throw GetStreamCopyException(ex);
			}
			throw;
		}
		finally
		{
			cancellationTokenRegistration.Dispose();
		}
		tempBuffer.ReallocateIfPooled();
		_bufferedContent = tempBuffer;
	}

	/// <summary>Serialize the HTTP content to a memory buffer as an asynchronous operation.</summary>
	/// <returns>The task object representing the asynchronous operation.</returns>
	public Task LoadIntoBufferAsync()
	{
		return LoadIntoBufferAsync(2147483647L);
	}

	/// <summary>Serialize the HTTP content to a memory buffer as an asynchronous operation.</summary>
	/// <param name="maxBufferSize">The maximum size, in bytes, of the buffer to use.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	public Task LoadIntoBufferAsync(long maxBufferSize)
	{
		return LoadIntoBufferAsync(maxBufferSize, CancellationToken.None);
	}

	public Task LoadIntoBufferAsync(CancellationToken cancellationToken)
	{
		return LoadIntoBufferAsync(2147483647L, cancellationToken);
	}

	public Task LoadIntoBufferAsync(long maxBufferSize, CancellationToken cancellationToken)
	{
		CheckDisposed();
		if (!CreateTemporaryBuffer(maxBufferSize, out var tempBuffer, out var error))
		{
			return Task.CompletedTask;
		}
		if (tempBuffer == null)
		{
			return Task.FromException(error);
		}
		try
		{
			Task task = SerializeToStreamAsync(tempBuffer, null, cancellationToken);
			CheckTaskNotNull(task);
			return LoadIntoBufferAsyncCore(task, tempBuffer);
		}
		catch (Exception ex)
		{
			tempBuffer.Dispose();
			if (StreamCopyExceptionNeedsWrapping(ex))
			{
				return Task.FromException(GetStreamCopyException(ex));
			}
			throw;
		}
	}

	private async Task LoadIntoBufferAsyncCore(Task serializeToStreamTask, LimitArrayPoolWriteStream tempBuffer)
	{
		try
		{
			await serializeToStreamTask.ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception ex)
		{
			tempBuffer.Dispose();
			Exception streamCopyException = GetStreamCopyException(ex);
			if (streamCopyException != ex)
			{
				throw streamCopyException;
			}
			throw;
		}
		tempBuffer.ReallocateIfPooled();
		_bufferedContent = tempBuffer;
	}

	protected virtual Stream CreateContentReadStream(CancellationToken cancellationToken)
	{
		LoadIntoBuffer(2147483647L, cancellationToken);
		return CreateMemoryStreamFromBufferedContent();
	}

	/// <summary>Serialize the HTTP content to a memory stream as an asynchronous operation.</summary>
	/// <returns>The task object representing the asynchronous operation.</returns>
	protected virtual Task<Stream> CreateContentReadStreamAsync()
	{
		return WaitAndReturnAsync(LoadIntoBufferAsync(), this, (Func<HttpContent, Stream>)((HttpContent s) => s.CreateMemoryStreamFromBufferedContent()));
	}

	protected virtual Task<Stream> CreateContentReadStreamAsync(CancellationToken cancellationToken)
	{
		return CreateContentReadStreamAsync();
	}

	internal virtual Stream TryCreateContentReadStream()
	{
		return null;
	}

	/// <summary>Determines whether the HTTP content has a valid length in bytes.</summary>
	/// <param name="length">The length in bytes of the HTTP content.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="length" /> is a valid length; otherwise, <see langword="false" />.</returns>
	protected internal abstract bool TryComputeLength(out long length);

	internal long? GetComputedOrBufferLength()
	{
		CheckDisposed();
		if (IsBuffered)
		{
			return _bufferedContent.Length;
		}
		if (_canCalculateLength)
		{
			if (TryComputeLength(out var length))
			{
				return length;
			}
			_canCalculateLength = false;
		}
		return null;
	}

	private bool CreateTemporaryBuffer(long maxBufferSize, out LimitArrayPoolWriteStream tempBuffer, out Exception error)
	{
		if (maxBufferSize > int.MaxValue)
		{
			throw new ArgumentOutOfRangeException("maxBufferSize", maxBufferSize, System.SR.Format(CultureInfo.InvariantCulture, System.SR.net_http_content_buffersize_limit, int.MaxValue));
		}
		if (IsBuffered)
		{
			tempBuffer = null;
			error = null;
			return false;
		}
		long valueOrDefault = Headers.ContentLength.GetValueOrDefault();
		if (valueOrDefault > maxBufferSize)
		{
			tempBuffer = null;
			error = CreateOverCapacityException(maxBufferSize);
		}
		else
		{
			tempBuffer = new LimitArrayPoolWriteStream((int)maxBufferSize, valueOrDefault, getFinalSizeFromPool: false);
			error = null;
		}
		return true;
	}

	/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpContent" /> and optionally disposes of the managed resources.</summary>
	/// <param name="disposing">
	///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing && !_disposed)
		{
			_disposed = true;
			if (_contentReadStream != null)
			{
				((_contentReadStream as Stream) ?? ((_contentReadStream is Task<Stream> { Status: TaskStatus.RanToCompletion } task) ? task.Result : null))?.Dispose();
				_contentReadStream = null;
			}
			if (IsBuffered)
			{
				_bufferedContent.Dispose();
			}
		}
	}

	/// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:System.Net.Http.HttpContent" />.</summary>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void CheckDisposed()
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
	}

	private void CheckTaskNotNull(Task task)
	{
		if (task == null)
		{
			InvalidOperationException ex = new InvalidOperationException(System.SR.net_http_content_no_task_returned);
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(this, ex, "CheckTaskNotNull");
			}
			throw ex;
		}
	}

	internal static bool StreamCopyExceptionNeedsWrapping(Exception e)
	{
		if (!(e is IOException))
		{
			return e is ObjectDisposedException;
		}
		return true;
	}

	private static Exception GetStreamCopyException(Exception originalException)
	{
		if (!StreamCopyExceptionNeedsWrapping(originalException))
		{
			return originalException;
		}
		return WrapStreamCopyException(originalException);
	}

	internal static Exception WrapStreamCopyException(Exception e)
	{
		return ExceptionDispatchInfo.SetCurrentStackTrace(new HttpRequestException((e is HttpIOException ex) ? ex.HttpRequestError : HttpRequestError.Unknown, System.SR.net_http_content_stream_copy_error, e));
	}

	private static int GetPreambleLength(ReadOnlySpan<byte> data, Encoding encoding)
	{
		switch (encoding.CodePage)
		{
		case 65001:
			if (!data.StartsWith(UTF8Preamble))
			{
				return 0;
			}
			return UTF8Preamble.Length;
		case 12000:
			if (!data.StartsWith(UTF32Preamble))
			{
				return 0;
			}
			return UTF32Preamble.Length;
		case 1200:
			if (!data.StartsWith(UnicodePreamble))
			{
				return 0;
			}
			return UnicodePreamble.Length;
		case 1201:
			if (!data.StartsWith(BigEndianUnicodePreamble))
			{
				return 0;
			}
			return BigEndianUnicodePreamble.Length;
		default:
		{
			byte[] preamble = encoding.GetPreamble();
			if (preamble == null || !data.StartsWith(preamble))
			{
				return 0;
			}
			return preamble.Length;
		}
		}
	}

	private static bool TryDetectEncoding(ReadOnlySpan<byte> data, [NotNullWhen(true)] out Encoding encoding, out int preambleLength)
	{
		if (data.StartsWith(UTF8Preamble))
		{
			encoding = Encoding.UTF8;
			preambleLength = UTF8Preamble.Length;
			return true;
		}
		if (data.StartsWith(UTF32Preamble))
		{
			encoding = Encoding.UTF32;
			preambleLength = UTF32Preamble.Length;
			return true;
		}
		if (data.StartsWith(UnicodePreamble))
		{
			encoding = Encoding.Unicode;
			preambleLength = UnicodePreamble.Length;
			return true;
		}
		if (data.StartsWith(BigEndianUnicodePreamble))
		{
			encoding = Encoding.BigEndianUnicode;
			preambleLength = BigEndianUnicodePreamble.Length;
			return true;
		}
		encoding = null;
		preambleLength = 0;
		return false;
	}

	private static async Task<TResult> WaitAndReturnAsync<TState, TResult>(Task waitTask, TState state, Func<TState, TResult> returnFunc)
	{
		await waitTask.ConfigureAwait(continueOnCapturedContext: false);
		return returnFunc(state);
	}

	private static HttpRequestException CreateOverCapacityException(long maxBufferSize)
	{
		return (HttpRequestException)ExceptionDispatchInfo.SetCurrentStackTrace(new HttpRequestException(HttpRequestError.ConfigurationLimitExceeded, System.SR.Format(CultureInfo.InvariantCulture, System.SR.net_http_content_buffersize_exceeded, maxBufferSize)));
	}
}
