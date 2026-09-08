namespace System.Security.Cryptography.X509Certificates;

public class X509ChainElement
{
	public X509Certificate2 Certificate { get; }

	public X509ChainStatus[] ChainElementStatus { get; }

	public string Information { get; }

	internal X509ChainElement(X509Certificate2 certificate, X509ChainStatus[] chainElementStatus, string information)
	{
		Certificate = certificate;
		ChainElementStatus = chainElementStatus;
		Information = information;
	}
}
