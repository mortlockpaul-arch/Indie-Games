using System;

namespace Microsoft.Quic;

[Flags]
internal enum QUIC_RECEIVE_FLAGS
{
	NONE = 0,
	ZERO_RTT = 1,
	FIN = 2
}
