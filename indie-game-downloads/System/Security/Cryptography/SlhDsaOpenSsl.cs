using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace System.Security.Cryptography;

[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public sealed class SlhDsaOpenSsl : SlhDsa
{
	private readonly SafeEvpPKeyHandle _key;

	[UnsupportedOSPlatform("android")]
	[UnsupportedOSPlatform("browser")]
	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("osx")]
	[UnsupportedOSPlatform("tvos")]
	[UnsupportedOSPlatform("windows")]
	public SlhDsaOpenSsl(SafeEvpPKeyHandle pkeyHandle)
		: base(AlgorithmFromHandle(pkeyHandle, out var upRefHandle))
	{
		_key = upRefHandle;
	}

	private static SlhDsaAlgorithm AlgorithmFromHandle(SafeEvpPKeyHandle pkeyHandle, out SafeEvpPKeyHandle upRefHandle)
	{
		throw new PlatformNotSupportedException();
	}

	public SafeEvpPKeyHandle DuplicateKeyHandle()
	{
		throw new PlatformNotSupportedException();
	}

	protected override void Dispose(bool disposing)
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

	protected override bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten)
	{
		throw new PlatformNotSupportedException();
	}
}
