using System;
using System.Runtime.InteropServices;

namespace Microsoft.Quic;

[StructLayout(LayoutKind.Explicit)]
internal struct QuicAddr
{
	[FieldOffset(0)]
	public QuicAddrIn Ipv4;

	[FieldOffset(0)]
	public QuicAddrIn6 Ipv6;

	[FieldOffset(0)]
	public QuicAddrFamilyAndLen FamilyLen;

	public static bool SockaddrHasLength
	{
		get
		{
			if (!OperatingSystem.IsFreeBSD() && !OperatingSystem.IsIOS() && !OperatingSystem.IsMacOS() && !OperatingSystem.IsMacCatalyst())
			{
				return OperatingSystem.IsTvOS();
			}
			return true;
		}
	}

	public int Family
	{
		get
		{
			if (SockaddrHasLength)
			{
				return FamilyLen.sin_family_bsd;
			}
			return FamilyLen.sin_family;
		}
		set
		{
			if (SockaddrHasLength)
			{
				FamilyLen.sin_family_bsd = (byte)value;
			}
			else
			{
				FamilyLen.sin_family = (ushort)value;
			}
		}
	}
}
