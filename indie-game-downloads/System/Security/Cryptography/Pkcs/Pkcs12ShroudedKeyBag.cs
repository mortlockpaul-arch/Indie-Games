namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs12ShroudedKeyBag : Pkcs12SafeBag
{
	public ReadOnlyMemory<byte> EncryptedPkcs8PrivateKey => base.EncodedBagValue;

	public Pkcs12ShroudedKeyBag(ReadOnlyMemory<byte> encryptedPkcs8PrivateKey, bool skipCopy = false)
		: base("1.2.840.113549.1.12.10.1.2", encryptedPkcs8PrivateKey, skipCopy)
	{
	}
}
