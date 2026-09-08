using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace System.Security.Cryptography;

[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public sealed class CompositeMLDsaCng : CompositeMLDsa
{
	[SupportedOSPlatform("windows")]
	public CompositeMLDsaCng(CngKey key)
		: base(AlgorithmFromHandle(key))
	{
		throw new PlatformNotSupportedException();
	}

	private static CompositeMLDsaAlgorithm AlgorithmFromHandle(CngKey key)
	{
		throw new PlatformNotSupportedException();
	}

	public CngKey GetKey()
	{
		throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "CompositeMLDsa"));
	}

	protected override int SignDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, Span<byte> destination)
	{
		throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "CompositeMLDsa"));
	}

	protected override int ExportCompositeMLDsaPrivateKeyCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "CompositeMLDsa"));
	}

	protected override int ExportCompositeMLDsaPublicKeyCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "CompositeMLDsa"));
	}

	protected override bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten)
	{
		throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "CompositeMLDsa"));
	}

	protected override bool VerifyDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, ReadOnlySpan<byte> signature)
	{
		throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "CompositeMLDsa"));
	}
}
