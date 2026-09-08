using System;

namespace Microsoft.Quic;

[Flags]
internal enum QUIC_STREAM_OPEN_FLAGS
{
	NONE = 0,
	UNIDIRECTIONAL = 1,
	ZERO_RTT = 2,
	DELAY_ID_FC_UPDATES = 4
}
