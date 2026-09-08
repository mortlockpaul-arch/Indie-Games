namespace System.Security.Cryptography.X509Certificates.Asn1;

[Flags]
internal enum KeyUsageFlagsAsn
{
	None = 0,
	DigitalSignature = 1,
	NonRepudiation = 2,
	KeyEncipherment = 4,
	DataEncipherment = 8,
	KeyAgreement = 0x10,
	KeyCertSign = 0x20,
	CrlSign = 0x40,
	EncipherOnly = 0x80,
	DecipherOnly = 0x100
}
