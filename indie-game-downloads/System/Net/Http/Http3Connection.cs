using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.QPack;
using System.Net.Quic;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
[SupportedOSPlatform("windows")]
internal sealed class Http3Connection : HttpConnectionBase
{
	private readonly HttpAuthority _authority;

	private readonly byte[] _altUsedEncodedHeader;

	private QuicConnection _connection;

	private Task _connectionClosedTask;

	private readonly Dictionary<QuicStream, Http3RequestStream> _activeRequests = new Dictionary<QuicStream, Http3RequestStream>();

	private long _firstRejectedStreamId = -1L;

	private QuicStream _clientControl;

	private Task _sendSettingsTask;

	private uint _maxHeaderListSize = uint.MaxValue;

	private bool _haveServerControlStream;

	private bool _haveServerQpackDecodeStream;

	private bool _haveServerQpackEncodeStream;

	private Exception _abortException;

	private int _availableRequestStreamsCount;

	private TaskCompletionSource<bool> _availableStreamsWaiter;

	public HttpAuthority Authority => _authority;

	public HttpConnectionPool Pool => _pool;

	public uint MaxHeaderListSize => _maxHeaderListSize;

	public byte[] AltUsedEncodedHeaderBytes => _altUsedEncodedHeader;

	public Exception AbortException => Volatile.Read(in _abortException);

	private object SyncObj => _activeRequests;

	private bool ShuttingDown => _firstRejectedStreamId != -1;

	public Http3Connection(HttpConnectionPool pool, HttpAuthority authority, bool includeAltUsedHeader)
		: base(pool)
	{
		_authority = authority;
		if (includeAltUsedHeader)
		{
			string text;
			if ((pool.Kind != HttpConnectionKind.Http || authority.Port != 80) && (pool.Kind != HttpConnectionKind.Https || authority.Port != 443))
			{
				IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
				DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(1, 2, invariantCulture);
				handler.AppendFormatted(authority.IdnHost);
				handler.AppendLiteral(":");
				handler.AppendFormatted(authority.Port);
				text = string.Create(invariantCulture, ref handler);
			}
			else
			{
				text = authority.IdnHost;
			}
			string value = text;
			_altUsedEncodedHeader = QPackEncoder.EncodeLiteralHeaderFieldWithoutNameReferenceToArray(KnownHeaders.AltUsed.Name, value);
		}
		uint lastSeenHttp3MaxHeaderListSize = _pool._lastSeenHttp3MaxHeaderListSize;
		if (lastSeenHttp3MaxHeaderListSize != 0)
		{
			_maxHeaderListSize = lastSeenHttp3MaxHeaderListSize;
		}
	}

	public void InitQuicConnection(QuicConnection connection, Activity connectionSetupActivity)
	{
		MarkConnectionAsEstablished(connectionSetupActivity, connection.RemoteEndPoint);
		_connection = connection;
		using (ExecutionContext.SuppressFlow())
		{
			_sendSettingsTask = SendSettingsAsync();
			AcceptStreamsAsync();
		}
	}

	public override void Dispose()
	{
		lock (SyncObj)
		{
			if (_firstRejectedStreamId == -1)
			{
				_firstRejectedStreamId = long.MaxValue;
				CheckForShutdown();
			}
		}
	}

