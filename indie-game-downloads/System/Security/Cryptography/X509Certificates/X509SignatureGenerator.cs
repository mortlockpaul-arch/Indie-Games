using System.Diagnostics.CodeAnalysis;

namespace System.Security.Cryptography.X509Certificates;

public abstract class X509SignatureGenerator
{
	private PublicKey _publicKey;

	public PublicKey PublicKey => _publicKey ?? (_publicKey = BuildPublicKey());

	public abstract byte[] GetSignatureAlgorithmIdentifier(HashAlgorithmName hashAlgorithm);

	public abstract byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm);

	protected abstract PublicKey BuildPublicKey();

	public static X509SignatureGenerator CreateForECDsa(ECDsa key)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		return new ECDsaX509SignatureGenerator(key);
	}

	public static X509SignatureGenerator CreateForRSA(RSA key, RSASignaturePadding signaturePadding)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		ArgumentNullException.ThrowIfNull(signaturePadding, "signaturePadding");
		if (signaturePadding == RSASignaturePadding.Pkcs1)
		{
			return new RSAPkcs1X509SignatureGenerator(key);
		}
		if (signaturePadding.Mode == RSASignaturePaddingMode.Pss)
		{
			return new RSAPssX509SignatureGenerator(key, signaturePadding);
		}
		throw new ArgumentException(System.SR.Cryptography_InvalidPaddingMode);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static X509SignatureGenerator CreateForMLDsa(MLDsa key)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		return new MLDsaX509SignatureGenerator(key);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static X509SignatureGenerator CreateForSlhDsa(SlhDsa key)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		return new SlhDsaX509SignatureGenerator(key);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static X509SignatureGenerator CreateForCompositeMLDsa(CompositeMLDsa key)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		throw new PlatformNotSupportedException();
	}
}
