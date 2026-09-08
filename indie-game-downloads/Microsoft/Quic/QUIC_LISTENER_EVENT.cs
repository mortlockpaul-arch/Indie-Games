using System.Net.Quic;
using System.Runtime.InteropServices;

namespace Microsoft.Quic;

internal struct QUIC_LISTENER_EVENT
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct _Anonymous_e__Union
	{
		internal struct _NEW_CONNECTION_e__Struct
		{
			internal unsafe QUIC_NEW_CONNECTION_INFO* Info;

			internal unsafe QUIC_HANDLE* Connection;
		}

		internal struct _STOP_COMPLETE_e__Struct
		{
			internal byte _bitfield;
		}

		[FieldOffset(0)]
		internal _NEW_CONNECTION_e__Struct NEW_CONNECTION;

		[FieldOffset(0)]
		internal _STOP_COMPLETE_e__Struct STOP_COMPLETE;
	}

	internal QUIC_LISTENER_EVENT_TYPE Type;

	internal _Anonymous_e__Union Anonymous;

	internal ref _Anonymous_e__Union._NEW_CONNECTION_e__Struct NEW_CONNECTION => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.NEW_CONNECTION, 1));

	public unsafe override string ToString()
	{
		if (Type == QUIC_LISTENER_EVENT_TYPE.NEW_CONNECTION)
		{
			return $"{{ {"Info"} = {{ {"QuicVersion"} = {NEW_CONNECTION.Info->QuicVersion}, {"LocalAddress"} = {MsQuicHelpers.QuicAddrToIPEndPoint(NEW_CONNECTION.Info->LocalAddress)}, {"RemoteAddress"} = {MsQuicHelpers.QuicAddrToIPEndPoint(NEW_CONNECTION.Info->RemoteAddress)} }}, {"Connection"} = 0x{(nint)NEW_CONNECTION.Connection:X11} }}";
		}
		return string.Empty;
	}
}
