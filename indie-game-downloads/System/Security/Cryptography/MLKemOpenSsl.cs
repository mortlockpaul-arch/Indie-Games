using System.Runtime.Versioning;

namespace System.Security.Cryptography;

public sealed class MLKemOpenSsl : MLKem
{
	private readonly SafeEvpPKeyHandle _key;

	private readonly bool _hasSeed;

	private readonly bool _hasDecapsulationKey;

	[UnsupportedOSPlatform("android")]
	[UnsupportedOSPlatform("browser")]
	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("osx")]
	[UnsupportedOSPlatform("tvos")]
	[UnsupportedOSPlatform("windows")]
	public MLKemOpenSsl(SafeEvpPKeyHandle pkeyHandle)
		: base(AlgorithmFromHandle(pkeyHandle, out var upRefHandle, out var hasSeed, out var hasDecapsulationKey))
	{
		_key = upRefHandle;
		_hasSeed = hasSeed;
		_hasDecapsulationKey = hasDecapsulationKey;
	}

	private static MLKemAlgorithm AlgorithmFromHandle(SafeEvpPKeyHandle pkeyHandle, out SafeEvpPKeyHandle upRefHandle, out bool hasSeed, out bool hasDecapsulationKey)
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

	protected override void DecapsulateCore(ReadOnlySpan<byte> ciphertext, Span<byte> sharedSecret)
	{
		throw new PlatformNotSupportedException();
	}

	protected override void EncapsulateCore(Span<byte> ciphertext, Span<byte> sharedSecret)
	{
		throw new PlatformNotSupportedException();
	}

	protected override void ExportPrivateSeedCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override void ExportDecapsulationKeyCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override void ExportEncapsulationKeyCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten)
	{
		throw new PlatformNotSupportedException();
	}
}
