using System;

namespace Microsoft.Quic;

[Flags]
internal enum QUIC_CONNECTION_SHUTDOWN_FLAGS
{
	NONE = 0,
	SILENT = 1
}
