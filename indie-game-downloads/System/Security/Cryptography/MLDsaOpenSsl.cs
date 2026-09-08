using System.Runtime.Versioning;

namespace System.Security.Cryptography;

public sealed class MLDsaOpenSsl : MLDsa
{
	private SafeEvpPKeyHandle _key;

	private bool _hasSeed;

	private bool _hasPrivateKey;

	[UnsupportedOSPlatform("android")]
	[UnsupportedOSPlatform("browser")]
	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("osx")]
	[UnsupportedOSPlatform("tvos")]
	[UnsupportedOSPlatform("windows")]
	public MLDsaOpenSsl(SafeEvpPKeyHandle pkeyHandle)
		: base(AlgorithmFromHandle(pkeyHandle, out var upRefHandle, out var hasSeed, out var hasPrivateKey))
	{
		_key = upRefHandle;
		_hasSeed = hasSeed;
		_hasPrivateKey = hasPrivateKey;
	}

	private static MLDsaAlgorithm AlgorithmFromHandle(SafeEvpPKeyHandle pkeyHandle, out SafeEvpPKeyHandle upRefHandle, out bool hasSeed, out bool hasPrivateKey)
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

	protected override void SignMuCore(ReadOnlySpan<byte> externalMu, Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override bool VerifyMuCore(ReadOnlySpan<byte> externalMu, ReadOnlySpan<byte> signature)
	{
		throw new PlatformNotSupportedException();
	}

	protected override void ExportMLDsaPublicKeyCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override void ExportMLDsaPrivateKeyCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override void ExportMLDsaPrivateSeedCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten)
	{
		throw new PlatformNotSupportedException();
	}
}
