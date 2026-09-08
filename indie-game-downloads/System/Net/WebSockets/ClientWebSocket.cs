using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets;

public sealed class ClientWebSocket : WebSocket
{
	private enum InternalState
	{
		Created,
		Connecting,
		Connected,
		Disposed
	}

	private InternalState _state;

	private WebSocketHandle _innerWebSocket;

	public ClientWebSocketOptions Options { get; }

	public override WebSocketCloseStatus? CloseStatus => _innerWebSocket?.WebSocket?.CloseStatus;

	public override string? CloseStatusDescription => _innerWebSocket?.WebSocket?.CloseStatusDescription;

	public override string? SubProtocol => _innerWebSocket?.WebSocket?.SubProtocol;

	public override WebSocketState State
	{
		get
		{
			if (_innerWebSocket != null)
			{
				return _innerWebSocket.State;
			}
			return _state switch
			{
				InternalState.Created => WebSocketState.None, 
				InternalState.Connecting => WebSocketState.Connecting, 
				_ => WebSocketState.Closed, 
			};
		}
	}

	public HttpStatusCode HttpStatusCode => _innerWebSocket?.HttpStatusCode ?? ((HttpStatusCode)0);

	public IReadOnlyDictionary<string, IEnumerable<string>>? HttpResponseHeaders
	{
		get
		{
			return _innerWebSocket?.HttpResponseHeaders;
		}
		set
		{
			if (_innerWebSocket != null)
			{
				_innerWebSocket.HttpResponseHeaders = value;
			}
		}
	}

	private WebSocket ConnectedWebSocket
	{
		get
		{
			ObjectDisposedException.ThrowIf(_state == InternalState.Disposed, this);
			if (_state != InternalState.Connected)
			{
				throw new InvalidOperationException(System.SR.net_WebSockets_NotConnected);
			}
			return _innerWebSocket.WebSocket;
		}
	}

	public ClientWebSocket()
	{
		_state = InternalState.Created;
		Options = WebSocketHandle.CreateDefaultOptions();
	}

	public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
	{
		return ConnectAsync(uri, null, cancellationToken);
	}

	public Task ConnectAsync(Uri uri, HttpMessageInvoker? invoker, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(uri, "uri");
		if (!uri.IsAbsoluteUri)
		{
			throw new ArgumentException(System.SR.net_uri_NotAbsolute, "uri");
		}
		if (uri.Scheme != "ws" && uri.Scheme != "wss")
		{
			throw new ArgumentException(System.SR.net_WebSockets_Scheme, "uri");
		}
		switch (Interlocked.CompareExchange(ref _state, InternalState.Connecting, InternalState.Created))
		{
		case InternalState.Disposed:
			throw new ObjectDisposedException(GetType().FullName);
		default:
			throw new InvalidOperationException(System.SR.net_WebSockets_AlreadyStarted);
		case InternalState.Created:
			Options.SetToReadOnly();
			return ConnectAsyncCore(uri, invoker, cancellationToken);
		}
	}

	private async Task ConnectAsyncCore(Uri uri, HttpMessageInvoker invoker, CancellationToken cancellationToken)
	{
		_innerWebSocket = new WebSocketHandle();
		try
		{
			await _innerWebSocket.ConnectAsync(uri, invoker, cancellationToken, Options).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch
		{
			Dispose();
			throw;
		}
		if (Interlocked.CompareExchange(ref _state, InternalState.Connected, InternalState.Connecting) != InternalState.Connecting)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
	}

	public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
	{
		return ConnectedWebSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
	}

	public override ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
	{
		return ConnectedWebSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
	}

	public override ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, WebSocketMessageFlags messageFlags, CancellationToken cancellationToken)
	{
		return ConnectedWebSocket.SendAsync(buffer, messageType, messageFlags, cancellationToken);
	}

	public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
	{
		return ConnectedWebSocket.ReceiveAsync(buffer, cancellationToken);
	}

	public override ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
	{
		return ConnectedWebSocket.ReceiveAsync(buffer, cancellationToken);
	}

	public override Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
	{
		return ConnectedWebSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);
	}

	public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
	{
		return ConnectedWebSocket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);
	}

	public override void Abort()
	{
		if (_state != InternalState.Disposed)
		{
			_innerWebSocket?.Abort();
			Dispose();
		}
	}

	public override void Dispose()
	{
		if (Interlocked.Exchange(ref _state, InternalState.Disposed) != InternalState.Disposed)
		{
			_innerWebSocket?.Dispose();
		}
	}
}
