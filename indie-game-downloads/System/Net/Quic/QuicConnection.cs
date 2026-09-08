using System.Buffers;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Quic;

namespace System.Net.Quic;

public sealed class QuicConnection : IAsyncDisposable
{
	private readonly struct SslConnectionOptions
	{
		private static readonly Oid s_serverAuthOid = new Oid("1.3.6.1.5.5.7.3.1", null);

		private static readonly Oid s_clientAuthOid = new Oid("1.3.6.1.5.5.7.3.2", null);

		private readonly QuicConnection _connection;

		private readonly bool _isClient;

		private readonly string _targetHost;

		private readonly bool _certificateRequired;

		private readonly X509RevocationMode _revocationMode;

		private readonly RemoteCertificateValidationCallback _validationCallback;

		private readonly X509ChainPolicy _certificateChainPolicy;

		internal string TargetHost => _targetHost;

		public SslConnectionOptions(QuicConnection connection, bool isClient, string targetHost, bool certificateRequired, X509RevocationMode revocationMode, RemoteCertificateValidationCallback validationCallback, X509ChainPolicy certificateChainPolicy)
		{
			_connection = connection;
			_isClient = isClient;
			_targetHost = targetHost;
			_certificateRequired = certificateRequired;
			_revocationMode = revocationMode;
			_validationCallback = validationCallback;
			_certificateChainPolicy = certificateChainPolicy;
		}

		internal unsafe async Task<bool> StartAsyncCertificateValidation(nint certificatePtr, nint chainPtr)
		{
			X509Certificate2 certificate = null;
			byte[] certDataRented = null;
			Memory<byte> certData = default(Memory<byte>);
			byte[] chainDataRented = null;
			Memory<byte> chainData = default(Memory<byte>);
			if (certificatePtr != IntPtr.Zero)
			{
				if (MsQuicApi.UsesSChannelBackend)
				{
					certificate = new X509Certificate2(certificatePtr);
				}
				else
				{
					if (((QUIC_BUFFER*)certificatePtr)->Length != 0)
					{
						certDataRented = ArrayPool<byte>.Shared.Rent((int)((QUIC_BUFFER*)certificatePtr)->Length);
						certData = certDataRented.AsMemory(0, (int)((QUIC_BUFFER*)certificatePtr)->Length);
						((QUIC_BUFFER*)certificatePtr)->Span.CopyTo(certData.Span);
					}
					if (((QUIC_BUFFER*)chainPtr)->Length != 0)
					{
						chainDataRented = ArrayPool<byte>.Shared.Rent((int)((QUIC_BUFFER*)chainPtr)->Length);
						chainData = chainDataRented.AsMemory(0, (int)((QUIC_BUFFER*)chainPtr)->Length);
						((QUIC_BUFFER*)chainPtr)->Span.CopyTo(chainData.Span);
					}
				}
			}
			if (MsQuicApi.SupportsAsyncCertValidation)
			{
				await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
			}
			QUIC_TLS_ALERT_CODES qUIC_TLS_ALERT_CODES;
			try
			{
				if (certData.Length > 0)
				{
					certificate = X509CertificateLoader.LoadCertificate(certData.Span);
				}
				qUIC_TLS_ALERT_CODES = _connection._sslConnectionOptions.ValidateCertificate(certificate, certData.Span, chainData.Span);
				_connection._remoteCertificate = certificate;
			}
			catch (Exception exception)
			{
				certificate?.Dispose();
				_connection._connectedTcs.TrySetException(exception);
				qUIC_TLS_ALERT_CODES = QUIC_TLS_ALERT_CODES.USER_CANCELED;
			}
			finally
			{
				if (certDataRented != null)
				{
					ArrayPool<byte>.Shared.Return(certDataRented);
				}
				if (chainDataRented != null)
				{
					ArrayPool<byte>.Shared.Return(chainDataRented);
				}
			}
			if (MsQuicApi.SupportsAsyncCertValidation)
			{
				int status = MsQuicApi.Api.ConnectionCertificateValidationComplete(_connection._handle, (qUIC_TLS_ALERT_CODES == QUIC_TLS_ALERT_CODES.SUCCESS) ? ((byte)1) : ((byte)0), qUIC_TLS_ALERT_CODES);
				if (MsQuic.StatusFailed(status) && System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(_connection, $"{_connection} ConnectionCertificateValidationComplete failed with {ThrowHelper.GetErrorMessageForStatus(status)}", "StartAsyncCertificateValidation");
				}
			}
			return qUIC_TLS_ALERT_CODES == QUIC_TLS_ALERT_CODES.SUCCESS;
		}

		private QUIC_TLS_ALERT_CODES ValidateCertificate(X509Certificate2 certificate, Span<byte> certData, Span<byte> chainData)
		{
			SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;
			bool flag = false;
			X509Chain x509Chain = null;
			try
			{
				if (certificate != null)
				{
					x509Chain = new X509Chain();
					if (_certificateChainPolicy != null)
					{
						x509Chain.ChainPolicy = _certificateChainPolicy;
					}
					else
					{
						x509Chain.ChainPolicy.RevocationMode = _revocationMode;
						x509Chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
					}
					if (x509Chain.ChainPolicy.ApplicationPolicy.Count == 0)
					{
						x509Chain.ChainPolicy.ApplicationPolicy.Add(_isClient ? s_serverAuthOid : s_clientAuthOid);
					}
					if (chainData.Length > 0)
					{
						X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
						x509Certificate2Collection.Import(chainData);
						x509Chain.ChainPolicy.ExtraStore.AddRange(x509Certificate2Collection);
					}
					bool checkCertName = !x509Chain.ChainPolicy.VerificationFlags.HasFlag(X509VerificationFlags.IgnoreInvalidName);
					sslPolicyErrors |= System.Net.CertificateValidation.BuildChainAndVerifyProperties(x509Chain, certificate, checkCertName, !_isClient, System.Net.Security.TargetHostNameHelper.NormalizeHostName(_targetHost), certData);
				}
				else if (_certificateRequired)
				{
					sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNotAvailable;
				}
				QUIC_TLS_ALERT_CODES result = QUIC_TLS_ALERT_CODES.SUCCESS;
				if (_validationCallback != null)
				{
					flag = true;
					if (!_validationCallback(_connection, certificate, x509Chain, sslPolicyErrors))
					{
						flag = false;
						if (_isClient)
						{
							throw new AuthenticationException(System.SR.net_quic_cert_custom_validation);
						}
						result = QUIC_TLS_ALERT_CODES.BAD_CERTIFICATE;
					}
				}
				else if (sslPolicyErrors != SslPolicyErrors.None)
				{
					if (_isClient)
					{
						throw new AuthenticationException(System.SR.Format(System.SR.net_quic_cert_chain_validation, sslPolicyErrors));
					}
					result = QUIC_TLS_ALERT_CODES.BAD_CERTIFICATE;
				}
				return result;
			}
			catch (Exception innerException)
			{
				if (flag)
				{
					throw new QuicException(QuicError.CallbackError, null, System.SR.net_quic_callback_error, innerException);
				}
				throw;
			}
			finally
			{
				if (x509Chain != null)
				{
					X509ChainElementCollection chainElements = x509Chain.ChainElements;
					for (int i = 0; i < chainElements.Count; i++)
					{
						chainElements[i].Certificate.Dispose();
					}
					x509Chain.Dispose();
				}
			}
		}
	}

