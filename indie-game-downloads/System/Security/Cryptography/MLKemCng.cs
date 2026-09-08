using System.Runtime.Versioning;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography;

public sealed class MLKemCng : MLKem
{
	private CngKey _key;

	[SupportedOSPlatform("windows")]
	public MLKemCng(CngKey key)
		: base(AlgorithmFromHandleWithPlatformCheck(key, out var duplicateKey))
	{
		_key = duplicateKey;
	}

	private static MLKemAlgorithm AlgorithmFromHandleWithPlatformCheck(CngKey key, out CngKey duplicateKey)
	{
		if (!Helpers.IsOSPlatformWindows)
		{
			throw new PlatformNotSupportedException();
		}
		return AlgorithmFromHandle(key, out duplicateKey);
	}

	[SupportedOSPlatform("windows")]
	private static MLKemAlgorithm AlgorithmFromHandle(CngKey key, out CngKey duplicateKey)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		MLKem.ThrowIfNotSupported();
		if (key.AlgorithmGroup != CngAlgorithmGroup.MLKem)
		{
			throw new ArgumentException(System.SR.Cryptography_ArgMLKemRequiresMLKemKey, "key");
		}
		MLKemAlgorithm result = AlgorithmFromHandleImpl(key);
		duplicateKey = key.HandleNoDuplicate.Duplicate(key.IsEphemeral);
		return result;
	}

	public CngKey GetKey()
	{
		ThrowIfDisposed();
		return _key.HandleNoDuplicate.Duplicate(_key.IsEphemeral);
	}

	private static MLKemAlgorithm AlgorithmFromHandleImpl(CngKey key)
	{
		string propertyAsString = key.HandleNoDuplicate.GetPropertyAsString("ParameterSetName", CngPropertyOptions.None);
		return propertyAsString switch
		{
			"512" => MLKemAlgorithm.MLKem512, 
			"768" => MLKemAlgorithm.MLKem768, 
			"1024" => MLKemAlgorithm.MLKem1024, 
			_ => throw DebugFailAndGetException(propertyAsString), 
		};
		static Exception DebugFailAndGetException(string parameterSet)
		{
			return new CryptographicException();
		}
	}

	protected override void DecapsulateCore(ReadOnlySpan<byte> ciphertext, Span<byte> sharedSecret)
	{
		using SafeNCryptKeyHandle hKey = _key.Handle;
		global::Interop.NCrypt.NCryptDecapsulate(hKey, ciphertext, sharedSecret, 0u);
	}

	protected override void EncapsulateCore(Span<byte> ciphertext, Span<byte> sharedSecret)
	{
		using SafeNCryptKeyHandle hKey = _key.Handle;
		global::Interop.NCrypt.NCryptEncapsulate(hKey, sharedSecret, ciphertext, out var _, out var _, 0u);
	}

	protected override void ExportPrivateSeedCore(Span<byte> destination)
	{
		if (CngPkcs8.AllowsOnlyEncryptedExport(_key))
		{
			throw new CryptographicException(System.SR.Cryptography_KeyNotExtractable);
		}
		ExportKey(global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PRIVATE_SEED_MAGIC, destination);
	}

	protected override void ExportDecapsulationKeyCore(Span<byte> destination)
	{
		if (CngPkcs8.AllowsOnlyEncryptedExport(_key))
		{
			throw new CryptographicException(System.SR.Cryptography_KeyNotExtractable);
		}
		ExportKey(global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PRIVATE_MAGIC, destination);
	}

	protected override void ExportEncapsulationKeyCore(Span<byte> destination)
	{
		ExportKey(global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PUBLIC_MAGIC, destination);
	}

	protected override bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten)
	{
		if (CngPkcs8.AllowsOnlyEncryptedExport(_key))
		{
			throw new CryptographicException(System.SR.Cryptography_KeyNotExtractable);
		}
		try
		{
			return MLKemPkcs8.TryExportPkcs8PrivateKey(this, hasSeed: true, hasDecapsulationKey: true, destination, out bytesWritten);
		}
		catch (CryptographicException)
		{
			try
			{
				return MLKemPkcs8.TryExportPkcs8PrivateKey(this, hasSeed: false, hasDecapsulationKey: true, destination, out bytesWritten);
			}
			catch (CryptographicException)
			{
				throw new CryptographicException(System.SR.Cryptography_KeyNotExtractable);
			}
		}
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

	private void ExportKey(global::Interop.BCrypt.KeyBlobMagicNumber kind, Span<byte> destination)
	{
		if (kind != global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PUBLIC_MAGIC && _key.ExportPolicy == CngExportPolicies.None)
		{
			throw new CryptographicException(System.SR.Cryptography_KeyNotExtractable);
		}
		string pszBlobType = PqcBlobHelpers.MLKemBlobMagicToBlobType(kind);
		using SafeNCryptKeyHandle hKey = _key.Handle;
		global::Interop.NCrypt.ErrorCode errorCode = global::Interop.NCrypt.NCryptExportKey(hKey, IntPtr.Zero, pszBlobType, IntPtr.Zero, null, 0, out var pcbResult, 0);
		if (errorCode != global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS)
		{
			throw errorCode.ToCryptographicException();
		}
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(pcbResult);
		PinAndClear pinAndClear = PinAndClear.Track(array);
		try
		{
			errorCode = global::Interop.NCrypt.NCryptExportKey(hKey, IntPtr.Zero, pszBlobType, IntPtr.Zero, array, pcbResult, out var pcbResult2, 0);
			if (errorCode != global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS)
			{
				throw errorCode.ToCryptographicException();
			}
			ReadCngMLKemBlob(kind, array.AsSpan(0, pcbResult2), destination);
		}
		finally
		{
			CryptographicOperations.ZeroMemory(array);
			pinAndClear.Dispose();
			System.Security.Cryptography.CryptoPool.Return(array, 0);
		}
	}
}
