using System.Formats.Asn1;
using System.Runtime.Versioning;
using System.Security.Cryptography.Asn1;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography;

public sealed class MLDsaCng : MLDsa
{
	private delegate void KeySelectorFunc(ref readonly MLDsaPrivateKeyAsn mldsaPrivateKeyAsn, MLDsaAlgorithm algorithm, Span<byte> destination);

	private CngKey _key;

	internal CngKey KeyNoDuplicate => _key;

	[SupportedOSPlatform("windows")]
	public MLDsaCng(CngKey key)
		: base(AlgorithmFromHandleWithPlatformCheck(key, out var duplicateKey))
	{
		_key = duplicateKey;
	}

	protected override void SignMuCore(ReadOnlySpan<byte> mu, Span<byte> destination)
	{
		throw new PlatformNotSupportedException();
	}

	protected override bool VerifyMuCore(ReadOnlySpan<byte> mu, ReadOnlySpan<byte> signature)
	{
		throw new PlatformNotSupportedException();
	}

	private static MLDsaAlgorithm AlgorithmFromHandleWithPlatformCheck(CngKey key, out CngKey duplicateKey)
	{
		if (!Helpers.IsOSPlatformWindows)
		{
			throw new PlatformNotSupportedException();
		}
		return AlgorithmFromHandle(key, out duplicateKey);
	}

