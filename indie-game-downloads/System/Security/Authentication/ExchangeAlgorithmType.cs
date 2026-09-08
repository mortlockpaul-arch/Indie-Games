namespace System.Security.Authentication;

[Obsolete("KeyExchangeAlgorithm, KeyExchangeStrength, CipherAlgorithm, CipherStrength, HashAlgorithm and HashStrength properties of SslStream are obsolete. Use NegotiatedCipherSuite instead.", DiagnosticId = "SYSLIB0058", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public enum ExchangeAlgorithmType
{
	None = 0,
	RsaSign = 9216,
	RsaKeyX = 41984,
	DiffieHellman = 43522
}
