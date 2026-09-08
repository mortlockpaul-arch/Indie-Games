using System;

namespace Microsoft.Quic;

[Flags]
internal enum QUIC_STREAM_SHUTDOWN_FLAGS
{
	NONE = 0,
	GRACEFUL = 1,
	ABORT_SEND = 2,
	ABORT_RECEIVE = 4,
	ABORT = ABORT_SEND | ABORT_RECEIVE,
	IMMEDIATE = 8,
	INLINE = 0x10
}
