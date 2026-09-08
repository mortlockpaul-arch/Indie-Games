using System.Runtime.InteropServices;
using Microsoft.Quic;

namespace System.Net.Quic;

internal sealed class MsQuicTlsSecret : IDisposable
{
	private unsafe QUIC_TLS_SECRETS* _tlsSecrets;

	public unsafe static MsQuicTlsSecret Create(MsQuicContextSafeHandle handle)
	{
		if (!SslKeyLogger.IsEnabled)
		{
			return null;
		}
		QUIC_TLS_SECRETS* ptr = null;
		try
		{
			ptr = (QUIC_TLS_SECRETS*)NativeMemory.AllocZeroed((nuint)sizeof(QUIC_TLS_SECRETS));
			MsQuicHelpers.SetMsQuicParameter(handle, 83886099u, (uint)sizeof(QUIC_TLS_SECRETS), (byte*)ptr);
			return (MsQuicTlsSecret)(handle.Disposable = new MsQuicTlsSecret(ptr));
		}
		catch (Exception ex)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(handle, $"Failed to set native memory for TLS secret: {ex}", "Create");
			}
			if (ptr != null)
			{
				NativeMemory.Free(ptr);
			}
			return null;
		}
	}

	private unsafe MsQuicTlsSecret(QUIC_TLS_SECRETS* tlsSecrets)
	{
		_tlsSecrets = tlsSecrets;
	}

	public unsafe void WriteSecret()
	{
		ReadOnlySpan<byte> clientRandom = ((_tlsSecrets->IsSet.ClientRandom != 0) ? new ReadOnlySpan<byte>(_tlsSecrets->ClientRandom, 32) : ReadOnlySpan<byte>.Empty);
		Span<byte> span = ((_tlsSecrets->IsSet.ClientHandshakeTrafficSecret != 0) ? new Span<byte>(_tlsSecrets->ClientHandshakeTrafficSecret, _tlsSecrets->SecretLength) : Span<byte>.Empty);
		Span<byte> span2 = ((_tlsSecrets->IsSet.ServerHandshakeTrafficSecret != 0) ? new Span<byte>(_tlsSecrets->ServerHandshakeTrafficSecret, _tlsSecrets->SecretLength) : Span<byte>.Empty);
		Span<byte> span3 = ((_tlsSecrets->IsSet.ClientTrafficSecret0 != 0) ? new Span<byte>(_tlsSecrets->ClientTrafficSecret0, _tlsSecrets->SecretLength) : Span<byte>.Empty);
		Span<byte> span4 = ((_tlsSecrets->IsSet.ServerTrafficSecret0 != 0) ? new Span<byte>(_tlsSecrets->ServerTrafficSecret0, _tlsSecrets->SecretLength) : Span<byte>.Empty);
		Span<byte> span5 = ((_tlsSecrets->IsSet.ClientEarlyTrafficSecret != 0) ? new Span<byte>(_tlsSecrets->ClientEarlyTrafficSecret, _tlsSecrets->SecretLength) : Span<byte>.Empty);
		SslKeyLogger.WriteSecrets(clientRandom, span, span2, span3, span4, span5);
		if (!span.IsEmpty)
		{
			span.Clear();
			_tlsSecrets->IsSet.ClientHandshakeTrafficSecret = 0;
		}
		if (!span2.IsEmpty)
		{
			span2.Clear();
			_tlsSecrets->IsSet.ServerHandshakeTrafficSecret = 0;
		}
		if (!span3.IsEmpty)
		{
			span3.Clear();
			_tlsSecrets->IsSet.ClientTrafficSecret0 = 0;
		}
		if (!span4.IsEmpty)
		{
			span4.Clear();
			_tlsSecrets->IsSet.ServerTrafficSecret0 = 0;
		}
		if (!span5.IsEmpty)
		{
			span5.Clear();
			_tlsSecrets->IsSet.ClientEarlyTrafficSecret = 0;
		}
	}

	public unsafe void Dispose()
	{
		if (_tlsSecrets == null)
		{
			return;
		}
		lock (this)
		{
			if (_tlsSecrets != null)
			{
				_ = _tlsSecrets;
				_tlsSecrets = null;
				NativeMemory.Free(_tlsSecrets);
			}
		}
	}
}
