using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Security.Cryptography;

[DebuggerDisplay("{Name,nq}")]
public sealed class MLKemAlgorithm : IEquatable<MLKemAlgorithm>
{
	public static MLKemAlgorithm MLKem512 { get; } = new MLKemAlgorithm("ML-KEM-512", 800, 1632, 768, "2.16.840.1.101.3.4.4.1");

	public static MLKemAlgorithm MLKem768 { get; } = new MLKemAlgorithm("ML-KEM-768", 1184, 2400, 1088, "2.16.840.1.101.3.4.4.2");

	public static MLKemAlgorithm MLKem1024 { get; } = new MLKemAlgorithm("ML-KEM-1024", 1568, 3168, 1568, "2.16.840.1.101.3.4.4.3");

	public string Name { get; }

	public int EncapsulationKeySizeInBytes { get; }

	public int DecapsulationKeySizeInBytes { get; }

	public int CiphertextSizeInBytes { get; }

	public int SharedSecretSizeInBytes { get; } = 32;

	public int PrivateSeedSizeInBytes { get; } = 64;

	internal string Oid { get; }

	private MLKemAlgorithm(string name, int encapsulationKeySizeInBytes, int decapsulationKeySizeInBytes, int ciphertextSizeInBytes, string oid)
	{
		Name = name;
		EncapsulationKeySizeInBytes = encapsulationKeySizeInBytes;
		DecapsulationKeySizeInBytes = decapsulationKeySizeInBytes;
		CiphertextSizeInBytes = ciphertextSizeInBytes;
		Oid = oid;
	}

	public bool Equals([NotNullWhen(true)] MLKemAlgorithm? other)
	{
		if ((object)other != null)
		{
			return other.Name == Name;
		}
		return false;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is MLKemAlgorithm mLKemAlgorithm)
		{
			return mLKemAlgorithm.Name == Name;
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

	public static bool operator ==(MLKemAlgorithm? left, MLKemAlgorithm? right)
	{
		return left?.Equals(right) ?? ((object)right == null);
	}

	public static bool operator !=(MLKemAlgorithm? left, MLKemAlgorithm? right)
	{
		return !(left == right);
	}

	internal static MLKemAlgorithm FromOid(string oid)
	{
		return oid switch
		{
			"2.16.840.1.101.3.4.4.1" => MLKem512, 
			"2.16.840.1.101.3.4.4.2" => MLKem768, 
			"2.16.840.1.101.3.4.4.3" => MLKem1024, 
			_ => null, 
		};
	}
}
