namespace Microsoft.Quic;

internal struct QuicAddrIn6
{
	public QuicAddrFamilyAndLen sin6_family;

	public ushort sin6_port;

	public uint sin6_flowinfo;

	public unsafe fixed byte sin6_addr[16];

	public uint sin6_scope_id;
}
