using System;

namespace Microsoft.Quic;

[Flags]
internal enum QUIC_STREAM_START_FLAGS
{
	NONE = 0,
	IMMEDIATE = 1,
	FAIL_BLOCKED = 2,
	SHUTDOWN_ON_FAIL = 4,
	INDICATE_PEER_ACCEPT = 8
}
