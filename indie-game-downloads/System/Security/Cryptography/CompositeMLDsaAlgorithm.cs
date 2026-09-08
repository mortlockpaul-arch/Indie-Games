using System.Diagnostics.CodeAnalysis;

namespace System.Security.Cryptography;

[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public sealed class CompositeMLDsaAlgorithm : IEquatable<CompositeMLDsaAlgorithm>
{
	public string Name { get; }

	public int MaxSignatureSizeInBytes { get; }

	internal int MinPrivateKeySizeInBytes { get; }

	internal int MaxPrivateKeySizeInBytes { get; }

	internal int MinPublicKeySizeInBytes { get; }

	internal int MaxPublicKeySizeInBytes { get; }

	internal int MinSignatureSizeInBytes { get; }

	internal string Oid { get; }

	public static CompositeMLDsaAlgorithm MLDsa44WithRSA2048Pss { get; } = CreateRsa("MLDSA44-RSA2048-PSS-SHA256", MLDsaAlgorithm.MLDsa44, 2048, "1.3.6.1.5.5.7.6.37");

	public static CompositeMLDsaAlgorithm MLDsa44WithRSA2048Pkcs15 { get; } = CreateRsa("MLDSA44-RSA2048-PKCS15-SHA256", MLDsaAlgorithm.MLDsa44, 2048, "1.3.6.1.5.5.7.6.38");

	public static CompositeMLDsaAlgorithm MLDsa44WithEd25519 { get; } = CreateEdDsa("MLDSA44-Ed25519-SHA512", MLDsaAlgorithm.MLDsa44, 256, "1.3.6.1.5.5.7.6.39");

	public static CompositeMLDsaAlgorithm MLDsa44WithECDsaP256 { get; } = CreateECDsa("MLDSA44-ECDSA-P256-SHA256", MLDsaAlgorithm.MLDsa44, 256, "1.3.6.1.5.5.7.6.40");

	public static CompositeMLDsaAlgorithm MLDsa65WithRSA3072Pss { get; } = CreateRsa("MLDSA65-RSA3072-PSS-SHA512", MLDsaAlgorithm.MLDsa65, 3072, "1.3.6.1.5.5.7.6.41");

	public static CompositeMLDsaAlgorithm MLDsa65WithRSA3072Pkcs15 { get; } = CreateRsa("MLDSA65-RSA3072-PKCS15-SHA512", MLDsaAlgorithm.MLDsa65, 3072, "1.3.6.1.5.5.7.6.42");

	public static CompositeMLDsaAlgorithm MLDsa65WithRSA4096Pss { get; } = CreateRsa("MLDSA65-RSA4096-PSS-SHA512", MLDsaAlgorithm.MLDsa65, 4096, "1.3.6.1.5.5.7.6.43");

	public static CompositeMLDsaAlgorithm MLDsa65WithRSA4096Pkcs15 { get; } = CreateRsa("MLDSA65-RSA4096-PKCS15-SHA512", MLDsaAlgorithm.MLDsa65, 4096, "1.3.6.1.5.5.7.6.44");

	public static CompositeMLDsaAlgorithm MLDsa65WithECDsaP256 { get; } = CreateECDsa("MLDSA65-ECDSA-P256-SHA512", MLDsaAlgorithm.MLDsa65, 256, "1.3.6.1.5.5.7.6.45");

	public static CompositeMLDsaAlgorithm MLDsa65WithECDsaP384 { get; } = CreateECDsa("MLDSA65-ECDSA-P384-SHA512", MLDsaAlgorithm.MLDsa65, 384, "1.3.6.1.5.5.7.6.46");

	public static CompositeMLDsaAlgorithm MLDsa65WithECDsaBrainpoolP256r1 { get; } = CreateECDsa("MLDSA65-ECDSA-brainpoolP256r1-SHA512", MLDsaAlgorithm.MLDsa65, 256, "1.3.6.1.5.5.7.6.47");

	public static CompositeMLDsaAlgorithm MLDsa65WithEd25519 { get; } = CreateEdDsa("MLDSA65-Ed25519-SHA512", MLDsaAlgorithm.MLDsa65, 256, "1.3.6.1.5.5.7.6.48");

	public static CompositeMLDsaAlgorithm MLDsa87WithECDsaP384 { get; } = CreateECDsa("MLDSA87-ECDSA-P384-SHA512", MLDsaAlgorithm.MLDsa87, 384, "1.3.6.1.5.5.7.6.49");

	public static CompositeMLDsaAlgorithm MLDsa87WithECDsaBrainpoolP384r1 { get; } = CreateECDsa("MLDSA87-ECDSA-brainpoolP384r1-SHA512", MLDsaAlgorithm.MLDsa87, 384, "1.3.6.1.5.5.7.6.50");

	public static CompositeMLDsaAlgorithm MLDsa87WithEd448 { get; } = CreateEdDsa("MLDSA87-Ed448-SHAKE256", MLDsaAlgorithm.MLDsa87, 456, "1.3.6.1.5.5.7.6.51");

	public static CompositeMLDsaAlgorithm MLDsa87WithRSA3072Pss { get; } = CreateRsa("MLDSA87-RSA3072-PSS-SHA512", MLDsaAlgorithm.MLDsa87, 3072, "1.3.6.1.5.5.7.6.52");

	public static CompositeMLDsaAlgorithm MLDsa87WithRSA4096Pss { get; } = CreateRsa("MLDSA87-RSA4096-PSS-SHA512", MLDsaAlgorithm.MLDsa87, 4096, "1.3.6.1.5.5.7.6.53");

	public static CompositeMLDsaAlgorithm MLDsa87WithECDsaP521 { get; } = CreateECDsa("MLDSA87-ECDSA-P521-SHA512", MLDsaAlgorithm.MLDsa87, 521, "1.3.6.1.5.5.7.6.54");

	private CompositeMLDsaAlgorithm(string name, int minPrivateKeySizeInBytes, int maxPrivateKeySizeInBytes, int minPublicKeySizeInBytes, int maxPublicKeySizeInBytes, int minSignatureSize, int maxSignatureSize, string oid)
	{
		Name = name;
		MinPrivateKeySizeInBytes = minPrivateKeySizeInBytes;
		MaxPrivateKeySizeInBytes = maxPrivateKeySizeInBytes;
		MinPublicKeySizeInBytes = minPublicKeySizeInBytes;
		MaxPublicKeySizeInBytes = maxPublicKeySizeInBytes;
		MinSignatureSizeInBytes = minSignatureSize;
		MaxSignatureSizeInBytes = maxSignatureSize;
		Oid = oid;
	}

	internal bool IsValidPrivateKeySize(int size)
	{
		if (MinPrivateKeySizeInBytes <= size)
		{
			return size <= MaxPrivateKeySizeInBytes;
		}
		return false;
	}

	internal bool IsValidPublicKeySize(int size)
	{
		if (MinPublicKeySizeInBytes <= size)
		{
			return size <= MaxPublicKeySizeInBytes;
		}
		return false;
	}

	internal bool IsValidSignatureSize(int size)
	{
		if (MinSignatureSizeInBytes <= size)
		{
			return size <= MaxSignatureSizeInBytes;
		}
		return false;
	}

	public bool Equals([NotNullWhen(true)] CompositeMLDsaAlgorithm? other)
	{
		if ((object)other != null)
		{
			return other.Name == Name;
		}
		return false;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is CompositeMLDsaAlgorithm compositeMLDsaAlgorithm)
		{
			return compositeMLDsaAlgorithm.Name == Name;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}

	public override string ToString()
	{
		return Name;
	}

	public static bool operator ==(CompositeMLDsaAlgorithm? left, CompositeMLDsaAlgorithm? right)
	{
		return left?.Equals(right) ?? ((object)right == null);
	}

	public static bool operator !=(CompositeMLDsaAlgorithm? left, CompositeMLDsaAlgorithm? right)
	{
		return !(left == right);
	}

	internal static CompositeMLDsaAlgorithm GetAlgorithmFromOid(string oid)
	{
		return oid switch
		{
			"1.3.6.1.5.5.7.6.37" => MLDsa44WithRSA2048Pss, 
			"1.3.6.1.5.5.7.6.38" => MLDsa44WithRSA2048Pkcs15, 
			"1.3.6.1.5.5.7.6.39" => MLDsa44WithEd25519, 
			"1.3.6.1.5.5.7.6.40" => MLDsa44WithECDsaP256, 
			"1.3.6.1.5.5.7.6.41" => MLDsa65WithRSA3072Pss, 
			"1.3.6.1.5.5.7.6.42" => MLDsa65WithRSA3072Pkcs15, 
			"1.3.6.1.5.5.7.6.43" => MLDsa65WithRSA4096Pss, 
			"1.3.6.1.5.5.7.6.44" => MLDsa65WithRSA4096Pkcs15, 
			"1.3.6.1.5.5.7.6.45" => MLDsa65WithECDsaP256, 
			"1.3.6.1.5.5.7.6.46" => MLDsa65WithECDsaP384, 
			"1.3.6.1.5.5.7.6.47" => MLDsa65WithECDsaBrainpoolP256r1, 
			"1.3.6.1.5.5.7.6.48" => MLDsa65WithEd25519, 
			"1.3.6.1.5.5.7.6.49" => MLDsa87WithECDsaP384, 
			"1.3.6.1.5.5.7.6.50" => MLDsa87WithECDsaBrainpoolP384r1, 
			"1.3.6.1.5.5.7.6.51" => MLDsa87WithEd448, 
			"1.3.6.1.5.5.7.6.52" => MLDsa87WithRSA3072Pss, 
			"1.3.6.1.5.5.7.6.53" => MLDsa87WithRSA4096Pss, 
			"1.3.6.1.5.5.7.6.54" => MLDsa87WithECDsaP521, 
			_ => null, 
		};
	}

	private static CompositeMLDsaAlgorithm CreateRsa(string name, MLDsaAlgorithm mldsaAlgorithm, int keySizeInBits, string oid)
	{
		int num = keySizeInBits / 8;
		int num2 = num + 1;
		int num3 = (num + 1) / 2 + 1;
		int num4 = 33;
		int num5 = 6 + (6 + num2 + 6 + num4);
		int num6 = 6 + (13 + num2 + 6 + num4 + 6 + num2 + 6 + num3 + 6 + num3 + 6 + num3 + 6 + num3 + 6 + num3);
		return new CompositeMLDsaAlgorithm(name, mldsaAlgorithm.PrivateSeedSizeInBytes + num, mldsaAlgorithm.PrivateSeedSizeInBytes + num6, mldsaAlgorithm.PublicKeySizeInBytes + num, mldsaAlgorithm.PublicKeySizeInBytes + num5, mldsaAlgorithm.SignatureSizeInBytes + num, mldsaAlgorithm.SignatureSizeInBytes + num, oid);
	}

	private static CompositeMLDsaAlgorithm CreateECDsa(string name, MLDsaAlgorithm mldsaAlgorithm, int keySizeInBits, string oid)
	{
		int num = (keySizeInBits + 7) / 8;
		int num2 = 3;
		int num3 = 1 + GetDerLengthLength(num) + num;
		int num4;
		switch (oid)
		{
		case "1.3.6.1.5.5.7.6.40":
		case "1.3.6.1.5.5.7.6.45":
			num4 = 10;
			break;
		case "1.3.6.1.5.5.7.6.46":
		case "1.3.6.1.5.5.7.6.49":
			num4 = 7;
			break;
		case "1.3.6.1.5.5.7.6.54":
			num4 = 7;
			break;
		case "1.3.6.1.5.5.7.6.47":
			num4 = 11;
			break;
		case "1.3.6.1.5.5.7.6.50":
			num4 = 11;
			break;
		default:
			num4 = AssertAndThrow(oid);
			break;
		}
		int num5 = num4;
		int num6 = 1 + GetDerLengthLength(num5) + num5;
		int num7 = 1 + GetDerLengthLength(num2 + num3 + num6) + num2 + num3 + num6;
		return new CompositeMLDsaAlgorithm(name, mldsaAlgorithm.PrivateSeedSizeInBytes + num7, mldsaAlgorithm.PrivateSeedSizeInBytes + num7, mldsaAlgorithm.PublicKeySizeInBytes + 1 + 2 * num, mldsaAlgorithm.PublicKeySizeInBytes + 1 + 2 * num, mldsaAlgorithm.SignatureSizeInBytes + 2 + 6, mldsaAlgorithm.SignatureSizeInBytes + AsymmetricAlgorithmHelpers.GetMaxDerSignatureSize(keySizeInBits), oid);
		static int AssertAndThrow(string text)
		{
			throw new CryptographicException();
		}
	}

	private static CompositeMLDsaAlgorithm CreateEdDsa(string name, MLDsaAlgorithm mldsaAlgorithm, int keySizeInBits, string oid)
	{
		int num = keySizeInBits / 8;
		return new CompositeMLDsaAlgorithm(name, mldsaAlgorithm.PrivateSeedSizeInBytes + num, mldsaAlgorithm.PrivateSeedSizeInBytes + num, mldsaAlgorithm.PublicKeySizeInBytes + num, mldsaAlgorithm.PublicKeySizeInBytes + num, mldsaAlgorithm.SignatureSizeInBytes + 2 * num, mldsaAlgorithm.SignatureSizeInBytes + 2 * num, oid);
	}

	private static int GetDerLengthLength(int payloadLength)
	{
		if (payloadLength <= 127)
		{
			return 1;
		}
		if (payloadLength <= 255)
		{
			return 2;
		}
		if (payloadLength <= 65535)
		{
			return 3;
		}
		if (payloadLength <= 16777215)
		{
			return 4;
		}
		return 5;
	}
}
