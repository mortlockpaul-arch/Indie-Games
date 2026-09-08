using System;

namespace Microsoft.Quic;

[Flags]
internal enum QUIC_ALLOWED_CIPHER_SUITE_FLAGS
{
	NONE = 0,
	AES_128_GCM_SHA256 = 1,
	AES_256_GCM_SHA384 = 2,
	CHACHA20_POLY1305_SHA256 = 4
}
