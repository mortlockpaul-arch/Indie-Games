namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs12KeyBag : Pkcs12SafeBag
{
	public Pkcs12KeyBag(ReadOnlyMemory<byte> pkcs8PrivateKey, bool skipCopy = false)
		: base("1.2.840.113549.1.12.10.1.1", pkcs8PrivateKey, skipCopy)
	{
	}
}
