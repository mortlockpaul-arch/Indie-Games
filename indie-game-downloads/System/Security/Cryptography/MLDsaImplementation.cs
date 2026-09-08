using System.Diagnostics.CodeAnalysis;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography;

[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
internal sealed class MLDsaImplementation : MLDsa
{
	private static readonly SafeBCryptAlgorithmHandle s_algHandle = OpenAlgorithmHandle();

	private readonly bool _hasSeed;

	private readonly bool _hasPrivateKey;

	private SafeBCryptKeyHandle _key;

	[MemberNotNullWhen(true, "s_algHandle")]
	internal static bool SupportsAny()
	{
		return s_algHandle != null;
	}

	[MemberNotNullWhen(true, "s_algHandle")]
	internal static bool IsAlgorithmSupported(MLDsaAlgorithm algorithm)
	{
		return SupportsAny();
	}

	internal static MLDsaImplementation GenerateKeyImpl(MLDsaAlgorithm algorithm)
	{
		string mLDsaParameterSet = PqcBlobHelpers.GetMLDsaParameterSet(algorithm);
		SafeBCryptKeyHandle safeBCryptKeyHandle = global::Interop.BCrypt.BCryptGenerateKeyPair(s_algHandle, 0);
		try
		{
			global::Interop.BCrypt.BCryptSetSZProperty(safeBCryptKeyHandle, "ParameterSetName", mLDsaParameterSet);
			global::Interop.BCrypt.BCryptFinalizeKeyPair(safeBCryptKeyHandle);
		}
		catch
		{
			safeBCryptKeyHandle?.Dispose();
			throw;
		}
		return new MLDsaImplementation(algorithm, safeBCryptKeyHandle, hasSeed: true, hasPrivateKey: true);
	}

	internal static MLDsaImplementation ImportPublicKey(MLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		SafeBCryptKeyHandle key = PqcBlobHelpers.EncodeMLDsaBlob(PqcBlobHelpers.GetMLDsaParameterSet(algorithm).AsSpan(), source, "PQDSAPUBLICBLOB", (ReadOnlySpan<byte> blob) => global::Interop.BCrypt.BCryptImportKeyPair(s_algHandle, "PQDSAPUBLICBLOB", blob));
		return new MLDsaImplementation(algorithm, key, hasSeed: false, hasPrivateKey: false);
	}

	internal static MLDsaImplementation ImportPrivateKey(MLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		SafeBCryptKeyHandle key = PqcBlobHelpers.EncodeMLDsaBlob(PqcBlobHelpers.GetMLDsaParameterSet(algorithm).AsSpan(), source, "PQDSAPRIVATEBLOB", (ReadOnlySpan<byte> blob) => global::Interop.BCrypt.BCryptImportKeyPair(s_algHandle, "PQDSAPRIVATEBLOB", blob));
		return new MLDsaImplementation(algorithm, key, hasSeed: false, hasPrivateKey: true);
	}

	internal static MLDsaImplementation ImportSeed(MLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		SafeBCryptKeyHandle key = PqcBlobHelpers.EncodeMLDsaBlob(PqcBlobHelpers.GetMLDsaParameterSet(algorithm).AsSpan(), source, "PQDSAPRIVATESEEDBLOB", (ReadOnlySpan<byte> blob) => global::Interop.BCrypt.BCryptImportKeyPair(s_algHandle, "PQDSAPRIVATESEEDBLOB", blob));
		return new MLDsaImplementation(algorithm, key, hasSeed: true, hasPrivateKey: true);
	}

	private MLDsaImplementation(MLDsaAlgorithm algorithm, SafeBCryptKeyHandle key, bool hasSeed, bool hasPrivateKey)
		: base(algorithm)
	{
		_key = key;
		_hasSeed = hasSeed;
		_hasPrivateKey = hasPrivateKey;
	}

	protected override void SignDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, Span<byte> destination)
	{
		if (!_hasPrivateKey)
		{
			throw new CryptographicException(System.SR.Cryptography_NoPrivateKeyAvailable);
		}
		global::Interop.BCrypt.BCryptSignHashPqcPure(_key, data, context, destination);
	}

	protected override bool VerifyDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, ReadOnlySpan<byte> signature)
	{
		return global::Interop.BCrypt.BCryptVerifySignaturePqcPure(_key, data, context, signature);
	}

	protected override void SignPreHashCore(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> context, string hashAlgorithmOid, Span<byte> destination)
	{
		if (!_hasPrivateKey)
		{
			throw new CryptographicException(System.SR.Cryptography_NoPrivateKeyAvailable);
		}
		string hashAlgorithmIdentifier = MapHashOidToAlgorithm(hashAlgorithmOid, out var _, out var _);
		global::Interop.BCrypt.BCryptSignHashPqcPreHash(_key, hash, hashAlgorithmIdentifier, context, destination);
	}

	protected override bool VerifyPreHashCore(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> context, string hashAlgorithmOid, ReadOnlySpan<byte> signature)
	{
		string hashAlgorithmIdentifier = MapHashOidToAlgorithm(hashAlgorithmOid, out var _, out var _);
		return global::Interop.BCrypt.BCryptVerifySignaturePqcPreHash(_key, hash, hashAlgorithmIdentifier, context, signature);
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
		ExportKey("PQDSAPUBLICBLOB", base.Algorithm.PublicKeySizeInBytes, destination);
	}

	protected override void ExportMLDsaPrivateKeyCore(Span<byte> destination)
	{
		if (!_hasPrivateKey)
		{
			throw new CryptographicException(System.SR.Cryptography_NoPrivateKeyAvailable);
		}
		ExportKey("PQDSAPRIVATEBLOB", base.Algorithm.PrivateKeySizeInBytes, destination);
	}

	protected override void ExportMLDsaPrivateSeedCore(Span<byte> destination)
	{
		if (!_hasSeed)
		{
			throw new CryptographicException(System.SR.Cryptography_PqcNoSeed);
		}
		ExportKey("PQDSAPRIVATESEEDBLOB", base.Algorithm.PrivateSeedSizeInBytes, destination);
	}

	protected override bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten)
	{
		return MLDsaPkcs8.TryExportPkcs8PrivateKey(this, _hasSeed, _hasPrivateKey, destination, out bytesWritten);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_key?.Dispose();
			_key = null;
		}
		base.Dispose(disposing);
	}

	private void ExportKey(string keyBlobType, int expectedKeySize, Span<byte> destination)
	{
		ArraySegment<byte> arraySegment = global::Interop.BCrypt.BCryptExportKey(_key, keyBlobType);
		try
		{
			ReadOnlySpan<byte> readOnlySpan = PqcBlobHelpers.DecodeMLDsaBlob(arraySegment, out var parameterSet, out var blobType);
			string mLDsaParameterSet = PqcBlobHelpers.GetMLDsaParameterSet(base.Algorithm);
			if (blobType != keyBlobType || readOnlySpan.Length != expectedKeySize || !parameterSet.SequenceEqual(mLDsaParameterSet.AsSpan()))
			{
				throw new CryptographicException();
			}
			readOnlySpan.CopyTo(destination);
		}
		finally
		{
			System.Security.Cryptography.CryptoPool.Return(arraySegment);
		}
	}

	private static SafeBCryptAlgorithmHandle OpenAlgorithmHandle()
	{
		if (!Helpers.IsOSPlatformWindows)
		{
			return null;
		}
		if (global::Interop.BCrypt.BCryptOpenAlgorithmProvider(out var phAlgorithm, "ML-DSA", null, global::Interop.BCrypt.BCryptOpenAlgorithmProviderFlags.None) != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			phAlgorithm.Dispose();
			return null;
		}
		return phAlgorithm;
	}

	internal CngKey CreateEphemeralCng()
	{
		string blobType = (_hasSeed ? "PQDSAPRIVATESEEDBLOB" : (_hasPrivateKey ? "PQDSAPRIVATEBLOB" : "PQDSAPUBLICBLOB"));
		CngKeyBlobFormat cngBlobFormat = (_hasSeed ? CngKeyBlobFormat.PQDsaPrivateSeedBlob : (_hasPrivateKey ? CngKeyBlobFormat.PQDsaPrivateBlob : CngKeyBlobFormat.PQDsaPublicBlob));
		CngKey cngKey = global::Interop.BCrypt.BCryptExportKey(_key, blobType, (ReadOnlySpan<byte> keyMaterial) => CngKey.Import(keyMaterial, cngBlobFormat));
		cngKey.ExportPolicy = CngExportPolicies.AllowExport | CngExportPolicies.AllowPlaintextExport;
		return cngKey;
	}
}
