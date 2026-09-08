using System.Runtime.InteropServices;

namespace Microsoft.Quic;

[StructLayout(LayoutKind.Explicit)]
internal struct QuicAddrFamilyAndLen
{
	[FieldOffset(0)]
	public ushort sin_family;

	[FieldOffset(0)]
	public byte sin_len;

	[FieldOffset(1)]
	public byte sin_family_bsd;
}
