namespace Microsoft.Quic;

internal struct QUIC_CERTIFICATE_FILE_PROTECTED
{
	internal unsafe sbyte* PrivateKeyFile;

	internal unsafe sbyte* CertificateFile;

	internal unsafe sbyte* PrivateKeyPassword;
}
