namespace System.Security.Authentication;

[Obsolete("KeyExchangeAlgorithm, KeyExchangeStrength, CipherAlgorithm, CipherStrength, HashAlgorithm and HashStrength properties of SslStream are obsolete. Use NegotiatedCipherSuite instead.", DiagnosticId = "SYSLIB0058", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public enum CipherAlgorithmType
{
	None = 0,
	Rc2 = 26114,
	Rc4 = 26625,
	Des = 26113,
	TripleDes = 26115,
	Aes = 26129,
	Aes128 = 26126,
	Aes192 = 26127,
	Aes256 = 26128,
	Null = 24576
}
