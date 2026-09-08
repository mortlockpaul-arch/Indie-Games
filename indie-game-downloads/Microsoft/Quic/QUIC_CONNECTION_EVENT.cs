using System.Net.Quic;
using System.Runtime.InteropServices;

namespace Microsoft.Quic;

internal struct QUIC_CONNECTION_EVENT
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct _Anonymous_e__Union
	{
		internal struct _CONNECTED_e__Struct
		{
			internal byte SessionResumed;

			internal byte NegotiatedAlpnLength;

			internal unsafe byte* NegotiatedAlpn;
		}

		internal struct _SHUTDOWN_INITIATED_BY_TRANSPORT_e__Struct
		{
			internal int Status;

			internal ulong ErrorCode;
		}

		internal struct _SHUTDOWN_INITIATED_BY_PEER_e__Struct
		{
			internal ulong ErrorCode;
		}

		internal struct _SHUTDOWN_COMPLETE_e__Struct
		{
			internal byte _bitfield;

			internal byte HandshakeCompleted => (byte)(_bitfield & 1);

			internal byte PeerAcknowledgedShutdown => (byte)((ulong)(_bitfield >> 1) & 1uL);

			internal byte AppCloseInProgress => (byte)((ulong)(_bitfield >> 2) & 1uL);
		}

		internal struct _LOCAL_ADDRESS_CHANGED_e__Struct
		{
			internal unsafe QuicAddr* Address;
		}

		internal struct _PEER_ADDRESS_CHANGED_e__Struct
		{
			internal unsafe QuicAddr* Address;
		}

		internal struct _PEER_STREAM_STARTED_e__Struct
		{
			internal unsafe QUIC_HANDLE* Stream;

			internal QUIC_STREAM_OPEN_FLAGS Flags;
		}

		internal struct _STREAMS_AVAILABLE_e__Struct
		{
			internal ushort BidirectionalCount;

			internal ushort UnidirectionalCount;
		}

		internal struct _PEER_NEEDS_STREAMS_e__Struct
		{
			internal byte Bidirectional;
		}

		internal struct _IDEAL_PROCESSOR_CHANGED_e__Struct
		{
			internal ushort IdealProcessor;

			internal ushort PartitionIndex;
		}

		internal struct _DATAGRAM_STATE_CHANGED_e__Struct
		{
			internal byte SendEnabled;

			internal ushort MaxSendLength;
		}

		internal struct _DATAGRAM_RECEIVED_e__Struct
		{
			internal unsafe QUIC_BUFFER* Buffer;

			internal QUIC_RECEIVE_FLAGS Flags;
		}

		internal struct _DATAGRAM_SEND_STATE_CHANGED_e__Struct
		{
			internal unsafe void* ClientContext;

			internal QUIC_DATAGRAM_SEND_STATE State;
		}

		internal struct _RESUMED_e__Struct
		{
			internal ushort ResumptionStateLength;

			internal unsafe byte* ResumptionState;
		}

		internal struct _RESUMPTION_TICKET_RECEIVED_e__Struct
		{
			internal uint ResumptionTicketLength;

			internal unsafe byte* ResumptionTicket;
		}

		internal struct _PEER_CERTIFICATE_RECEIVED_e__Struct
		{
			internal unsafe void* Certificate;

			internal uint DeferredErrorFlags;

			internal int DeferredStatus;

			internal unsafe void* Chain;
		}

		internal struct _RELIABLE_RESET_NEGOTIATED_e__Struct
		{
			internal byte IsNegotiated;
		}

		internal struct _ONE_WAY_DELAY_NEGOTIATED_e__Struct
		{
			internal byte SendNegotiated;

			internal byte ReceiveNegotiated;
		}

		[FieldOffset(0)]
		internal _CONNECTED_e__Struct CONNECTED;

		[FieldOffset(0)]
		internal _SHUTDOWN_INITIATED_BY_TRANSPORT_e__Struct SHUTDOWN_INITIATED_BY_TRANSPORT;

		[FieldOffset(0)]
		internal _SHUTDOWN_INITIATED_BY_PEER_e__Struct SHUTDOWN_INITIATED_BY_PEER;

		[FieldOffset(0)]
		internal _SHUTDOWN_COMPLETE_e__Struct SHUTDOWN_COMPLETE;

		[FieldOffset(0)]
		internal _LOCAL_ADDRESS_CHANGED_e__Struct LOCAL_ADDRESS_CHANGED;

		[FieldOffset(0)]
		internal _PEER_ADDRESS_CHANGED_e__Struct PEER_ADDRESS_CHANGED;

		[FieldOffset(0)]
		internal _PEER_STREAM_STARTED_e__Struct PEER_STREAM_STARTED;

		[FieldOffset(0)]
		internal _STREAMS_AVAILABLE_e__Struct STREAMS_AVAILABLE;

		[FieldOffset(0)]
		internal _PEER_NEEDS_STREAMS_e__Struct PEER_NEEDS_STREAMS;

		[FieldOffset(0)]
		internal _IDEAL_PROCESSOR_CHANGED_e__Struct IDEAL_PROCESSOR_CHANGED;

		[FieldOffset(0)]
		internal _DATAGRAM_STATE_CHANGED_e__Struct DATAGRAM_STATE_CHANGED;

		[FieldOffset(0)]
		internal _DATAGRAM_RECEIVED_e__Struct DATAGRAM_RECEIVED;

		[FieldOffset(0)]
		internal _DATAGRAM_SEND_STATE_CHANGED_e__Struct DATAGRAM_SEND_STATE_CHANGED;

		[FieldOffset(0)]
		internal _RESUMED_e__Struct RESUMED;

		[FieldOffset(0)]
		internal _RESUMPTION_TICKET_RECEIVED_e__Struct RESUMPTION_TICKET_RECEIVED;

		[FieldOffset(0)]
		internal _PEER_CERTIFICATE_RECEIVED_e__Struct PEER_CERTIFICATE_RECEIVED;

		[FieldOffset(0)]
		internal _RELIABLE_RESET_NEGOTIATED_e__Struct RELIABLE_RESET_NEGOTIATED;

		[FieldOffset(0)]
		internal _ONE_WAY_DELAY_NEGOTIATED_e__Struct ONE_WAY_DELAY_NEGOTIATED;
	}

	internal QUIC_CONNECTION_EVENT_TYPE Type;

	internal _Anonymous_e__Union Anonymous;

	internal ref _Anonymous_e__Union._CONNECTED_e__Struct CONNECTED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.CONNECTED, 1));

	internal ref _Anonymous_e__Union._SHUTDOWN_INITIATED_BY_TRANSPORT_e__Struct SHUTDOWN_INITIATED_BY_TRANSPORT => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.SHUTDOWN_INITIATED_BY_TRANSPORT, 1));

	internal ref _Anonymous_e__Union._SHUTDOWN_INITIATED_BY_PEER_e__Struct SHUTDOWN_INITIATED_BY_PEER => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.SHUTDOWN_INITIATED_BY_PEER, 1));

	internal ref _Anonymous_e__Union._SHUTDOWN_COMPLETE_e__Struct SHUTDOWN_COMPLETE => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.SHUTDOWN_COMPLETE, 1));

	internal ref _Anonymous_e__Union._LOCAL_ADDRESS_CHANGED_e__Struct LOCAL_ADDRESS_CHANGED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.LOCAL_ADDRESS_CHANGED, 1));

	internal ref _Anonymous_e__Union._PEER_ADDRESS_CHANGED_e__Struct PEER_ADDRESS_CHANGED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.PEER_ADDRESS_CHANGED, 1));

	internal ref _Anonymous_e__Union._PEER_STREAM_STARTED_e__Struct PEER_STREAM_STARTED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.PEER_STREAM_STARTED, 1));

	internal ref _Anonymous_e__Union._STREAMS_AVAILABLE_e__Struct STREAMS_AVAILABLE => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.STREAMS_AVAILABLE, 1));

	internal ref _Anonymous_e__Union._PEER_NEEDS_STREAMS_e__Struct PEER_NEEDS_STREAMS => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.PEER_NEEDS_STREAMS, 1));

	internal ref _Anonymous_e__Union._IDEAL_PROCESSOR_CHANGED_e__Struct IDEAL_PROCESSOR_CHANGED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.IDEAL_PROCESSOR_CHANGED, 1));

	internal ref _Anonymous_e__Union._DATAGRAM_STATE_CHANGED_e__Struct DATAGRAM_STATE_CHANGED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.DATAGRAM_STATE_CHANGED, 1));

	internal ref _Anonymous_e__Union._DATAGRAM_RECEIVED_e__Struct DATAGRAM_RECEIVED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.DATAGRAM_RECEIVED, 1));

	internal ref _Anonymous_e__Union._DATAGRAM_SEND_STATE_CHANGED_e__Struct DATAGRAM_SEND_STATE_CHANGED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.DATAGRAM_SEND_STATE_CHANGED, 1));

	internal ref _Anonymous_e__Union._RESUMED_e__Struct RESUMED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.RESUMED, 1));

	internal ref _Anonymous_e__Union._RESUMPTION_TICKET_RECEIVED_e__Struct RESUMPTION_TICKET_RECEIVED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.RESUMPTION_TICKET_RECEIVED, 1));

	internal ref _Anonymous_e__Union._PEER_CERTIFICATE_RECEIVED_e__Struct PEER_CERTIFICATE_RECEIVED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.PEER_CERTIFICATE_RECEIVED, 1));

	internal ref _Anonymous_e__Union._RELIABLE_RESET_NEGOTIATED_e__Struct RELIABLE_RESET_NEGOTIATED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.RELIABLE_RESET_NEGOTIATED, 1));

	internal ref _Anonymous_e__Union._ONE_WAY_DELAY_NEGOTIATED_e__Struct ONE_WAY_DELAY_NEGOTIATED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.ONE_WAY_DELAY_NEGOTIATED, 1));

	public unsafe override string ToString()
	{
		return Type switch
		{
			QUIC_CONNECTION_EVENT_TYPE.CONNECTED => $"{{ {"SessionResumed"} = {CONNECTED.SessionResumed} }}", 
			QUIC_CONNECTION_EVENT_TYPE.SHUTDOWN_INITIATED_BY_TRANSPORT => $"{{ {"Status"} = {SHUTDOWN_INITIATED_BY_TRANSPORT.Status}, {"ErrorCode"} = {SHUTDOWN_INITIATED_BY_TRANSPORT.ErrorCode} }}", 
			QUIC_CONNECTION_EVENT_TYPE.SHUTDOWN_INITIATED_BY_PEER => $"{{ {"ErrorCode"} = {SHUTDOWN_INITIATED_BY_PEER.ErrorCode} }}", 
			QUIC_CONNECTION_EVENT_TYPE.SHUTDOWN_COMPLETE => $"{{ {"HandshakeCompleted"} = {SHUTDOWN_COMPLETE.HandshakeCompleted}, {"PeerAcknowledgedShutdown"} = {SHUTDOWN_COMPLETE.PeerAcknowledgedShutdown}, {"AppCloseInProgress"} = {SHUTDOWN_COMPLETE.AppCloseInProgress} }}", 
			QUIC_CONNECTION_EVENT_TYPE.LOCAL_ADDRESS_CHANGED => $"{{ {"Address"} = {MsQuicHelpers.QuicAddrToIPEndPoint(LOCAL_ADDRESS_CHANGED.Address)} }}", 
			QUIC_CONNECTION_EVENT_TYPE.PEER_ADDRESS_CHANGED => $"{{ {"Address"} = {MsQuicHelpers.QuicAddrToIPEndPoint(PEER_ADDRESS_CHANGED.Address)} }}", 
			QUIC_CONNECTION_EVENT_TYPE.PEER_STREAM_STARTED => $"{{ {"Stream"} = 0x{(nint)PEER_STREAM_STARTED.Stream:X11} {"Flags"} = {PEER_STREAM_STARTED.Flags} }}", 
			QUIC_CONNECTION_EVENT_TYPE.STREAMS_AVAILABLE => $"{{ {"BidirectionalCount"} = {STREAMS_AVAILABLE.BidirectionalCount}, {"UnidirectionalCount"} = {STREAMS_AVAILABLE.UnidirectionalCount} }}", 
			QUIC_CONNECTION_EVENT_TYPE.PEER_NEEDS_STREAMS => $"{{ {"Bidirectional"} = {PEER_NEEDS_STREAMS.Bidirectional} }}", 
			QUIC_CONNECTION_EVENT_TYPE.IDEAL_PROCESSOR_CHANGED => $"{{ {"IdealProcessor"} = {IDEAL_PROCESSOR_CHANGED.IdealProcessor}, {"PartitionIndex"} = {IDEAL_PROCESSOR_CHANGED.PartitionIndex} }}", 
			QUIC_CONNECTION_EVENT_TYPE.DATAGRAM_STATE_CHANGED => $"{{ {"SendEnabled"} = {DATAGRAM_STATE_CHANGED.SendEnabled}, {"MaxSendLength"} = {DATAGRAM_STATE_CHANGED.MaxSendLength} }}", 
			QUIC_CONNECTION_EVENT_TYPE.DATAGRAM_RECEIVED => $"{{ {"Flags"} = {DATAGRAM_RECEIVED.Flags} }}", 
			QUIC_CONNECTION_EVENT_TYPE.DATAGRAM_SEND_STATE_CHANGED => $"{{ {"ClientContext"} = 0x{(nint)DATAGRAM_SEND_STATE_CHANGED.ClientContext:X11}, {"State"} = {DATAGRAM_SEND_STATE_CHANGED.State} }}", 
			QUIC_CONNECTION_EVENT_TYPE.RESUMED => $"{{ {"ResumptionStateLength"} = {RESUMED.ResumptionStateLength} }}", 
			QUIC_CONNECTION_EVENT_TYPE.RESUMPTION_TICKET_RECEIVED => $"{{ {"ResumptionTicketLength"} = {RESUMPTION_TICKET_RECEIVED.ResumptionTicketLength} }}", 
			QUIC_CONNECTION_EVENT_TYPE.PEER_CERTIFICATE_RECEIVED => $"{{ {"DeferredStatus"} = {PEER_CERTIFICATE_RECEIVED.DeferredStatus}, {"DeferredErrorFlags"} = {PEER_CERTIFICATE_RECEIVED.DeferredErrorFlags}, {"Certificate"} = 0x{(nint)PEER_CERTIFICATE_RECEIVED.Certificate:X11} }}", 
			QUIC_CONNECTION_EVENT_TYPE.RELIABLE_RESET_NEGOTIATED => $"{{ {"IsNegotiated"} = {RELIABLE_RESET_NEGOTIATED.IsNegotiated} }}", 
			QUIC_CONNECTION_EVENT_TYPE.ONE_WAY_DELAY_NEGOTIATED => $"{{ {"SendNegotiated"} = {ONE_WAY_DELAY_NEGOTIATED.SendNegotiated}, {"ReceiveNegotiated"} = {ONE_WAY_DELAY_NEGOTIATED.ReceiveNegotiated} }}", 
			_ => string.Empty, 
		};
	}
}
