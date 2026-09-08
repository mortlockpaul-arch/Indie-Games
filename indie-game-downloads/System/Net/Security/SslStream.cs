using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Security;

public class SslStream : AuthenticatedStream
{
	private struct SslBuffer
	{
		private ArrayBuffer _buffer = new ArrayBuffer(0, usePool: true);

		private int _decryptedLength = 0;

		private int _decryptedPadding = 0;

		private bool _isValid = false;

		public bool IsValid => _isValid;

		public Span<byte> DecryptedSpan => _buffer.ActiveSpan.Slice(0, _decryptedLength);

		public int DecryptedLength => _decryptedLength;

		public int ActiveLength => _buffer.ActiveLength;

		public ReadOnlySpan<byte> EncryptedReadOnlySpan => _buffer.ActiveSpan.Slice(_decryptedLength + _decryptedPadding);

		public int EncryptedLength => _buffer.ActiveLength - _decryptedPadding - _decryptedLength;

		public Memory<byte> AvailableMemory => _buffer.AvailableMemory;

		public SslBuffer()
		{
		}

		public ReadOnlySpan<byte> DecryptedReadOnlySpanSliced(int length)
		{
			return _buffer.ActiveSpan.Slice(0, length);
		}

		public Span<byte> EncryptedSpanSliced(int length)
		{
			return _buffer.ActiveSpan.Slice(_decryptedLength + _decryptedPadding, length);
		}

		public void Commit(int byteCount)
		{
			_buffer.Commit(byteCount);
		}

		public void EnsureAvailableSpace(int byteCount)
		{
			_isValid = true;
			_buffer.EnsureAvailableSpace(byteCount);
		}

		public void Discard(int byteCount)
		{
			_buffer.Discard(byteCount);
			_decryptedLength -= byteCount;
			if (_decryptedLength == 0)
			{
				_buffer.Discard(_decryptedPadding);
				_decryptedPadding = 0;
			}
		}

		public void DiscardEncrypted(int byteCount)
		{
			_buffer.Discard(byteCount);
		}

		public void OnDecrypted(int decryptedOffset, int decryptedCount, int frameSize)
		{
			if (decryptedCount > 0)
			{
				_buffer.Discard(decryptedOffset);
				_decryptedPadding = frameSize - decryptedOffset - decryptedCount;
				_decryptedLength = decryptedCount;
			}
			else
			{
				_buffer.Discard(frameSize);
			}
		}

		public void ReturnBuffer()
		{
			_buffer.ClearAndReturnBuffer();
			_decryptedLength = 0;
			_decryptedPadding = 0;
			_isValid = false;
		}
	}

	private enum NestedState
	{
		StreamNotInUse,
		StreamInUse,
		StreamDisposed
	}

	internal sealed class PoolingPointerMemoryManager : MemoryManager<byte>
	{
		private unsafe byte* _pointer;

		private int _length;

		protected override void Dispose(bool disposing)
		{
		}

		public unsafe void Reset(byte* pointer, int length)
		{
			_pointer = pointer;
			_length = length;
		}

		public unsafe override Span<byte> GetSpan()
		{
			return new Span<byte>(_pointer, _length);
		}

		public unsafe override MemoryHandle Pin(int elementIndex = 0)
		{
			return new MemoryHandle(_pointer + elementIndex);
		}

		public override void Unpin()
		{
		}
	}

	private enum ConnectionStatus
	{
		NoHandshake,
		HandshakeCompleted,
		Disposed
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CEnsureFullTlsFrameAsync_003Ed__170<TIOAdapter> : IAsyncStateMachine where TIOAdapter : IReadWriteAdapter
	{
		public int _003C_003E1__state;

		public PoolingAsyncValueTaskMethodBuilder<int> _003C_003Et__builder;

		public SslStream _003C_003E4__this;

		public CancellationToken cancellationToken;

		public int estimatedSize;

		private int _003CframeSize_003E5__2;

		private ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			SslStream sslStream = _003C_003E4__this;
			int result;
			try
			{
				ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_00aa;
				}
				if (num == 1)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_0167;
				}
				if (!sslStream.HaveFullTlsFrame(out _003CframeSize_003E5__2))
				{
					awaiter = TIOAdapter.ReadAsync(sslStream.InnerStream, Memory<byte>.Empty, cancellationToken).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00aa;
				}
				result = _003CframeSize_003E5__2;
				goto end_IL_000e;
				IL_0167:
				int result2 = awaiter.GetResult();
				if (result2 != 0)
				{
					sslStream._buffer.Commit(result2);
					if (_003CframeSize_003E5__2 == int.MaxValue && sslStream._buffer.EncryptedLength > 5)
					{
						_003CframeSize_003E5__2 = sslStream.GetFrameSize(sslStream._buffer.EncryptedReadOnlySpan);
						sslStream._buffer.EnsureAvailableSpace(_003CframeSize_003E5__2 - sslStream._buffer.EncryptedLength);
					}
					goto IL_01ef;
				}
				if (sslStream._buffer.EncryptedLength != 0)
				{
					throw new IOException(System.SR.net_io_eof);
				}
				result = 0;
				goto end_IL_000e;
				IL_00aa:
				awaiter.GetResult();
				sslStream._buffer.EnsureAvailableSpace((_003CframeSize_003E5__2 == int.MaxValue) ? estimatedSize : (_003CframeSize_003E5__2 - sslStream._buffer.EncryptedLength));
				goto IL_01ef;
				IL_01ef:
				if (sslStream._buffer.EncryptedLength < _003CframeSize_003E5__2)
				{
					awaiter = TIOAdapter.ReadAsync(sslStream.InnerStream, sslStream._buffer.AvailableMemory, cancellationToken).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0167;
				}
				result = _003CframeSize_003E5__2;
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
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
	private struct _003CReadAsyncInternal_003Ed__172<TIOAdapter> : IAsyncStateMachine where TIOAdapter : IReadWriteAdapter
	{
		public int _003C_003E1__state;

		public PoolingAsyncValueTaskMethodBuilder<int> _003C_003Et__builder;

		public SslStream _003C_003E4__this;

		public Memory<byte> buffer;

		public CancellationToken cancellationToken;

		private int _003CprocessedLength_003E5__2;

		private ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			SslStream sslStream = _003C_003E4__this;
			int result;
			try
			{
				if ((uint)num > 1u)
				{
					sslStream.ThrowIfExceptionalOrNotAuthenticated();
					if (Interlocked.CompareExchange(ref sslStream._nestedRead, NestedState.StreamInUse, NestedState.StreamNotInUse) != NestedState.StreamNotInUse)
					{
						ObjectDisposedException.ThrowIf(sslStream._nestedRead == NestedState.StreamDisposed, sslStream);
						throw new NotSupportedException(System.SR.Format(System.SR.net_io_invalidnestedcall, "read"));
					}
				}
				try
				{
					ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter awaiter;
					if (num == 0)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_014c;
					}
					ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter2;
					if (num == 1)
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_028b;
					}
					_003CprocessedLength_003E5__2 = 0;
					int frameSize = int.MaxValue;
					if (sslStream._buffer.DecryptedLength == 0)
					{
						goto IL_00c5;
					}
					_003CprocessedLength_003E5__2 = sslStream.CopyDecryptedData(buffer);
					if (_003CprocessedLength_003E5__2 != buffer.Length && sslStream.HaveFullTlsFrame(out frameSize))
					{
						buffer = buffer.Slice(_003CprocessedLength_003E5__2);
						goto IL_00c5;
					}
					result = _003CprocessedLength_003E5__2;
					goto end_IL_004c;
					IL_00c5:
					if (!sslStream._receivedEOF || frameSize != int.MaxValue)
					{
						goto IL_00dc;
					}
					result = 0;
					goto end_IL_004c;
					IL_02bc:
					if (sslStream._buffer.DecryptedLength > 0)
					{
						int num2 = sslStream.CopyDecryptedData(buffer);
						_003CprocessedLength_003E5__2 += num2;
						if (num2 == buffer.Length)
						{
							goto IL_0351;
						}
						buffer = buffer.Slice(num2);
					}
					if (_003CprocessedLength_003E5__2 == 0)
					{
						goto IL_00dc;
					}
					if (sslStream.HaveFullTlsFrame(out var frameSize2))
					{
						TlsFrameHelper.TryGetFrameHeader(sslStream._buffer.EncryptedReadOnlySpan, ref sslStream._lastFrame.Header);
						if (sslStream._lastFrame.Header.Type == TlsContentType.AppData)
						{
							goto IL_00dc;
						}
					}
					goto IL_0351;
					IL_00dc:
					awaiter = sslStream.EnsureFullTlsFrameAsync<TIOAdapter>(cancellationToken, 16448).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_014c;
					IL_028b:
					awaiter2.GetResult();
					goto IL_02bc;
					IL_014c:
					frameSize2 = awaiter.GetResult();
					if (frameSize2 == 0)
					{
						sslStream._receivedEOF = true;
					}
					else
					{
						SecurityStatusPal securityStatusPal = sslStream.DecryptData(frameSize2);
						if (securityStatusPal.ErrorCode == SecurityStatusPalErrorCode.OK)
						{
							goto IL_02bc;
						}
						byte[] array = null;
						if (sslStream._buffer.DecryptedLength != 0)
						{
							array = new byte[sslStream._buffer.DecryptedLength];
							sslStream._buffer.DecryptedSpan.CopyTo(array);
							sslStream._buffer.Discard(sslStream._buffer.DecryptedLength);
						}
						if (System.Net.NetEventSource.Log.IsEnabled())
						{
							System.Net.NetEventSource.Info(null, $"***Processing an error Status = {securityStatusPal}", "ReadAsyncInternal");
						}
						if (securityStatusPal.ErrorCode == SecurityStatusPalErrorCode.Renegotiate)
						{
							if (sslStream._handshakeWaiter == null)
							{
								throw new IOException(System.SR.net_ssl_io_renego);
							}
							awaiter2 = sslStream.ReplyOnReAuthenticationAsync<TIOAdapter>(array, cancellationToken).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_028b;
						}
						if (securityStatusPal.ErrorCode != SecurityStatusPalErrorCode.ContextExpired)
						{
							throw new IOException(System.SR.net_io_decrypt, SslStreamPal.GetException(securityStatusPal));
						}
						sslStream._receivedEOF = true;
					}
					goto IL_0351;
					IL_0351:
					result = _003CprocessedLength_003E5__2;
					end_IL_004c:;
				}
				catch (Exception ex)
				{
					if (ex is IOException || (ex is OperationCanceledException && cancellationToken.IsCancellationRequested))
					{
						throw;
					}
					throw new IOException(System.SR.net_io_read, ex);
				}
				finally
				{
					if (num < 0)
					{
						sslStream.ReturnReadBufferIfEmpty();
						sslStream._nestedRead = NestedState.StreamNotInUse;
					}
				}
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
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

	private static readonly ExceptionDispatchInfo s_disposedSentinel = ExceptionDispatchInfo.Capture(new ObjectDisposedException("SslStream", (string?)null));

	private ExceptionDispatchInfo _exception;

	private bool _shutdown;

	private bool _handshakeCompleted;

	private SslBuffer _buffer = new SslBuffer();

	private NestedState _nestedWrite;

	private NestedState _nestedRead;

	private PoolingPointerMemoryManager _readPointerMemoryManager;

	private PoolingPointerMemoryManager _writePointerMemoryManager;

	internal readonly SslAuthenticationOptions _sslAuthenticationOptions = new SslAuthenticationOptions();

	private NestedState _nestedAuth;

	private bool _isRenego;

	private TlsFrameHelper.TlsFrameInfo _lastFrame;

	private volatile TaskCompletionSource<bool> _handshakeWaiter;

	private bool _receivedEOF;

	private ConnectionStatus _connectionOpenedStatus;

	private static volatile int s_disableTlsResume = -1;

	private SafeFreeCredentials _credentialsHandle;

	private SafeDeleteSslContext _securityContext;

	private SslConnectionInfo _connectionInfo;