	private void CheckForShutdown()
	{
		if (_activeRequests.Count != 0 || _connection == null)
		{
			return;
		}
		_availableStreamsWaiter?.SetResult(result: false);
		_availableStreamsWaiter = null;
		if (_connectionClosedTask == null)
		{
			_connectionClosedTask = _connection.CloseAsync(256L).AsTask();
		}
		QuicConnection connection = _connection;
		_connection = null;
		_connectionClosedTask.ContinueWith((Func<Task, Task>)async delegate(Task closeTask)
		{
			if (closeTask.IsFaulted && System.Net.NetEventSource.Log.IsEnabled())
			{
				Trace($"{"QuicConnection"} failed to close: {closeTask.Exception.InnerException}", "CheckForShutdown");
			}
			try
			{
				await connection.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception value)
			{
				Trace($"{"QuicConnection"} failed to dispose: {value}", "CheckForShutdown");
			}
			if (_clientControl != null)
			{
				await _sendSettingsTask.ConfigureAwait(continueOnCapturedContext: false);
				await _clientControl.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				_clientControl = null;
			}
		}, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		MarkConnectionAsClosed();
	}

	public bool TryReserveStream()
	{
		bool flag = !_pool.Settings.EnableMultipleHttp3Connections;
		lock (SyncObj)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				Trace($"_availableRequestStreamsCount = {_availableRequestStreamsCount}", "TryReserveStream");
			}
			bool result = _availableRequestStreamsCount > 0;
			if (flag || _availableRequestStreamsCount > 0)
			{
				_availableRequestStreamsCount--;
			}
			return result;
		}
	}

	public void ReleaseStream()
	{
		lock (SyncObj)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				Trace($"_availableRequestStreamsCount = {_availableRequestStreamsCount}", "ReleaseStream");
			}
			_availableRequestStreamsCount++;
			_availableStreamsWaiter?.SetResult(!ShuttingDown);
			_availableStreamsWaiter = null;
		}
	}

	public void StreamCapacityCallback(QuicConnection connection, QuicStreamCapacityChangedArgs args)
	{
		lock (SyncObj)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				Trace($"_availableRequestStreamsCount = {_availableRequestStreamsCount} + bidirectionalStreamsCountIncrement = {args.BidirectionalIncrement}", "StreamCapacityCallback");
			}
			_availableRequestStreamsCount += args.BidirectionalIncrement;
			_availableStreamsWaiter?.SetResult(!ShuttingDown);
			_availableStreamsWaiter = null;
		}
	}

	public Task<bool> WaitForAvailableStreamsAsync()
	{
		lock (SyncObj)
		{
			if (ShuttingDown)
			{
				return Task.FromResult(result: false);
			}
			if (_availableRequestStreamsCount > 0)
			{
				return Task.FromResult(result: true);
			}
			_availableStreamsWaiter = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			return _availableStreamsWaiter.Task;
		}
	}

	public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, WaitForHttp3ConnectionActivity waitForConnectionActivity, bool streamAvailable, CancellationToken cancellationToken)
	{
		QuicStream quicStream = null;
		Http3RequestStream requestStream = null;
		HttpResponseMessage result;
		try
		{
			_ = 1;
			try
			{
				Exception exception = null;
				try
				{
					QuicConnection connection = _connection;
					if (connection != null)
					{
						if (!waitForConnectionActivity.Started && !streamAvailable)
						{
							waitForConnectionActivity.Start();
						}
						quicStream = await connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						requestStream = new Http3RequestStream(request, this, quicStream);
						lock (SyncObj)
						{
							if (_activeRequests.Count == 0)
							{
								MarkConnectionAsNotIdle();
							}
							_activeRequests.Add(quicStream, requestStream);
						}
					}
				}
				catch (ObjectDisposedException ex)
				{
					exception = ex;
				}
				catch (QuicException ex2) when (ex2.QuicError != QuicError.OperationAborted)
				{
					exception = ex2;
				}
				finally
				{
					waitForConnectionActivity.Stop(request, Pool, exception);
				}
				if (quicStream == null)
				{
					throw new HttpRequestException(HttpRequestError.Unknown, System.SR.net_http_request_aborted, null, RequestRetryType.RetryOnConnectionFailure);
				}
				requestStream.StreamId = quicStream.Id;
				bool flag;
				lock (SyncObj)
				{
					flag = _firstRejectedStreamId != -1 && requestStream.StreamId >= _firstRejectedStreamId;
				}
				if (flag)
				{
					throw new HttpRequestException(HttpRequestError.Unknown, System.SR.net_http_request_aborted, null, RequestRetryType.RetryOnConnectionFailure);
				}
				if (base.ConnectionSetupActivity != null)
				{
					ConnectionSetupDistributedTracing.AddConnectionLinkToRequestActivity(base.ConnectionSetupActivity);
				}
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					Trace($"Sending request: {request}", "SendAsync");
				}
				Task<HttpResponseMessage> task = requestStream.SendAsync(cancellationToken);
				requestStream = null;
				result = await task.ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (QuicException ex3) when (ex3.QuicError == QuicError.OperationAborted)
			{
				throw new HttpRequestException(HttpRequestError.Unknown, System.SR.net_http_client_execution_error, _abortException, RequestRetryType.RetryOnConnectionFailure);
			}
		}
		finally
		{
			if (requestStream != null)
			{
				await requestStream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		return result;
	}

	internal Exception Abort(Exception abortException)
	{
		Exception ex = Interlocked.CompareExchange(ref _abortException, abortException, null);
		if (ex != null)
		{
			if (System.Net.NetEventSource.Log.IsEnabled() && ex != abortException)
			{
				Trace($"{"abortException"}=={abortException}", "Abort");
			}
			return ex;
		}
		_pool.InvalidateHttp3Connection(this, dispose: false);
		long errorCode = (abortException as HttpProtocolException)?.ErrorCode ?? 258;
		lock (SyncObj)
		{
			if (_firstRejectedStreamId == -1)
			{
				_firstRejectedStreamId = long.MaxValue;
			}
			if (_connection != null && _connectionClosedTask == null)
			{
				_connectionClosedTask = _connection.CloseAsync(errorCode).AsTask();
			}
			CheckForShutdown();
			return abortException;
		}
	}

	private void OnServerGoAway(long firstRejectedStreamId)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Trace($"GOAWAY received. First rejected stream ID = {firstRejectedStreamId}", "OnServerGoAway");
		}
		_pool.InvalidateHttp3Connection(this, dispose: false);
		List<Http3RequestStream> list = new List<Http3RequestStream>();
		lock (SyncObj)
		{
			if (_firstRejectedStreamId != -1 && firstRejectedStreamId > _firstRejectedStreamId)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					Trace("HTTP/3 server sent GOAWAY with increasing stream ID. Retried requests may have been double-processed by server.", "OnServerGoAway");
				}
				return;
			}
			_firstRejectedStreamId = firstRejectedStreamId;
			foreach (KeyValuePair<QuicStream, Http3RequestStream> activeRequest in _activeRequests)
			{
				if (activeRequest.Value.StreamId >= firstRejectedStreamId)
				{
					list.Add(activeRequest.Value);
				}
			}
			CheckForShutdown();
		}
		foreach (Http3RequestStream item in list)
		{
			item.GoAway();
		}
	}

	public void RemoveStream(QuicStream stream)
	{
		lock (SyncObj)
		{
			if (_activeRequests.Remove(stream))
			{
				if (ShuttingDown)
				{
					CheckForShutdown();
				}
				if (_activeRequests.Count == 0)
				{
					MarkConnectionAsIdle();
				}
			}
		}
	}

	public override void Trace(string message, [CallerMemberName] string memberName = null)
	{
		Trace(0L, (_connection != null) ? $"{_connection} {message}" : message, memberName);
	}

	internal void Trace(long streamId, string message, [CallerMemberName] string memberName = null)
	{
		System.Net.NetEventSource.Log.HandlerMessage(_pool?.GetHashCode() ?? 0, GetHashCode(), (int)streamId, memberName, message);
	}

	private async Task SendSettingsAsync()
	{
		_ = 1;
		try
		{
			_clientControl = await _connection.OpenOutboundStreamAsync(QuicStreamType.Unidirectional).ConfigureAwait(continueOnCapturedContext: false);
			_clientControl.WritesClosed.ContinueWith(delegate(Task t)
			{
				if (t.Exception?.InnerException is QuicException { QuicError: QuicError.StreamAborted })
				{
					Abort(HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.ClosedCriticalStream));
				}
			}, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);
			await _clientControl.WriteAsync(_pool.Settings.Http3SettingsFrame, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (QuicException ex) when (ex.QuicError == QuicError.ConnectionAborted)
		{
			Http3ErrorCode value = (Http3ErrorCode)ex.ApplicationErrorCode.Value;
			Abort(HttpProtocolException.CreateHttp3ConnectionException(value, System.SR.net_http_http3_connection_close));
		}
		catch (Exception abortException)
		{
			Abort(abortException);
		}
	}

	public static byte[] BuildSettingsFrame(HttpConnectionSettings settings)
	{
		Span<byte> span = stackalloc byte[12];
		int num = VariableLengthIntegerHelper.WriteInteger(span.Slice(4), settings.MaxResponseHeadersByteLength);
		int num2 = 1 + num;
		span[0] = 0;
		span[1] = 4;
		span[2] = (byte)num2;
		span[3] = 6;
		return span.Slice(0, 4 + num).ToArray();
	}

	private async Task AcceptStreamsAsync()
	{
		try
		{
			while (true)
			{
				ValueTask<QuicStream> valueTask;
				lock (SyncObj)
				{
					if (ShuttingDown)
					{
						break;
					}
					valueTask = _connection.AcceptInboundStreamAsync(CancellationToken.None);
				}
				ProcessServerStreamAsync(await valueTask.ConfigureAwait(continueOnCapturedContext: false));
			}
		}
		catch (QuicException ex) when (ex.QuicError == QuicError.OperationAborted)
		{
		}
		catch (QuicException ex2) when (ex2.QuicError == QuicError.ConnectionAborted)
		{
			Http3ErrorCode value = (Http3ErrorCode)ex2.ApplicationErrorCode.Value;
			Abort(HttpProtocolException.CreateHttp3ConnectionException(value, System.SR.net_http_http3_connection_close));
		}
		catch (Exception abortException)
		{
			Abort(abortException);
		}
	}

	private async Task ProcessServerStreamAsync(QuicStream stream)
	{
		System.Net.ArrayBuffer buffer = default(System.Net.ArrayBuffer);
		try
		{
			ConfiguredAsyncDisposable I_0 = stream.ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				if (stream.CanWrite)
				{
					throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.StreamCreationError);
				}
				buffer = new System.Net.ArrayBuffer(32, usePool: true);
				int num;
				try
				{
					num = await stream.ReadAsync(buffer.AvailableMemory, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (QuicException ex) when (ex.QuicError == QuicError.StreamAborted)
				{
					num = 0;
				}
				if (num == 0)
				{
					return;
				}
				buffer.Commit(num);
				switch (buffer.ActiveSpan[0])
				{
				case 0:
				{
					if (Interlocked.Exchange(ref _haveServerControlStream, value: true))
					{
						throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.StreamCreationError);
					}
					buffer.Discard(1);
					System.Net.ArrayBuffer buffer2 = buffer;
					buffer = default(System.Net.ArrayBuffer);
					await ProcessServerControlStreamAsync(stream, buffer2).ConfigureAwait(continueOnCapturedContext: false);
					return;
				}
				case 3:
					if (Interlocked.Exchange(ref _haveServerQpackDecodeStream, value: true))
					{
						throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.StreamCreationError);
					}
					buffer.Dispose();
					await stream.CopyToAsync(Stream.Null).ConfigureAwait(continueOnCapturedContext: false);
					return;
				case 2:
					if (Interlocked.Exchange(ref _haveServerQpackEncodeStream, value: true))
					{
						throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.StreamCreationError);
					}
					buffer.Dispose();
					await stream.CopyToAsync(Stream.Null).ConfigureAwait(continueOnCapturedContext: false);
					return;
				case 1:
					throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.IdError);
				}
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					long value;
					int bytesRead;
					while (!VariableLengthIntegerHelper.TryRead(buffer.ActiveSpan, out value, out bytesRead))
					{
						buffer.EnsureAvailableSpace(8);
						num = await stream.ReadAsync(buffer.AvailableMemory, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
						if (num == 0)
						{
							value = -1L;
							break;
						}
						buffer.Commit(num);
					}
					System.Net.NetEventSource.Info(this, $"Ignoring server-initiated stream of unknown type {value}.", "ProcessServerStreamAsync");
				}
				stream.Abort(QuicAbortDirection.Read, 259L);
			}
			finally
			{
				IAsyncDisposable asyncDisposable = I_0 as IAsyncDisposable;
				if (asyncDisposable != null)
				{
					await asyncDisposable.DisposeAsync();
				}
			}
		}
		catch (QuicException ex2) when (ex2.QuicError == QuicError.OperationAborted)
		{
		}
		catch (QuicException ex3) when (ex3.QuicError == QuicError.ConnectionAborted)
		{
			Http3ErrorCode value2 = (Http3ErrorCode)ex3.ApplicationErrorCode.Value;
			Abort(HttpProtocolException.CreateHttp3ConnectionException(value2, System.SR.net_http_http3_connection_close));
		}
		catch (Exception abortException)
		{
			Abort(abortException);
		}
		finally
		{
			buffer.Dispose();
		}
	}

	private async Task ProcessServerControlStreamAsync(QuicStream stream, System.Net.ArrayBuffer buffer)
	{
		try
		{
			using (buffer)
			{
				Http3FrameType? http3FrameType;
				long settingsPayloadLength;
				(http3FrameType, settingsPayloadLength) = await ReadFrameEnvelopeAsync().ConfigureAwait(continueOnCapturedContext: false);
				if (!http3FrameType.HasValue)
				{
					throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.ClosedCriticalStream);
				}
				if (http3FrameType != Http3FrameType.Settings)
				{
					throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.MissingSettings);
				}
				await ProcessSettingsFrameAsync(settingsPayloadLength).ConfigureAwait(continueOnCapturedContext: false);
				while (true)
				{
					(http3FrameType, settingsPayloadLength) = await ReadFrameEnvelopeAsync().ConfigureAwait(continueOnCapturedContext: false);
					if (!http3FrameType.HasValue)
					{
						break;
					}
					Http3FrameType valueOrDefault = http3FrameType.GetValueOrDefault();
					if ((ulong)valueOrDefault <= 13uL)
					{
						switch ((int)valueOrDefault)
						{
						case 7:
							await ProcessGoAwayFrameAsync(settingsPayloadLength).ConfigureAwait(continueOnCapturedContext: false);
							continue;
						case 4:
							throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.UnexpectedFrame);
						case 0:
						case 1:
						case 2:
						case 6:
						case 8:
						case 9:
						case 13:
							if (System.Net.NetEventSource.Log.IsEnabled())
							{
								Trace($"Received reserved frame: {http3FrameType}", "ProcessServerControlStreamAsync");
							}
							throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.UnexpectedFrame);
						case 3:
						case 5:
							throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.IdError);
						}
					}
					await SkipUnknownPayloadAsync(settingsPayloadLength).ConfigureAwait(continueOnCapturedContext: false);
				}
				bool shuttingDown;
				lock (SyncObj)
				{
					shuttingDown = ShuttingDown;
				}
				if (!shuttingDown)
				{
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						Trace("Control stream closed by the server.", "ProcessServerControlStreamAsync");
					}
					throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.ClosedCriticalStream);
				}
			}
		}
		catch (QuicException ex) when (ex.QuicError == QuicError.StreamAborted)
		{
			throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.ClosedCriticalStream);
		}
		async ValueTask ProcessGoAwayFrameAsync(long goawayPayloadLength)
		{
			long value;
			int bytesRead;
			while (!VariableLengthIntegerHelper.TryRead(buffer.ActiveSpan, out value, out bytesRead))
			{
				buffer.EnsureAvailableSpace(8);
				bytesRead = await stream.ReadAsync(buffer.AvailableMemory, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
				if (bytesRead == 0)
				{
					throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.FrameError);
				}
				buffer.Commit(bytesRead);
			}
			buffer.Discard(bytesRead);
			if (bytesRead != goawayPayloadLength)
			{
				throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.FrameError);
			}
			OnServerGoAway(value);
		}
		async ValueTask ProcessSettingsFrameAsync(long num)
		{
			while (num != 0L)
			{
				long a;
				long b;
				int bytesRead;
				while (!Http3Frame.TryReadIntegerPair(buffer.ActiveSpan, out a, out b, out bytesRead))
				{
					buffer.EnsureAvailableSpace(16);
					bytesRead = await stream.ReadAsync(buffer.AvailableMemory, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
					if (bytesRead == 0)
					{
						throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.FrameError);
					}
					buffer.Commit(bytesRead);
				}
				num -= bytesRead;
				if (num < 0)
				{
					throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.FrameError);
				}
				buffer.Discard(bytesRead);
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					Trace($"Applying setting {(Http3SettingType)a}={b}", "ProcessServerControlStreamAsync");
				}
				switch ((Http3SettingType)a)
				{
				case Http3SettingType.MaxHeaderListSize:
					_maxHeaderListSize = (uint)Math.Min((ulong)b, 4294967295uL);
					_pool._lastSeenHttp3MaxHeaderListSize = _maxHeaderListSize;
					break;
				case Http3SettingType.ReservedHttp2EnablePush:
				case Http3SettingType.ReservedHttp2MaxConcurrentStreams:
				case Http3SettingType.ReservedHttp2InitialWindowSize:
				case Http3SettingType.ReservedHttp2MaxFrameSize:
					throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.SettingsError);
				}
			}
		}
		async ValueTask<(Http3FrameType? frameType, long payloadLength)> ReadFrameEnvelopeAsync()
		{
			long a;
			long b;
			int bytesRead;
			while (!Http3Frame.TryReadIntegerPair(buffer.ActiveSpan, out a, out b, out bytesRead))
			{
				buffer.EnsureAvailableSpace(16);
				bytesRead = await stream.ReadAsync(buffer.AvailableMemory, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
				if (bytesRead == 0)
				{
					if (buffer.ActiveLength == 0)
					{
						return (null, 0L);
					}
					throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.FrameError);
				}
				buffer.Commit(bytesRead);
			}
			buffer.Discard(bytesRead);
			return ((Http3FrameType)a, b);
		}
		async ValueTask SkipUnknownPayloadAsync(long payloadLength)
		{
			while (payloadLength != 0L)
			{
				if (buffer.ActiveLength == 0)
				{
					int num = await stream.ReadAsync(buffer.AvailableMemory, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
					if (num == 0)
					{
						throw HttpProtocolException.CreateHttp3ConnectionException(Http3ErrorCode.FrameError);
					}
					buffer.Commit(num);
				}
				long num2 = Math.Min(payloadLength, buffer.ActiveLength);
				buffer.Discard((int)num2);
				payloadLength -= num2;
			}
		}
	}
}
