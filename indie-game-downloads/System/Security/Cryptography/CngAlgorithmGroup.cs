using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography;

public sealed class CngAlgorithmGroup : IEquatable<CngAlgorithmGroup>
{
	[CompilerGenerated]
	private static CngAlgorithmGroup _003CMLDsa_003Ek__BackingField;

	[CompilerGenerated]
	private static CngAlgorithmGroup _003CMLKem_003Ek__BackingField;

	[CompilerGenerated]
	private static CngAlgorithmGroup _003CSlhDsa_003Ek__BackingField;

	private static CngAlgorithmGroup s_dh;

	private static CngAlgorithmGroup s_dsa;

	private static CngAlgorithmGroup s_ecdh;

	private static CngAlgorithmGroup s_ecdsa;

	private static CngAlgorithmGroup s_rsa;

	private readonly string _algorithmGroup;

	public string AlgorithmGroup => _algorithmGroup;

	public static CngAlgorithmGroup DiffieHellman => s_dh ?? (s_dh = new CngAlgorithmGroup("DH"));

	public static CngAlgorithmGroup Dsa => s_dsa ?? (s_dsa = new CngAlgorithmGroup("DSA"));

	public static CngAlgorithmGroup ECDiffieHellman => s_ecdh ?? (s_ecdh = new CngAlgorithmGroup("ECDH"));

	public static CngAlgorithmGroup ECDsa => s_ecdsa ?? (s_ecdsa = new CngAlgorithmGroup("ECDSA"));

	public static CngAlgorithmGroup Rsa => s_rsa ?? (s_rsa = new CngAlgorithmGroup("RSA"));

	public static CngAlgorithmGroup MLDsa => _003CMLDsa_003Ek__BackingField ?? (_003CMLDsa_003Ek__BackingField = new CngAlgorithmGroup("MLDSA"));

	public static CngAlgorithmGroup MLKem => _003CMLKem_003Ek__BackingField ?? (_003CMLKem_003Ek__BackingField = new CngAlgorithmGroup("MLKEM"));

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static CngAlgorithmGroup SlhDsa => _003CSlhDsa_003Ek__BackingField ?? (_003CSlhDsa_003Ek__BackingField = new CngAlgorithmGroup("SLHDSA"));

	public CngAlgorithmGroup(string algorithmGroup)
	{
		ArgumentException.ThrowIfNullOrEmpty(algorithmGroup, "algorithmGroup");
		_algorithmGroup = algorithmGroup;
	}

	public static bool operator ==(CngAlgorithmGroup? left, CngAlgorithmGroup? right)
	{
		return left?.Equals(right) ?? ((object)right == null);
	}

	public static bool operator !=(CngAlgorithmGroup? left, CngAlgorithmGroup? right)
	{
		if ((object)left == null)
		{
			return (object)right != null;
		}
		return !left.Equals(right);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return Equals(obj as CngAlgorithmGroup);
	}

	public bool Equals([NotNullWhen(true)] CngAlgorithmGroup? other)
	{
		if ((object)other == null)
		{
			return false;
		}
		return _algorithmGroup.Equals(other.AlgorithmGroup);
	}

	public override int GetHashCode()
	{
		return _algorithmGroup.GetHashCode();
	}

	public override string ToString()
	{
		return _algorithmGroup;
	}
}
