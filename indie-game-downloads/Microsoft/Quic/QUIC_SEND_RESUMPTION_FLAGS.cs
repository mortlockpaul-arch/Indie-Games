using System;

namespace Microsoft.Quic;

[Flags]
internal enum QUIC_SEND_RESUMPTION_FLAGS
{
	NONE = 0,
	FINAL = 1
}
