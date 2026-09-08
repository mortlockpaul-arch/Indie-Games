using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Security.Cryptography;

[DebuggerDisplay("{Name,nq}")]
[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public sealed class SlhDsaAlgorithm : IEquatable<SlhDsaAlgorithm>
{
	public string Name { get; }

	public int PrivateKeySizeInBytes { get; }

	public int PublicKeySizeInBytes { get; }

	public int SignatureSizeInBytes { get; }

	internal string Oid { get; }

	public static SlhDsaAlgorithm SlhDsaSha2_128s { get; } = new SlhDsaAlgorithm("SLH-DSA-SHA2-128s", 16, 7856, "2.16.840.1.101.3.4.3.20");

	public static SlhDsaAlgorithm SlhDsaShake128s { get; } = new SlhDsaAlgorithm("SLH-DSA-SHAKE-128s", 16, 7856, "2.16.840.1.101.3.4.3.26");

	public static SlhDsaAlgorithm SlhDsaSha2_128f { get; } = new SlhDsaAlgorithm("SLH-DSA-SHA2-128f", 16, 17088, "2.16.840.1.101.3.4.3.21");

	public static SlhDsaAlgorithm SlhDsaShake128f { get; } = new SlhDsaAlgorithm("SLH-DSA-SHAKE-128f", 16, 17088, "2.16.840.1.101.3.4.3.27");

	public static SlhDsaAlgorithm SlhDsaSha2_192s { get; } = new SlhDsaAlgorithm("SLH-DSA-SHA2-192s", 24, 16224, "2.16.840.1.101.3.4.3.22");

	public static SlhDsaAlgorithm SlhDsaShake192s { get; } = new SlhDsaAlgorithm("SLH-DSA-SHAKE-192s", 24, 16224, "2.16.840.1.101.3.4.3.28");

	public static SlhDsaAlgorithm SlhDsaSha2_192f { get; } = new SlhDsaAlgorithm("SLH-DSA-SHA2-192f", 24, 35664, "2.16.840.1.101.3.4.3.23");

	public static SlhDsaAlgorithm SlhDsaShake192f { get; } = new SlhDsaAlgorithm("SLH-DSA-SHAKE-192f", 24, 35664, "2.16.840.1.101.3.4.3.29");

	public static SlhDsaAlgorithm SlhDsaSha2_256s { get; } = new SlhDsaAlgorithm("SLH-DSA-SHA2-256s", 32, 29792, "2.16.840.1.101.3.4.3.24");

	public static SlhDsaAlgorithm SlhDsaShake256s { get; } = new SlhDsaAlgorithm("SLH-DSA-SHAKE-256s", 32, 29792, "2.16.840.1.101.3.4.3.30");

	public static SlhDsaAlgorithm SlhDsaSha2_256f { get; } = new SlhDsaAlgorithm("SLH-DSA-SHA2-256f", 32, 49856, "2.16.840.1.101.3.4.3.25");

	public static SlhDsaAlgorithm SlhDsaShake256f { get; } = new SlhDsaAlgorithm("SLH-DSA-SHAKE-256f", 32, 49856, "2.16.840.1.101.3.4.3.31");

	private SlhDsaAlgorithm(string name, int n, int signatureSizeInBytes, string oid)
	{
		Name = name;
		PrivateKeySizeInBytes = 4 * n;
		PublicKeySizeInBytes = 2 * n;
		SignatureSizeInBytes = signatureSizeInBytes;
		Oid = oid;
	}

	internal static SlhDsaAlgorithm GetAlgorithmFromOid(string oid)
	{
		return oid switch
		{
			"2.16.840.1.101.3.4.3.20" => SlhDsaSha2_128s, 
			"2.16.840.1.101.3.4.3.26" => SlhDsaShake128s, 
			"2.16.840.1.101.3.4.3.21" => SlhDsaSha2_128f, 
			"2.16.840.1.101.3.4.3.27" => SlhDsaShake128f, 
			"2.16.840.1.101.3.4.3.22" => SlhDsaSha2_192s, 
			"2.16.840.1.101.3.4.3.28" => SlhDsaShake192s, 
			"2.16.840.1.101.3.4.3.23" => SlhDsaSha2_192f, 
			"2.16.840.1.101.3.4.3.29" => SlhDsaShake192f, 
			"2.16.840.1.101.3.4.3.24" => SlhDsaSha2_256s, 
			"2.16.840.1.101.3.4.3.30" => SlhDsaShake256s, 
			"2.16.840.1.101.3.4.3.25" => SlhDsaSha2_256f, 
			"2.16.840.1.101.3.4.3.31" => SlhDsaShake256f, 
			_ => null, 
		};
	}

	public bool Equals([NotNullWhen(true)] SlhDsaAlgorithm? other)
	{
		if ((object)other != null)
		{
			return other.Name == Name;
		}
		return false;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is SlhDsaAlgorithm slhDsaAlgorithm)
		{
			return slhDsaAlgorithm.Name == Name;
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

	public static bool operator ==(SlhDsaAlgorithm? left, SlhDsaAlgorithm? right)
	{
		return left?.Equals(right) ?? ((object)right == null);
	}

	public static bool operator !=(SlhDsaAlgorithm? left, SlhDsaAlgorithm? right)
	{
		return !(left == right);
	}
}
