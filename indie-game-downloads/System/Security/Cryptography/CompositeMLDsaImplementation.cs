using Internal.Cryptography;

namespace System.Security.Cryptography;

internal sealed class CompositeMLDsaImplementation : CompositeMLDsa
{
	internal static bool SupportsAny()
	{
		if (!Helpers.IsOSPlatformWindows)
		{
			return false;
		}
		return CompositeMLDsaManaged.SupportsAny();
	}

	internal static bool IsAlgorithmSupportedImpl(CompositeMLDsaAlgorithm algorithm)
	{
		if (!Helpers.IsOSPlatformWindows)
		{
			return false;
		}
		return CompositeMLDsaManaged.IsAlgorithmSupportedImpl(algorithm);
	}

	internal static CompositeMLDsa GenerateKeyImpl(CompositeMLDsaAlgorithm algorithm)
	{
		if (!Helpers.IsOSPlatformWindows)
		{
			throw new PlatformNotSupportedException();
		}
		return CompositeMLDsaManaged.GenerateKeyImpl(algorithm);
	}

	internal static CompositeMLDsa ImportCompositeMLDsaPublicKeyImpl(CompositeMLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		if (!Helpers.IsOSPlatformWindows)
		{
			throw new PlatformNotSupportedException();
		}
		return CompositeMLDsaManaged.ImportCompositeMLDsaPublicKeyImpl(algorithm, source);
	}

	internal static CompositeMLDsa ImportCompositeMLDsaPrivateKeyImpl(CompositeMLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		if (!Helpers.IsOSPlatformWindows)
		{
			throw new PlatformNotSupportedException();
		}
		return CompositeMLDsaManaged.ImportCompositeMLDsaPrivateKeyImpl(algorithm, source);
	}

	protected override int SignDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override bool VerifyDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, ReadOnlySpan<byte> signature)
	{
		throw new PlatformNotSupportedException();
	}

	protected override bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten)
	{
		throw new PlatformNotSupportedException();
	}

	protected override int ExportCompositeMLDsaPublicKeyCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override int ExportCompositeMLDsaPrivateKeyCore(Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}
}
