namespace Microsoft.Quic;

internal struct QUIC_CERTIFICATE_FILE
{
	internal unsafe sbyte* PrivateKeyFile;

	internal unsafe sbyte* CertificateFile;
}
