using System.Runtime.InteropServices;

namespace Microsoft.Quic;

internal struct QUIC_STREAM_EVENT
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct _Anonymous_e__Union
	{
		internal struct _START_COMPLETE_e__Struct
		{
			internal int Status;

			internal ulong ID;

			internal byte _bitfield;

			internal byte PeerAccepted => (byte)(_bitfield & 1);
		}

		internal struct _RECEIVE_e__Struct
		{
			internal ulong AbsoluteOffset;

			internal ulong TotalBufferLength;

			internal unsafe QUIC_BUFFER* Buffers;

			internal uint BufferCount;

			internal QUIC_RECEIVE_FLAGS Flags;
		}

		internal struct _SEND_COMPLETE_e__Struct
		{
			internal byte Canceled;

			internal unsafe void* ClientContext;
		}

		internal struct _PEER_SEND_ABORTED_e__Struct
		{
			internal ulong ErrorCode;
		}

		internal struct _PEER_RECEIVE_ABORTED_e__Struct
		{
			internal ulong ErrorCode;
		}

		internal struct _SEND_SHUTDOWN_COMPLETE_e__Struct
		{
			internal byte Graceful;
		}

		internal struct _SHUTDOWN_COMPLETE_e__Struct
		{
			internal byte ConnectionShutdown;

			internal byte _bitfield;

			internal ulong ConnectionErrorCode;

			internal int ConnectionCloseStatus;

			internal byte ConnectionShutdownByApp => (byte)((ulong)(_bitfield >> 1) & 1uL);

			internal byte ConnectionClosedRemotely => (byte)((ulong)(_bitfield >> 2) & 1uL);
		}

		internal struct _IDEAL_SEND_BUFFER_SIZE_e__Struct
		{
			internal ulong ByteCount;
		}

		[FieldOffset(0)]
		internal _START_COMPLETE_e__Struct START_COMPLETE;

		[FieldOffset(0)]
		internal _RECEIVE_e__Struct RECEIVE;

		[FieldOffset(0)]
		internal _SEND_COMPLETE_e__Struct SEND_COMPLETE;

		[FieldOffset(0)]
		internal _PEER_SEND_ABORTED_e__Struct PEER_SEND_ABORTED;

		[FieldOffset(0)]
		internal _PEER_RECEIVE_ABORTED_e__Struct PEER_RECEIVE_ABORTED;

		[FieldOffset(0)]
		internal _SEND_SHUTDOWN_COMPLETE_e__Struct SEND_SHUTDOWN_COMPLETE;

		[FieldOffset(0)]
		internal _SHUTDOWN_COMPLETE_e__Struct SHUTDOWN_COMPLETE;

		[FieldOffset(0)]
		internal _IDEAL_SEND_BUFFER_SIZE_e__Struct IDEAL_SEND_BUFFER_SIZE;
	}

	internal QUIC_STREAM_EVENT_TYPE Type;

	internal _Anonymous_e__Union Anonymous;

	internal ref _Anonymous_e__Union._START_COMPLETE_e__Struct START_COMPLETE => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.START_COMPLETE, 1));

	internal ref _Anonymous_e__Union._RECEIVE_e__Struct RECEIVE => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.RECEIVE, 1));

	internal ref _Anonymous_e__Union._SEND_COMPLETE_e__Struct SEND_COMPLETE => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.SEND_COMPLETE, 1));

	internal ref _Anonymous_e__Union._PEER_SEND_ABORTED_e__Struct PEER_SEND_ABORTED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.PEER_SEND_ABORTED, 1));

	internal ref _Anonymous_e__Union._PEER_RECEIVE_ABORTED_e__Struct PEER_RECEIVE_ABORTED => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.PEER_RECEIVE_ABORTED, 1));

	internal ref _Anonymous_e__Union._SEND_SHUTDOWN_COMPLETE_e__Struct SEND_SHUTDOWN_COMPLETE => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.SEND_SHUTDOWN_COMPLETE, 1));

	internal ref _Anonymous_e__Union._SHUTDOWN_COMPLETE_e__Struct SHUTDOWN_COMPLETE => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.SHUTDOWN_COMPLETE, 1));

	internal ref _Anonymous_e__Union._IDEAL_SEND_BUFFER_SIZE_e__Struct IDEAL_SEND_BUFFER_SIZE => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.IDEAL_SEND_BUFFER_SIZE, 1));

	public override string ToString()
	{
		return Type switch
		{
			QUIC_STREAM_EVENT_TYPE.START_COMPLETE => $"{{ {"Status"} = {START_COMPLETE.Status}, {"ID"} = {START_COMPLETE.ID}, {"PeerAccepted"} = {START_COMPLETE.PeerAccepted} }}", 
			QUIC_STREAM_EVENT_TYPE.RECEIVE => $"{{ {"AbsoluteOffset"} = {RECEIVE.AbsoluteOffset}, {"TotalBufferLength"} = {RECEIVE.TotalBufferLength}, {"Flags"} = {RECEIVE.Flags} }}", 
			QUIC_STREAM_EVENT_TYPE.SEND_COMPLETE => $"{{ {"Canceled"} = {SEND_COMPLETE.Canceled} }}", 
			QUIC_STREAM_EVENT_TYPE.PEER_SEND_ABORTED => $"{{ {"ErrorCode"} = {PEER_SEND_ABORTED.ErrorCode} }}", 
			QUIC_STREAM_EVENT_TYPE.PEER_RECEIVE_ABORTED => $"{{ {"ErrorCode"} = {PEER_RECEIVE_ABORTED.ErrorCode} }}", 
			QUIC_STREAM_EVENT_TYPE.SEND_SHUTDOWN_COMPLETE => $"{{ {"Graceful"} = {SEND_SHUTDOWN_COMPLETE.Graceful} }}", 
			QUIC_STREAM_EVENT_TYPE.SHUTDOWN_COMPLETE => $"{{ {"ConnectionShutdown"} = {SHUTDOWN_COMPLETE.ConnectionShutdown}, {"ConnectionShutdownByApp"} = {SHUTDOWN_COMPLETE.ConnectionShutdownByApp}, {"ConnectionClosedRemotely"} = {SHUTDOWN_COMPLETE.ConnectionClosedRemotely}, {"ConnectionErrorCode"} = {SHUTDOWN_COMPLETE.ConnectionErrorCode}, {"ConnectionCloseStatus"} = {SHUTDOWN_COMPLETE.ConnectionCloseStatus} }}", 
			QUIC_STREAM_EVENT_TYPE.IDEAL_SEND_BUFFER_SIZE => $"{{ {"ByteCount"} = {IDEAL_SEND_BUFFER_SIZE.ByteCount} }}", 
			_ => string.Empty, 
		};
	}
}