	[SupportedOSPlatform("windows")]
	private static MLDsaAlgorithm AlgorithmFromHandle(CngKey key, out CngKey duplicateKey)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		MLDsa.ThrowIfNotSupported();
		if (key.AlgorithmGroup != CngAlgorithmGroup.MLDsa)
		{
			throw new ArgumentException(System.SR.Cryptography_ArgMLDsaRequiresMLDsaKey, "key");
		}
		MLDsaAlgorithm result = AlgorithmFromHandleImpl(key);
		duplicateKey = key.HandleNoDuplicate.Duplicate(key.IsEphemeral);
		return result;
	}

	public CngKey GetKey()
	{
		ThrowIfDisposed();
		return _key.HandleNoDuplicate.Duplicate(_key.IsEphemeral);
	}

	internal MLDsaCng(CngKey key, bool transferOwnership)
		: base(AlgorithmFromHandleNoDuplicate(key))
	{
		_key = key;
	}

	private static MLDsaAlgorithm AlgorithmFromHandleNoDuplicate(CngKey key)
	{
		if (key.AlgorithmGroup != CngAlgorithmGroup.MLDsa)
		{
			throw new CryptographicException(System.SR.Cryptography_ArgMLDsaRequiresMLDsaKey);
		}
		return AlgorithmFromHandleImpl(key);
	}

	private static MLDsaAlgorithm AlgorithmFromHandleImpl(CngKey key)
	{
		string propertyAsString = key.HandleNoDuplicate.GetPropertyAsString("ParameterSetName", CngPropertyOptions.None);
		return propertyAsString switch
		{
			"44" => MLDsaAlgorithm.MLDsa44, 
			"65" => MLDsaAlgorithm.MLDsa65, 
			"87" => MLDsaAlgorithm.MLDsa87, 
			_ => throw DebugFailAndGetException(propertyAsString), 
		};
		static Exception DebugFailAndGetException(string parameterSet)
		{
			throw new CryptographicException();
		}
	}

	protected override void ExportMLDsaPublicKeyCore(Span<byte> destination)
	{
		ExportKey(CngKeyBlobFormat.PQDsaPublicBlob, base.Algorithm.PublicKeySizeInBytes, destination);
	}

	protected override void ExportMLDsaPrivateSeedCore(Span<byte> destination)
	{
		if (CngPkcs8.AllowsOnlyEncryptedExport(_key))
		{
			ExportKeyWithEncryptedOnlyExport(delegate(ref readonly MLDsaPrivateKeyAsn mldsaPrivateKeyAsn, MLDsaAlgorithm algorithm, Span<byte> destination2)
			{
				ReadOnlyMemory<byte>? readOnlyMemory = mldsaPrivateKeyAsn.Seed ?? mldsaPrivateKeyAsn.Both?.Seed;
				if (readOnlyMemory.HasValue)
				{
					ReadOnlyMemory<byte> valueOrDefault = readOnlyMemory.GetValueOrDefault();
					if (valueOrDefault.Length != algorithm.PrivateSeedSizeInBytes)
					{
						throw new CryptographicException(System.SR.Argument_PrivateSeedWrongSizeForAlgorithm);
					}
					valueOrDefault.Span.CopyTo(destination2);
					return;
				}
				throw new CryptographicException(System.SR.Cryptography_NotValidPrivateKey);
			}, base.Algorithm, destination);
		}
		else
		{
			ExportKey(CngKeyBlobFormat.PQDsaPrivateSeedBlob, base.Algorithm.PrivateSeedSizeInBytes, destination);
		}
	}

	protected override void ExportMLDsaPrivateKeyCore(Span<byte> destination)
	{
		if (CngPkcs8.AllowsOnlyEncryptedExport(_key))
		{
			ExportKeyWithEncryptedOnlyExport(delegate(ref readonly MLDsaPrivateKeyAsn mldsaPrivateKeyAsn, MLDsaAlgorithm algorithm, Span<byte> destination2)
			{
				ReadOnlyMemory<byte>? readOnlyMemory = mldsaPrivateKeyAsn.ExpandedKey ?? mldsaPrivateKeyAsn.Both?.ExpandedKey;
				if (readOnlyMemory.HasValue)
				{
					ReadOnlyMemory<byte> valueOrDefault = readOnlyMemory.GetValueOrDefault();
					if (valueOrDefault.Length != algorithm.PrivateKeySizeInBytes)
					{
						throw new CryptographicException(System.SR.Argument_PrivateKeyWrongSizeForAlgorithm);
					}
					valueOrDefault.Span.CopyTo(destination2);
					return;
				}
				ReadOnlyMemory<byte>? seed = mldsaPrivateKeyAsn.Seed;
				if (seed.HasValue)
				{
					ReadOnlyMemory<byte> valueOrDefault2 = seed.GetValueOrDefault();
					if (valueOrDefault2.Length != algorithm.PrivateSeedSizeInBytes)
					{
						throw new CryptographicException(System.SR.Argument_PrivateSeedWrongSizeForAlgorithm);
					}
					using MLDsa mLDsa = MLDsaImplementation.ImportSeed(algorithm, valueOrDefault2.Span);
					mLDsa.ExportMLDsaPrivateKey(destination2);
					return;
				}
				throw new CryptographicException(System.SR.Cryptography_NotValidPrivateKey);
			}, base.Algorithm, destination);
		}
		else
		{
			ExportKey(CngKeyBlobFormat.PQDsaPrivateBlob, base.Algorithm.PrivateKeySizeInBytes, destination);
		}
	}

	protected override bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten)
	{
		if (CngPkcs8.AllowsOnlyEncryptedExport(_key))
		{
			ArraySegment<byte> rentedPkcs8ForEncryptedOnlyExport = GetRentedPkcs8ForEncryptedOnlyExport();
			try
			{
				if (destination.Length < rentedPkcs8ForEncryptedOnlyExport.Count)
				{
					bytesWritten = 0;
					return false;
				}
				bytesWritten = rentedPkcs8ForEncryptedOnlyExport.Count;
				rentedPkcs8ForEncryptedOnlyExport.AsSpan().CopyTo(destination);
				return true;
			}
			finally
			{
				System.Security.Cryptography.CryptoPool.Return(rentedPkcs8ForEncryptedOnlyExport);
			}
		}
		return _key.TryExportKeyBlob("PKCS8_PRIVATEKEY", destination, out bytesWritten);
	}

	protected unsafe override void SignDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, Span<byte> destination)
	{
		using SafeNCryptKeyHandle keyHandle = _key.Handle;
		fixed (byte* ptr = context)
		{
			void* pbCtx = ptr;
			global::Interop.BCrypt.BCRYPT_PQDSA_PADDING_INFO bCRYPT_PQDSA_PADDING_INFO = new global::Interop.BCrypt.BCRYPT_PQDSA_PADDING_INFO
			{
				pbCtx = (nint)pbCtx,
				cbCtx = context.Length
			};
			keyHandle.SignHash(data, destination, global::Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PQDSA_FLAG, &bCRYPT_PQDSA_PADDING_INFO);
		}
	}

	protected unsafe override bool VerifyDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, ReadOnlySpan<byte> signature)
	{
		using SafeNCryptKeyHandle keyHandle = _key.Handle;
		fixed (byte* ptr = context)
		{
			void* pbCtx = ptr;
			global::Interop.BCrypt.BCRYPT_PQDSA_PADDING_INFO bCRYPT_PQDSA_PADDING_INFO = new global::Interop.BCrypt.BCRYPT_PQDSA_PADDING_INFO
			{
				pbCtx = (nint)pbCtx,
				cbCtx = context.Length
			};
			return keyHandle.VerifyHash(data, signature, global::Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PQDSA_FLAG, &bCRYPT_PQDSA_PADDING_INFO);
		}
	}

	protected unsafe override void SignPreHashCore(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> context, string hashAlgorithmOid, Span<byte> destination)
	{
		string text = MapHashOidToAlgorithm(hashAlgorithmOid, out var _, out var _);
		using SafeNCryptKeyHandle keyHandle = _key.Handle;
		fixed (char* pszPreHashAlgId = text)
		{
			fixed (byte* ptr = context)
			{
				void* pbCtx = ptr;
				global::Interop.BCrypt.BCRYPT_PQDSA_PADDING_INFO bCRYPT_PQDSA_PADDING_INFO = new global::Interop.BCrypt.BCRYPT_PQDSA_PADDING_INFO
				{
					pbCtx = (nint)pbCtx,
					cbCtx = context.Length,
					pszPreHashAlgId = (nint)pszPreHashAlgId
				};
				keyHandle.SignHash(hash, destination, global::Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PQDSA_FLAG, &bCRYPT_PQDSA_PADDING_INFO);
			}
		}
	}

	protected unsafe override bool VerifyPreHashCore(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> context, string hashAlgorithmOid, ReadOnlySpan<byte> signature)
	{
		string text = MapHashOidToAlgorithm(hashAlgorithmOid, out var _, out var _);
		using SafeNCryptKeyHandle keyHandle = _key.Handle;
		fixed (char* pszPreHashAlgId = text)
		{
			fixed (byte* ptr = context)
			{
				void* pbCtx = ptr;
				global::Interop.BCrypt.BCRYPT_PQDSA_PADDING_INFO bCRYPT_PQDSA_PADDING_INFO = new global::Interop.BCrypt.BCRYPT_PQDSA_PADDING_INFO
				{
					pbCtx = (nint)pbCtx,
					cbCtx = context.Length,
					pszPreHashAlgId = (nint)pszPreHashAlgId
				};
				return keyHandle.VerifyHash(hash, signature, global::Interop.NCrypt.AsymmetricPaddingMode.NCRYPT_PAD_PQDSA_FLAG, &bCRYPT_PQDSA_PADDING_INFO);
			}
		}
	}

	[SupportedOSPlatform("windows")]
	internal static MLDsaCng ImportPkcs8PrivateKey(byte[] source, out int bytesRead)
	{
		int bytesConsumed;
		try
		{
			AsnDecoder.ReadEncodedValue(source, AsnEncodingRules.BER, out var _, out var _, out bytesConsumed);
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
		bytesRead = bytesConsumed;
		ReadOnlySpan<byte> keyBlob = source.AsSpan(0, bytesConsumed);
		CngKey cngKey;
		try
		{
			cngKey = CngKey.Import(keyBlob, CngKeyBlobFormat.Pkcs8PrivateBlob);
		}
		catch (AsnContentException inner2)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner2);
		}
		cngKey.ExportPolicy = CngExportPolicies.AllowExport | CngExportPolicies.AllowPlaintextExport;
		return new MLDsaCng(cngKey, transferOwnership: true);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_key.Dispose();
			_key = null;
		}
		base.Dispose(disposing);
	}

	private void ExportKey(CngKeyBlobFormat blobFormat, int expectedKeySize, Span<byte> destination)
	{
		byte[] array = _key.Export(blobFormat);
		using (PinAndClear.Track(array))
		{
			ReadOnlySpan<byte> readOnlySpan = PqcBlobHelpers.DecodeMLDsaBlob(array, out var parameterSet, out var blobType);
			string mLDsaParameterSet = PqcBlobHelpers.GetMLDsaParameterSet(base.Algorithm);
			if (blobType != blobFormat.Format || readOnlySpan.Length != expectedKeySize || !parameterSet.SequenceEqual(mLDsaParameterSet.AsSpan()))
			{
				throw new CryptographicException();
			}
			readOnlySpan.CopyTo(destination);
		}
	}

	private void ExportKeyWithEncryptedOnlyExport(KeySelectorFunc keySelector, MLDsaAlgorithm algorithm, Span<byte> destination)
	{
		ArraySegment<byte> rentedPkcs8ForEncryptedOnlyExport = GetRentedPkcs8ForEncryptedOnlyExport();
		byte[] array = null;
		try
		{
			ReadOnlyMemory<byte> encoded = KeyFormatHelper.ReadPkcs8(MLDsa.KnownOids, rentedPkcs8ForEncryptedOnlyExport.AsMemory(), out var _);
			MLDsaPrivateKeyAsn mldsaPrivateKeyAsn;
			try
			{
				mldsaPrivateKeyAsn = MLDsaPrivateKeyAsn.Decode(encoded, AsnEncodingRules.BER);
			}
			catch (AsnContentException inner)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
			}
			keySelector(in mldsaPrivateKeyAsn, algorithm, destination);
		}
		finally
		{
			CryptographicOperations.ZeroMemory(array);
			System.Security.Cryptography.CryptoPool.Return(rentedPkcs8ForEncryptedOnlyExport);
		}
	}

	private ArraySegment<byte> GetRentedPkcs8ForEncryptedOnlyExport()
	{
		byte[] array = _key.ExportPkcs8KeyBlob("DotnetExportPhrase".AsSpan(), 1);
		using (PinAndClear.Track(array))
		{
			int bytesRead;
			return KeyFormatHelper.DecryptPkcs8("DotnetExportPhrase".AsSpan(), array, out bytesRead);
		}
	}
}