	private X509Certificate _selectedClientCertificate;

	private X509Certificate2 _remoteCertificate;

	private bool _remoteCertificateExposed;

	private int _localClientCertificateUsed = -1;

	private int _headerSize = 5;

	private int _trailerSize = 16;

	private int _maxDataSize = 16354;

	private static readonly Oid s_serverAuthOid = new Oid("1.3.6.1.5.5.7.3.1", "1.3.6.1.5.5.7.3.1");

	private static readonly Oid s_clientAuthOid = new Oid("1.3.6.1.5.5.7.3.2", "1.3.6.1.5.5.7.3.2");

	public TransportContext TransportContext => new SslStreamContext(this);

	public override bool IsAuthenticated
	{
		get
		{
			if (IsValidContext && _exception == null)
			{
				return _handshakeCompleted;
			}
			return false;
		}
	}

	public override bool IsMutuallyAuthenticated
	{
		get
		{
			if (IsAuthenticated && (IsServer ? LocalServerCertificate : LocalClientCertificate) != null)
			{
				return IsRemoteCertificateAvailable;
			}
			return false;
		}
	}

	public override bool IsEncrypted => IsAuthenticated;

	public override bool IsSigned => IsAuthenticated;

	public override bool IsServer => _sslAuthenticationOptions.IsServer;

	public virtual SslProtocols SslProtocol
	{
		get
		{
			ThrowIfExceptionalOrNotHandshake();
			return GetSslProtocolInternal();
		}
	}

	public virtual bool CheckCertRevocationStatus => _sslAuthenticationOptions.CertificateRevocationCheckMode != X509RevocationMode.NoCheck;

	public virtual X509Certificate? LocalCertificate
	{
		get
		{
			ThrowIfExceptionalOrNotAuthenticated();
			if (!IsServer)
			{
				return LocalClientCertificate;
			}
			return LocalServerCertificate;
		}
	}

	public virtual X509Certificate? RemoteCertificate
	{
		get
		{
			ThrowIfExceptionalOrNotAuthenticated();
			_remoteCertificateExposed = true;
			return _remoteCertificate;
		}
	}

	public SslApplicationProtocol NegotiatedApplicationProtocol
	{
		get
		{
			ThrowIfExceptionalOrNotHandshake();
			if (_connectionInfo.ApplicationProtocol == null)
			{
				return default(SslApplicationProtocol);
			}
			return new SslApplicationProtocol(_connectionInfo.ApplicationProtocol, copy: false);
		}
	}

	[CLSCompliant(false)]
	public virtual TlsCipherSuite NegotiatedCipherSuite
	{
		get
		{
			ThrowIfExceptionalOrNotHandshake();
			return _connectionInfo.TlsCipherSuite;
		}
	}

	[Obsolete("KeyExchangeAlgorithm, KeyExchangeStrength, CipherAlgorithm, CipherStrength, HashAlgorithm and HashStrength properties of SslStream are obsolete. Use NegotiatedCipherSuite instead.", DiagnosticId = "SYSLIB0058", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual CipherAlgorithmType CipherAlgorithm
	{
		get
		{
			ThrowIfExceptionalOrNotHandshake();
			return (CipherAlgorithmType)_connectionInfo.DataCipherAlg;
		}
	}

	[Obsolete("KeyExchangeAlgorithm, KeyExchangeStrength, CipherAlgorithm, CipherStrength, HashAlgorithm and HashStrength properties of SslStream are obsolete. Use NegotiatedCipherSuite instead.", DiagnosticId = "SYSLIB0058", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual int CipherStrength
	{
		get
		{
			ThrowIfExceptionalOrNotHandshake();
			return _connectionInfo.DataKeySize;
		}
	}

	[Obsolete("KeyExchangeAlgorithm, KeyExchangeStrength, CipherAlgorithm, CipherStrength, HashAlgorithm and HashStrength properties of SslStream are obsolete. Use NegotiatedCipherSuite instead.", DiagnosticId = "SYSLIB0058", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual HashAlgorithmType HashAlgorithm
	{
		get
		{
			ThrowIfExceptionalOrNotHandshake();
			return (HashAlgorithmType)_connectionInfo.DataHashAlg;
		}
	}

	[Obsolete("KeyExchangeAlgorithm, KeyExchangeStrength, CipherAlgorithm, CipherStrength, HashAlgorithm and HashStrength properties of SslStream are obsolete. Use NegotiatedCipherSuite instead.", DiagnosticId = "SYSLIB0058", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual int HashStrength
	{
		get
		{
			ThrowIfExceptionalOrNotHandshake();
			return _connectionInfo.DataHashKeySize;
		}
	}

	[Obsolete("KeyExchangeAlgorithm, KeyExchangeStrength, CipherAlgorithm, CipherStrength, HashAlgorithm and HashStrength properties of SslStream are obsolete. Use NegotiatedCipherSuite instead.", DiagnosticId = "SYSLIB0058", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual ExchangeAlgorithmType KeyExchangeAlgorithm
	{
		get
		{
			ThrowIfExceptionalOrNotHandshake();
			return (ExchangeAlgorithmType)_connectionInfo.KeyExchangeAlg;
		}
	}

	[Obsolete("KeyExchangeAlgorithm, KeyExchangeStrength, CipherAlgorithm, CipherStrength, HashAlgorithm and HashStrength properties of SslStream are obsolete. Use NegotiatedCipherSuite instead.", DiagnosticId = "SYSLIB0058", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual int KeyExchangeStrength
	{
		get
		{
			ThrowIfExceptionalOrNotHandshake();
			return _connectionInfo.KeyExchKeySize;
		}
	}

	public string TargetHostName => _sslAuthenticationOptions.TargetHost;

	public override bool CanSeek => false;

	public override bool CanRead
	{
		get
		{
			if (IsAuthenticated)
			{
				return InnerStream.CanRead;
			}
			return false;
		}
	}

	public override bool CanTimeout => InnerStream.CanTimeout;

	public override bool CanWrite
	{
		get
		{
			if (IsAuthenticated && InnerStream.CanWrite)
			{
				return !_shutdown;
			}
			return false;
		}
	}

	public override int ReadTimeout
	{
		get
		{
			return InnerStream.ReadTimeout;
		}
		set
		{
			InnerStream.ReadTimeout = value;
		}
	}

	public override int WriteTimeout
	{
		get
		{
			return InnerStream.WriteTimeout;
		}
		set
		{
			InnerStream.WriteTimeout = value;
		}
	}

	public override long Length => InnerStream.Length;

	public override long Position
	{
		get
		{
			return InnerStream.Position;
		}
		set
		{
			throw new NotSupportedException(System.SR.net_noseek);
		}
	}

	internal new Stream InnerStream => base.InnerStream;

	private object _handshakeLock => _sslAuthenticationOptions;

	internal static bool DisableTlsResume
	{
		get
		{
			int num = s_disableTlsResume;
			if (num != -1)
			{
				return num != 0;
			}
			if (AppContext.TryGetSwitch("System.Net.Security.DisableTlsResume", out var isEnabled))
			{
				s_disableTlsResume = (isEnabled ? 1 : 0);
			}
			else
			{
				string environmentVariable = Environment.GetEnvironmentVariable("DOTNET_SYSTEM_NET_SECURITY_DISABLETLSRESUME");
				s_disableTlsResume = ((environmentVariable != null && (environmentVariable == "1" || environmentVariable.Equals("true", StringComparison.OrdinalIgnoreCase))) ? 1 : 0);
			}
			return s_disableTlsResume != 0;
		}
	}

	internal X509Certificate? LocalServerCertificate => _sslAuthenticationOptions.CertificateContext?.TargetCertificate;

	private bool IsLocalClientCertificateUsed
	{
		get
		{
			if (_localClientCertificateUsed == -1)
			{
				_localClientCertificateUsed = (CertificateValidationPal.IsLocalCertificateUsed(_credentialsHandle, _securityContext) ? 1 : 0);
			}
			return _localClientCertificateUsed == 1;
		}
	}

	internal X509Certificate? LocalClientCertificate
	{
		get
		{
			if (_selectedClientCertificate != null && IsLocalClientCertificateUsed)
			{
				return _selectedClientCertificate;
			}
			return null;
		}
	}

	internal bool IsRemoteCertificateAvailable => _remoteCertificate != null;

	internal int MaxDataSize => _maxDataSize;

	internal bool IsValidContext
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (_securityContext != null)
			{
				return !_securityContext.IsInvalid;
			}
			return false;
		}
	}

	internal bool RemoteCertRequired => _sslAuthenticationOptions.RemoteCertRequired;

	public SslStream(Stream innerStream)
		: this(innerStream, leaveInnerStreamOpen: false, null, null)
	{
	}

	public SslStream(Stream innerStream, bool leaveInnerStreamOpen)
		: this(innerStream, leaveInnerStreamOpen, null, null, EncryptionPolicy.RequireEncryption)
	{
	}

	public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback? userCertificateValidationCallback)
		: this(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, null, EncryptionPolicy.RequireEncryption)
	{
	}

	public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback? userCertificateValidationCallback, LocalCertificateSelectionCallback? userCertificateSelectionCallback)
		: this(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback, EncryptionPolicy.RequireEncryption)
	{
	}

	public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback? userCertificateValidationCallback, LocalCertificateSelectionCallback? userCertificateSelectionCallback, EncryptionPolicy encryptionPolicy)
		: base(innerStream, leaveInnerStreamOpen)
	{
		if (encryptionPolicy != EncryptionPolicy.RequireEncryption && encryptionPolicy != EncryptionPolicy.AllowNoEncryption && encryptionPolicy != EncryptionPolicy.NoEncryption)
		{
			throw new ArgumentException(System.SR.Format(System.SR.net_invalid_enum, "EncryptionPolicy"), "encryptionPolicy");
		}
		_sslAuthenticationOptions.EncryptionPolicy = encryptionPolicy;
		_sslAuthenticationOptions.CertValidationDelegate = userCertificateValidationCallback;
		_sslAuthenticationOptions.CertSelectionDelegate = userCertificateSelectionCallback;
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.SslStreamCtor(this, innerStream);
		}
	}

	public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, AsyncCallback? asyncCallback, object? asyncState)
	{
		return BeginAuthenticateAsClient(targetHost, null, SslProtocols.None, checkCertificateRevocation: false, asyncCallback, asyncState);
	}

	public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection? clientCertificates, bool checkCertificateRevocation, AsyncCallback? asyncCallback, object? asyncState)
	{
		return BeginAuthenticateAsClient(targetHost, clientCertificates, SslProtocols.None, checkCertificateRevocation, asyncCallback, asyncState);
	}

	public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection? clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback? asyncCallback, object? asyncState)
	{
		SslClientAuthenticationOptions sslClientAuthenticationOptions = new SslClientAuthenticationOptions
		{
			TargetHost = targetHost,
			ClientCertificates = clientCertificates,
			EnabledSslProtocols = enabledSslProtocols,
			CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
			EncryptionPolicy = _sslAuthenticationOptions.EncryptionPolicy
		};
		return BeginAuthenticateAsClient(sslClientAuthenticationOptions, CancellationToken.None, asyncCallback, asyncState);
	}

	internal IAsyncResult BeginAuthenticateAsClient(SslClientAuthenticationOptions sslClientAuthenticationOptions, CancellationToken cancellationToken, AsyncCallback asyncCallback, object asyncState)
	{
		return TaskToAsyncResult.Begin(AuthenticateAsClientAsync(sslClientAuthenticationOptions, cancellationToken), asyncCallback, asyncState);
	}

	public virtual void EndAuthenticateAsClient(IAsyncResult asyncResult)
	{
		TaskToAsyncResult.End(asyncResult);
	}

	public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, AsyncCallback? asyncCallback, object? asyncState)
	{
		return BeginAuthenticateAsServer(serverCertificate, clientCertificateRequired: false, SslProtocols.None, checkCertificateRevocation: false, asyncCallback, asyncState);
	}

	public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, bool checkCertificateRevocation, AsyncCallback? asyncCallback, object? asyncState)
	{
		return BeginAuthenticateAsServer(serverCertificate, clientCertificateRequired, SslProtocols.None, checkCertificateRevocation, asyncCallback, asyncState);
	}

	public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback? asyncCallback, object? asyncState)
	{
		SslServerAuthenticationOptions sslServerAuthenticationOptions = new SslServerAuthenticationOptions
		{
			ServerCertificate = serverCertificate,
			ClientCertificateRequired = clientCertificateRequired,
			EnabledSslProtocols = enabledSslProtocols,
			CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
			EncryptionPolicy = _sslAuthenticationOptions.EncryptionPolicy
		};
		return BeginAuthenticateAsServer(sslServerAuthenticationOptions, CancellationToken.None, asyncCallback, asyncState);
	}

