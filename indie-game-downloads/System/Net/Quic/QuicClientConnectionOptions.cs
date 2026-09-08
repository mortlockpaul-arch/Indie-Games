using System.Net.Security;

namespace System.Net.Quic;

public sealed class QuicClientConnectionOptions : QuicConnectionOptions
{
	public SslClientAuthenticationOptions ClientAuthenticationOptions { get; set; }

	public EndPoint RemoteEndPoint { get; set; }

	public IPEndPoint? LocalEndPoint { get; set; }

	public QuicClientConnectionOptions()
	{
		base.MaxInboundBidirectionalStreams = 0;
		base.MaxInboundUnidirectionalStreams = 0;
	}

	internal override void Validate(string argumentName)
	{
		base.Validate(argumentName);
		ThrowHelper.ValidateNotNull(argumentName, System.SR.net_quic_not_null_open_connection, ClientAuthenticationOptions, "ClientAuthenticationOptions");
		ThrowHelper.ValidateNotNull(argumentName, System.SR.net_quic_not_null_open_connection, RemoteEndPoint, "RemoteEndPoint");
	}
}
