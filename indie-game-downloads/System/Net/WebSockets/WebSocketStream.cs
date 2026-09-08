using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets;

public class WebSocketStream : Stream
{
	private sealed class ReadWriteStream(WebSocket webSocket, WebSocketMessageType writeMessageType, TimeSpan? closeTimeout) : WebSocketStream(webSocket)
	{
		private readonly WebSocketMessageType _messageType = writeMessageType;

		private readonly TimeSpan? _closeTimeout = closeTimeout;

		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_disposed)
			{
				return ValueTask.FromException(ExceptionDispatchInfo.SetCurrentStackTrace(new ObjectDisposedException(GetType().FullName)));
			}
			if (!CanWrite)
			{
				return ValueTask.FromException(ExceptionDispatchInfo.SetCurrentStackTrace(new NotSupportedException(System.SR.NotWriteableStream)));
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return ValueTask.FromCanceled(cancellationToken);
			}
			return base.WebSocket.SendAsync(buffer, _messageType, endOfMessage: true, cancellationToken);
		}

		public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			ObjectDisposedException.ThrowIf(_disposed, this);
			if (!CanRead)
			{
				throw new NotSupportedException(System.SR.NotReadableStream);
			}
			cancellationToken.ThrowIfCancellationRequested();
			while (base.WebSocket.State < WebSocketState.CloseReceived)
			{
				ValueWebSocketReceiveResult valueWebSocketReceiveResult = await base.WebSocket.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				if (valueWebSocketReceiveResult.MessageType == WebSocketMessageType.Close)
				{
					break;
				}
				if (valueWebSocketReceiveResult.Count > 0 || buffer.IsEmpty)
				{
					return valueWebSocketReceiveResult.Count;
				}
			}
			return 0;
		}

		public override async ValueTask DisposeAsync()
		{
			if (_disposed)
			{
				return;
			}
			_disposed = true;
			TimeSpan? closeTimeout = _closeTimeout;
			if (!closeTimeout.HasValue)
			{
				return;
			}
			TimeSpan valueOrDefault = closeTimeout.GetValueOrDefault();
			if (base.WebSocket.State < WebSocketState.Closed)
			{
				CancellationTokenSource cts = null;
				CancellationToken cancellationToken;
				if (valueOrDefault == default(TimeSpan))
				{
					cancellationToken = new CancellationToken(canceled: true);
				}
				else if (valueOrDefault == Timeout.InfiniteTimeSpan)
				{
					cancellationToken = CancellationToken.None;
				}
				else
				{
					cts = new CancellationTokenSource(valueOrDefault);
					cancellationToken = cts.Token;
				}
				try
				{
					await base.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
				}
				finally
				{
					cts?.Dispose();
				}
			}
			base.WebSocket.Dispose();
		}
	}

	private sealed class WriteMessageStream(WebSocket webSocket, WebSocketMessageType writeMessageType) : WebSocketStream(webSocket)
	{
		private readonly WebSocketMessageType _messageType = writeMessageType;

		public override bool CanRead => false;

		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_disposed)
			{
				return ValueTask.FromException(ExceptionDispatchInfo.SetCurrentStackTrace(new ObjectDisposedException(GetType().FullName)));
			}
			if (!CanWrite)
			{
				return ValueTask.FromException(ExceptionDispatchInfo.SetCurrentStackTrace(new NotSupportedException(System.SR.NotWriteableStream)));
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return ValueTask.FromCanceled(cancellationToken);
			}
			return base.WebSocket.SendAsync(buffer, _messageType, endOfMessage: false, cancellationToken);
		}

		public override ValueTask DisposeAsync()
		{
			if (!_disposed)
			{
				_disposed = true;
				return base.WebSocket.SendAsync(ReadOnlyMemory<byte>.Empty, _messageType, endOfMessage: true, CancellationToken.None);
			}
			return default(ValueTask);
		}
	}

	private sealed class ReadMessageStream : WebSocketStream
	{
		private bool _eof;

		public override bool CanWrite => false;

		public ReadMessageStream(WebSocket webSocket)
			: base(webSocket)
		{
		}

		public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			ObjectDisposedException.ThrowIf(_disposed, this);
			if (!CanRead)
			{
				throw new NotSupportedException(System.SR.NotReadableStream);
			}
			cancellationToken.ThrowIfCancellationRequested();
			while (!_eof && base.WebSocket.State < WebSocketState.CloseReceived)
			{
				ValueWebSocketReceiveResult valueWebSocketReceiveResult = await base.WebSocket.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				if (valueWebSocketReceiveResult.MessageType == WebSocketMessageType.Close)
				{
					break;
				}
				if (valueWebSocketReceiveResult.EndOfMessage)
				{
					_eof = true;
				}
				if (valueWebSocketReceiveResult.Count > 0 || buffer.IsEmpty)
				{
					return valueWebSocketReceiveResult.Count;
				}
			}
			return 0;
		}

		public override ValueTask DisposeAsync()
		{
			_disposed = true;
			if (!_eof && base.WebSocket.State < WebSocketState.CloseReceived)
			{
				base.WebSocket.Abort();
			}
			return default(ValueTask);
		}
	}

	private bool _disposed;

	public WebSocket WebSocket { get; }

	public override bool CanRead
	{
		get
		{
			bool flag = !_disposed;
			if (flag)
			{
				WebSocketState state = WebSocket.State;
				bool flag2 = (uint)(state - 2) <= 1u;
				flag = flag2;
			}
			return flag;
		}
	}

	public override bool CanWrite
	{
		get
		{
			bool flag = !_disposed;
			if (flag)
			{
				WebSocketState state = WebSocket.State;
				bool flag2 = ((state == WebSocketState.Open || state == WebSocketState.CloseReceived) ? true : false);
				flag = flag2;
			}
			return flag;
		}
	}

	public override bool CanSeek => false;

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

	private WebSocketStream(WebSocket webSocket)
	{
		WebSocket = webSocket;
	}

	public static WebSocketStream Create(WebSocket webSocket, WebSocketMessageType writeMessageType, bool ownsWebSocket = false)
	{
		ArgumentNullException.ThrowIfNull(webSocket, "webSocket");
		ManagedWebSocket.ThrowIfInvalidMessageType(writeMessageType, "writeMessageType");
		return new ReadWriteStream(webSocket, writeMessageType, ownsWebSocket ? new TimeSpan?(TimeSpan.FromSeconds(16L)) : ((TimeSpan?)null));
	}

	public static WebSocketStream Create(WebSocket webSocket, WebSocketMessageType writeMessageType, TimeSpan closeTimeout)
	{
		ArgumentNullException.ThrowIfNull(webSocket, "webSocket");
		ManagedWebSocket.ThrowIfInvalidMessageType(writeMessageType, "writeMessageType");
		if (closeTimeout < TimeSpan.Zero && closeTimeout != Timeout.InfiniteTimeSpan)
		{
			throw new ArgumentOutOfRangeException("closeTimeout", System.SR.net_WebSockets_TimeoutOutOfRange);
		}
		return new ReadWriteStream(webSocket, writeMessageType, closeTimeout);
	}

	public static WebSocketStream CreateWritableMessageStream(WebSocket webSocket, WebSocketMessageType writeMessageType)
	{
		ArgumentNullException.ThrowIfNull(webSocket, "webSocket");
		ManagedWebSocket.ThrowIfInvalidMessageType(writeMessageType, "writeMessageType");
		return new WriteMessageStream(webSocket, writeMessageType);
	}

	public static WebSocketStream CreateReadableMessageStream(WebSocket webSocket)
	{
		ArgumentNullException.ThrowIfNull(webSocket, "webSocket");
		return new ReadMessageStream(webSocket);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			DisposeAsync().AsTask().GetAwaiter().GetResult();
		}
	}

	public override void Flush()
	{
	}

	public override Task FlushAsync(CancellationToken cancellationToken)
	{
		if (!cancellationToken.IsCancellationRequested)
		{
			return Task.CompletedTask;
		}
		return Task.FromCanceled(cancellationToken);
	}

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		Stream.ValidateBufferArguments(buffer, offset, count);
		return ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
	}

	public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ValueTask.FromException<int>(ExceptionDispatchInfo.SetCurrentStackTrace(new NotSupportedException()));
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
	{
		return TaskToAsyncResult.Begin(ReadAsync(buffer, offset, count), callback, state);
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		return TaskToAsyncResult.End<int>(asyncResult);
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		Stream.ValidateBufferArguments(buffer, offset, count);
		return WriteAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
	}

	public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ValueTask.FromException(ExceptionDispatchInfo.SetCurrentStackTrace(new NotSupportedException()));
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
	{
		return TaskToAsyncResult.Begin(WriteAsync(buffer, offset, count), callback, state);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		TaskToAsyncResult.End(asyncResult);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return ReadAsync(buffer, offset, count, default(CancellationToken)).GetAwaiter().GetResult();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		WriteAsync(buffer, offset, count, default(CancellationToken)).GetAwaiter().GetResult();
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