	private readonly MsQuicContextSafeHandle _handle;

	private bool _disposed;

	private readonly ValueTaskSource _connectedTcs = new ValueTaskSource();

	private readonly ResettableValueTaskSource _shutdownTcs = new ResettableValueTaskSource
	{
		CancellationAction = delegate(object target)
		{
			try
			{
				if (target is QuicConnection quicConnection)
				{
					quicConnection._shutdownTcs.TrySetResult();
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}
	};

	private readonly TaskCompletionSource _connectionCloseTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

	private readonly CancellationTokenSource _shutdownTokenSource = new CancellationTokenSource();

	private readonly Channel<QuicStream> _acceptQueue = Channel.CreateUnbounded<QuicStream>(new UnboundedChannelOptions
	{
		SingleWriter = true
	});

	private SslConnectionOptions _sslConnectionOptions;

	private MsQuicSafeHandle _configuration;

	private bool _canAccept;

	private long _defaultStreamErrorCode;

	private long _defaultCloseErrorCode;

	private IPEndPoint _remoteEndPoint;

	private IPEndPoint _localEndPoint;

	private Action<QuicConnection, QuicStreamCapacityChangedArgs> _streamCapacityCallback;

	private Action<QuicStreamType> _decrementStreamCapacity;

	private int _bidirectionalStreamCapacity;

	private int _unidirectionalStreamCapacity;

	private bool _remoteCertificateExposed;

	private X509Certificate2 _remoteCertificate;

	private SslApplicationProtocol _negotiatedApplicationProtocol;

	private TlsCipherSuite _negotiatedCipherSuite;

	private SslProtocols _negotiatedSslProtocol;

	private readonly MsQuicTlsSecret _tlsSecret;

	[SupportedOSPlatformGuard("windows")]
	[SupportedOSPlatformGuard("linux")]
	[SupportedOSPlatformGuard("osx")]
	public static bool IsSupported => MsQuicApi.IsQuicSupported;

	internal CancellationToken ConnectionShutdownToken => _shutdownTokenSource.Token;

	public IPEndPoint RemoteEndPoint => _remoteEndPoint;

	public IPEndPoint LocalEndPoint => _localEndPoint;

	public string TargetHostName => _sslConnectionOptions.TargetHost;

	public X509Certificate? RemoteCertificate
	{
		get
		{
			_remoteCertificateExposed = true;
			return _remoteCertificate;
		}
	}

	public SslApplicationProtocol NegotiatedApplicationProtocol => _negotiatedApplicationProtocol;

	[CLSCompliant(false)]
	public TlsCipherSuite NegotiatedCipherSuite => _negotiatedCipherSuite;

	public SslProtocols SslProtocol => _negotiatedSslProtocol;

	public static ValueTask<QuicConnection> ConnectAsync(QuicClientConnectionOptions options, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (!IsSupported)
		{
			throw new PlatformNotSupportedException(System.SR.Format(System.SR.SystemNetQuic_PlatformNotSupported, MsQuicApi.NotSupportedReason ?? "General loading failure."));
		}
		options.Validate("options");
		return StartConnectAsync(options, cancellationToken);
		static async ValueTask<QuicConnection> StartConnectAsync(QuicClientConnectionOptions quicClientConnectionOptions, CancellationToken token)
		{
			QuicConnection connection = new QuicConnection();
			using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
			if (quicClientConnectionOptions.HandshakeTimeout != Timeout.InfiniteTimeSpan && quicClientConnectionOptions.HandshakeTimeout != TimeSpan.Zero)
			{
				linkedCts.CancelAfter(quicClientConnectionOptions.HandshakeTimeout);
			}
			try
			{
				await connection.FinishConnectAsync(quicClientConnectionOptions, linkedCts.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (OperationCanceledException)
			{
				await connection.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				token.ThrowIfCancellationRequested();
				throw new QuicException(QuicError.ConnectionTimeout, null, System.SR.Format(System.SR.net_quic_handshake_timeout, quicClientConnectionOptions.HandshakeTimeout));
			}
			catch
			{
				await connection.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				throw;
			}
			return connection;
		}
	}

	private async void OnStreamCapacityIncreased(int bidirectionalIncrement, int unidirectionalIncrement)
	{
		if (_streamCapacityCallback == null || (bidirectionalIncrement == 0 && unidirectionalIncrement == 0))
		{
			return;
		}
		await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
		try
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"{this} Signaling StreamCapacityIncreased with {bidirectionalIncrement} bidirectional increment (absolute value {_bidirectionalStreamCapacity}) and {unidirectionalIncrement} unidirectional increment (absolute value {_unidirectionalStreamCapacity}).", "OnStreamCapacityIncreased");
			}
			_streamCapacityCallback(this, new QuicStreamCapacityChangedArgs
			{
				BidirectionalIncrement = bidirectionalIncrement,
				UnidirectionalIncrement = unidirectionalIncrement
			});
		}
		catch (Exception ex)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, FormattableStringFactory.Create("{0} {1} failed with {2}.", this, "StreamCapacityCallback", ex), "OnStreamCapacityIncreased");
			}
		}
	}

	public override string ToString()
	{
		return _handle.ToString();
	}

	private unsafe QuicConnection()
	{
		GCHandle gCHandle = GCHandle.Alloc(this, GCHandleType.Weak);
		try
		{
			Unsafe.SkipInit(out QUIC_HANDLE* handle);
			ThrowHelper.ThrowIfMsQuicError(MsQuicApi.Api.ConnectionOpen(MsQuicApi.Api.Registration, (delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void*, QUIC_CONNECTION_EVENT*, int>)(&NativeCallback), (void*)GCHandle.ToIntPtr(gCHandle), &handle), "ConnectionOpen failed");
			_handle = new MsQuicContextSafeHandle(handle, gCHandle, SafeHandleType.Connection);
		}
		catch
		{
			gCHandle.Free();
			throw;
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, $"{this} New outbound connection.", ".ctor");
		}
		_decrementStreamCapacity = DecrementStreamCapacity;
		_tlsSecret = MsQuicTlsSecret.Create(_handle);
	}

	internal unsafe QuicConnection(QUIC_HANDLE* handle, QUIC_NEW_CONNECTION_INFO* info)
	{
		GCHandle gCHandle = GCHandle.Alloc(this, GCHandleType.Weak);
		try
		{
			_handle = new MsQuicContextSafeHandle(handle, gCHandle, SafeHandleType.Connection);
			delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void*, QUIC_CONNECTION_EVENT*, int> callback = &NativeCallback;
			MsQuicApi.Api.SetCallbackHandler(_handle, callback, (void*)GCHandle.ToIntPtr(gCHandle));
		}
		catch
		{
			gCHandle.Free();
			throw;
		}
		_remoteEndPoint = MsQuicHelpers.QuicAddrToIPEndPoint(info->RemoteAddress);
		_localEndPoint = MsQuicHelpers.QuicAddrToIPEndPoint(info->LocalAddress);
		_decrementStreamCapacity = DecrementStreamCapacity;
		_tlsSecret = MsQuicTlsSecret.Create(_handle);
	}

	private unsafe async ValueTask FinishConnectAsync(QuicClientConnectionOptions options, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (_connectedTcs.TryInitialize(out var valueTask, this, cancellationToken))
		{
			_canAccept = options.MaxInboundBidirectionalStreams > 0 || options.MaxInboundUnidirectionalStreams > 0;
			_defaultStreamErrorCode = options.DefaultStreamErrorCode;
			_defaultCloseErrorCode = options.DefaultCloseErrorCode;
			_streamCapacityCallback = options.StreamCapacityCallback;
			if (!options.RemoteEndPoint.TryParse(out var host, out var address, out var port))
			{
				throw new ArgumentException(System.SR.Format(System.SR.net_quic_unsupported_endpoint_type, options.RemoteEndPoint.GetType()), "options");
			}
			if (address == null)
			{
				IPAddress[] obj = await Dns.GetHostAddressesAsync(host, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				cancellationToken.ThrowIfCancellationRequested();
				if (obj.Length == 0)
				{
					throw new SocketException(11001);
				}
				address = obj[0];
			}
			QuicAddr value = new IPEndPoint(address, port).ToQuicAddr();
			MsQuicHelpers.SetMsQuicParameter(_handle, 83886082u, value);
			if (options.LocalEndPoint != null)
			{
				QuicAddr value2 = options.LocalEndPoint.ToQuicAddr();
				MsQuicHelpers.SetMsQuicParameter(_handle, 83886081u, value2);
			}
			_sslConnectionOptions = new SslConnectionOptions(this, isClient: true, options.ClientAuthenticationOptions.TargetHost ?? host ?? address.ToString(), certificateRequired: true, options.ClientAuthenticationOptions.CertificateRevocationCheckMode, options.ClientAuthenticationOptions.RemoteCertificateValidationCallback, options.ClientAuthenticationOptions.CertificateChainPolicy?.Clone());
			_configuration = MsQuicConfiguration.Create(options);
			nint num = Marshal.StringToCoTaskMemUTF8((IPAddress.IsValid(options.ClientAuthenticationOptions.TargetHost.AsSpan()) ? null : options.ClientAuthenticationOptions.TargetHost) ?? host ?? string.Empty);
			try
			{
				ThrowHelper.ThrowIfMsQuicError(MsQuicApi.Api.ConnectionStart(_handle, _configuration, (ushort)value.Family, (sbyte*)num, (ushort)port), "ConnectionStart failed");
			}
			finally
			{
				Marshal.FreeCoTaskMem(num);
			}
		}
		await valueTask.ConfigureAwait(continueOnCapturedContext: false);
	}

	internal ValueTask FinishHandshakeAsync(QuicServerConnectionOptions options, string targetHost, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (_connectedTcs.TryInitialize(out var valueTask, this, cancellationToken))
		{
			_canAccept = options.MaxInboundBidirectionalStreams > 0 || options.MaxInboundUnidirectionalStreams > 0;
			_defaultStreamErrorCode = options.DefaultStreamErrorCode;
			_defaultCloseErrorCode = options.DefaultCloseErrorCode;
			_streamCapacityCallback = options.StreamCapacityCallback;
			if (IPAddress.IsValid(targetHost.AsSpan()))
			{
				targetHost = string.Empty;
			}
			_sslConnectionOptions = new SslConnectionOptions(this, isClient: false, targetHost, options.ServerAuthenticationOptions.ClientCertificateRequired, options.ServerAuthenticationOptions.CertificateRevocationCheckMode, options.ServerAuthenticationOptions.RemoteCertificateValidationCallback, options.ServerAuthenticationOptions.CertificateChainPolicy?.Clone());
			_configuration = MsQuicConfiguration.Create(options, targetHost);
			ThrowHelper.ThrowIfMsQuicError(MsQuicApi.Api.ConnectionSetConfiguration(_handle, _configuration), "ConnectionSetConfiguration failed");
		}
		return valueTask;
	}

	private void DecrementStreamCapacity(QuicStreamType streamType)
	{
		if (streamType == QuicStreamType.Unidirectional)
		{
			_unidirectionalStreamCapacity--;
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"{this} decremented stream count for {streamType} to {_unidirectionalStreamCapacity}.", "DecrementStreamCapacity");
			}
		}
		if (streamType == QuicStreamType.Bidirectional)
		{
			_bidirectionalStreamCapacity--;
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"{this} decremented stream count for {streamType} to {_bidirectionalStreamCapacity}.", "DecrementStreamCapacity");
			}
		}
	}

	public async ValueTask<QuicStream> OpenOutboundStreamAsync(QuicStreamType type, CancellationToken cancellationToken = default(CancellationToken))
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		QuicStream stream = null;
		try
		{
			stream = new QuicStream(_handle, type, _defaultStreamErrorCode);
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"{this} New outbound {type} stream {stream}.", "OpenOutboundStreamAsync");
			}
			await stream.StartAsync(_decrementStreamCapacity, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			if (stream != null)
			{
				await stream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			ObjectDisposedException.ThrowIf(_disposed, this);
			if (ex2 is QuicException { QuicError: QuicError.InternalError } ex3 && (ex3.HResult == MsQuic.QUIC_STATUS_ABORTED || ex3.HResult == MsQuic.QUIC_STATUS_INVALID_STATE))
			{
				await _connectionCloseTcs.Task.ConfigureAwait(continueOnCapturedContext: false);
			}
			ExceptionDispatchInfo.Capture((ex as Exception) ?? throw ex).Throw();
		}
		return stream;
	}

	public async ValueTask<QuicStream> AcceptInboundStreamAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		if (!_canAccept)
		{
			throw new InvalidOperationException(System.SR.net_quic_accept_not_allowed);
		}
		GCHandle keepObject = GCHandle.Alloc(this);
		try
		{
			return await _acceptQueue.Reader.ReadAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (ChannelClosedException ex) when (ex.InnerException != null)
		{
			ExceptionDispatchInfo.Throw(ex.InnerException);
			throw;
		}
		finally
		{
			keepObject.Free();
		}
	}

	public ValueTask CloseAsync(long errorCode, CancellationToken cancellationToken = default(CancellationToken))
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		ThrowHelper.ValidateErrorCode("errorCode", errorCode, "CloseAsync.errorCode");
		if (_shutdownTcs.TryGetValueTask(out var valueTask, this, cancellationToken))
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"{this} Closing connection, Error code = {errorCode}", "CloseAsync");
			}
			MsQuicApi.Api.ConnectionShutdown(_handle, QUIC_CONNECTION_SHUTDOWN_FLAGS.NONE, (ulong)errorCode);
		}
		return valueTask;
	}

	private unsafe int HandleEventConnected(ref QUIC_CONNECTION_EVENT._Anonymous_e__Union._CONNECTED_e__Struct data)
	{
		_negotiatedApplicationProtocol = new SslApplicationProtocol(new Span<byte>(data.NegotiatedAlpn, data.NegotiatedAlpnLength).ToArray());
		QUIC_HANDSHAKE_INFO msQuicParameter = MsQuicHelpers.GetMsQuicParameter<QUIC_HANDSHAKE_INFO>(_handle, 100663296u);
		_negotiatedCipherSuite = (TlsCipherSuite)msQuicParameter.CipherSuite;
		_negotiatedSslProtocol = (SslProtocols)msQuicParameter.TlsProtocolVersion;
		QuicAddr msQuicParameter2 = MsQuicHelpers.GetMsQuicParameter<QuicAddr>(_handle, 83886082u);
		_remoteEndPoint = MsQuicHelpers.QuicAddrToIPEndPoint(&msQuicParameter2);
		QuicAddr msQuicParameter3 = MsQuicHelpers.GetMsQuicParameter<QuicAddr>(_handle, 83886081u);
		_localEndPoint = MsQuicHelpers.QuicAddrToIPEndPoint(&msQuicParameter3);
		_tlsSecret?.WriteSecret();
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, $"{this} Connection connected {LocalEndPoint} -> {RemoteEndPoint} for {_negotiatedApplicationProtocol} protocol", "HandleEventConnected");
		}
		_connectedTcs.TrySetResult();
		return MsQuic.QUIC_STATUS_SUCCESS;
	}

	private int HandleEventShutdownInitiatedByTransport(ref QUIC_CONNECTION_EVENT._Anonymous_e__Union._SHUTDOWN_INITIATED_BY_TRANSPORT_e__Struct data)
	{
		Exception ex = ExceptionDispatchInfo.SetCurrentStackTrace(ThrowHelper.GetExceptionForMsQuicStatus(data.Status, (long)data.ErrorCode));
		_connectedTcs.TrySetException(ex);
		if (_connectionCloseTcs.TrySetException(ex))
		{
			_ = _connectionCloseTcs.Task.Exception;
		}
		_acceptQueue.Writer.TryComplete(ex);
		return MsQuic.QUIC_STATUS_SUCCESS;
	}

	private int HandleEventShutdownInitiatedByPeer(ref QUIC_CONNECTION_EVENT._Anonymous_e__Union._SHUTDOWN_INITIATED_BY_PEER_e__Struct data)
	{
		Exception ex = ExceptionDispatchInfo.SetCurrentStackTrace(ThrowHelper.GetConnectionAbortedException((long)data.ErrorCode));
		if (_connectionCloseTcs.TrySetException(ex))
		{
			_ = _connectionCloseTcs.Task.Exception;
		}
		_acceptQueue.Writer.TryComplete(ex);
		return MsQuic.QUIC_STATUS_SUCCESS;
	}

	private int HandleEventShutdownComplete()
	{
		_tlsSecret?.WriteSecret();
		Exception ex = ExceptionDispatchInfo.SetCurrentStackTrace(_disposed ? ((SystemException)new ObjectDisposedException(GetType().FullName)) : ((SystemException)ThrowHelper.GetOperationAbortedException()));
		if (_connectionCloseTcs.TrySetException(ex))
		{
			_ = _connectionCloseTcs.Task.Exception;
		}
		_acceptQueue.Writer.TryComplete(ex);
		_connectedTcs.TrySetException(ex);
		_shutdownTokenSource.Cancel();
		_shutdownTcs.TrySetResult(final: true);
		return MsQuic.QUIC_STATUS_SUCCESS;
	}

	private unsafe int HandleEventLocalAddressChanged(ref QUIC_CONNECTION_EVENT._Anonymous_e__Union._LOCAL_ADDRESS_CHANGED_e__Struct data)
	{
		_localEndPoint = MsQuicHelpers.QuicAddrToIPEndPoint(data.Address);
		return MsQuic.QUIC_STATUS_SUCCESS;
	}

	private unsafe int HandleEventPeerAddressChanged(ref QUIC_CONNECTION_EVENT._Anonymous_e__Union._PEER_ADDRESS_CHANGED_e__Struct data)
	{
		_remoteEndPoint = MsQuicHelpers.QuicAddrToIPEndPoint(data.Address);
		return MsQuic.QUIC_STATUS_SUCCESS;
	}

	private unsafe int HandleEventPeerStreamStarted(ref QUIC_CONNECTION_EVENT._Anonymous_e__Union._PEER_STREAM_STARTED_e__Struct data)
	{
		QuicStream quicStream = new QuicStream(_handle, data.Stream, data.Flags, _defaultStreamErrorCode);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			QuicStreamType quicStreamType = ((!data.Flags.HasFlag(QUIC_STREAM_OPEN_FLAGS.UNIDIRECTIONAL)) ? QuicStreamType.Bidirectional : QuicStreamType.Unidirectional);
			System.Net.NetEventSource.Info(this, $"{this} New inbound {quicStreamType} stream {quicStream}, Id = {quicStream.Id}.", "HandleEventPeerStreamStarted");
		}
		if (!_acceptQueue.Writer.TryWrite(quicStream))
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(this, $"{this} Unable to enqueue incoming stream {quicStream}", "HandleEventPeerStreamStarted");
			}
			quicStream.Dispose();
			return MsQuic.QUIC_STATUS_SUCCESS;
		}
		data.Flags |= QUIC_STREAM_OPEN_FLAGS.DELAY_ID_FC_UPDATES;
		return MsQuic.QUIC_STATUS_SUCCESS;
	}

	private int HandleEventStreamsAvailable(ref QUIC_CONNECTION_EVENT._Anonymous_e__Union._STREAMS_AVAILABLE_e__Struct data)
	{
		int bidirectionalIncrement = 0;
		int unidirectionalIncrement = 0;
		if (data.BidirectionalCount > 0)
		{
			bidirectionalIncrement = data.BidirectionalCount - _bidirectionalStreamCapacity;
			_bidirectionalStreamCapacity = data.BidirectionalCount;
		}
		if (data.UnidirectionalCount > 0)
		{
			unidirectionalIncrement = data.UnidirectionalCount - _unidirectionalStreamCapacity;
			_unidirectionalStreamCapacity = data.UnidirectionalCount;
		}
		OnStreamCapacityIncreased(bidirectionalIncrement, unidirectionalIncrement);
		return MsQuic.QUIC_STATUS_SUCCESS;
	}

	private unsafe int HandleEventPeerCertificateReceived(ref QUIC_CONNECTION_EVENT._Anonymous_e__Union._PEER_CERTIFICATE_RECEIVED_e__Struct data)
	{
		_tlsSecret?.WriteSecret();
		Task<bool> task = _sslConnectionOptions.StartAsyncCertificateValidation((nint)data.Certificate, (nint)data.Chain);
		if (task.IsCompletedSuccessfully)
		{
			if (!task.Result)
			{
				return MsQuic.QUIC_STATUS_BAD_CERTIFICATE;
			}
			return MsQuic.QUIC_STATUS_SUCCESS;
		}
		return MsQuic.QUIC_STATUS_PENDING;
	}

	private int HandleConnectionEvent(ref QUIC_CONNECTION_EVENT connectionEvent)
	{
		return connectionEvent.Type switch
		{
			QUIC_CONNECTION_EVENT_TYPE.CONNECTED => HandleEventConnected(ref connectionEvent.CONNECTED), 
			QUIC_CONNECTION_EVENT_TYPE.SHUTDOWN_INITIATED_BY_TRANSPORT => HandleEventShutdownInitiatedByTransport(ref connectionEvent.SHUTDOWN_INITIATED_BY_TRANSPORT), 
			QUIC_CONNECTION_EVENT_TYPE.SHUTDOWN_INITIATED_BY_PEER => HandleEventShutdownInitiatedByPeer(ref connectionEvent.SHUTDOWN_INITIATED_BY_PEER), 
			QUIC_CONNECTION_EVENT_TYPE.SHUTDOWN_COMPLETE => HandleEventShutdownComplete(), 
			QUIC_CONNECTION_EVENT_TYPE.LOCAL_ADDRESS_CHANGED => HandleEventLocalAddressChanged(ref connectionEvent.LOCAL_ADDRESS_CHANGED), 
			QUIC_CONNECTION_EVENT_TYPE.PEER_ADDRESS_CHANGED => HandleEventPeerAddressChanged(ref connectionEvent.PEER_ADDRESS_CHANGED), 
			QUIC_CONNECTION_EVENT_TYPE.PEER_STREAM_STARTED => HandleEventPeerStreamStarted(ref connectionEvent.PEER_STREAM_STARTED), 
			QUIC_CONNECTION_EVENT_TYPE.STREAMS_AVAILABLE => HandleEventStreamsAvailable(ref connectionEvent.STREAMS_AVAILABLE), 
			QUIC_CONNECTION_EVENT_TYPE.PEER_CERTIFICATE_RECEIVED => HandleEventPeerCertificateReceived(ref connectionEvent.PEER_CERTIFICATE_RECEIVED), 
			_ => MsQuic.QUIC_STATUS_SUCCESS, 
		};
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
	private unsafe static int NativeCallback(QUIC_HANDLE* connection, void* context, QUIC_CONNECTION_EVENT* connectionEvent)
	{
		GCHandle gCHandle = GCHandle.FromIntPtr((nint)context);
		if (!gCHandle.IsAllocated || !(gCHandle.Target is QuicConnection quicConnection))
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(null, $"Received event {connectionEvent->Type} for [conn][{(nint)connection:X11}] while connection is already disposed", "NativeCallback");
			}
			return MsQuic.QUIC_STATUS_INVALID_STATE;
		}
		try
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(quicConnection, $"{quicConnection} Received event {connectionEvent->Type} {connectionEvent->ToString()}", "NativeCallback");
			}
			return quicConnection.HandleConnectionEvent(ref *connectionEvent);
		}
		catch (Exception ex)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(quicConnection, $"{quicConnection} Exception while processing event {connectionEvent->Type}: {ex}", "NativeCallback");
			}
			return MsQuic.QUIC_STATUS_INTERNAL_ERROR;
		}
	}

	public async ValueTask DisposeAsync()
	{
		if (!Interlocked.Exchange(ref _disposed, value: true))
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"{this} Disposing.", "DisposeAsync");
			}
			if (_shutdownTcs.TryGetValueTask(out var valueTask, this))
			{
				MsQuicApi.Api.ConnectionShutdown(_handle, QUIC_CONNECTION_SHUTDOWN_FLAGS.NONE, (ulong)_defaultCloseErrorCode);
			}
			else if (!valueTask.IsCompletedSuccessfully)
			{
				MsQuicApi.Api.ConnectionShutdown(_handle, QUIC_CONNECTION_SHUTDOWN_FLAGS.SILENT, (ulong)_defaultCloseErrorCode);
			}
			await _shutdownTcs.GetFinalTask(this).ConfigureAwait(continueOnCapturedContext: false);
			_handle.Dispose();
			_shutdownTokenSource.Dispose();
			_configuration?.Dispose();
			if (!_remoteCertificateExposed)
			{
				_remoteCertificate?.Dispose();
			}
			_acceptQueue.Writer.TryComplete(ExceptionDispatchInfo.SetCurrentStackTrace(new ObjectDisposedException(GetType().FullName)));
			QuicStream item;
			while (_acceptQueue.Reader.TryRead(out item))
			{
				await item.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
		}
	}
}