	private IAsyncResult BeginAuthenticateAsServer(SslServerAuthenticationOptions sslServerAuthenticationOptions, CancellationToken cancellationToken, AsyncCallback asyncCallback, object asyncState)
	{
		return TaskToAsyncResult.Begin(AuthenticateAsServerAsync(sslServerAuthenticationOptions, cancellationToken), asyncCallback, asyncState);
	}

	public virtual void EndAuthenticateAsServer(IAsyncResult asyncResult)
	{
		TaskToAsyncResult.End(asyncResult);
	}

	public virtual void AuthenticateAsClient(string targetHost)
	{
		AuthenticateAsClient(targetHost, null, SslProtocols.None, checkCertificateRevocation: false);
	}

	public virtual void AuthenticateAsClient(string targetHost, X509CertificateCollection? clientCertificates, bool checkCertificateRevocation)
	{
		AuthenticateAsClient(targetHost, clientCertificates, SslProtocols.None, checkCertificateRevocation);
	}

	public virtual void AuthenticateAsClient(string targetHost, X509CertificateCollection? clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
	{
		SslClientAuthenticationOptions sslClientAuthenticationOptions = new SslClientAuthenticationOptions
		{
			TargetHost = targetHost,
			ClientCertificates = clientCertificates,
			EnabledSslProtocols = enabledSslProtocols,
			CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
			EncryptionPolicy = _sslAuthenticationOptions.EncryptionPolicy
		};
		AuthenticateAsClient(sslClientAuthenticationOptions);
	}

	public void AuthenticateAsClient(SslClientAuthenticationOptions sslClientAuthenticationOptions)
	{
		ArgumentNullException.ThrowIfNull(sslClientAuthenticationOptions, "sslClientAuthenticationOptions");
		ThrowIfExceptional();
		_sslAuthenticationOptions.UpdateOptions(sslClientAuthenticationOptions);
		ProcessAuthenticationAsync().GetAwaiter().GetResult();
	}

	public virtual void AuthenticateAsServer(X509Certificate serverCertificate)
	{
		AuthenticateAsServer(serverCertificate, clientCertificateRequired: false, SslProtocols.None, checkCertificateRevocation: false);
	}

	public virtual void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, bool checkCertificateRevocation)
	{
		AuthenticateAsServer(serverCertificate, clientCertificateRequired, SslProtocols.None, checkCertificateRevocation);
	}

	public virtual void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
	{
		SslServerAuthenticationOptions sslServerAuthenticationOptions = new SslServerAuthenticationOptions
		{
			ServerCertificate = serverCertificate,
			ClientCertificateRequired = clientCertificateRequired,
			EnabledSslProtocols = enabledSslProtocols,
			CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
			EncryptionPolicy = _sslAuthenticationOptions.EncryptionPolicy
		};
		AuthenticateAsServer(sslServerAuthenticationOptions);
	}

	public void AuthenticateAsServer(SslServerAuthenticationOptions sslServerAuthenticationOptions)
	{
		ArgumentNullException.ThrowIfNull(sslServerAuthenticationOptions, "sslServerAuthenticationOptions");
		_sslAuthenticationOptions.UpdateOptions(sslServerAuthenticationOptions);
		ProcessAuthenticationAsync().GetAwaiter().GetResult();
	}

	public virtual Task AuthenticateAsClientAsync(string targetHost)
	{
		return AuthenticateAsClientAsync(targetHost, null, checkCertificateRevocation: false);
	}

	public virtual Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection? clientCertificates, bool checkCertificateRevocation)
	{
		return AuthenticateAsClientAsync(targetHost, clientCertificates, SslProtocols.None, checkCertificateRevocation);
	}

	public virtual Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection? clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
	{
		SslClientAuthenticationOptions sslClientAuthenticationOptions = new SslClientAuthenticationOptions
		{
			TargetHost = targetHost,
			ClientCertificates = clientCertificates,
			EnabledSslProtocols = enabledSslProtocols,
			CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
			EncryptionPolicy = _sslAuthenticationOptions.EncryptionPolicy
		};
		return AuthenticateAsClientAsync(sslClientAuthenticationOptions);
	}

	public Task AuthenticateAsClientAsync(SslClientAuthenticationOptions sslClientAuthenticationOptions, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(sslClientAuthenticationOptions, "sslClientAuthenticationOptions");
		ThrowIfExceptional();
		_sslAuthenticationOptions.UpdateOptions(sslClientAuthenticationOptions);
		return ProcessAuthenticationAsync(isAsync: true, cancellationToken);
	}

