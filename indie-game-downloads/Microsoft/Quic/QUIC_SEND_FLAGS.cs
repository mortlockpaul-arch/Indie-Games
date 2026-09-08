using System;

namespace Microsoft.Quic;

[Flags]
internal enum QUIC_SEND_FLAGS
{
	NONE = 0,
	ALLOW_0_RTT = 1,
	START = 2,
	FIN = 4,
	DGRAM_PRIORITY = 8,
	DELAY_SEND = 0x10
}
