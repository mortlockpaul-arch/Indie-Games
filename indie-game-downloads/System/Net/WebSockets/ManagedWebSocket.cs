using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets;

internal sealed class ManagedWebSocket : WebSocket
{
	private sealed class Utf8MessageState
	{
		internal bool SequenceInProgress;

		internal int AdditionalBytesExpected;

		internal int ExpectedValueMin;

		internal int CurrentDecodeBits;
	}

	private enum MessageOpcode : byte
	{
		Continuation = 0,
		Text = 1,
		Binary = 2,
		Close = 8,
		Ping = 9,
		Pong = 10
	}

	[StructLayout(LayoutKind.Auto)]
	private struct MessageHeader
	{
		internal MessageOpcode Opcode;

		internal bool Fin;

		internal long PayloadLength;

		internal bool Compressed;

		internal int Mask;

		internal bool Processed { get; set; }

		internal bool EndOfMessage
		{
			get
			{
				if (Fin && Processed)
				{
					return PayloadLength == 0;
				}
				return false;
			}
		}
	}

	private sealed class KeepAlivePingState
	{
		private readonly ManagedWebSocket _parent;

		private object StateUpdateLock => _parent.StateUpdateLock;

		internal int DelayMs { get; }

		internal int TimeoutMs { get; }

		internal int HeartBeatIntervalMs => Math.Max(Math.Min(DelayMs, TimeoutMs) / 4, 1);

		internal long PingPayload { get; private set; }

		internal bool PingSent { get; private set; }

		internal long PingTimeoutTimestamp { get; private set; }

		internal long NextPingRequestTimestamp { get; private set; }

		internal Exception Exception { get; private set; }

		public KeepAlivePingState(TimeSpan keepAliveInterval, TimeSpan keepAliveTimeout, ManagedWebSocket parent)
		{
			DelayMs = TimeSpanToMs(keepAliveInterval);
			TimeoutMs = TimeSpanToMs(keepAliveTimeout);
			NextPingRequestTimestamp = Environment.TickCount64 + DelayMs;
			PingTimeoutTimestamp = -1L;
			_parent = parent;
			static int TimeSpanToMs(TimeSpan value)
			{
				return (int)Math.Clamp((long)value.TotalMilliseconds, 1L, 2147483647L);
			}
		}

		internal void OnDataReceived()
		{
			lock (StateUpdateLock)
			{
				NextPingRequestTimestamp = Environment.TickCount64 + DelayMs;
			}
		}