	public virtual Task AuthenticateAsServerAsync(X509Certificate serverCertificate)
	{
		return AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired: false, SslProtocols.None, checkCertificateRevocation: false);
	}

	public virtual Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, bool checkCertificateRevocation)
	{
		SslServerAuthenticationOptions sslServerAuthenticationOptions = new SslServerAuthenticationOptions
		{
			ServerCertificate = serverCertificate,
			ClientCertificateRequired = clientCertificateRequired,
			CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
			EncryptionPolicy = _sslAuthenticationOptions.EncryptionPolicy
		};
		return AuthenticateAsServerAsync(sslServerAuthenticationOptions);
	}

	public virtual Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
	{
		SslServerAuthenticationOptions sslServerAuthenticationOptions = new SslServerAuthenticationOptions
		{
			ServerCertificate = serverCertificate,
			ClientCertificateRequired = clientCertificateRequired,
			EnabledSslProtocols = enabledSslProtocols,
			CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
			EncryptionPolicy = _sslAuthenticationOptions.EncryptionPolicy
		};
		return AuthenticateAsServerAsync(sslServerAuthenticationOptions);
	}

	public Task AuthenticateAsServerAsync(SslServerAuthenticationOptions sslServerAuthenticationOptions, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(sslServerAuthenticationOptions, "sslServerAuthenticationOptions");
		_sslAuthenticationOptions.UpdateOptions(sslServerAuthenticationOptions);
		return ProcessAuthenticationAsync(isAsync: true, cancellationToken);
	}

	public Task AuthenticateAsServerAsync(ServerOptionsSelectionCallback optionsCallback, object? state, CancellationToken cancellationToken = default(CancellationToken))
	{
		_sslAuthenticationOptions.UpdateOptions(optionsCallback, state);
		return ProcessAuthenticationAsync(isAsync: true, cancellationToken);
	}

	public virtual Task ShutdownAsync()
	{
		ThrowIfExceptionalOrNotAuthenticatedOrShutdown();
		ProtocolToken protocolToken = CreateShutdownToken();
		_shutdown = true;
		if (protocolToken.Size > 0 && protocolToken.Payload != null)
		{
			return InnerStream.WriteAsync(new ReadOnlyMemory<byte>(protocolToken.Payload, 0, protocolToken.Size)).AsTask();
		}
		return Task.CompletedTask;
	}

	internal SslProtocols GetSslProtocolInternal()
	{
		if (_connectionInfo.Protocol == 0)
		{
			return SslProtocols.None;
		}
		int protocol = _connectionInfo.Protocol;
		SslProtocols sslProtocols = SslProtocols.None;
		if ((protocol & 0xC) != 0)
		{
			sslProtocols |= SslProtocols.Ssl2;
		}
		if ((protocol & 0x30) != 0)
		{
			sslProtocols |= SslProtocols.Ssl3;
		}
		if ((protocol & 0xC0) != 0)
		{
			sslProtocols |= SslProtocols.Tls;
		}
		if ((protocol & 0x300) != 0)
		{
			sslProtocols |= SslProtocols.Tls11;
		}
		if ((protocol & 0xC00) != 0)
		{
			sslProtocols |= SslProtocols.Tls12;
		}
		if ((protocol & 0x3000) != 0)
		{
			sslProtocols |= SslProtocols.Tls13;
		}
		return sslProtocols;
	}

	public override void SetLength(long value)
	{
		InnerStream.SetLength(value);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException(System.SR.net_noseek);
	}

	public override void Flush()
	{
		InnerStream.Flush();
	}

	public override Task FlushAsync(CancellationToken cancellationToken)
	{
		return InnerStream.FlushAsync(cancellationToken);
	}

	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("windows")]
	[SupportedOSPlatform("freebsd")]
	public virtual Task NegotiateClientCertificateAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfExceptionalOrNotAuthenticated();
		if (RemoteCertificate != null)
		{
			throw new InvalidOperationException(System.SR.net_ssl_certificate_exist);
		}
		return RenegotiateAsync<AsyncReadWriteAdapter>(cancellationToken);
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			CloseInternal();
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	public override async ValueTask DisposeAsync()
	{
		try
		{
			CloseInternal();
		}
		finally
		{
			await base.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private unsafe static PoolingPointerMemoryManager RentPointerMemoryManager(ref PoolingPointerMemoryManager field, byte* pointer, int length)
	{
		PoolingPointerMemoryManager obj = Interlocked.Exchange(ref field, null) ?? new PoolingPointerMemoryManager();
		obj.Reset(pointer, length);
		return obj;
	}

	private unsafe static void ReturnPointerMemoryManager(ref PoolingPointerMemoryManager field, PoolingPointerMemoryManager manager)
	{
		manager.Reset(null, 0);
		field = manager;
	}

	public override int ReadByte()
	{
		ThrowIfExceptionalOrNotAuthenticated();
		if (Interlocked.Exchange(ref _nestedRead, NestedState.StreamInUse) == NestedState.StreamInUse)
		{
			throw new NotSupportedException(System.SR.Format(System.SR.net_io_invalidnestedcall, "read"));
		}
		try
		{
			if (_buffer.DecryptedLength > 0)
			{
				byte result = _buffer.DecryptedSpan[0];
				_buffer.Discard(1);
				ReturnReadBufferIfEmpty();
				return result;
			}
		}
		finally
		{
			_nestedRead = NestedState.StreamNotInUse;
		}
		byte reference = 0;
		if (Read(new Span<byte>(ref reference)) != 1)
		{
			return -1;
		}
		return reference;
	}

	public unsafe override int Read(Span<byte> buffer)
	{
		ThrowIfExceptionalOrNotAuthenticated();
		fixed (byte* reference = &MemoryMarshal.GetReference(buffer))
		{
			PoolingPointerMemoryManager poolingPointerMemoryManager = RentPointerMemoryManager(ref _readPointerMemoryManager, reference, buffer.Length);
			try
			{
				return ReadAsyncInternal<SyncReadWriteAdapter>(poolingPointerMemoryManager.Memory, default(CancellationToken)).GetAwaiter().GetResult();
			}
			finally
			{
				ReturnPointerMemoryManager(ref _readPointerMemoryManager, poolingPointerMemoryManager);
			}
		}
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		ThrowIfExceptionalOrNotAuthenticated();
		Stream.ValidateBufferArguments(buffer, offset, count);
		return ReadAsyncInternal<SyncReadWriteAdapter>(new Memory<byte>(buffer, offset, count), default(CancellationToken)).GetAwaiter().GetResult();
	}

	public override void WriteByte(byte value)
	{
		Write(new ReadOnlySpan<byte>(in value));
	}

	public unsafe override void Write(ReadOnlySpan<byte> buffer)
	{
		ThrowIfExceptionalOrNotAuthenticated();
		fixed (byte* reference = &MemoryMarshal.GetReference(buffer))
		{
			PoolingPointerMemoryManager poolingPointerMemoryManager = RentPointerMemoryManager(ref _writePointerMemoryManager, reference, buffer.Length);
			try
			{
				WriteAsyncInternal<SyncReadWriteAdapter>(poolingPointerMemoryManager.Memory, default(CancellationToken)).GetAwaiter().GetResult();
			}
			finally
			{
				ReturnPointerMemoryManager(ref _writePointerMemoryManager, poolingPointerMemoryManager);
			}
		}
	}

	public void Write(byte[] buffer)
	{
		Write(buffer, 0, buffer.Length);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		ThrowIfExceptionalOrNotAuthenticated();
		Stream.ValidateBufferArguments(buffer, offset, count);
		WriteAsyncInternal<SyncReadWriteAdapter>(new ReadOnlyMemory<byte>(buffer, offset, count), default(CancellationToken)).GetAwaiter().GetResult();
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? asyncCallback, object? asyncState)
	{
		ThrowIfExceptionalOrNotAuthenticated();
		return TaskToAsyncResult.Begin(ReadAsync(buffer, offset, count, CancellationToken.None), asyncCallback, asyncState);
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		ThrowIfExceptionalOrNotAuthenticated();
		return TaskToAsyncResult.End<int>(asyncResult);
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? asyncCallback, object? asyncState)
	{
		ThrowIfExceptionalOrNotAuthenticated();
		return TaskToAsyncResult.Begin(WriteAsync(buffer, offset, count, CancellationToken.None), asyncCallback, asyncState);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		ThrowIfExceptionalOrNotAuthenticated();
		TaskToAsyncResult.End(asyncResult);
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		ThrowIfExceptionalOrNotAuthenticated();
		Stream.ValidateBufferArguments(buffer, offset, count);
		return WriteAsyncInternal<AsyncReadWriteAdapter>(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();
	}

	public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfExceptionalOrNotAuthenticated();
		return WriteAsyncInternal<AsyncReadWriteAdapter>(buffer, cancellationToken);
	}

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		ThrowIfExceptionalOrNotAuthenticated();
		Stream.ValidateBufferArguments(buffer, offset, count);
		return ReadAsyncInternal<AsyncReadWriteAdapter>(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
	}

	public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfExceptionalOrNotAuthenticated();
		return ReadAsyncInternal<AsyncReadWriteAdapter>(buffer, cancellationToken);
	}

	private void ThrowIfExceptional()
	{
		ExceptionDispatchInfo exception = _exception;
		if (exception != null)
		{
			ThrowExceptional(exception);
		}
		void ThrowExceptional(ExceptionDispatchInfo e)
		{
			ObjectDisposedException.ThrowIf(e == s_disposedSentinel, this);
			e.Throw();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ThrowIfExceptionalOrNotAuthenticated()
	{
		ThrowIfExceptional();
		if (!IsAuthenticated)
		{
			ThrowNotAuthenticated();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ThrowIfExceptionalOrNotHandshake()
	{
		ThrowIfExceptional();
		if (_connectionInfo.Protocol == 0)
		{
			ThrowNotAuthenticated();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ThrowIfExceptionalOrNotAuthenticatedOrShutdown()
	{
		ThrowIfExceptional();
		if (!IsAuthenticated)
		{
			ThrowNotAuthenticated();
		}
		if (_shutdown)
		{
			ThrowAlreadyShutdown();
		}
		static void ThrowAlreadyShutdown()
		{
			throw new InvalidOperationException(System.SR.net_ssl_io_already_shutdown);
		}
	}

	private static void ThrowNotAuthenticated()
	{
		throw new InvalidOperationException(System.SR.net_auth_noauth);
	}

	private void SetException(Exception e)
	{
		if (_exception == null)
		{
			_exception = ExceptionDispatchInfo.Capture(e);
		}
		CloseContext();
	}

	private void CloseInternal()
	{
		_exception = s_disposedSentinel;
		CloseContext();
		if (Interlocked.Exchange(ref _nestedRead, NestedState.StreamDisposed) == NestedState.StreamNotInUse && Interlocked.Exchange(ref _nestedAuth, NestedState.StreamDisposed) == NestedState.StreamNotInUse)
		{
			_buffer.ReturnBuffer();
		}
		if (!_buffer.IsValid)
		{
			GC.SuppressFinalize(this);
		}
		if (NetSecurityTelemetry.Log.IsEnabled() && Interlocked.Exchange(ref _connectionOpenedStatus, ConnectionStatus.Disposed) == ConnectionStatus.HandshakeCompleted)
		{
			NetSecurityTelemetry.Log.ConnectionClosed(GetSslProtocolInternal());
		}
	}

	private ProtocolToken EncryptData(ReadOnlyMemory<byte> buffer)
	{
		ThrowIfExceptionalOrNotAuthenticated();
		lock (_handshakeLock)
		{
			if (_handshakeWaiter != null)
			{
				return new ProtocolToken
				{
					Status = new SecurityStatusPal(SecurityStatusPalErrorCode.TryAgain)
				};
			}
			return Encrypt(buffer);
		}
	}

	private Task ProcessAuthenticationAsync(bool isAsync = false, CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfExceptional();
		if (NetSecurityTelemetry.AnyTelemetryEnabled())
		{
			return ProcessAuthenticationWithTelemetryAsync(isAsync, cancellationToken);
		}
		if (!isAsync)
		{
			return ForceAuthenticationAsync<SyncReadWriteAdapter>(IsServer, null, cancellationToken);
		}
		return ForceAuthenticationAsync<AsyncReadWriteAdapter>(IsServer, null, cancellationToken);
	}

	private async Task ProcessAuthenticationWithTelemetryAsync(bool isAsync, CancellationToken cancellationToken)
	{
		long startingTimestamp;
		if (NetSecurityTelemetry.Log.IsEnabled())
		{
			NetSecurityTelemetry.Log.HandshakeStart(IsServer, _sslAuthenticationOptions.TargetHost);
			startingTimestamp = Stopwatch.GetTimestamp();
		}
		else
		{
			startingTimestamp = 0L;
		}
		Activity activity = NetSecurityTelemetry.StartActivity(this);
		Exception exception = null;
		try
		{
			await (isAsync ? ForceAuthenticationAsync<AsyncReadWriteAdapter>(IsServer, null, cancellationToken) : ForceAuthenticationAsync<SyncReadWriteAdapter>(IsServer, null, cancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
			if (startingTimestamp != 0L)
			{
				bool connectionOpen = Interlocked.CompareExchange(ref _connectionOpenedStatus, ConnectionStatus.HandshakeCompleted, ConnectionStatus.NoHandshake) == ConnectionStatus.NoHandshake;
				SslProtocols sslProtocolInternal = GetSslProtocolInternal();
				NetSecurityTelemetry.Log.HandshakeCompleted(sslProtocolInternal, startingTimestamp, connectionOpen);
			}
		}
		catch (Exception ex)
		{
			exception = ex;
			if (startingTimestamp != 0L)
			{
				NetSecurityTelemetry.Log.HandshakeFailed(IsServer, startingTimestamp, ex.Message);
			}
			throw;
		}
		finally
		{
			NetSecurityTelemetry.StopActivity(activity, exception, this);
		}
	}

	private async Task ReplyOnReAuthenticationAsync<TIOAdapter>(byte[] buffer, CancellationToken cancellationToken) where TIOAdapter : IReadWriteAdapter
	{
		try
		{
			await ForceAuthenticationAsync<TIOAdapter>(receiveFirst: false, buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			_handshakeWaiter.SetResult(result: true);
			_handshakeWaiter = null;
		}
	}

	private async Task RenegotiateAsync<TIOAdapter>(CancellationToken cancellationToken) where TIOAdapter : IReadWriteAdapter
	{
		if (Interlocked.CompareExchange(ref _nestedAuth, NestedState.StreamInUse, NestedState.StreamNotInUse) != NestedState.StreamNotInUse)
		{
			ObjectDisposedException.ThrowIf(_nestedAuth == NestedState.StreamDisposed, this);
			throw new InvalidOperationException(System.SR.Format(System.SR.net_io_invalidnestedcall, "authenticate"));
		}
		if (Interlocked.CompareExchange(ref _nestedRead, NestedState.StreamInUse, NestedState.StreamNotInUse) != NestedState.StreamNotInUse)
		{
			ObjectDisposedException.ThrowIf(_nestedRead == NestedState.StreamDisposed, this);
			throw new NotSupportedException(System.SR.Format(System.SR.net_io_invalidnestedcall, "read"));
		}
		if (Interlocked.Exchange(ref _nestedWrite, NestedState.StreamInUse) != NestedState.StreamNotInUse)
		{
			_nestedRead = NestedState.StreamNotInUse;
			throw new NotSupportedException(System.SR.Format(System.SR.net_io_invalidnestedcall, "write"));
		}
		ProtocolToken token = new ProtocolToken
		{
			RentBuffer = true
		};
		try
		{
			if (_buffer.ActiveLength > 0)
			{
				throw new InvalidOperationException(System.SR.net_ssl_renegotiate_buffer);
			}
			_sslAuthenticationOptions.RemoteCertRequired = true;
			_isRenego = true;
			token = Renegotiate();
			if (token.Size > 0)
			{
				await TIOAdapter.WriteAsync(InnerStream, token.AsMemory(), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				await TIOAdapter.FlushAsync(InnerStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			token.ReleasePayload();
			if (token.Status.ErrorCode != SecurityStatusPalErrorCode.OK)
			{
				if (token.Status.ErrorCode != SecurityStatusPalErrorCode.NoRenegotiation)
				{
					throw SslStreamPal.GetException(token.Status);
				}
				return;
			}
			do
			{
				token = ProcessTlsFrame(await ReceiveHandshakeFrameAsync<TIOAdapter>(cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
				if (token.Size > 0)
				{
					await TIOAdapter.WriteAsync(InnerStream, token.AsMemory(), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					await TIOAdapter.FlushAsync(InnerStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				token.ReleasePayload();
			}
			while (token.Status.ErrorCode == SecurityStatusPalErrorCode.ContinueNeeded);
			CompleteHandshake(_sslAuthenticationOptions);
		}
		finally
		{
			if (_buffer.ActiveLength == 0)
			{
				_buffer.ReturnBuffer();
			}
			token.ReleasePayload();
			_nestedRead = NestedState.StreamNotInUse;
			_nestedWrite = NestedState.StreamNotInUse;
			_isRenego = false;
		}
	}

	private async Task ForceAuthenticationAsync<TIOAdapter>(bool receiveFirst, byte[] reAuthenticationData, CancellationToken cancellationToken) where TIOAdapter : IReadWriteAdapter
	{
		bool handshakeCompleted = false;
		ProtocolToken token = new ProtocolToken
		{
			RentBuffer = true
		};
		if (reAuthenticationData == null && Interlocked.Exchange(ref _nestedAuth, NestedState.StreamInUse) == NestedState.StreamInUse)
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.net_io_invalidnestedcall, "authenticate"));
		}
		try
		{
			if (!receiveFirst)
			{
				token = NextMessage(reAuthenticationData, out var _);
				if (token.Size > 0)
				{
					await TIOAdapter.WriteAsync(InnerStream, new ReadOnlyMemory<byte>(token.Payload, 0, token.Size), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					await TIOAdapter.FlushAsync(InnerStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						System.Net.NetEventSource.Log.SentFrame(this, token.Payload);
					}
				}
				token.ReleasePayload();
				if (token.Failed)
				{
					throw new AuthenticationException(System.SR.net_auth_SSPI, token.GetException());
				}
				if (token.Status.ErrorCode == SecurityStatusPalErrorCode.OK)
				{
					handshakeCompleted = true;
				}
			}
			if (!handshakeCompleted)
			{
				_buffer.EnsureAvailableSpace(4160);
			}
			while (!handshakeCompleted)
			{
				token = ProcessTlsFrame(await ReceiveHandshakeFrameAsync<TIOAdapter>(cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
				ReadOnlyMemory<byte> payload = default(ReadOnlyMemory<byte>);
				if (token.Size > 0)
				{
					payload = token.AsMemory();
				}
				else if (token.Failed && (_lastFrame.Header.Type == TlsContentType.Handshake || _lastFrame.Header.Type == TlsContentType.ChangeCipherSpec))
				{
					payload = TlsFrameHelper.CreateAlertFrame(_lastFrame.Header.Version, TlsAlertDescription.ProtocolVersion);
				}
				if (!payload.IsEmpty)
				{
					await TIOAdapter.WriteAsync(InnerStream, payload, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					await TIOAdapter.FlushAsync(InnerStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						System.Net.NetEventSource.Log.SentFrame(this, payload.Span);
					}
				}
				token.ReleasePayload();
				if (token.Failed)
				{
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						System.Net.NetEventSource.Error(this, token.Status, "ForceAuthenticationAsync");
					}
					if (_lastFrame.Header.Type == TlsContentType.Alert && _lastFrame.AlertDescription != TlsAlertDescription.CloseNotify && token.Status.ErrorCode == SecurityStatusPalErrorCode.IllegalMessage)
					{
						throw new AuthenticationException(System.SR.Format(System.SR.net_auth_tls_alert, _lastFrame.AlertDescription.ToString()), token.GetException());
					}
					throw new AuthenticationException(System.SR.net_auth_SSPI, token.GetException());
				}
				if (token.Status.ErrorCode == SecurityStatusPalErrorCode.OK)
				{
					handshakeCompleted = true;
				}
			}
			CompleteHandshake(_sslAuthenticationOptions);
		}
		finally
		{
			if (reAuthenticationData == null)
			{
				_nestedAuth = NestedState.StreamNotInUse;
				_isRenego = false;
			}
			token.ReleasePayload();
			_localClientCertificateUsed = -1;
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.SspiSelectedCipherSuite("ForceAuthenticationAsync", SslProtocol, CipherAlgorithm, CipherStrength, HashAlgorithm, HashStrength, KeyExchangeAlgorithm, KeyExchangeStrength);
		}
	}

	private async ValueTask<int> ReceiveHandshakeFrameAsync<TIOAdapter>(CancellationToken cancellationToken) where TIOAdapter : IReadWriteAdapter
	{
		int frameSize = await EnsureFullTlsFrameAsync<TIOAdapter>(cancellationToken, 4160).ConfigureAwait(continueOnCapturedContext: false);
		if (frameSize == 0)
		{
			throw new IOException(System.SR.net_io_eof);
		}
		switch (_lastFrame.Header.Type)
		{
		case TlsContentType.Alert:
			if (TlsFrameHelper.TryGetFrameInfo(_buffer.EncryptedReadOnlySpan, ref _lastFrame) && System.Net.NetEventSource.Log.IsEnabled() && _lastFrame.AlertDescription != TlsAlertDescription.CloseNotify)
			{
				System.Net.NetEventSource.Error(this, $"Received TLS alert {_lastFrame.AlertDescription}", "ReceiveHandshakeFrameAsync");
			}
			break;
		case TlsContentType.Handshake:
		{
			if (_isRenego || _buffer.EncryptedReadOnlySpan[(_lastFrame.Header.Version == SslProtocols.Ssl2) ? 2 : 5] != 1 || !_sslAuthenticationOptions.IsServer)
			{
				break;
			}
			TlsFrameHelper.ProcessingOptions processingOptions = ((!System.Net.NetEventSource.Log.IsEnabled()) ? TlsFrameHelper.ProcessingOptions.ServerName : TlsFrameHelper.ProcessingOptions.All);
			if (OperatingSystem.IsMacOS() && _sslAuthenticationOptions.IsServer)
			{
				processingOptions |= TlsFrameHelper.ProcessingOptions.RawApplicationProtocol;
			}
			if (!TlsFrameHelper.TryGetFrameInfo(_buffer.EncryptedReadOnlySpan, ref _lastFrame, processingOptions) && System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(this, $"Failed to parse TLS hello.", "ReceiveHandshakeFrameAsync");
			}
			if (_lastFrame.HandshakeType == TlsHandshakeType.ClientHello)
			{
				if (_lastFrame.TargetName != null)
				{
					_sslAuthenticationOptions.TargetHost = _lastFrame.TargetName;
				}
				if (_sslAuthenticationOptions.ServerOptionDelegate != null)
				{
					SslServerAuthenticationOptions sslServerAuthenticationOptions = await _sslAuthenticationOptions.ServerOptionDelegate(this, new SslClientHelloInfo(_sslAuthenticationOptions.TargetHost, _lastFrame.SupportedVersions), _sslAuthenticationOptions.UserState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					_sslAuthenticationOptions.UpdateOptions(sslServerAuthenticationOptions);
				}
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Log.ReceivedFrame(this, _lastFrame);
			}
			break;
		}
		case TlsContentType.AppData:
			if (_isRenego && SslProtocol != SslProtocols.Tls13)
			{
				throw new InvalidOperationException(System.SR.net_ssl_renegotiate_data);
			}
			break;
		}
		return frameSize;
	}

	private ProtocolToken ProcessTlsFrame(int frameSize)
	{
		int i = frameSize;
		ReadOnlySpan<byte> encryptedReadOnlySpan;
		for (encryptedReadOnlySpan = _buffer.EncryptedReadOnlySpan; encryptedReadOnlySpan.Length - i > 5; i += frameSize)
		{
			TlsFrameHeader header = default(TlsFrameHeader);
			if (!TlsFrameHelper.TryGetFrameHeader(encryptedReadOnlySpan.Slice(i), ref header))
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(this, "invalid TLS frame size", "ProcessTlsFrame");
				}
				throw new AuthenticationException(System.SR.net_frame_read_size);
			}
			frameSize = header.Length;
			if ((header.Type != TlsContentType.Handshake && header.Type != TlsContentType.ChangeCipherSpec && !_isRenego) || frameSize > encryptedReadOnlySpan.Length - i)
			{
				break;
			}
		}
		ProtocolToken result = NextMessage(encryptedReadOnlySpan.Slice(0, i), out var consumed);
		_buffer.DiscardEncrypted(consumed);
		return result;
	}

	private void SendAuthResetSignal(ReadOnlySpan<byte> alert, ExceptionDispatchInfo exception)
	{
		SetException(exception.SourceException);
		if (alert.Length == 0)
		{
			exception.Throw();
		}
		InnerStream.Write(alert);
		exception.Throw();
	}

	private bool CompleteHandshake(ref ProtocolToken alertToken, out SslPolicyErrors sslPolicyErrors, out X509ChainStatusFlags chainStatus)
	{
		ProcessHandshakeSuccess();
		if (_nestedAuth != NestedState.StreamInUse)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(this, $"Ignoring unsolicited renegotiated certificate.", "CompleteHandshake");
			}
			sslPolicyErrors = SslPolicyErrors.None;
			chainStatus = X509ChainStatusFlags.NoError;
			return true;
		}
		if (!VerifyRemoteCertificate(_sslAuthenticationOptions.CertValidationDelegate, _sslAuthenticationOptions.CertificateContext?.Trust, ref alertToken, out sslPolicyErrors, out chainStatus))
		{
			_handshakeCompleted = false;
			return false;
		}
		_handshakeCompleted = true;
		return true;
	}

	private void CompleteHandshake(SslAuthenticationOptions sslAuthenticationOptions)
	{
		ProtocolToken alertToken = default(ProtocolToken);
		if (!CompleteHandshake(ref alertToken, out var sslPolicyErrors, out var chainStatus))
		{
			if (sslAuthenticationOptions.CertValidationDelegate != null)
			{
				SendAuthResetSignal(new ReadOnlySpan<byte>(alertToken.Payload), ExceptionDispatchInfo.Capture(new AuthenticationException(System.SR.net_ssl_io_cert_custom_validation, null)));
			}
			else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors && chainStatus != X509ChainStatusFlags.NoError)
			{
				SendAuthResetSignal(new ReadOnlySpan<byte>(alertToken.Payload), ExceptionDispatchInfo.Capture(new AuthenticationException(System.SR.Format(System.SR.net_ssl_io_cert_chain_validation, chainStatus), null)));
			}
			else
			{
				SendAuthResetSignal(new ReadOnlySpan<byte>(alertToken.Payload), ExceptionDispatchInfo.Capture(new AuthenticationException(System.SR.Format(System.SR.net_ssl_io_cert_validation, sslPolicyErrors), null)));
			}
		}
	}

	private async ValueTask WriteAsyncChunked<TIOAdapter>(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken) where TIOAdapter : IReadWriteAdapter
	{
		do
		{
			int chunkBytes = Math.Min(buffer.Length, MaxDataSize);
			await WriteSingleChunk<TIOAdapter>(buffer.Slice(0, chunkBytes), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			buffer = buffer.Slice(chunkBytes);
		}
		while (buffer.Length != 0);
	}

	private ValueTask WriteSingleChunk<TIOAdapter>(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken) where TIOAdapter : IReadWriteAdapter
	{
		ProtocolToken token;
		while (true)
		{
			token = EncryptData(buffer);
			if (token.Status.ErrorCode != SecurityStatusPalErrorCode.TryAgain)
			{
				break;
			}
			TaskCompletionSource<bool> handshakeWaiter = _handshakeWaiter;
			if (handshakeWaiter != null)
			{
				Task task = TIOAdapter.WaitAsync(handshakeWaiter);
				if (!task.IsCompletedSuccessfully)
				{
					return WaitAndWriteAsync(buffer, task, cancellationToken);
				}
			}
		}
		if (token.Status.ErrorCode != SecurityStatusPalErrorCode.OK)
		{
			token.ReleasePayload();
			return ValueTask.FromException(ExceptionDispatchInfo.SetCurrentStackTrace(new IOException(System.SR.net_io_encrypt, SslStreamPal.GetException(token.Status))));
		}
		ValueTask valueTask = TIOAdapter.WriteAsync(InnerStream, token.AsMemory(), cancellationToken);
		if (valueTask.IsCompletedSuccessfully)
		{
			token.ReleasePayload();
			return valueTask;
		}
		return CompleteWriteAsync(valueTask, token);
		static async ValueTask CompleteWriteAsync(ValueTask writeTask, ProtocolToken protocolToken)
		{
			try
			{
				await writeTask.ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				protocolToken.ReleasePayload();
			}
		}
		async ValueTask WaitAndWriteAsync(ReadOnlyMemory<byte> buffer2, Task waitTask, CancellationToken cancellationToken2)
		{
			ProtocolToken token2 = default(ProtocolToken);
			try
			{
				await waitTask.ConfigureAwait(continueOnCapturedContext: false);
				token2 = EncryptData(buffer2);
				if (token2.Status.ErrorCode == SecurityStatusPalErrorCode.TryAgain)
				{
					await WriteSingleChunk<TIOAdapter>(buffer2, cancellationToken2).ConfigureAwait(continueOnCapturedContext: false);
				}
				else
				{
					if (token2.Status.ErrorCode != SecurityStatusPalErrorCode.OK)
					{
						throw new IOException(System.SR.net_io_encrypt, SslStreamPal.GetException(token2.Status));
					}
					await TIOAdapter.WriteAsync(InnerStream, token2.AsMemory(), cancellationToken2).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			finally
			{
				token2.ReleasePayload();
			}
		}
	}

	~SslStream()
	{
		Dispose(disposing: false);
	}

	private void ReturnReadBufferIfEmpty()
	{
		if (_buffer.ActiveLength == 0)
		{
			_buffer.ReturnBuffer();
		}
	}

	private bool HaveFullTlsFrame(out int frameSize)
	{
		frameSize = GetFrameSize(_buffer.EncryptedReadOnlySpan);
		return _buffer.EncryptedLength >= frameSize;
	}

	[AsyncStateMachine(typeof(_003CEnsureFullTlsFrameAsync_003Ed__170<>))]
	[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
	private ValueTask<int> EnsureFullTlsFrameAsync<TIOAdapter>(CancellationToken cancellationToken, int estimatedSize) where TIOAdapter : IReadWriteAdapter
	{
		Unsafe.SkipInit(out _003CEnsureFullTlsFrameAsync_003Ed__170<TIOAdapter> stateMachine);
		stateMachine._003C_003Et__builder = PoolingAsyncValueTaskMethodBuilder<int>.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.cancellationToken = cancellationToken;
		stateMachine.estimatedSize = estimatedSize;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	private SecurityStatusPal DecryptData(int frameSize)
	{
		SecurityStatusPal result;
		lock (_handshakeLock)
		{
			ThrowIfExceptionalOrNotAuthenticated();
			result = Decrypt(_buffer.EncryptedSpanSliced(frameSize), out var outputOffset, out var outputCount);
			_buffer.OnDecrypted(outputOffset, outputCount, frameSize);
			if (result.ErrorCode == SecurityStatusPalErrorCode.Renegotiate && (_sslAuthenticationOptions.AllowRenegotiation || SslProtocol == SslProtocols.Tls13 || _nestedAuth != NestedState.StreamNotInUse))
			{
				_handshakeWaiter = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			}
		}
		return result;
	}

	[AsyncStateMachine(typeof(_003CReadAsyncInternal_003Ed__172<>))]
	[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
	private ValueTask<int> ReadAsyncInternal<TIOAdapter>(Memory<byte> buffer, CancellationToken cancellationToken) where TIOAdapter : IReadWriteAdapter
	{
		Unsafe.SkipInit(out _003CReadAsyncInternal_003Ed__172<TIOAdapter> stateMachine);
		stateMachine._003C_003Et__builder = PoolingAsyncValueTaskMethodBuilder<int>.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.buffer = buffer;
		stateMachine.cancellationToken = cancellationToken;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	private async ValueTask WriteAsyncInternal<TIOAdapter>(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken) where TIOAdapter : IReadWriteAdapter
	{
		ThrowIfExceptionalOrNotAuthenticatedOrShutdown();
		_ = buffer.Length;
		if (Interlocked.Exchange(ref _nestedWrite, NestedState.StreamInUse) == NestedState.StreamInUse)
		{
			throw new NotSupportedException(System.SR.Format(System.SR.net_io_invalidnestedcall, "write"));
		}
		try
		{
			await ((buffer.Length < MaxDataSize) ? WriteSingleChunk<TIOAdapter>(buffer, cancellationToken) : WriteAsyncChunked<TIOAdapter>(buffer, cancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception ex)
		{
			if (ex is IOException || (ex is OperationCanceledException && cancellationToken.IsCancellationRequested))
			{
				throw;
			}
			throw new IOException(System.SR.net_io_write, ex);
		}
		finally
		{
			_nestedWrite = NestedState.StreamNotInUse;
		}
	}

	private int CopyDecryptedData(Memory<byte> buffer)
	{
		int num = Math.Min(_buffer.DecryptedLength, buffer.Length);
		if (num != 0)
		{
			_buffer.DecryptedReadOnlySpanSliced(num).CopyTo(buffer.Span);
			_buffer.Discard(num);
		}
		return num;
	}

	private int GetFrameSize(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < 5)
		{
			return int.MaxValue;
		}
		if (!TlsFrameHelper.TryGetFrameHeader(buffer, ref _lastFrame.Header))
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(this, "invalid TLS frame size", "GetFrameSize");
			}
			throw new AuthenticationException(System.SR.net_frame_read_size);
		}
		return _lastFrame.Header.Length;
	}

	internal ChannelBinding GetChannelBinding(ChannelBindingKind kind)
	{
		ChannelBinding result = null;
		if (_securityContext != null)
		{
			result = SslStreamPal.QueryContextChannelBinding(_securityContext, kind);
		}
		return result;
	}

	internal void CloseContext()
	{
		if (!_remoteCertificateExposed)
		{
			_remoteCertificate?.Dispose();
			_remoteCertificate = null;
		}
		_securityContext?.Dispose();
		_credentialsHandle?.Dispose();
		_sslAuthenticationOptions.Dispose();
	}

	internal static X509Certificate2 FindCertificateWithPrivateKey(object instance, bool isServer, X509Certificate certificate)
	{
		if (certificate == null)
		{
			return null;
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.LocatingPrivateKey(certificate, instance);
		}
		try
		{
			X509Certificate2 x509Certificate = MakeEx(certificate);
			if (x509Certificate == null)
			{
				return null;
			}
			if (x509Certificate.HasPrivateKey)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Log.CertIsType2(instance);
				}
				return x509Certificate;
			}
			Span<byte> span = stackalloc byte[64];
			x509Certificate.TryGetCertHash(HashAlgorithmName.SHA512, span, out var _);
			if (certificate != x509Certificate)
			{
				x509Certificate.Dispose();
			}
			X509Certificate2 x509Certificate2 = FindCertWithPrivateKey(isServer, span) ?? FindCertWithPrivateKey(!isServer, span);
			if (x509Certificate2 != null)
			{
				return x509Certificate2;
			}
		}
		catch (CryptographicException)
		{
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.NotFoundCertInStore(instance);
		}
		return null;
		X509Certificate2 FindCertWithPrivateKey(bool flag, ReadOnlySpan<byte> certHash)
		{
			X509Store x509Store = CertificateValidationPal.EnsureStoreOpened(flag);
			if (x509Store != null)
			{
				X509Certificate2Collection certificates = x509Store.Certificates;
				X509Certificate2Collection x509Certificate2Collection = certificates.FindByThumbprint(HashAlgorithmName.SHA512, certHash);
				X509Certificate2 x509Certificate3 = null;
				try
				{
					if (x509Certificate2Collection.Count > 0)
					{
						x509Certificate3 = x509Certificate2Collection[0];
						if (x509Certificate3.HasPrivateKey)
						{
							if (System.Net.NetEventSource.Log.IsEnabled())
							{
								System.Net.NetEventSource.Log.FoundCertInStore(flag, instance);
							}
							return x509Certificate3;
						}
					}
				}
				finally
				{
					for (int i = 0; i < certificates.Count; i++)
					{
						X509Certificate2 x509Certificate4 = certificates[i];
						if (x509Certificate4 != x509Certificate3)
						{
							x509Certificate4.Dispose();
						}
					}
				}
			}
			return null;
		}
	}

	private static X509Certificate2 MakeEx(X509Certificate certificate)
	{
		if (certificate.GetType() == typeof(X509Certificate2))
		{
			return (X509Certificate2)certificate;
		}
		X509Certificate2 result = null;
		try
		{
			if (certificate.Handle != IntPtr.Zero)
			{
				result = new X509Certificate2(certificate);
			}
		}
		catch (SecurityException)
		{
		}
		catch (CryptographicException)
		{
		}
		return result;
	}

	private string[] GetRequestCertificateAuthorities()
	{
		string[] result = Array.Empty<string>();
		if (IsValidContext)
		{
			result = CertificateValidationPal.GetRequestCertificateAuthorities(_securityContext);
		}
		return result;
	}

	internal X509Certificate2 SelectClientCertificate()
	{
		X509Certificate x509Certificate = null;
		X509Certificate2 x509Certificate2 = null;
		List<X509Certificate> list = null;
		if (_sslAuthenticationOptions.CertificateContext != null)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Log.CertificateFromCertContext(this);
			}
			_selectedClientCertificate = _sslAuthenticationOptions.CertificateContext.TargetCertificate;
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"Selected cert = {_selectedClientCertificate}", "SelectClientCertificate");
			}
			return _sslAuthenticationOptions.CertificateContext.TargetCertificate;
		}
		if (_sslAuthenticationOptions.CertSelectionDelegate != null)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, "Calling CertificateSelectionCallback", "SelectClientCertificate");
			}
			X509Certificate2 x509Certificate3 = null;
			try
			{
				string[] requestCertificateAuthorities = GetRequestCertificateAuthorities();
				x509Certificate3 = CertificateValidationPal.GetRemoteCertificate(_securityContext);
				SslAuthenticationOptions sslAuthenticationOptions = _sslAuthenticationOptions;
				if (sslAuthenticationOptions.ClientCertificates == null)
				{
					X509CertificateCollection x509CertificateCollection = (sslAuthenticationOptions.ClientCertificates = new X509CertificateCollection());
				}
				x509Certificate = _sslAuthenticationOptions.CertSelectionDelegate(this, _sslAuthenticationOptions.TargetHost, _sslAuthenticationOptions.ClientCertificates, x509Certificate3, requestCertificateAuthorities);
			}
			finally
			{
				x509Certificate3?.Dispose();
			}
			if (x509Certificate != null)
			{
				EnsureInitialized(ref list).Add(x509Certificate);
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Log.CertificateFromDelegate(this);
				}
			}
			else if (_sslAuthenticationOptions.ClientCertificates == null || _sslAuthenticationOptions.ClientCertificates.Count == 0)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Log.NoDelegateNoClientCert(this);
				}
			}
			else if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Log.NoDelegateButClientCert(this);
			}
		}
		else if (_credentialsHandle == null && _sslAuthenticationOptions.ClientCertificates != null && _sslAuthenticationOptions.ClientCertificates.Count > 0)
		{
			x509Certificate = _sslAuthenticationOptions.ClientCertificates[0];
			if (x509Certificate != null)
			{
				EnsureInitialized(ref list).Add(x509Certificate);
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Log.AttemptingRestartUsingCert(x509Certificate, this);
			}
		}
		else if (_sslAuthenticationOptions.ClientCertificates != null && _sslAuthenticationOptions.ClientCertificates.Count > 0)
		{
			string[] requestCertificateAuthorities = GetRequestCertificateAuthorities();
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				if (requestCertificateAuthorities == null || requestCertificateAuthorities.Length == 0)
				{
					System.Net.NetEventSource.Log.NoIssuersTryAllCerts(this);
				}
				else
				{
					System.Net.NetEventSource.Log.LookForMatchingCerts(requestCertificateAuthorities.Length, this);
				}
			}
			for (int i = 0; i < _sslAuthenticationOptions.ClientCertificates.Count; i++)
			{
				if (requestCertificateAuthorities != null && requestCertificateAuthorities.Length != 0)
				{
					X509Certificate2 x509Certificate4 = null;
					X509Chain x509Chain = null;
					try
					{
						x509Certificate4 = MakeEx(_sslAuthenticationOptions.ClientCertificates[i]);
						if (x509Certificate4 == null)
						{
							continue;
						}
						if (System.Net.NetEventSource.Log.IsEnabled())
						{
							System.Net.NetEventSource.Info(this, $"Root cert: {x509Certificate4}", "SelectClientCertificate");
						}
						x509Chain = new X509Chain();
						x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
						x509Chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreInvalidName;
						x509Chain.Build(x509Certificate4);
						bool flag = false;
						if (x509Chain.ChainElements.Count > 0)
						{
							int count = x509Chain.ChainElements.Count;
							for (int j = 0; j < count; j++)
							{
								string issuer = x509Chain.ChainElements[j].Certificate.Issuer;
								flag = Array.IndexOf(requestCertificateAuthorities, issuer) != -1;
								if (flag)
								{
									if (System.Net.NetEventSource.Log.IsEnabled())
									{
										System.Net.NetEventSource.Info(this, $"Matched {issuer}", "SelectClientCertificate");
									}
									break;
								}
								if (System.Net.NetEventSource.Log.IsEnabled())
								{
									System.Net.NetEventSource.Info(this, $"No match: {issuer}", "SelectClientCertificate");
								}
							}
						}
						if (!flag)
						{
							continue;
						}
						goto IL_0434;
					}
					finally
					{
						if (x509Chain != null)
						{
							int count2 = x509Chain.ChainElements.Count;
							for (int k = 0; k < count2; k++)
							{
								x509Chain.ChainElements[k].Certificate.Dispose();
							}
							x509Chain.Dispose();
						}
						if (x509Certificate4 != null && x509Certificate4 != _sslAuthenticationOptions.ClientCertificates[i])
						{
							x509Certificate4.Dispose();
						}
					}
				}
				goto IL_0434;
				IL_0434:
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Log.SelectedCert(_sslAuthenticationOptions.ClientCertificates[i], this);
				}
				EnsureInitialized(ref list).Add(_sslAuthenticationOptions.ClientCertificates[i]);
			}
		}
		x509Certificate = null;
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			if (list != null && list.Count != 0)
			{
				System.Net.NetEventSource.Log.CertsAfterFiltering(list.Count, this);
				System.Net.NetEventSource.Log.FindingMatchingCerts(this);
			}
			else
			{
				System.Net.NetEventSource.Log.CertsAfterFiltering(0, this);
				System.Net.NetEventSource.Info(this, "No client certificate to choose from", "SelectClientCertificate");
			}
		}
		if (list != null)
		{
			for (int l = 0; l < list.Count; l++)
			{
				x509Certificate = list[l];
				if ((x509Certificate2 = FindCertificateWithPrivateKey(this, _sslAuthenticationOptions.IsServer, x509Certificate)) != null)
				{
					break;
				}
				x509Certificate = null;
				x509Certificate2 = null;
			}
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, $"Selected cert = {x509Certificate2}", "SelectClientCertificate");
		}
		_selectedClientCertificate = x509Certificate;
		return x509Certificate2;
	}

	internal bool AcquireClientCredentials(ref byte[] thumbPrint, bool newCredentialsRequested = false)
	{
		bool result = false;
		X509Certificate2 x509Certificate = SelectClientCertificate();
		if (newCredentialsRequested)
		{
			UpdateCertificateContext(x509Certificate);
			if (!SslStreamPal.TryUpdateClintCertificate(_credentialsHandle, _securityContext, _sslAuthenticationOptions))
			{
			}
		}
		try
		{
			byte[] array = x509Certificate?.GetCertHash(HashAlgorithmName.SHA512);
			SafeFreeCredentials safeFreeCredentials = SslSessionsCache.TryCachedCredential(array, _sslAuthenticationOptions.EnabledSslProtocols, _sslAuthenticationOptions.IsServer, _sslAuthenticationOptions.EncryptionPolicy, _sslAuthenticationOptions.CertificateRevocationCheckMode != X509RevocationMode.NoCheck, _sslAuthenticationOptions.AllowTlsResume, sendTrustList: false, _sslAuthenticationOptions.AllowRsaPssPadding, _sslAuthenticationOptions.AllowRsaPkcs1Padding);
			if (!newCredentialsRequested && safeFreeCredentials == null && x509Certificate != null)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Info(this, "Reset to anonymous session.", "AcquireClientCredentials");
				}
				if (_selectedClientCertificate != x509Certificate)
				{
					x509Certificate.Dispose();
				}
				array = null;
				x509Certificate = null;
				_selectedClientCertificate = null;
			}
			if (safeFreeCredentials != null)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Log.UsingCachedCredential(this);
				}
				_credentialsHandle = safeFreeCredentials;
				result = true;
				UpdateCertificateContext(x509Certificate);
			}
			else
			{
				UpdateCertificateContext(x509Certificate);
				_credentialsHandle = AcquireCredentialsHandle(_sslAuthenticationOptions, newCredentialsRequested);
				thumbPrint = array;
			}
		}
		finally
		{
			UpdateCertificateContext(x509Certificate);
		}
		return result;
		void UpdateCertificateContext(X509Certificate2 cert)
		{
			if (cert != null && _sslAuthenticationOptions.CertificateContext == null)
			{
				_sslAuthenticationOptions.SetCertificateContextFromCert(cert);
			}
		}
	}

	private static List<T> EnsureInitialized<T>(ref List<T> list)
	{
		return list ?? (list = new List<T>());
	}

	private bool AcquireServerCredentials(ref byte[] thumbPrint)
	{
		X509Certificate x509Certificate = null;
		X509Certificate2 x509Certificate2 = null;
		bool result = false;
		if (_sslAuthenticationOptions.ServerCertSelectionDelegate != null)
		{
			x509Certificate = _sslAuthenticationOptions.ServerCertSelectionDelegate(this, _sslAuthenticationOptions.TargetHost);
			if (x509Certificate == null)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(this, $"ServerCertSelectionDelegate returned no certificate for '{_sslAuthenticationOptions.TargetHost}'.", "AcquireServerCredentials");
				}
				throw new AuthenticationException(System.SR.net_ssl_io_no_server_cert);
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, "ServerCertSelectionDelegate selected Cert", "AcquireServerCredentials");
			}
		}
		else if (_sslAuthenticationOptions.CertSelectionDelegate != null)
		{
			X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
			x509CertificateCollection.Add(_sslAuthenticationOptions.CertificateContext.TargetCertificate);
			x509Certificate = _sslAuthenticationOptions.CertSelectionDelegate(this, string.Empty, x509CertificateCollection, null, Array.Empty<string>());
			if (x509Certificate == null)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(this, $"CertSelectionDelegate returned no certificaete for '{_sslAuthenticationOptions.TargetHost}'.", "AcquireServerCredentials");
				}
				throw new NotSupportedException(System.SR.net_ssl_io_no_server_cert);
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, "CertSelectionDelegate selected Cert", "AcquireServerCredentials");
			}
		}
		else if (_sslAuthenticationOptions.CertificateContext != null)
		{
			x509Certificate2 = _sslAuthenticationOptions.CertificateContext.TargetCertificate;
		}
		if (x509Certificate2 == null)
		{
			if (x509Certificate == null)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(this, "Certiticate callback returned no certificaete.", "AcquireServerCredentials");
				}
				throw new NotSupportedException(System.SR.net_ssl_io_no_server_cert);
			}
			x509Certificate2 = FindCertificateWithPrivateKey(this, _sslAuthenticationOptions.IsServer, x509Certificate);
			if (x509Certificate2 == null)
			{
				throw new NotSupportedException(System.SR.net_ssl_io_no_server_cert);
			}
			_sslAuthenticationOptions.SetCertificateContextFromCert(x509Certificate2);
		}
		byte[] certHash = x509Certificate2.GetCertHash(HashAlgorithmName.SHA512);
		bool sendTrustList = _sslAuthenticationOptions.CertificateContext.Trust?._sendTrustInHandshake ?? false;
		SafeFreeCredentials safeFreeCredentials = SslSessionsCache.TryCachedCredential(certHash, _sslAuthenticationOptions.EnabledSslProtocols, _sslAuthenticationOptions.IsServer, _sslAuthenticationOptions.EncryptionPolicy, _sslAuthenticationOptions.CertificateRevocationCheckMode != X509RevocationMode.NoCheck, _sslAuthenticationOptions.AllowTlsResume, sendTrustList, _sslAuthenticationOptions.AllowRsaPssPadding, _sslAuthenticationOptions.AllowRsaPkcs1Padding);
		if (safeFreeCredentials != null)
		{
			_credentialsHandle = safeFreeCredentials;
			result = true;
		}
		else
		{
			_credentialsHandle = AcquireCredentialsHandle(_sslAuthenticationOptions);
			thumbPrint = certHash;
		}
		return result;
	}

	private static SafeFreeCredentials AcquireCredentialsHandle(SslAuthenticationOptions sslAuthenticationOptions, bool newCredentialsRequested = false)
	{
		SafeFreeCredentials safeFreeCredentials = SslStreamPal.AcquireCredentialsHandle(sslAuthenticationOptions, newCredentialsRequested);
		if (sslAuthenticationOptions.CertificateContext != null && safeFreeCredentials != null)
		{
			SslStreamCertificateContext certificateContext = sslAuthenticationOptions.CertificateContext;
			safeFreeCredentials._expiry = GetExpiryTimestamp(certificateContext);
			if (safeFreeCredentials._expiry < DateTime.UtcNow)
			{
				certificateContext = certificateContext.Duplicate();
				safeFreeCredentials._expiry = GetExpiryTimestamp(certificateContext);
			}
		}
		return safeFreeCredentials;
		static DateTime GetExpiryTimestamp(SslStreamCertificateContext sslStreamCertificateContext)
		{
			DateTime notAfter = sslStreamCertificateContext.TargetCertificate.NotAfter;
			foreach (X509Certificate2 intermediateCertificate in sslStreamCertificateContext.IntermediateCertificates)
			{
				if (intermediateCertificate.NotAfter < notAfter)
				{
					notAfter = intermediateCertificate.NotAfter;
				}
			}
			return notAfter.ToUniversalTime();
		}
	}

	internal ProtocolToken NextMessage(ReadOnlySpan<byte> incomingBuffer, out int consumed)
	{
		ProtocolToken result = GenerateToken(incomingBuffer, out consumed);
		if (System.Net.NetEventSource.Log.IsEnabled() && result.Failed)
		{
			System.Net.NetEventSource.Error(this, $"Authentication failed. Status: {result.Status}, Exception message: {result.GetException().Message}", "NextMessage");
		}
		return result;
	}

	private ProtocolToken GenerateToken(ReadOnlySpan<byte> inputBuffer, out int consumed)
	{
		bool flag = false;
		bool sendTrustList = false;
		byte[] thumbPrint = null;
		ProtocolToken result = new ProtocolToken
		{
			RentBuffer = true
		};
		bool flag2 = _securityContext == null;
		try
		{
			do
			{
				thumbPrint = null;
				if (flag2)
				{
					flag = (_sslAuthenticationOptions.IsServer ? AcquireServerCredentials(ref thumbPrint) : AcquireClientCredentials(ref thumbPrint));
				}
				int consumed2;
				if (_sslAuthenticationOptions.IsServer)
				{
					sendTrustList = _sslAuthenticationOptions.CertificateContext?.Trust?._sendTrustInHandshake == true;
					result = SslStreamPal.AcceptSecurityContext(ref _credentialsHandle, ref _securityContext, inputBuffer, out consumed, _sslAuthenticationOptions);
					if (result.Status.ErrorCode == SecurityStatusPalErrorCode.HandshakeStarted)
					{
						result.Status = SslStreamPal.SelectApplicationProtocol(_credentialsHandle, _securityContext, _sslAuthenticationOptions, _lastFrame.RawApplicationProtocols);
						if (result.Status.ErrorCode == SecurityStatusPalErrorCode.OK)
						{
							result = SslStreamPal.AcceptSecurityContext(ref _credentialsHandle, ref _securityContext, ReadOnlySpan<byte>.Empty, out consumed2, _sslAuthenticationOptions);
						}
					}
					continue;
				}
				string targetName = TargetHostNameHelper.NormalizeHostName(_sslAuthenticationOptions.TargetHost);
				result = SslStreamPal.InitializeSecurityContext(ref _credentialsHandle, ref _securityContext, targetName, inputBuffer, out consumed, _sslAuthenticationOptions);
				if (result.Status.ErrorCode == SecurityStatusPalErrorCode.CredentialsNeeded)
				{
					flag2 = true;
					flag = AcquireClientCredentials(ref thumbPrint, newCredentialsRequested: true);
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						System.Net.NetEventSource.Info(this, "InitializeSecurityContext() returned 'CredentialsNeeded'.", "GenerateToken");
					}
					result = SslStreamPal.InitializeSecurityContext(ref _credentialsHandle, ref _securityContext, targetName, ReadOnlySpan<byte>.Empty, out consumed2, _sslAuthenticationOptions);
				}
			}
			while (flag && _credentialsHandle == null);
		}
		finally
		{
			if (flag2)
			{
				_credentialsHandle?.Dispose();
				if (!flag && _securityContext != null && !_securityContext.IsInvalid && _credentialsHandle != null && !_credentialsHandle.IsInvalid)
				{
					SslSessionsCache.CacheCredential(_credentialsHandle, thumbPrint, _sslAuthenticationOptions.EnabledSslProtocols, _sslAuthenticationOptions.IsServer, _sslAuthenticationOptions.EncryptionPolicy, _sslAuthenticationOptions.CertificateRevocationCheckMode != X509RevocationMode.NoCheck, _sslAuthenticationOptions.AllowTlsResume, sendTrustList, _sslAuthenticationOptions.AllowRsaPssPadding, _sslAuthenticationOptions.AllowRsaPkcs1Padding);
				}
			}
		}
		return result;
	}

	internal ProtocolToken Renegotiate()
	{
		return SslStreamPal.Renegotiate(ref _credentialsHandle, ref _securityContext, _sslAuthenticationOptions);
	}

	internal void ProcessHandshakeSuccess()
	{
		SslStreamPal.QueryContextStreamSizes(_securityContext, out var streamSizes);
		_headerSize = streamSizes.Header;
		_trailerSize = streamSizes.Trailer;
		_maxDataSize = streamSizes.MaximumMessage;
		SslStreamPal.QueryContextConnectionInfo(_securityContext, ref _connectionInfo);
	}

	internal ProtocolToken Encrypt(ReadOnlyMemory<byte> buffer)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.DumpBuffer(this, buffer.Span, "Encrypt");
		}
		ProtocolToken result = SslStreamPal.EncryptMessage(_securityContext, buffer, _headerSize, _trailerSize);
		if (result.Status.ErrorCode != SecurityStatusPalErrorCode.OK && System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Error(this, $"ERROR {result.Status}", "Encrypt");
		}
		return result;
	}

	internal SecurityStatusPal Decrypt(Span<byte> buffer, out int outputOffset, out int outputCount)
	{
		SecurityStatusPal result = SslStreamPal.DecryptMessage(_securityContext, buffer, out outputOffset, out outputCount);
		if (System.Net.NetEventSource.Log.IsEnabled() && result.ErrorCode == SecurityStatusPalErrorCode.OK)
		{
			System.Net.NetEventSource.DumpBuffer(this, buffer.Slice(outputOffset, outputCount), "Decrypt");
		}
		return result;
	}

	internal bool VerifyRemoteCertificate(RemoteCertificateValidationCallback remoteCertValidationCallback, SslCertificateTrust trust, ref ProtocolToken alertToken, out SslPolicyErrors sslPolicyErrors, out X509ChainStatusFlags chainStatus)
	{
		sslPolicyErrors = SslPolicyErrors.None;
		chainStatus = X509ChainStatusFlags.NoError;
		bool flag = false;
		X509Chain chain = null;
		int valueOrDefault = (_sslAuthenticationOptions.CertificateChainPolicy?.ExtraStore?.Count).GetValueOrDefault();
		try
		{
			X509Certificate2 remoteCertificate = CertificateValidationPal.GetRemoteCertificate(_securityContext, ref chain, _sslAuthenticationOptions.CertificateChainPolicy);
			if (_remoteCertificate != null && remoteCertificate != null)
			{
				ReadOnlyMemory<byte> rawDataMemory = remoteCertificate.RawDataMemory;
				ReadOnlySpan<byte> span = rawDataMemory.Span;
				rawDataMemory = _remoteCertificate.RawDataMemory;
				if (span.SequenceEqual(rawDataMemory.Span))
				{
					remoteCertificate.Dispose();
					return true;
				}
			}
			if (remoteCertificate == null)
			{
				if (System.Net.NetEventSource.Log.IsEnabled() && RemoteCertRequired)
				{
					System.Net.NetEventSource.Error(this, $"Remote certificate required, but no remote certificate received", "VerifyRemoteCertificate");
				}
				sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNotAvailable;
			}
			else
			{
				if (chain == null)
				{
					chain = new X509Chain();
				}
				if (_sslAuthenticationOptions.CertificateChainPolicy != null)
				{
					chain.ChainPolicy = _sslAuthenticationOptions.CertificateChainPolicy;
				}
				else
				{
					chain.ChainPolicy.RevocationMode = _sslAuthenticationOptions.CertificateRevocationCheckMode;
					chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
					if (trust != null)
					{
						chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
						if (trust._store != null)
						{
							chain.ChainPolicy.CustomTrustStore.AddRange(trust._store.Certificates);
						}
						if (trust._trustList != null)
						{
							chain.ChainPolicy.CustomTrustStore.AddRange(trust._trustList);
						}
					}
				}
				if (chain.ChainPolicy.ApplicationPolicy.Count == 0)
				{
					chain.ChainPolicy.ApplicationPolicy.Add(_sslAuthenticationOptions.IsServer ? s_clientAuthOid : s_serverAuthOid);
				}
				sslPolicyErrors |= CertificateValidationPal.VerifyCertificateProperties(_securityContext, chain, remoteCertificate, _sslAuthenticationOptions.CheckCertName, _sslAuthenticationOptions.IsServer, TargetHostNameHelper.NormalizeHostName(_sslAuthenticationOptions.TargetHost));
			}
			_remoteCertificate = remoteCertificate;
			if (remoteCertValidationCallback != null)
			{
				flag = remoteCertValidationCallback(this, remoteCertificate, chain, sslPolicyErrors);
			}
			else
			{
				if (!RemoteCertRequired)
				{
					sslPolicyErrors &= ~SslPolicyErrors.RemoteCertificateNotAvailable;
				}
				flag = sslPolicyErrors == SslPolicyErrors.None;
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				LogCertificateValidation(remoteCertValidationCallback, sslPolicyErrors, flag, chain);
				System.Net.NetEventSource.Info(this, $"Cert validation, remote cert = {_remoteCertificate}", "VerifyRemoteCertificate");
			}
			if (!flag)
			{
				CreateFatalHandshakeAlertToken(sslPolicyErrors, chain, ref alertToken);
				if (chain != null)
				{
					X509ChainStatus[] chainStatus2 = chain.ChainStatus;
					foreach (X509ChainStatus x509ChainStatus in chainStatus2)
					{
						chainStatus |= x509ChainStatus.Status;
					}
				}
			}
		}
		finally
		{
			if (chain != null)
			{
				if (remoteCertValidationCallback == null)
				{
					for (int j = valueOrDefault; j < chain.ChainPolicy.ExtraStore.Count; j++)
					{
						chain.ChainPolicy.ExtraStore[j].Dispose();
					}
					int count = chain.ChainElements.Count;
					for (int k = 0; k < count; k++)
					{
						chain.ChainElements[k].Certificate.Dispose();
					}
				}
				chain.Dispose();
			}
		}
		return flag;
	}

	private void CreateFatalHandshakeAlertToken(SslPolicyErrors sslPolicyErrors, X509Chain chain, ref ProtocolToken alertToken)
	{
		TlsAlertMessage tlsAlertMessage = sslPolicyErrors switch
		{
			SslPolicyErrors.RemoteCertificateChainErrors => GetAlertMessageFromChain(chain), 
			SslPolicyErrors.RemoteCertificateNameMismatch => TlsAlertMessage.BadCertificate, 
			_ => TlsAlertMessage.CertificateUnknown, 
		};
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, $"alertMessage:{tlsAlertMessage}", "CreateFatalHandshakeAlertToken");
		}
		SecurityStatusPal securityStatusPal = SslStreamPal.ApplyAlertToken(_securityContext, TlsAlertType.Fatal, tlsAlertMessage);
		if (securityStatusPal.ErrorCode != SecurityStatusPalErrorCode.OK)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"ApplyAlertToken() returned {securityStatusPal.ErrorCode}", "CreateFatalHandshakeAlertToken");
			}
			if (securityStatusPal.Exception != null)
			{
				ExceptionDispatchInfo.Throw(securityStatusPal.Exception);
			}
		}
		alertToken = GenerateAlertToken();
	}

	private ProtocolToken CreateShutdownToken()
	{
		SecurityStatusPal securityStatusPal = SslStreamPal.ApplyShutdownToken(_securityContext);
		if (securityStatusPal.ErrorCode != SecurityStatusPalErrorCode.OK)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"ApplyAlertToken() returned {securityStatusPal.ErrorCode}", "CreateShutdownToken");
			}
			if (securityStatusPal.Exception != null)
			{
				ExceptionDispatchInfo.Throw(securityStatusPal.Exception);
			}
			return default(ProtocolToken);
		}
		int consumed;
		return GenerateToken(default(ReadOnlySpan<byte>), out consumed);
	}

	private ProtocolToken GenerateAlertToken()
	{
		int consumed;
		return GenerateToken(default(ReadOnlySpan<byte>), out consumed);
	}

	private static TlsAlertMessage GetAlertMessageFromChain(X509Chain chain)
	{
		X509ChainStatus[] chainStatus = chain.ChainStatus;
		for (int i = 0; i < chainStatus.Length; i++)
		{
			X509ChainStatus x509ChainStatus = chainStatus[i];
			if (x509ChainStatus.Status != X509ChainStatusFlags.NoError)
			{
				if ((x509ChainStatus.Status & (X509ChainStatusFlags.UntrustedRoot | X509ChainStatusFlags.Cyclic | X509ChainStatusFlags.PartialChain)) != X509ChainStatusFlags.NoError)
				{
					return TlsAlertMessage.UnknownCA;
				}
				if ((x509ChainStatus.Status & (X509ChainStatusFlags.Revoked | X509ChainStatusFlags.OfflineRevocation)) != X509ChainStatusFlags.NoError)
				{
					return TlsAlertMessage.CertificateRevoked;
				}
				if ((x509ChainStatus.Status & (X509ChainStatusFlags.NotTimeValid | X509ChainStatusFlags.NotTimeNested | X509ChainStatusFlags.CtlNotTimeValid)) != X509ChainStatusFlags.NoError)
				{
					return TlsAlertMessage.CertificateExpired;
				}
				if ((x509ChainStatus.Status & X509ChainStatusFlags.CtlNotValidForUsage) != X509ChainStatusFlags.NoError)
				{
					return TlsAlertMessage.UnsupportedCert;
				}
				if (((x509ChainStatus.Status & (X509ChainStatusFlags.NotSignatureValid | X509ChainStatusFlags.InvalidExtension | X509ChainStatusFlags.InvalidPolicyConstraints | X509ChainStatusFlags.CtlNotSignatureValid)) | X509ChainStatusFlags.NoIssuanceChainPolicy | X509ChainStatusFlags.NotValidForUsage) != X509ChainStatusFlags.NoError)
				{
					return TlsAlertMessage.BadCertificate;
				}
				return TlsAlertMessage.CertificateUnknown;
			}
		}
		return TlsAlertMessage.BadCertificate;
	}

	private void LogCertificateValidation(RemoteCertificateValidationCallback remoteCertValidationCallback, SslPolicyErrors sslPolicyErrors, bool success, X509Chain chain)
	{
		if (!System.Net.NetEventSource.Log.IsEnabled())
		{
			return;
		}
		if (sslPolicyErrors != SslPolicyErrors.None)
		{
			System.Net.NetEventSource.Log.RemoteCertificateError(this, System.SR.net_log_remote_cert_has_errors);
			if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNotAvailable) != SslPolicyErrors.None)
			{
				System.Net.NetEventSource.Log.RemoteCertificateError(this, System.SR.net_log_remote_cert_not_available);
			}
			if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != SslPolicyErrors.None)
			{
				System.Net.NetEventSource.Log.RemoteCertificateError(this, System.SR.net_log_remote_cert_name_mismatch);
			}
			if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != SslPolicyErrors.None)
			{
				string text = "ChainStatus: ";
				X509ChainStatus[] chainStatus = chain.ChainStatus;
				foreach (X509ChainStatus x509ChainStatus in chainStatus)
				{
					text = text + "\t" + x509ChainStatus.StatusInformation;
				}
				System.Net.NetEventSource.Log.RemoteCertificateError(this, text);
			}
		}
		if (success)
		{
			if (remoteCertValidationCallback != null)
			{
				System.Net.NetEventSource.Log.RemoteCertDeclaredValid(this);
			}
			else
			{
				System.Net.NetEventSource.Log.RemoteCertHasNoErrors(this);
			}
		}
		else if (remoteCertValidationCallback != null)
		{
			System.Net.NetEventSource.Log.RemoteCertUserDeclaredInvalid(this);
		}
	}
}
