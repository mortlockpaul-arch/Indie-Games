using System.Diagnostics.CodeAnalysis;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography;

internal sealed class MLKemImplementation : MLKem
{
	private static readonly SafeBCryptAlgorithmHandle s_algHandle = OpenAlgorithmHandle();

	private readonly bool _hasSeed;

	private readonly bool _hasDecapsulationKey;

	private SafeBCryptKeyHandle _key;

	[MemberNotNullWhen(true, "s_algHandle")]
	internal new static bool IsSupported
	{
		[MemberNotNullWhen(true, "s_algHandle")]
		get
		{
			return s_algHandle != null;
		}
	}

	private MLKemImplementation(MLKemAlgorithm algorithm, SafeBCryptKeyHandle key, bool hasSeed, bool hasDecapsulationKey)
		: base(algorithm)
	{
		_key = key;
		_hasSeed = hasSeed;
		_hasDecapsulationKey = hasDecapsulationKey;
	}

	internal static MLKemImplementation GenerateKeyImpl(MLKemAlgorithm algorithm)
	{
		string mLKemParameterSet = PqcBlobHelpers.GetMLKemParameterSet(algorithm);
		SafeBCryptKeyHandle safeBCryptKeyHandle = global::Interop.BCrypt.BCryptGenerateKeyPair(s_algHandle, 0);
		try
		{
			global::Interop.BCrypt.BCryptSetSZProperty(safeBCryptKeyHandle, "ParameterSetName", mLKemParameterSet);
			global::Interop.BCrypt.BCryptFinalizeKeyPair(safeBCryptKeyHandle);
		}
		catch
		{
			safeBCryptKeyHandle.Dispose();
			throw;
		}
		return new MLKemImplementation(algorithm, safeBCryptKeyHandle, hasSeed: true, hasDecapsulationKey: true);
	}

	internal static MLKemImplementation ImportPrivateSeedImpl(MLKemAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		SafeBCryptKeyHandle key = ImportKey(global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PRIVATE_SEED_MAGIC, algorithm, source);
		return new MLKemImplementation(algorithm, key, hasSeed: true, hasDecapsulationKey: true);
	}

	internal static MLKemImplementation ImportDecapsulationKeyImpl(MLKemAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		SafeBCryptKeyHandle key = ImportKey(global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PRIVATE_MAGIC, algorithm, source);
		return new MLKemImplementation(algorithm, key, hasSeed: false, hasDecapsulationKey: true);
	}

	internal static MLKemImplementation ImportEncapsulationKeyImpl(MLKemAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		SafeBCryptKeyHandle key = ImportKey(global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PUBLIC_MAGIC, algorithm, source);
		return new MLKemImplementation(algorithm, key, hasSeed: false, hasDecapsulationKey: false);
	}

	protected override void DecapsulateCore(ReadOnlySpan<byte> ciphertext, Span<byte> sharedSecret)
	{
		MLKem.ThrowIfNoDecapsulationKey(_hasDecapsulationKey);
		global::Interop.BCrypt.BCryptDecapsulate(_key, ciphertext, sharedSecret, 0u);
	}

	protected override void EncapsulateCore(Span<byte> ciphertext, Span<byte> sharedSecret)
	{
		global::Interop.BCrypt.BCryptEncapsulate(_key, sharedSecret, ciphertext, out var _, out var _, 0u);
	}

	protected override void ExportPrivateSeedCore(Span<byte> destination)
	{
		MLKem.ThrowIfNoSeed(_hasSeed);
		ExportKey(global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PRIVATE_SEED_MAGIC, destination);
	}

	protected override void ExportDecapsulationKeyCore(Span<byte> destination)
	{
		MLKem.ThrowIfNoDecapsulationKey(_hasDecapsulationKey);
		ExportKey(global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PRIVATE_MAGIC, destination);
	}

	protected override void ExportEncapsulationKeyCore(Span<byte> destination)
	{
		ExportKey(global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PUBLIC_MAGIC, destination);
	}

	protected override bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten)
	{
		return MLKemPkcs8.TryExportPkcs8PrivateKey(this, _hasSeed, _hasDecapsulationKey, destination, out bytesWritten);
	}

	private static SafeBCryptAlgorithmHandle OpenAlgorithmHandle()
	{
		if (!Helpers.IsOSPlatformWindows)
		{
			return null;
		}
		if (global::Interop.BCrypt.BCryptOpenAlgorithmProvider(out var phAlgorithm, "ML-KEM", null, global::Interop.BCrypt.BCryptOpenAlgorithmProviderFlags.None) != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			phAlgorithm.Dispose();
			return null;
		}
		return phAlgorithm;
	}

	private void ExportKey(global::Interop.BCrypt.KeyBlobMagicNumber kind, Span<byte> destination)
	{
		string blobType = PqcBlobHelpers.MLKemBlobMagicToBlobType(kind);
		ArraySegment<byte> arraySegment = global::Interop.BCrypt.BCryptExportKey(_key, blobType);
		try
		{
			ReadCngMLKemBlob(kind, arraySegment, destination);
		}
		finally
		{
			if (kind == global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PUBLIC_MAGIC)
			{
				System.Security.Cryptography.CryptoPool.Return(arraySegment, 0);
			}
			else
			{
				System.Security.Cryptography.CryptoPool.Return(arraySegment);
			}
		}
	}

	private static SafeBCryptKeyHandle ImportKey(global::Interop.BCrypt.KeyBlobMagicNumber kind, MLKemAlgorithm algorithm, ReadOnlySpan<byte> key)
	{
		return PqcBlobHelpers.EncodeMLKemBlob(kind, algorithm, key, s_algHandle, (SafeBCryptAlgorithmHandle algHandle, string blobKind, ReadOnlySpan<byte> blob) => global::Interop.BCrypt.BCryptImportKeyPair(algHandle, blobKind, blob));
	}
}
