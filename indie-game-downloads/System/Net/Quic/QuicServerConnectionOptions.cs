using System.Net.Security;

namespace System.Net.Quic;

public sealed class QuicServerConnectionOptions : QuicConnectionOptions
{
	public SslServerAuthenticationOptions ServerAuthenticationOptions { get; set; }

	public QuicServerConnectionOptions()
	{
		base.MaxInboundBidirectionalStreams = 100;
		base.MaxInboundUnidirectionalStreams = 10;
	}

	internal override void Validate(string argumentName)
	{
		base.Validate(argumentName);
		ThrowHelper.ValidateNotNull(argumentName, System.SR.net_quic_not_null_accept_connection, ServerAuthenticationOptions, "ServerAuthenticationOptions");
	}
}
