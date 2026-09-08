namespace System.Security.Authentication;

[Obsolete("KeyExchangeAlgorithm, KeyExchangeStrength, CipherAlgorithm, CipherStrength, HashAlgorithm and HashStrength properties of SslStream are obsolete. Use NegotiatedCipherSuite instead.", DiagnosticId = "SYSLIB0058", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public enum HashAlgorithmType
{
	None = 0,
	Md5 = 32771,
	Sha1 = 32772,
	Sha256 = 32780,
	Sha384 = 32781,
	Sha512 = 32782
}
