using System.Diagnostics.CodeAnalysis;

namespace System.Security.Cryptography;

[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
internal sealed class SlhDsaImplementation : SlhDsa
{
	internal static SlhDsaImplementation GenerateKeyCore(SlhDsaAlgorithm algorithm)
	{
		throw new PlatformNotSupportedException();
	}

	internal static SlhDsaImplementation ImportPublicKey(SlhDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		throw new PlatformNotSupportedException();
	}

	internal static SlhDsaImplementation ImportPrivateKey(SlhDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		throw new PlatformNotSupportedException();
	}

	protected override void SignDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override bool VerifyDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, ReadOnlySpan<byte> signature)
	{
		throw new PlatformNotSupportedException();
	}

	protected override void SignPreHashCore(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> context, string hashAlgorithmOid, Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override bool VerifyPreHashCore(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> context, string hashAlgorithmOid, ReadOnlySpan<byte> signature)
	{
		throw new PlatformNotSupportedException();
	}

	protected override void ExportSlhDsaPublicKeyCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override void ExportSlhDsaPrivateKeyCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}
}
