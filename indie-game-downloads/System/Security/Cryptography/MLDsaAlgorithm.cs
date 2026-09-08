using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Security.Cryptography;

[DebuggerDisplay("{Name,nq}")]
public sealed class MLDsaAlgorithm : IEquatable<MLDsaAlgorithm>
{
	public string Name { get; }

	public int PrivateKeySizeInBytes { get; }

	public int PrivateSeedSizeInBytes => 32;

	public int PublicKeySizeInBytes { get; }

	public int SignatureSizeInBytes { get; }

	public int MuSizeInBytes => 64;

	internal string Oid { get; }

	internal int LambdaCollisionStrength { get; }

	public static MLDsaAlgorithm MLDsa44 { get; } = new MLDsaAlgorithm("ML-DSA-44", 2560, 1312, 2420, 128, "2.16.840.1.101.3.4.3.17");

	public static MLDsaAlgorithm MLDsa65 { get; } = new MLDsaAlgorithm("ML-DSA-65", 4032, 1952, 3309, 192, "2.16.840.1.101.3.4.3.18");

	public static MLDsaAlgorithm MLDsa87 { get; } = new MLDsaAlgorithm("ML-DSA-87", 4896, 2592, 4627, 256, "2.16.840.1.101.3.4.3.19");

	private MLDsaAlgorithm(string name, int privateKeySizeInBytes, int publicKeySizeInBytes, int signatureSizeInBytes, int lambdaCollisionStrength, string oid)
	{
		Name = name;
		PrivateKeySizeInBytes = privateKeySizeInBytes;
		PublicKeySizeInBytes = publicKeySizeInBytes;
		SignatureSizeInBytes = signatureSizeInBytes;
		LambdaCollisionStrength = lambdaCollisionStrength;
		Oid = oid;
	}

	internal static MLDsaAlgorithm GetMLDsaAlgorithmFromOid(string oid)
	{
		return oid switch
		{
			"2.16.840.1.101.3.4.3.17" => MLDsa44, 
			"2.16.840.1.101.3.4.3.18" => MLDsa65, 
			"2.16.840.1.101.3.4.3.19" => MLDsa87, 
			_ => null, 
		};
	}

	public bool Equals([NotNullWhen(true)] MLDsaAlgorithm? other)
	{
		if ((object)other != null)
		{
			return other.Name == Name;
		}
		return false;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is MLDsaAlgorithm mLDsaAlgorithm)
		{
			return mLDsaAlgorithm.Name == Name;
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

	public static bool operator ==(MLDsaAlgorithm? left, MLDsaAlgorithm? right)
	{
		return left?.Equals(right) ?? ((object)right == null);
	}

	public static bool operator !=(MLDsaAlgorithm? left, MLDsaAlgorithm? right)
	{
		return !(left == right);
	}
}
