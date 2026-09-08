namespace Microsoft.Quic;

internal struct QuicAddrIn
{
	public QuicAddrFamilyAndLen sin_family;

	public ushort sin_port;

	public unsafe fixed byte sin_addr[4];
}
