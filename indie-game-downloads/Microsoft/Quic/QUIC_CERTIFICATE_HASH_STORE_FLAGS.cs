using System;

namespace Microsoft.Quic;

[Flags]
internal enum QUIC_CERTIFICATE_HASH_STORE_FLAGS
{
	NONE = 0,
	MACHINE_STORE = 1
}
