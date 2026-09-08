namespace Microsoft.Quic;

internal struct QUIC_CERTIFICATE_PKCS12
{
	internal unsafe byte* Asn1Blob;

	internal uint Asn1BlobLength;

	internal unsafe sbyte* PrivateKeyPassword;
}
