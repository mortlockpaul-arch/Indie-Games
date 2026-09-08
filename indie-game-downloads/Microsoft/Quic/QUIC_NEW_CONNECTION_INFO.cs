using System.Net.Quic;

namespace Microsoft.Quic;

internal struct QUIC_NEW_CONNECTION_INFO
{
	internal uint QuicVersion;

	internal unsafe QuicAddr* LocalAddress;

	internal unsafe QuicAddr* RemoteAddress;

	internal uint CryptoBufferLength;

	internal ushort ClientAlpnListLength;

	internal ushort ServerNameLength;

	internal byte NegotiatedAlpnLength;

	internal unsafe byte* CryptoBuffer;

	internal unsafe byte* ClientAlpnList;

	internal unsafe byte* NegotiatedAlpn;

	internal unsafe sbyte* ServerName;

	public unsafe override string ToString()
	{
		return $"{{ {"QuicVersion"} = {QuicVersion}, {"LocalAddress"} = {MsQuicHelpers.QuicAddrToIPEndPoint(LocalAddress)}, {"RemoteAddress"} = {MsQuicHelpers.QuicAddrToIPEndPoint(RemoteAddress)} }}";
	}
}
