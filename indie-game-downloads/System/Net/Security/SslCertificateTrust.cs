using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security;

public sealed class SslCertificateTrust
{
	internal X509Store _store;

	internal X509Certificate2Collection _trustList;

	internal bool _sendTrustInHandshake;

	public static SslCertificateTrust CreateForX509Store(X509Store store, bool sendTrustInHandshake = false)
	{
		if (sendTrustInHandshake && store.Location != StoreLocation.LocalMachine)
		{
			throw new PlatformNotSupportedException(System.SR.net_ssl_trust_store);
		}
		if (sendTrustInHandshake && !OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS() && !OperatingSystem.IsWindowsVersionAtLeast(6, 2))
		{
			throw new PlatformNotSupportedException(System.SR.net_ssl_trust_handshake);
		}
		if (!store.IsOpen)
		{
			store.Open(OpenFlags.OpenExistingOnly);
		}
		return new SslCertificateTrust
		{
			_store = store,
			_sendTrustInHandshake = sendTrustInHandshake
		};
	}

	public static SslCertificateTrust CreateForX509Collection(X509Certificate2Collection trustList, bool sendTrustInHandshake = false)
	{
		if (sendTrustInHandshake)
		{
			throw new PlatformNotSupportedException(System.SR.net_ssl_trust_collection);
		}
		return new SslCertificateTrust
		{
			_trustList = trustList,
			_sendTrustInHandshake = sendTrustInHandshake
		};
	}

	private SslCertificateTrust()
	{
	}
}