		internal void OnPongResponseReceived(long pongPayload)
		{
			lock (StateUpdateLock)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Trace(this, $"pongPayload={pongPayload}", "OnPongResponseReceived");
				}
				if (!PingSent)
				{
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						System.Net.NetEventSource.Trace(this, "Not waiting for Pong. Skipping.", "OnPongResponseReceived");
					}
				}
				else if (pongPayload == PingPayload)
				{
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						System.Net.NetEventSource.PongResponseReceived(this, pongPayload);
					}
					PingTimeoutTimestamp = long.MaxValue;
					PingSent = false;
				}
				else if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Trace(this, $"Expected payload {PingPayload}. Skipping.", "OnPongResponseReceived");
				}
			}
		}

		internal void OnNextPingRequestCore()
		{
			PingSent = true;
			PingTimeoutTimestamp = Environment.TickCount64 + TimeoutMs;
			long pingPayload = PingPayload + 1;
			PingPayload = pingPayload;
		}

		internal void OnKeepAliveFaulted(Exception exc)
		{
			lock (StateUpdateLock)
			{
				OnKeepAliveFaultedCore(exc);
			}
		}

		internal void OnKeepAliveFaultedCore(Exception exc)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.TraceErrorMsg(this, exc, "OnKeepAliveFaultedCore");
			}
			if (_parent._disposed)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Trace(this, "WebSocket already disposed, skipping...", "OnKeepAliveFaultedCore");
				}
			}
			else if (_parent.State == WebSocketState.Closed)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Trace(this, "WebSocket is already closed, skipping...", "OnKeepAliveFaultedCore");
				}
			}
			else if (_parent.State == WebSocketState.Aborted)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Trace(this, "WebSocket is already aborted, skipping...", "OnKeepAliveFaultedCore");
				}
				_parent.Dispose();
			}
			else
			{
				Exception = exc;
				_parent.OnAbortedCore();
				_parent.DisposeCore();
			}
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CEnsureBufferContainsAsync_003Ed__72 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public PoolingAsyncValueTaskMethodBuilder _003C_003Et__builder;

		public ManagedWebSocket _003C_003E4__this;

		public int minimumRequiredBytes;

		public CancellationToken cancellationToken;

		private int _003CbytesToRead_003E5__2;

		private ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			ManagedWebSocket managedWebSocket = _003C_003E4__this;
			try
			{
				ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_0113;
				}
				if (managedWebSocket._receiveBufferCount < minimumRequiredBytes)
				{
					if (managedWebSocket._receiveBufferCount > 0)
					{
						managedWebSocket._receiveBuffer.Span.Slice(managedWebSocket._receiveBufferOffset, managedWebSocket._receiveBufferCount).CopyTo(managedWebSocket._receiveBuffer.Span);
					}
					managedWebSocket._receiveBufferOffset = 0;
					if (managedWebSocket._receiveBufferCount < minimumRequiredBytes)
					{
						_003CbytesToRead_003E5__2 = minimumRequiredBytes - managedWebSocket._receiveBufferCount;
						awaiter = managedWebSocket._stream.ReadAtLeastAsync(managedWebSocket._receiveBuffer.Slice(managedWebSocket._receiveBufferCount), _003CbytesToRead_003E5__2, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0113;
					}
				}
				goto end_IL_000e;
				IL_0113:
				int result = awaiter.GetResult();
				managedWebSocket._receiveBufferCount += result;
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Trace(managedWebSocket, $"bytesRead={result}", "EnsureBufferContainsAsync");
				}
				if (result < _003CbytesToRead_003E5__2)
				{
					managedWebSocket.ThrowEOFUnexpected();
				}
				managedWebSocket._keepAlivePingState?.OnDataReceived();
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult();
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CReceiveAsyncPrivate_003Ed__61<TResult> : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public PoolingAsyncValueTaskMethodBuilder<TResult> _003C_003Et__builder;

		public ManagedWebSocket _003C_003E4__this;

		public Memory<byte> payloadBuffer;

		public CancellationToken cancellationToken;

		private CancellationTokenRegistration _003Cregistration_003E5__2;

		private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _003C_003Eu__1;

		private MessageHeader _003Cheader_003E5__3;

		private int _003CtotalBytesReceived_003E5__4;

		private ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter _003C_003Eu__2;

		private ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter _003C_003Eu__3;

		private int _003CbytesToRead_003E5__5;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			ManagedWebSocket managedWebSocket = _003C_003E4__this;
			TResult receiveResult;
			try
			{
				if ((uint)num > 8u)
				{
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						System.Net.NetEventSource.ReceiveAsyncPrivateStarted(managedWebSocket, payloadBuffer.Length, "ReceiveAsyncPrivate");
					}
					_003Cregistration_003E5__2 = default(CancellationTokenRegistration);
				}
				try
				{
					ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						if ((uint)(num - 1) <= 7u)
						{
							goto IL_010e;
						}
						_003Cregistration_003E5__2 = cancellationToken.Register(delegate(object s)
						{
							((ManagedWebSocket)s).Abort();
						}, managedWebSocket);
						awaiter = managedWebSocket._receiveMutex.EnterAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					awaiter.GetResult();
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						System.Net.NetEventSource.MutexEntered(managedWebSocket._receiveMutex, "ReceiveAsyncPrivate");
					}
					goto IL_010e;
					IL_010e:
					try
					{
						ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter awaiter3;
						ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter awaiter2;
						string text;
						int result;
						Span<byte> span;
						long num4;
						switch (num)
						{
						default:
							managedWebSocket.ThrowIfDisposed();
							goto IL_013c;
						case 1:
							awaiter3 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_021b;
						case 2:
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_028f;
						case 3:
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_035e;
						case 4:
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_03e5;
						case 5:
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_0556;
						case 6:
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_05e4;
						case 7:
							awaiter3 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_0862;
						case 8:
							{
								awaiter2 = _003C_003Eu__3;
								_003C_003Eu__3 = default(ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_0aae;
							}
							IL_013c:
							_003Cheader_003E5__3 = managedWebSocket._lastReceiveHeader;
							if (_003Cheader_003E5__3.Processed)
							{
								if (System.Net.NetEventSource.Log.IsEnabled())
								{
									System.Net.NetEventSource.Trace(managedWebSocket, "Reading the next frame header", "ReceiveAsyncPrivate");
								}
								if (managedWebSocket._receiveBufferCount < (managedWebSocket._isServer ? 14 : 10))
								{
									if (managedWebSocket._receiveBufferCount < 2)
									{
										if (payloadBuffer.IsEmpty)
										{
											awaiter3 = managedWebSocket._stream.ReadAsync(Memory<byte>.Empty, cancellationToken).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
											if (!awaiter3.IsCompleted)
											{
												num = (_003C_003E1__state = 1);
												_003C_003Eu__2 = awaiter3;
												_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
												return;
											}
											goto IL_021b;
										}
										goto IL_0223;
									}
									goto IL_0296;
								}
								goto IL_0365;
							}
							goto IL_04c7;
							IL_0365:
							text = managedWebSocket.TryParseMessageHeaderFromReceiveBuffer(out _003Cheader_003E5__3);
							if (text != null)
							{
								awaiter2 = managedWebSocket.CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted, text).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = (_003C_003E1__state = 4);
									_003C_003Eu__3 = awaiter2;
									_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
									return;
								}
								goto IL_03e5;
							}
							goto IL_03ec;
							IL_0556:
							awaiter2.GetResult();
							goto IL_013c;
							IL_035e:
							awaiter2.GetResult();
							goto IL_0365;
							IL_0994:
							if (_003Cheader_003E5__3.Compressed)
							{
								_003Cheader_003E5__3.Processed = managedWebSocket._inflater.Inflate(payloadBuffer.Span, out _003CtotalBytesReceived_003E5__4) && _003Cheader_003E5__3.PayloadLength == 0;
							}
							else
							{
								_003Cheader_003E5__3.Processed = _003Cheader_003E5__3.PayloadLength == 0;
							}
							if (_003Cheader_003E5__3.Opcode != MessageOpcode.Text || TryValidateUtf8(payloadBuffer.Span.Slice(0, _003CtotalBytesReceived_003E5__4), _003Cheader_003E5__3.EndOfMessage, managedWebSocket._utf8TextState))
							{
								break;
							}
							awaiter2 = managedWebSocket.CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.InvalidPayloadData, WebSocketError.Faulted).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 8);
								_003C_003Eu__3 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_0aae;
							IL_0aae:
							awaiter2.GetResult();
							break;
							IL_03e5:
							awaiter2.GetResult();
							goto IL_03ec;
							IL_05e4:
							awaiter2.GetResult();
							receiveResult = managedWebSocket.GetReceiveResult<TResult>(0, WebSocketMessageType.Close, endOfMessage: true);
							goto end_IL_010e;
							IL_0862:
							result = awaiter3.GetResult();
							if (System.Net.NetEventSource.Log.IsEnabled())
							{
								System.Net.NetEventSource.Trace(managedWebSocket, $"bytesRead={result}", "ReceiveAsyncPrivate");
							}
							if (result < _003CbytesToRead_003E5__5)
							{
								managedWebSocket.ThrowEOFUnexpected();
							}
							managedWebSocket._keepAlivePingState?.OnDataReceived();
							_003CtotalBytesReceived_003E5__4 += result;
							goto IL_08d8;
							IL_08d8:
							if (managedWebSocket._isServer)
							{
								Span<byte> toMask;
								if (!_003Cheader_003E5__3.Compressed)
								{
									span = payloadBuffer.Span;
									toMask = span.Slice(0, _003CtotalBytesReceived_003E5__4);
								}
								else
								{
									toMask = managedWebSocket._inflater.Span.Slice(0, _003CtotalBytesReceived_003E5__4);
								}
								managedWebSocket._receivedMaskOffsetOffset = ApplyMask(toMask, _003Cheader_003E5__3.Mask, managedWebSocket._receivedMaskOffsetOffset);
							}
							_003Cheader_003E5__3.PayloadLength -= _003CtotalBytesReceived_003E5__4;
							if (_003Cheader_003E5__3.Compressed)
							{
								managedWebSocket._inflater.AddBytes(_003CtotalBytesReceived_003E5__4, _003Cheader_003E5__3.Fin && _003Cheader_003E5__3.PayloadLength == 0);
							}
							goto IL_0994;
							IL_03ec:
							managedWebSocket._receivedMaskOffsetOffset = 0;
							if (System.Net.NetEventSource.Log.IsEnabled())
							{
								System.Net.NetEventSource.Trace(managedWebSocket, $"Next frame opcode={_003Cheader_003E5__3.Opcode}, fin={_003Cheader_003E5__3.Fin}, compressed={_003Cheader_003E5__3.Compressed}, payloadLength={_003Cheader_003E5__3.PayloadLength}", "ReceiveAsyncPrivate");
							}
							if (_003Cheader_003E5__3.PayloadLength == 0L && _003Cheader_003E5__3.Compressed)
							{
								managedWebSocket._inflater.AddBytes(0, _003Cheader_003E5__3.Fin);
							}
							goto IL_04c7;
							IL_021b:
							awaiter3.GetResult();
							goto IL_0223;
							IL_0223:
							awaiter2 = managedWebSocket.EnsureBufferContainsAsync(2, cancellationToken).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_028f;
							IL_04c7:
							if (_003Cheader_003E5__3.Opcode == MessageOpcode.Ping || _003Cheader_003E5__3.Opcode == MessageOpcode.Pong)
							{
								awaiter2 = managedWebSocket.HandleReceivedPingPongAsync(_003Cheader_003E5__3, cancellationToken).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = (_003C_003E1__state = 5);
									_003C_003Eu__3 = awaiter2;
									_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
									return;
								}
								goto IL_0556;
							}
							if (_003Cheader_003E5__3.Opcode == MessageOpcode.Close)
							{
								awaiter2 = managedWebSocket.HandleReceivedCloseAsync(_003Cheader_003E5__3, cancellationToken).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = (_003C_003E1__state = 6);
									_003C_003Eu__3 = awaiter2;
									_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
									return;
								}
								goto IL_05e4;
							}
							if (_003Cheader_003E5__3.Opcode == MessageOpcode.Continuation)
							{
								_003Cheader_003E5__3.Opcode = managedWebSocket._lastReceiveHeader.Opcode;
								_003Cheader_003E5__3.Compressed = managedWebSocket._lastReceiveHeader.Compressed;
							}
							if (!_003Cheader_003E5__3.Processed && payloadBuffer.Length != 0)
							{
								_003CtotalBytesReceived_003E5__4 = 0;
								if (_003Cheader_003E5__3.PayloadLength > 0)
								{
									if (_003Cheader_003E5__3.Compressed)
									{
										managedWebSocket._inflater.Prepare(_003Cheader_003E5__3.PayloadLength, payloadBuffer.Length);
									}
									int length;
									Span<byte> span2;
									if (!_003Cheader_003E5__3.Compressed)
									{
										length = payloadBuffer.Length;
									}
									else
									{
										span2 = managedWebSocket._inflater.Span;
										length = span2.Length;
									}
									int num2 = (int)Math.Min(length, _003Cheader_003E5__3.PayloadLength);
									if (managedWebSocket._receiveBufferCount > 0)
									{
										int num3 = Math.Min(num2, managedWebSocket._receiveBufferCount);
										span2 = managedWebSocket._receiveBuffer.Span;
										span = span2.Slice(managedWebSocket._receiveBufferOffset, num3);
										span.CopyTo(_003Cheader_003E5__3.Compressed ? managedWebSocket._inflater.Span : payloadBuffer.Span);
										managedWebSocket.ConsumeFromBuffer(num3);
										_003CtotalBytesReceived_003E5__4 += num3;
									}
									if (_003CtotalBytesReceived_003E5__4 < num2)
									{
										_003CbytesToRead_003E5__5 = num2 - _003CtotalBytesReceived_003E5__4;
										Memory<byte> buffer = (_003Cheader_003E5__3.Compressed ? managedWebSocket._inflater.Memory.Slice(_003CtotalBytesReceived_003E5__4, _003CbytesToRead_003E5__5) : payloadBuffer.Slice(_003CtotalBytesReceived_003E5__4, _003CbytesToRead_003E5__5));
										awaiter3 = managedWebSocket._stream.ReadAtLeastAsync(buffer, _003CbytesToRead_003E5__5, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
										if (!awaiter3.IsCompleted)
										{
											num = (_003C_003E1__state = 7);
											_003C_003Eu__2 = awaiter3;
											_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
											return;
										}
										goto IL_0862;
									}
									goto IL_08d8;
								}
								goto IL_0994;
							}
							managedWebSocket._lastReceiveHeader = _003Cheader_003E5__3;
							receiveResult = managedWebSocket.GetReceiveResult<TResult>(0, (_003Cheader_003E5__3.Opcode != MessageOpcode.Text) ? WebSocketMessageType.Binary : WebSocketMessageType.Text, _003Cheader_003E5__3.EndOfMessage);
							goto end_IL_010e;
							IL_028f:
							awaiter2.GetResult();
							goto IL_0296;
							IL_0296:
							num4 = managedWebSocket._receiveBuffer.Span[managedWebSocket._receiveBufferOffset + 1] & 0x7F;
							if (managedWebSocket._isServer || num4 > 125)
							{
								int minimumRequiredBytes = 2 + (managedWebSocket._isServer ? 4 : 0) + ((num4 > 125) ? ((num4 == 126) ? 2 : 8) : 0);
								awaiter2 = managedWebSocket.EnsureBufferContainsAsync(minimumRequiredBytes, cancellationToken).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = (_003C_003E1__state = 3);
									_003C_003Eu__3 = awaiter2;
									_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
									return;
								}
								goto IL_035e;
							}
							goto IL_0365;
						}
						if (_003Cheader_003E5__3.Processed && System.Net.NetEventSource.Log.IsEnabled())
						{
							System.Net.NetEventSource.Trace(managedWebSocket, "Data frame fully processed", "ReceiveAsyncPrivate");
						}
						managedWebSocket._lastReceiveHeader = _003Cheader_003E5__3;
						receiveResult = managedWebSocket.GetReceiveResult<TResult>(_003CtotalBytesReceived_003E5__4, (_003Cheader_003E5__3.Opcode != MessageOpcode.Text) ? WebSocketMessageType.Binary : WebSocketMessageType.Text, _003Cheader_003E5__3.EndOfMessage);
						end_IL_010e:;
					}
					finally
					{
						if (num < 0)
						{
							managedWebSocket._receiveMutex.Exit();
							if (System.Net.NetEventSource.Log.IsEnabled())
							{
								System.Net.NetEventSource.MutexExited(managedWebSocket._receiveMutex, "ReceiveAsyncPrivate");
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						System.Net.NetEventSource.TraceException(managedWebSocket, ex, "ReceiveAsyncPrivate");
					}
					if (ex is OperationCanceledException)
					{
						throw;
					}
					if (managedWebSocket._state == WebSocketState.Aborted)
					{
						Exception innerException = ex;
						if (managedWebSocket._keepAlivePingState?.Exception != null)
						{
							innerException = ExceptionDispatchInfo.SetCurrentStackTrace(new AggregateException(managedWebSocket._keepAlivePingState.Exception, ex));
						}
						throw new OperationCanceledException("Aborted", innerException);
					}
					managedWebSocket.OnAborted();
					if (ex is WebSocketException)
					{
						throw;
					}
					throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely, ex);
				}
				finally
				{
					if (num < 0)
					{
						_003Cregistration_003E5__2.Dispose();
						if (System.Net.NetEventSource.Log.IsEnabled())
						{
							System.Net.NetEventSource.ReceiveAsyncPrivateCompleted(managedWebSocket, "ReceiveAsyncPrivate");
						}
					}
				}
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003Cregistration_003E5__2 = default(CancellationTokenRegistration);
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Cregistration_003E5__2 = default(CancellationTokenRegistration);
			_003C_003Et__builder.SetResult(receiveResult);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	private static readonly UTF8Encoding s_textEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

	private readonly Stream _stream;

	private readonly bool _isServer;

	private readonly string _subprotocol;

	private readonly Timer _keepAliveTimer;

	private readonly Memory<byte> _receiveBuffer;

	private readonly Utf8MessageState _utf8TextState = new Utf8MessageState();

	private readonly AsyncMutex _sendMutex = new AsyncMutex();

	private readonly AsyncMutex _receiveMutex = new AsyncMutex();

	private WebSocketState _state = WebSocketState.Open;

	private bool _disposed;

	private bool _sentCloseFrame;

	private bool _receivedCloseFrame;

	private WebSocketCloseStatus? _closeStatus;

	private string _closeStatusDescription;

	private MessageHeader _lastReceiveHeader = new MessageHeader
	{
		Opcode = MessageOpcode.Text,
		Fin = true,
		Processed = true
	};

	private int _receiveBufferOffset;

	private int _receiveBufferCount;

	private int _receivedMaskOffsetOffset;

	private byte[] _sendBuffer;

	private bool _lastSendWasFragment;

	private bool _lastSendHadDisableCompression;

	private readonly WebSocketInflater _inflater;

	private readonly WebSocketDeflater _deflater;

	private readonly KeepAlivePingState _keepAlivePingState;

	private object StateUpdateLock => _sendMutex;

	public override WebSocketCloseStatus? CloseStatus => _closeStatus;

	public override string CloseStatusDescription => _closeStatusDescription;

	public override WebSocketState State => _state;

	public override string SubProtocol => _subprotocol;

	private bool IsUnsolicitedPongKeepAlive => _keepAlivePingState == null;

	internal ManagedWebSocket(Stream stream, bool isServer, string subprotocol, TimeSpan keepAliveInterval, TimeSpan keepAliveTimeout)
	{
		_stream = stream;
		_isServer = isServer;
		_subprotocol = subprotocol;
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Associate(this, stream, ".ctor");
			System.Net.NetEventSource.Associate(this, _sendMutex, ".ctor");
			System.Net.NetEventSource.Associate(this, _receiveMutex, ".ctor");
		}
		_receiveBuffer = new byte[125];
		if (!(keepAliveInterval > TimeSpan.Zero))
		{
			return;
		}
		long num = (long)keepAliveInterval.TotalMilliseconds;
		if (keepAliveTimeout > TimeSpan.Zero)
		{
			_keepAlivePingState = new KeepAlivePingState(keepAliveInterval, keepAliveTimeout, this);
			num = _keepAlivePingState.HeartBeatIntervalMs;
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Associate(this, _keepAlivePingState, ".ctor");
				System.Net.NetEventSource.Trace(this, $"Enabling Ping/Pong Keep-Alive strategy: ping delay={_keepAlivePingState.DelayMs}ms, timeout={_keepAlivePingState.TimeoutMs}ms, heartbeat={num}ms", ".ctor");
			}
		}
		else if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, $"Enabling Unsolicited Pong Keep-Alive strategy: heartbeat={num}ms", ".ctor");
		}
		_keepAliveTimer = new Timer(delegate(object s)
		{
			if (((WeakReference<ManagedWebSocket>)s).TryGetTarget(out var target))
			{
				target.HeartBeat();
			}
		}, new WeakReference<ManagedWebSocket>(this), num, num);
	}

	internal ManagedWebSocket(Stream stream, WebSocketCreationOptions options)
		: this(stream, options.IsServer, options.SubProtocol, options.KeepAliveInterval, options.KeepAliveTimeout)
	{
		WebSocketDeflateOptions dangerousDeflateOptions = options.DangerousDeflateOptions;
		if (dangerousDeflateOptions != null)
		{
			if (options.IsServer)
			{
				_inflater = new WebSocketInflater(dangerousDeflateOptions.ClientMaxWindowBits, dangerousDeflateOptions.ClientContextTakeover);
				_deflater = new WebSocketDeflater(dangerousDeflateOptions.ServerMaxWindowBits, dangerousDeflateOptions.ServerContextTakeover);
			}
			else
			{
				_inflater = new WebSocketInflater(dangerousDeflateOptions.ServerMaxWindowBits, dangerousDeflateOptions.ServerContextTakeover);
				_deflater = new WebSocketDeflater(dangerousDeflateOptions.ClientMaxWindowBits, dangerousDeflateOptions.ClientContextTakeover);
			}
		}
	}

	public override void Dispose()
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "Dispose");
		}
		lock (StateUpdateLock)
		{
			DisposeCore();
		}
	}

	private void DisposeCore()
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, $"{"_disposed"}={_disposed}", "DisposeCore");
		}
		if (!_disposed)
		{
			_disposed = true;
			_keepAliveTimer?.Dispose();
			_stream.Dispose();
			WebSocketState state = _state;
			if (state < WebSocketState.Aborted)
			{
				_state = WebSocketState.Closed;
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Trace(this, $"State transition from {state} to {_state}", "DisposeCore");
			}
			DisposeSafe(_inflater, _receiveMutex);
			DisposeSafe(_deflater, _sendMutex);
		}
	}

	private static void DisposeSafe(IDisposable resource, AsyncMutex mutex)
	{
		if (resource == null)
		{
			return;
		}
		Task task = mutex.EnterAsync(CancellationToken.None);
		if (task.IsCompleted)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.MutexEntered(mutex, "DisposeSafe");
			}
			resource.Dispose();
			mutex.Exit();
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.MutexExited(mutex, "DisposeSafe");
			}
			return;
		}
		task.GetAwaiter().UnsafeOnCompleted(delegate
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.MutexEntered(mutex, "DisposeSafe");
			}
			resource.Dispose();
			mutex.Exit();
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.MutexExited(mutex, "DisposeSafe");
			}
		});
	}

	public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "SendAsync");
		}
		ThrowIfInvalidMessageType(messageType, "messageType");
		WebSocketValidate.ValidateArraySegment(buffer, "buffer");
		return SendAsync(buffer, messageType, endOfMessage ? WebSocketMessageFlags.EndOfMessage : WebSocketMessageFlags.None, cancellationToken).AsTask();
	}

	public override ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
	{
		return SendAsync(buffer, messageType, endOfMessage ? WebSocketMessageFlags.EndOfMessage : WebSocketMessageFlags.None, cancellationToken);
	}

	public override ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, WebSocketMessageFlags messageFlags, CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "SendAsync");
		}
		ThrowIfInvalidMessageType(messageType, "messageType");
		try
		{
			ThrowIfInvalidState(ManagedWebSocketStates.Open | ManagedWebSocketStates.CloseReceived);
		}
		catch (Exception exception)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.TraceException(this, exception, "SendAsync");
			}
			return ValueTask.FromException(exception);
		}
		bool flag = messageFlags.HasFlag(WebSocketMessageFlags.EndOfMessage);
		bool flag2 = messageFlags.HasFlag(WebSocketMessageFlags.DisableCompression);
		MessageOpcode opcode;
		if (_lastSendWasFragment)
		{
			if (_lastSendHadDisableCompression != flag2)
			{
				throw new ArgumentException(System.SR.net_WebSockets_Argument_MessageFlagsHasDifferentCompressionOptions, "messageFlags");
			}
			opcode = MessageOpcode.Continuation;
		}
		else
		{
			opcode = ((messageType != WebSocketMessageType.Binary) ? MessageOpcode.Text : MessageOpcode.Binary);
		}
		ValueTask result = SendFrameAsync(opcode, flag, flag2, buffer, cancellationToken);
		_lastSendWasFragment = !flag;
		_lastSendHadDisableCompression = flag2;
		return result;
	}

	public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "ReceiveAsync");
		}
		WebSocketValidate.ValidateArraySegment(buffer, "buffer");
		try
		{
			ThrowIfInvalidState(ManagedWebSocketStates.Open | ManagedWebSocketStates.CloseSent);
			return ReceiveAsyncPrivate<WebSocketReceiveResult>(buffer, cancellationToken).AsTask();
		}
		catch (Exception exception)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.TraceException(this, exception, "ReceiveAsync");
			}
			return Task.FromException<WebSocketReceiveResult>(exception);
		}
	}

	public override ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "ReceiveAsync");
		}
		try
		{
			ThrowIfInvalidState(ManagedWebSocketStates.Open | ManagedWebSocketStates.CloseSent);
			return ReceiveAsyncPrivate<ValueWebSocketReceiveResult>(buffer, cancellationToken);
		}
		catch (Exception exception)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.TraceException(this, exception, "ReceiveAsync");
			}
			return ValueTask.FromException<ValueWebSocketReceiveResult>(exception);
		}
	}

	public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "CloseAsync");
		}
		WebSocketValidate.ValidateCloseStatus(closeStatus, statusDescription);
		try
		{
			ThrowIfInvalidState(ManagedWebSocketStates.Open | ManagedWebSocketStates.CloseSent | ManagedWebSocketStates.CloseReceived);
		}
		catch (Exception exception)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.TraceException(this, exception, "CloseAsync");
			}
			return Task.FromException(exception);
		}
		return CloseAsyncPrivate(closeStatus, statusDescription, cancellationToken);
	}

	public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "CloseOutputAsync");
		}
		WebSocketValidate.ValidateCloseStatus(closeStatus, statusDescription);
		return CloseOutputAsyncCore(closeStatus, statusDescription, cancellationToken);
	}

	private async Task CloseOutputAsyncCore(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "CloseOutputAsyncCore");
		}
		ThrowIfInvalidState(ManagedWebSocketStates.Open | ManagedWebSocketStates.CloseReceived);
		await SendCloseFrameAsync(closeStatus, statusDescription, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		lock (StateUpdateLock)
		{
			if (_receivedCloseFrame)
			{
				DisposeCore();
			}
		}
	}

	public override void Abort()
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "Abort");
		}
		OnAborted();
		Dispose();
	}

	private void OnAborted()
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "OnAborted");
		}
		lock (StateUpdateLock)
		{
			OnAbortedCore();
		}
	}

	private void OnAbortedCore()
	{
		WebSocketState state = _state;
		if (state != WebSocketState.Closed && state != WebSocketState.Aborted)
		{
			_state = ((state != WebSocketState.None && state != WebSocketState.Connecting) ? WebSocketState.Aborted : WebSocketState.Closed);
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, $"State transition from {state} to {_state}", "OnAbortedCore");
		}
	}

	private ValueTask SendFrameAsync(MessageOpcode opcode, bool endOfMessage, bool disableCompression, ReadOnlyMemory<byte> payloadBuffer, CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.SendFrameAsyncStarted(this, opcode.ToString(), payloadBuffer.Length, "SendFrameAsync");
		}
		Task task = _sendMutex.EnterAsync(cancellationToken);
		if (!cancellationToken.CanBeCanceled && task.IsCompletedSuccessfully)
		{
			return SendFrameLockAcquiredNonCancelableAsync(opcode, endOfMessage, disableCompression, payloadBuffer);
		}
		return SendFrameFallbackAsync(opcode, endOfMessage, disableCompression, payloadBuffer, task, cancellationToken);
	}

	private ValueTask SendFrameLockAcquiredNonCancelableAsync(MessageOpcode opcode, bool endOfMessage, bool disableCompression, ReadOnlyMemory<byte> payloadBuffer)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.MutexEntered(_sendMutex, "SendFrameLockAcquiredNonCancelableAsync");
		}
		ValueTask writeTask = default(ValueTask);
		bool flag = true;
		try
		{
			int length = WriteFrameToSendBuffer(opcode, endOfMessage, disableCompression, payloadBuffer.Span);
			writeTask = _stream.WriteAsync(new ReadOnlyMemory<byte>(_sendBuffer, 0, length));
			if (writeTask.IsCompleted)
			{
				writeTask.GetAwaiter().GetResult();
				ValueTask valueTask = new ValueTask(_stream.FlushAsync());
				if (valueTask.IsCompleted)
				{
					return valueTask;
				}
				flag = false;
				return WaitForWriteTaskAsync(valueTask, shouldFlush: false);
			}
			flag = false;
		}
		catch (Exception ex)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.TraceException(this, ex, "SendFrameLockAcquiredNonCancelableAsync");
			}
			return ValueTask.FromException((ex is OperationCanceledException) ? ex : ((_state == WebSocketState.Aborted) ? CreateOperationCanceledException(ex) : ExceptionDispatchInfo.SetCurrentStackTrace(new WebSocketException(WebSocketError.ConnectionClosedPrematurely, ex))));
		}
		finally
		{
			if (flag)
			{
				ReleaseSendBuffer();
				_sendMutex.Exit();
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.MutexExited(_sendMutex, "SendFrameLockAcquiredNonCancelableAsync");
					System.Net.NetEventSource.SendFrameAsyncCompleted(this, "SendFrameLockAcquiredNonCancelableAsync");
				}
			}
		}
		return WaitForWriteTaskAsync(writeTask, shouldFlush: true);
	}

	private async ValueTask WaitForWriteTaskAsync(ValueTask writeTask, bool shouldFlush)
	{
		_ = 1;
		try
		{
			await writeTask.ConfigureAwait(continueOnCapturedContext: false);
			if (shouldFlush)
			{
				await _stream.FlushAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch (Exception ex)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.TraceException(this, ex, "WaitForWriteTaskAsync");
			}
			if (ex is OperationCanceledException)
			{
				throw;
			}
			throw (_state == WebSocketState.Aborted) ? ((SystemException)CreateOperationCanceledException(ex)) : ((SystemException)new WebSocketException(WebSocketError.ConnectionClosedPrematurely, ex));
		}
		finally
		{
			ReleaseSendBuffer();
			_sendMutex.Exit();
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.MutexExited(_sendMutex, "WaitForWriteTaskAsync");
				System.Net.NetEventSource.SendFrameAsyncCompleted(this, "WaitForWriteTaskAsync");
			}
		}
	}

	private async ValueTask SendFrameFallbackAsync(MessageOpcode opcode, bool endOfMessage, bool disableCompression, ReadOnlyMemory<byte> payloadBuffer, Task lockTask, CancellationToken cancellationToken)
	{
		await lockTask.ConfigureAwait(continueOnCapturedContext: false);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.MutexEntered(_sendMutex, "SendFrameFallbackAsync");
		}
		try
		{
			int length = WriteFrameToSendBuffer(opcode, endOfMessage, disableCompression, payloadBuffer.Span);
			using (cancellationToken.Register(delegate(object s)
			{
				((ManagedWebSocket)s).Abort();
			}, this))
			{
				await _stream.WriteAsync(new ReadOnlyMemory<byte>(_sendBuffer, 0, length), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				await _stream.FlushAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch (Exception ex)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.TraceException(this, ex, "SendFrameFallbackAsync");
			}
			if (ex is OperationCanceledException)
			{
				throw;
			}
			throw (_state == WebSocketState.Aborted) ? ((SystemException)CreateOperationCanceledException(ex, cancellationToken)) : ((SystemException)new WebSocketException(WebSocketError.ConnectionClosedPrematurely, ex));
		}
		finally
		{
			ReleaseSendBuffer();
			_sendMutex.Exit();
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.MutexExited(_sendMutex, "SendFrameFallbackAsync");
				System.Net.NetEventSource.SendFrameAsyncCompleted(this, "SendFrameFallbackAsync");
			}
		}
	}

	private int WriteFrameToSendBuffer(MessageOpcode opcode, bool endOfMessage, bool disableCompression, ReadOnlySpan<byte> payloadBuffer)
	{
		ThrowIfDisposed();
		if (_deflater != null && !disableCompression)
		{
			payloadBuffer = _deflater.Deflate(payloadBuffer, endOfMessage);
		}
		int length = payloadBuffer.Length;
		AllocateSendBuffer(length + 14);
		int? num = null;
		int num2;
		if (_isServer)
		{
			num2 = WriteHeader(opcode, _sendBuffer, payloadBuffer, endOfMessage, useMask: false, _deflater != null && !disableCompression);
		}
		else
		{
			num = WriteHeader(opcode, _sendBuffer, payloadBuffer, endOfMessage, useMask: true, _deflater != null && !disableCompression);
			num2 = num.GetValueOrDefault() + 4;
		}
		if (payloadBuffer.Length > 0)
		{
			payloadBuffer.CopyTo(new Span<byte>(_sendBuffer, num2, length));
			_deflater?.ReleaseBuffer();
			if (num.HasValue)
			{
				ApplyMask(new Span<byte>(_sendBuffer, num2, length), _sendBuffer, num.Value, 0);
			}
		}
		return num2 + length;
	}

	private static int WriteHeader(MessageOpcode opcode, byte[] sendBuffer, ReadOnlySpan<byte> payload, bool endOfMessage, bool useMask, bool compressed)
	{
		sendBuffer[0] = (byte)opcode;
		if (endOfMessage)
		{
			sendBuffer[0] |= 128;
		}
		if (compressed && opcode != MessageOpcode.Continuation)
		{
			sendBuffer[0] |= 64;
		}
		int num;
		if (payload.Length <= 125)
		{
			sendBuffer[1] = (byte)payload.Length;
			num = 2;
		}
		else if (payload.Length <= 65535)
		{
			sendBuffer[1] = 126;
			BinaryPrimitives.WriteUInt16BigEndian(sendBuffer.AsSpan(2), (ushort)payload.Length);
			num = 4;
		}
		else
		{
			sendBuffer[1] = 127;
			BinaryPrimitives.WriteUInt64BigEndian(sendBuffer.AsSpan(2), (ulong)payload.Length);
			num = 10;
		}
		if (useMask)
		{
			sendBuffer[1] |= 128;
			WriteRandomMask(sendBuffer, num);
		}
		return num;
	}

	private static void WriteRandomMask(byte[] buffer, int offset)
	{
		RandomNumberGenerator.Fill(buffer.AsSpan(offset, 4));
	}

	[AsyncStateMachine(typeof(_003CReceiveAsyncPrivate_003Ed__61<>))]
	[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
	private ValueTask<TResult> ReceiveAsyncPrivate<TResult>(Memory<byte> payloadBuffer, CancellationToken cancellationToken)
	{
		Unsafe.SkipInit(out _003CReceiveAsyncPrivate_003Ed__61<TResult> stateMachine);
		stateMachine._003C_003Et__builder = PoolingAsyncValueTaskMethodBuilder<TResult>.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.payloadBuffer = payloadBuffer;
		stateMachine.cancellationToken = cancellationToken;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private TResult GetReceiveResult<TResult>(int count, WebSocketMessageType messageType, bool endOfMessage)
	{
		if (typeof(TResult) == typeof(ValueWebSocketReceiveResult))
		{
			return (TResult)(object)new ValueWebSocketReceiveResult(count, messageType, endOfMessage);
		}
		return (TResult)(object)new WebSocketReceiveResult(count, messageType, endOfMessage, _closeStatus, _closeStatusDescription);
	}

	private async ValueTask HandleReceivedCloseAsync(MessageHeader header, CancellationToken cancellationToken)
	{
		lock (StateUpdateLock)
		{
			_receivedCloseFrame = true;
			WebSocketState state = _state;
			if (_sentCloseFrame && state < WebSocketState.Closed)
			{
				_state = WebSocketState.Closed;
			}
			else if (state < WebSocketState.CloseReceived)
			{
				_state = WebSocketState.CloseReceived;
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Trace(this, $"State transition from {state} to {_state}", "HandleReceivedCloseAsync");
			}
		}
		WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure;
		string closeStatusDescription = string.Empty;
		if (header.PayloadLength == 1)
		{
			await CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted).ConfigureAwait(continueOnCapturedContext: false);
		}
		else if (header.PayloadLength >= 2)
		{
			if (_receiveBufferCount < header.PayloadLength)
			{
				await EnsureBufferContainsAsync((int)header.PayloadLength, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (_isServer)
			{
				ApplyMask(_receiveBuffer.Span.Slice(_receiveBufferOffset, (int)header.PayloadLength), header.Mask, 0);
			}
			closeStatus = (WebSocketCloseStatus)BinaryPrimitives.ReadUInt16BigEndian(_receiveBuffer.Span.Slice(_receiveBufferOffset));
			if (!IsValidCloseStatus(closeStatus))
			{
				await CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (header.PayloadLength > 2)
			{
				try
				{
					closeStatusDescription = s_textEncoding.GetString(_receiveBuffer.Span.Slice(_receiveBufferOffset + 2, (int)header.PayloadLength - 2));
				}
				catch (DecoderFallbackException innerException)
				{
					await CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted, null, innerException).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			ConsumeFromBuffer((int)header.PayloadLength);
		}
		_closeStatus = closeStatus;
		_closeStatusDescription = closeStatusDescription;
		if (!_isServer && _sentCloseFrame)
		{
			await WaitForServerToCloseConnectionAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private async ValueTask WaitForServerToCloseConnectionAsync(CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "WaitForServerToCloseConnectionAsync");
		}
		ValueTask<int> valueTask = _stream.ReadAsync(_receiveBuffer, cancellationToken);
		if (valueTask.IsCompletedSuccessfully)
		{
			valueTask.GetAwaiter().GetResult();
			return;
		}
		Task task = valueTask.AsTask();
		try
		{
			await task.WaitAsync(TimeSpan.FromMilliseconds(1000L)).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch
		{
			LogExceptions(task);
			Abort();
		}
	}

	private async ValueTask HandleReceivedPingPongAsync(MessageHeader header, CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "HandleReceivedPingPongAsync");
		}
		if (header.PayloadLength > 0 && _receiveBufferCount < header.PayloadLength)
		{
			await EnsureBufferContainsAsync((int)header.PayloadLength, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		bool num = header.Opcode == MessageOpcode.Ping;
		bool flag = header.Opcode == MessageOpcode.Pong && _keepAlivePingState != null && header.PayloadLength == 8;
		if ((num | flag) && _isServer)
		{
			ApplyMask(_receiveBuffer.Span.Slice(_receiveBufferOffset, (int)header.PayloadLength), header.Mask, 0);
		}
		if (num)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Trace(this, "Processing incoming Ping", "HandleReceivedPingPongAsync");
			}
			await SendFrameAsync(MessageOpcode.Pong, endOfMessage: true, disableCompression: true, _receiveBuffer.Slice(_receiveBufferOffset, (int)header.PayloadLength), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		else if (flag)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Trace(this, "Processing incoming Pong", "HandleReceivedPingPongAsync");
			}
			long pongPayload = BinaryPrimitives.ReadInt64BigEndian(_receiveBuffer.Span.Slice(_receiveBufferOffset, (int)header.PayloadLength));
			lock (StateUpdateLock)
			{
				_keepAlivePingState.OnPongResponseReceived(pongPayload);
			}
		}
		else if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, "Received Unsolicited Pong. Skipping.", "HandleReceivedPingPongAsync");
		}
		if (header.PayloadLength > 0)
		{
			ConsumeFromBuffer((int)header.PayloadLength);
		}
	}

	private static bool IsValidCloseStatus(WebSocketCloseStatus closeStatus)
	{
		if (closeStatus < WebSocketCloseStatus.NormalClosure || closeStatus >= (WebSocketCloseStatus)5000)
		{
			return false;
		}
		if (closeStatus >= (WebSocketCloseStatus)3000)
		{
			return true;
		}
		if ((uint)(closeStatus - 1000) <= 3u || (uint)(closeStatus - 1007) <= 7u)
		{
			return true;
		}
		return false;
	}

	private async ValueTask CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus closeStatus, WebSocketError error, string errorMessage = null, Exception innerException = null)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, errorMessage, "CloseWithReceiveErrorAndThrowAsync");
		}
		if (!_sentCloseFrame)
		{
			await CloseOutputAsync(closeStatus, string.Empty, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
		}
		_receiveBufferCount = 0;
		throw (errorMessage != null) ? new WebSocketException(error, errorMessage, innerException) : new WebSocketException(error, innerException);
	}

	private string TryParseMessageHeaderFromReceiveBuffer(out MessageHeader resultHeader)
	{
		MessageHeader messageHeader = default(MessageHeader);
		Span<byte> span = _receiveBuffer.Span;
		messageHeader.Fin = (span[_receiveBufferOffset] & 0x80) != 0;
		bool num = (span[_receiveBufferOffset] & 0x30) != 0;
		messageHeader.Opcode = (MessageOpcode)(span[_receiveBufferOffset] & 0xF);
		messageHeader.Compressed = (span[_receiveBufferOffset] & 0x40) != 0;
		bool flag = (span[_receiveBufferOffset + 1] & 0x80) != 0;
		messageHeader.PayloadLength = span[_receiveBufferOffset + 1] & 0x7F;
		ConsumeFromBuffer(2);
		if (messageHeader.PayloadLength == 126)
		{
			messageHeader.PayloadLength = BinaryPrimitives.ReadUInt16BigEndian(span.Slice(_receiveBufferOffset));
			ConsumeFromBuffer(2);
		}
		else if (messageHeader.PayloadLength == 127)
		{
			messageHeader.PayloadLength = BinaryPrimitives.ReadInt64BigEndian(span.Slice(_receiveBufferOffset));
			ConsumeFromBuffer(8);
		}
		if (num)
		{
			resultHeader = default(MessageHeader);
			return System.SR.net_Websockets_ReservedBitsSet;
		}
		if (messageHeader.PayloadLength < 0)
		{
			resultHeader = default(MessageHeader);
			return System.SR.net_Websockets_InvalidPayloadLength;
		}
		if (messageHeader.Compressed && _inflater == null)
		{
			resultHeader = default(MessageHeader);
			return System.SR.net_Websockets_PerMessageCompressedFlagWhenNotEnabled;
		}
		if (flag)
		{
			if (!_isServer)
			{
				resultHeader = default(MessageHeader);
				return System.SR.net_Websockets_ClientReceivedMaskedFrame;
			}
			messageHeader.Mask = CombineMaskBytes(span, _receiveBufferOffset);
			ConsumeFromBuffer(4);
		}
		else if (_isServer)
		{
			resultHeader = default(MessageHeader);
			return System.SR.net_Websockets_ServerReceivedUnmaskedFrame;
		}
		switch (messageHeader.Opcode)
		{
		case MessageOpcode.Continuation:
			if (_lastReceiveHeader.Fin)
			{
				resultHeader = default(MessageHeader);
				return System.SR.net_Websockets_ContinuationFromFinalFrame;
			}
			if (messageHeader.Compressed)
			{
				resultHeader = default(MessageHeader);
				return System.SR.net_Websockets_PerMessageCompressedFlagInContinuation;
			}
			messageHeader.Compressed = _lastReceiveHeader.Compressed;
			break;
		case MessageOpcode.Text:
		case MessageOpcode.Binary:
			if (!_lastReceiveHeader.Fin)
			{
				resultHeader = default(MessageHeader);
				return System.SR.net_Websockets_NonContinuationAfterNonFinalFrame;
			}
			break;
		case MessageOpcode.Close:
		case MessageOpcode.Ping:
		case MessageOpcode.Pong:
			if (messageHeader.PayloadLength > 125 || !messageHeader.Fin)
			{
				resultHeader = default(MessageHeader);
				return System.SR.net_Websockets_InvalidControlMessage;
			}
			break;
		default:
			resultHeader = default(MessageHeader);
			return System.SR.Format(System.SR.net_Websockets_UnknownOpcode, messageHeader.Opcode);
		}
		messageHeader.Processed = messageHeader.PayloadLength == 0L && !messageHeader.Compressed;
		resultHeader = messageHeader;
		return null;
	}

	private async Task CloseAsyncPrivate(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.CloseAsyncPrivateStarted(this, "CloseAsyncPrivate");
		}
		try
		{
			if (!_sentCloseFrame)
			{
				await SendCloseFrameAsync(closeStatus, statusDescription, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (State == WebSocketState.CloseSent)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Trace(this, "Waiting for a close frame", "CloseAsyncPrivate");
				}
				byte[] closeBuffer = ArrayPool<byte>.Shared.Rent(139);
				try
				{
					while (!_receivedCloseFrame)
					{
						ValueTask<ValueWebSocketReceiveResult> receiveTask = default(ValueTask<ValueWebSocketReceiveResult>);
						try
						{
							await _receiveMutex.EnterAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
							if (System.Net.NetEventSource.Log.IsEnabled())
							{
								System.Net.NetEventSource.MutexEntered(_receiveMutex, "CloseAsyncPrivate");
							}
							try
							{
								if (!_receivedCloseFrame)
								{
									receiveTask = ReceiveAsyncPrivate<ValueWebSocketReceiveResult>(closeBuffer, cancellationToken);
								}
							}
							finally
							{
								_receiveMutex.Exit();
								if (System.Net.NetEventSource.Log.IsEnabled())
								{
									System.Net.NetEventSource.MutexExited(_receiveMutex, "CloseAsyncPrivate");
								}
							}
						}
						catch (OperationCanceledException)
						{
							Abort();
							throw;
						}
						await receiveTask.ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				finally
				{
					ArrayPool<byte>.Shared.Return(closeBuffer);
				}
			}
			lock (StateUpdateLock)
			{
				DisposeCore();
			}
		}
		catch (Exception exception)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.TraceException(this, exception, "CloseAsyncPrivate");
			}
			throw;
		}
		finally
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.CloseAsyncPrivateCompleted(this, "CloseAsyncPrivate");
			}
		}
	}

	private async ValueTask SendCloseFrameAsync(WebSocketCloseStatus closeStatus, string closeStatusDescription, CancellationToken cancellationToken)
	{
		byte[] buffer = null;
		try
		{
			int num = 2;
			if (string.IsNullOrEmpty(closeStatusDescription))
			{
				buffer = ArrayPool<byte>.Shared.Rent(num);
			}
			else
			{
				num += s_textEncoding.GetByteCount(closeStatusDescription);
				buffer = ArrayPool<byte>.Shared.Rent(num);
				s_textEncoding.GetBytes(closeStatusDescription, 0, closeStatusDescription.Length, buffer, 2);
			}
			BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)closeStatus);
			await SendFrameAsync(MessageOpcode.Close, endOfMessage: true, disableCompression: true, new Memory<byte>(buffer, 0, num), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			if (buffer != null)
			{
				ArrayPool<byte>.Shared.Return(buffer);
			}
		}
		lock (StateUpdateLock)
		{
			_sentCloseFrame = true;
			WebSocketState state = _state;
			if (_receivedCloseFrame && state < WebSocketState.Closed)
			{
				_state = WebSocketState.Closed;
			}
			else if (state < WebSocketState.CloseSent)
			{
				_state = WebSocketState.CloseSent;
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Trace(this, $"State transition from {state} to {_state}", "SendCloseFrameAsync");
			}
		}
		if (!_isServer && _receivedCloseFrame)
		{
			await WaitForServerToCloseConnectionAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private void ConsumeFromBuffer(int count)
	{
		_receiveBufferCount -= count;
		_receiveBufferOffset += count;
	}

	[AsyncStateMachine(typeof(_003CEnsureBufferContainsAsync_003Ed__72))]
	[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
	private ValueTask EnsureBufferContainsAsync(int minimumRequiredBytes, CancellationToken cancellationToken)
	{
		Unsafe.SkipInit(out _003CEnsureBufferContainsAsync_003Ed__72 stateMachine);
		stateMachine._003C_003Et__builder = PoolingAsyncValueTaskMethodBuilder.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.minimumRequiredBytes = minimumRequiredBytes;
		stateMachine.cancellationToken = cancellationToken;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	private void ThrowEOFUnexpected()
	{
		ThrowIfDisposed();
		throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);
	}

	private void AllocateSendBuffer(int minLength)
	{
		_sendBuffer = ArrayPool<byte>.Shared.Rent(minLength);
	}

	private void ReleaseSendBuffer()
	{
		byte[] sendBuffer = _sendBuffer;
		if (sendBuffer != null)
		{
			_sendBuffer = null;
			ArrayPool<byte>.Shared.Return(sendBuffer);
		}
	}

	private static int CombineMaskBytes(ReadOnlySpan<byte> buffer, int maskOffset)
	{
		return BitConverter.ToInt32(buffer.Slice(maskOffset));
	}

	private static int ApplyMask(Span<byte> toMask, byte[] mask, int maskOffset, int maskOffsetIndex)
	{
		return ApplyMask(toMask, CombineMaskBytes(mask, maskOffset), maskOffsetIndex);
	}

	private unsafe static int ApplyMask(Span<byte> toMask, int mask, int maskIndex)
	{
		fixed (byte* reference = &MemoryMarshal.GetReference(toMask))
		{
			byte* ptr2;
			byte* ptr = (ptr2 = reference) + toMask.Length;
			if (ptr - ptr2 >= 4)
			{
				int num = (int)(BitConverter.IsLittleEndian ? BitOperations.RotateRight((uint)mask, maskIndex * 8) : BitOperations.RotateLeft((uint)mask, maskIndex * 8));
				if (Vector.IsHardwareAccelerated && ptr - ptr2 >= Vector<byte>.Count)
				{
					Vector<byte> vector = Vector.AsVectorByte(new Vector<int>(num));
					do
					{
						*(Vector<byte>*)ptr2 ^= vector;
						ptr2 += Vector<byte>.Count;
					}
					while (ptr - ptr2 >= Vector<byte>.Count);
				}
				for (; ptr - ptr2 >= 4; ptr2 += 4)
				{
					*(int*)ptr2 ^= num;
				}
			}
			byte* ptr3 = (byte*)(&mask);
			while (ptr2 != ptr)
			{
				byte* intPtr = ptr2++;
				*intPtr ^= ptr3[maskIndex];
				maskIndex = (maskIndex + 1) & 3;
			}
		}
		return maskIndex;
	}

	private static OperationCanceledException CreateOperationCanceledException(Exception innerException, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (OperationCanceledException)ExceptionDispatchInfo.SetCurrentStackTrace(new OperationCanceledException(new OperationCanceledException().Message, innerException, cancellationToken));
	}

	private void ThrowIfDisposed()
	{
		ThrowIfInvalidState(ManagedWebSocketStates.All);
	}

	private void ThrowIfInvalidState(ManagedWebSocketStates validStates)
	{
		bool disposed = _disposed;
		WebSocketState state = _state;
		Exception ex = null;
		if (_keepAlivePingState != null)
		{
			lock (StateUpdateLock)
			{
				disposed = _disposed;
				state = _state;
				ex = _keepAlivePingState.Exception;
			}
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, $"_state={state}, _disposed={disposed}, _keepAlivePingState.Exception={ex}", "ThrowIfInvalidState");
		}
		WebSocketStateHelper.ThrowIfInvalidState(state, disposed, ex, validStates);
	}

	private static bool TryValidateUtf8(ReadOnlySpan<byte> span, bool endOfMessage, Utf8MessageState state)
	{
		if (endOfMessage && !state.SequenceInProgress)
		{
			return Utf8.IsValid(span);
		}
		int num = 0;
		while (num < span.Length)
		{
			if (!state.SequenceInProgress)
			{
				int num2 = span.Slice(num).IndexOfAnyExceptInRange((byte)0, (byte)127);
				if (num2 < 0)
				{
					break;
				}
				num += num2;
				state.SequenceInProgress = true;
				byte b = span[num];
				num++;
				if ((b & 0xC0) == 128)
				{
					return false;
				}
				if ((b & 0xE0) == 192)
				{
					state.AdditionalBytesExpected = 1;
					state.CurrentDecodeBits = b & 0x1F;
					state.ExpectedValueMin = 128;
				}
				else if ((b & 0xF0) == 224)
				{
					state.AdditionalBytesExpected = 2;
					state.CurrentDecodeBits = b & 0xF;
					state.ExpectedValueMin = 2048;
				}
				else
				{
					if ((b & 0xF8) != 240)
					{
						return false;
					}
					state.AdditionalBytesExpected = 3;
					state.CurrentDecodeBits = b & 7;
					state.ExpectedValueMin = 65536;
				}
			}
			while (state.AdditionalBytesExpected > 0 && num < span.Length)
			{
				byte b2 = span[num];
				if ((b2 & 0xC0) != 128)
				{
					return false;
				}
				num++;
				state.AdditionalBytesExpected--;
				state.CurrentDecodeBits = (state.CurrentDecodeBits << 6) | (b2 & 0x3F);
				if (state.AdditionalBytesExpected == 1 && state.CurrentDecodeBits >= 864 && state.CurrentDecodeBits <= 895)
				{
					return false;
				}
				if (state.AdditionalBytesExpected == 2 && state.CurrentDecodeBits >= 272)
				{
					return false;
				}
			}
			if (state.AdditionalBytesExpected == 0)
			{
				state.SequenceInProgress = false;
				if (state.CurrentDecodeBits < state.ExpectedValueMin)
				{
					return false;
				}
			}
		}
		if (endOfMessage)
		{
			return !state.SequenceInProgress;
		}
		return true;
	}

	private void LogExceptions(ValueTask t)
	{
		if (t.IsCompletedSuccessfully)
		{
			t.GetAwaiter().GetResult();
		}
		else
		{
			LogExceptions(t.AsTask());
		}
	}

	private void LogExceptions(Task t)
	{
		if (t.IsCompleted)
		{
			if (t.IsFaulted)
			{
				LogFaulted(t, this);
			}
		}
		else
		{
			t.ContinueWith(LogFaulted, this, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}
		static void LogFaulted(Task task, object thisObj)
		{
			Exception innerException = task.Exception.InnerException;
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.TraceException(thisObj, innerException, "LogExceptions");
			}
		}
	}

	internal static void ThrowIfInvalidMessageType(WebSocketMessageType messageType, [CallerArgumentExpression("messageType")] string paramName = null)
	{
		if ((uint)messageType > 1u)
		{
			ThrowInvalidMessageType(paramName);
		}
		static void ThrowInvalidMessageType(string paramName2)
		{
			throw new ArgumentException(System.SR.Format(System.SR.net_WebSockets_Argument_InvalidMessageType, "Close", "SendAsync", "Binary", "Text", "CloseOutputAsync"), paramName2);
		}
	}

	private void HeartBeat()
	{
		if (IsUnsolicitedPongKeepAlive)
		{
			UnsolicitedPongHeartBeat();
		}
		else
		{
			KeepAlivePingHeartBeat();
		}
	}

	private void UnsolicitedPongHeartBeat()
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "UnsolicitedPongHeartBeat");
		}
		LogExceptions(TrySendKeepAliveFrameAsync(MessageOpcode.Pong));
	}

	private ValueTask TrySendKeepAliveFrameAsync(MessageOpcode opcode, ReadOnlyMemory<byte> payload = default(ReadOnlyMemory<byte>))
	{
		if (!WebSocketStateHelper.IsValidSendState(_state))
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Trace(this, $"Cannot send keep-alive frame in {"_state"}={_state}", "TrySendKeepAliveFrameAsync");
			}
			return ValueTask.CompletedTask;
		}
		return SendFrameAsync(opcode, endOfMessage: true, disableCompression: true, payload, CancellationToken.None);
	}

	private void KeepAlivePingHeartBeat()
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, null, "KeepAlivePingHeartBeat");
		}
		bool flag = false;
		long pingPayload = -1L;
		try
		{
			lock (StateUpdateLock)
			{
				if (_keepAlivePingState.Exception != null)
				{
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						System.Net.NetEventSource.Trace(this, "KeepAlive already faulted, skipping... (exception: " + _keepAlivePingState.Exception.Message + ")", "KeepAlivePingHeartBeat");
					}
					return;
				}
				long tickCount = Environment.TickCount64;
				if (_keepAlivePingState.PingSent)
				{
					if (tickCount > _keepAlivePingState.PingTimeoutTimestamp)
					{
						if (System.Net.NetEventSource.Log.IsEnabled())
						{
							System.Net.NetEventSource.Trace(this, $"Keep-alive ping timed out after {_keepAlivePingState.TimeoutMs}ms. Expected pong with payload {_keepAlivePingState.PingPayload}", "KeepAlivePingHeartBeat");
						}
						Exception exc = ExceptionDispatchInfo.SetCurrentStackTrace(new WebSocketException(WebSocketError.Faulted, System.SR.net_Websockets_KeepAlivePingTimeout));
						_keepAlivePingState.OnKeepAliveFaultedCore(exc);
						return;
					}
				}
				else if (tickCount > _keepAlivePingState.NextPingRequestTimestamp)
				{
					_keepAlivePingState.OnNextPingRequestCore();
					flag = true;
					pingPayload = _keepAlivePingState.PingPayload;
				}
			}
			if (flag)
			{
				LogExceptions(SendPingAsync(pingPayload));
			}
		}
		catch (Exception ex)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.TraceException(this, ex, "KeepAlivePingHeartBeat");
			}
			_keepAlivePingState.OnKeepAliveFaulted(ex);
		}
	}

	private async ValueTask SendPingAsync(long pingPayload)
	{
		byte[] array = new byte[8];
		BinaryPrimitives.WriteInt64BigEndian(array, pingPayload);
		await TrySendKeepAliveFrameAsync(MessageOpcode.Ping, array).ConfigureAwait(continueOnCapturedContext: false);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.KeepAlivePingSent(this, pingPayload);
		}
	}
}
