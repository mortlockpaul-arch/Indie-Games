namespace Microsoft.Quic;

internal struct QUIC_CERTIFICATE_HASH_STORE
{
	internal QUIC_CERTIFICATE_HASH_STORE_FLAGS Flags;

	internal unsafe fixed byte ShaHash[20];

	internal unsafe fixed sbyte StoreName[128];
}
