using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography;

internal static class CngHelpers
{
	private static readonly CngKeyBlobFormat s_cipherKeyBlobFormat = new CngKeyBlobFormat("CipherKeyBlob");

	internal static CryptographicException ToCryptographicException(this global::Interop.NCrypt.ErrorCode errorCode)
	{
		return ((int)errorCode).ToCryptographicException();
	}

	internal static SafeNCryptProviderHandle OpenStorageProvider(this CngProvider provider)
	{
		global::Interop.NCrypt.ErrorCode errorCode = global::Interop.NCrypt.NCryptOpenStorageProvider(out var phProvider, provider.Provider, 0);
		if (errorCode != global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS)
		{
			phProvider.Dispose();
			throw errorCode.ToCryptographicException();
		}
		return phProvider;
	}

	internal unsafe static void SetExportPolicy(this SafeNCryptKeyHandle keyHandle, CngExportPolicies exportPolicy)
	{
		global::Interop.NCrypt.ErrorCode errorCode = global::Interop.NCrypt.NCryptSetProperty(keyHandle, "Export Policy", &exportPolicy, 4, CngPropertyOptions.Persist);
		if (errorCode != global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS)
		{
			throw errorCode.ToCryptographicException();
		}
	}

	internal unsafe static byte[] GetProperty(this SafeNCryptHandle ncryptHandle, string propertyName, CngPropertyOptions options)
	{
		global::Interop.NCrypt.ErrorCode errorCode = global::Interop.NCrypt.NCryptGetProperty(ncryptHandle, propertyName, null, 0, out var pcbResult, options);
		switch (errorCode)
		{
		case global::Interop.NCrypt.ErrorCode.NTE_NOT_FOUND:
			return null;
		default:
			throw errorCode.ToCryptographicException();
		case global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS:
		{
			byte[] array = new byte[pcbResult];
			fixed (byte* pbOutput = array)
			{
				errorCode = global::Interop.NCrypt.NCryptGetProperty(ncryptHandle, propertyName, pbOutput, array.Length, out pcbResult, options);
			}
			switch (errorCode)
			{
			case global::Interop.NCrypt.ErrorCode.NTE_NOT_FOUND:
				return null;
			default:
				throw errorCode.ToCryptographicException();
			case global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS:
				Array.Resize(ref array, pcbResult);
				return array;
			}
		}
		}
	}

	internal unsafe static string GetPropertyAsString(this SafeNCryptHandle ncryptHandle, string propertyName, CngPropertyOptions options)
	{
		byte[] property = ncryptHandle.GetProperty(propertyName, options);
		if (property == null)
		{
			return null;
		}
		if (property.Length == 0)
		{
			return string.Empty;
		}
		fixed (byte* ptr = &property[0])
		{
			return Marshal.PtrToStringUni((nint)ptr);
		}
	}

	internal static bool TryExportKeyBlob(this SafeNCryptKeyHandle handle, string blobType, Span<byte> destination, out int bytesWritten)
	{
		Span<byte> span = default(Span<byte>);
		global::Interop.NCrypt.ErrorCode errorCode = global::Interop.NCrypt.NCryptExportKey(handle, IntPtr.Zero, blobType, IntPtr.Zero, ref MemoryMarshal.GetReference(span), span.Length, out var pcbResult, 0);
		if (errorCode != global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS)
		{
			throw errorCode.ToCryptographicException();
		}
		if (pcbResult > destination.Length)
		{
			bytesWritten = 0;
			return false;
		}
		errorCode = global::Interop.NCrypt.NCryptExportKey(handle, IntPtr.Zero, blobType, IntPtr.Zero, ref MemoryMarshal.GetReference(destination), destination.Length, out pcbResult, 0);
		if (errorCode != global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS)
		{
			throw errorCode.ToCryptographicException();
		}
		bytesWritten = pcbResult;
		return true;
	}

	internal unsafe static bool ExportPkcs8KeyBlob(bool allocate, SafeNCryptKeyHandle keyHandle, ReadOnlySpan<char> password, int kdfCount, Span<byte> destination, out int bytesWritten, out byte[] allocated)
	{
		using SafeUnicodeStringHandle safeUnicodeStringHandle = new SafeUnicodeStringHandle(password);
		ReadOnlySpan<byte> span = "1.2.840.113549.1.12.1.3\0"u8;
		fixed (byte* reference = &MemoryMarshal.GetReference(span))
		{
			global::Interop.NCrypt.NCryptBuffer* ptr = stackalloc global::Interop.NCrypt.NCryptBuffer[3];
			global::Interop.NCrypt.PBE_PARAMS pBE_PARAMS = default(global::Interop.NCrypt.PBE_PARAMS);
			Span<byte> data = new Span<byte>(pBE_PARAMS.rgbSalt, 8);
			RandomNumberGenerator.Fill(data);
			pBE_PARAMS.Params.cbSalt = data.Length;
			pBE_PARAMS.Params.iIterations = kdfCount;
			*ptr = new global::Interop.NCrypt.NCryptBuffer
			{
				BufferType = global::Interop.NCrypt.BufferType.PkcsSecret,
				cbBuffer = checked(2 * (password.Length + 1)),
				pvBuffer = safeUnicodeStringHandle.DangerousGetHandle()
			};
			if (ptr->pvBuffer == IntPtr.Zero)
			{
				ptr->cbBuffer = 0;
			}
			ptr[1] = new global::Interop.NCrypt.NCryptBuffer
			{
				BufferType = global::Interop.NCrypt.BufferType.PkcsAlgOid,
				cbBuffer = span.Length,
				pvBuffer = (nint)reference
			};
			ptr[2] = new global::Interop.NCrypt.NCryptBuffer
			{
				BufferType = global::Interop.NCrypt.BufferType.PkcsAlgParam,
				cbBuffer = sizeof(global::Interop.NCrypt.PBE_PARAMS),
				pvBuffer = (nint)(&pBE_PARAMS)
			};
			global::Interop.NCrypt.NCryptBufferDesc pParameterList = new global::Interop.NCrypt.NCryptBufferDesc
			{
				cBuffers = 3,
				pBuffers = (nint)ptr,
				ulVersion = 0
			};
			global::Interop.NCrypt.ErrorCode errorCode = global::Interop.NCrypt.NCryptExportKey(keyHandle, IntPtr.Zero, "PKCS8_PRIVATEKEY", ref pParameterList, ref MemoryMarshal.GetReference(default(Span<byte>)), 0, out var pcbResult, 0);
			if (errorCode != global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS)
			{
				throw errorCode.ToCryptographicException();
			}
			allocated = null;
			if (allocate)
			{
				allocated = new byte[pcbResult];
				destination = allocated;
			}
			else if (pcbResult > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			errorCode = global::Interop.NCrypt.NCryptExportKey(keyHandle, IntPtr.Zero, "PKCS8_PRIVATEKEY", ref pParameterList, ref MemoryMarshal.GetReference(destination), destination.Length, out pcbResult, 0);
			if (errorCode != global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS)
			{
				throw errorCode.ToCryptographicException();
			}
			if (allocate && pcbResult != destination.Length)
			{
				byte[] array = new byte[pcbResult];
				destination.Slice(0, pcbResult).CopyTo(array);
				Array.Clear(allocated, 0, pcbResult);
				allocated = array;
			}
			bytesWritten = pcbResult;
			return true;
		}
	}

	[SupportedOSPlatform("windows")]
	internal static CngKey Duplicate(this SafeNCryptKeyHandle keyHandle, bool isEphemeral)
	{
		return CngKey.Open(keyHandle, isEphemeral ? CngKeyHandleOpenOptions.EphemeralKey : CngKeyHandleOpenOptions.None);
	}

	public unsafe static byte[] SignHash(this SafeNCryptKeyHandle keyHandle, ReadOnlySpan<byte> hash, global::Interop.NCrypt.AsymmetricPaddingMode paddingMode, void* pPaddingInfo, int estimatedSize)
	{
		byte[] array = new byte[estimatedSize];
		global::Interop.NCrypt.ErrorCode errorCode = global::Interop.NCrypt.NCryptSignHash(keyHandle, pPaddingInfo, hash, array, out var pcbResult, paddingMode);
		if (errorCode == global::Interop.NCrypt.ErrorCode.STATUS_UNSUCCESSFUL)
		{
			errorCode = global::Interop.NCrypt.NCryptSignHash(keyHandle, pPaddingInfo, hash, array, out pcbResult, paddingMode);
		}
		if (errorCode.IsBufferTooSmall())
		{
			array = new byte[pcbResult];
			errorCode = global::Interop.NCrypt.NCryptSignHash(keyHandle, pPaddingInfo, hash, array, out pcbResult, paddingMode);
		}
		if (errorCode == global::Interop.NCrypt.ErrorCode.STATUS_UNSUCCESSFUL)
		{
			errorCode = global::Interop.NCrypt.NCryptSignHash(keyHandle, pPaddingInfo, hash, array, out pcbResult, paddingMode);
		}
		if (errorCode != global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS)
		{
			throw errorCode.ToCryptographicException();
		}
		Array.Resize(ref array, pcbResult);
		return array;
	}

	public unsafe static void SignHash(this SafeNCryptKeyHandle keyHandle, ReadOnlySpan<byte> hash, Span<byte> destination, global::Interop.NCrypt.AsymmetricPaddingMode paddingMode, void* pPaddingInfo)
	{
		global::Interop.NCrypt.ErrorCode errorCode = global::Interop.NCrypt.NCryptSignHash(keyHandle, pPaddingInfo, hash, destination, out var pcbResult, paddingMode);
		if (errorCode == global::Interop.NCrypt.ErrorCode.STATUS_UNSUCCESSFUL)
		{
			errorCode = global::Interop.NCrypt.NCryptSignHash(keyHandle, pPaddingInfo, hash, destination, out pcbResult, paddingMode);
		}
		if (errorCode != global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS)
		{
			throw errorCode.ToCryptographicException();
		}
	}

	public unsafe static bool TrySignHash(this SafeNCryptKeyHandle keyHandle, ReadOnlySpan<byte> hash, Span<byte> signature, global::Interop.NCrypt.AsymmetricPaddingMode paddingMode, void* pPaddingInfo, out int bytesWritten)
	{
		for (int i = 0; i <= 1; i++)
		{
			global::Interop.NCrypt.ErrorCode errorCode = global::Interop.NCrypt.NCryptSignHash(keyHandle, pPaddingInfo, hash, signature, out var pcbResult, paddingMode);
			global::Interop.NCrypt.ErrorCode errorCode2 = errorCode;
			if (errorCode2 == global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS)
			{
				bytesWritten = pcbResult;
				return true;
			}
			if (!errorCode2.IsBufferTooSmall())
			{
				if (errorCode2 != global::Interop.NCrypt.ErrorCode.STATUS_UNSUCCESSFUL)
				{
					throw errorCode.ToCryptographicException();
				}
				continue;
			}
			bytesWritten = 0;
			return false;
		}
		throw global::Interop.NCrypt.ErrorCode.STATUS_UNSUCCESSFUL.ToCryptographicException();
	}

	public unsafe static bool VerifyHash(this SafeNCryptKeyHandle keyHandle, ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature, global::Interop.NCrypt.AsymmetricPaddingMode paddingMode, void* pPaddingInfo)
	{
		global::Interop.NCrypt.ErrorCode errorCode = global::Interop.NCrypt.NCryptVerifySignature(keyHandle, pPaddingInfo, hash, hash.Length, signature, signature.Length, paddingMode);
		if (errorCode == global::Interop.NCrypt.ErrorCode.STATUS_UNSUCCESSFUL)
		{
			errorCode = global::Interop.NCrypt.NCryptVerifySignature(keyHandle, pPaddingInfo, hash, hash.Length, signature, signature.Length, paddingMode);
		}
		return errorCode == global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS;
	}

	public static int GetPropertyAsDword(this SafeNCryptHandle ncryptHandle, string propertyName, CngPropertyOptions options)
	{
		return GetPropertyAsPrimitive<int>(ncryptHandle, propertyName, options);
	}

	internal static nint GetPropertyAsIntPtr(this SafeNCryptHandle ncryptHandle, string propertyName, CngPropertyOptions options)
	{
		return GetPropertyAsPrimitive<nint>(ncryptHandle, propertyName, options);
	}

	private unsafe static T GetPropertyAsPrimitive<T>(SafeNCryptHandle ncryptHandle, string propertyName, CngPropertyOptions options) where T : unmanaged
	{
		Unsafe.SkipInit(out T val);
		global::Interop.NCrypt.ErrorCode errorCode = global::Interop.NCrypt.NCryptGetProperty(ncryptHandle, propertyName, &val, sizeof(T), out var _, options);
		return errorCode switch
		{
			global::Interop.NCrypt.ErrorCode.NTE_NOT_FOUND => default(T), 
			global::Interop.NCrypt.ErrorCode.ERROR_SUCCESS => val, 
			_ => throw errorCode.ToCryptographicException(), 
		};
	}

	internal static byte[] GetSymmetricKeyDataIfExportable(this CngKey cngKey, string algorithm)
	{
		using MemoryStream input = new MemoryStream(cngKey.Export(s_cipherKeyBlobFormat));
		using BinaryReader binaryReader = new BinaryReader(input, Encoding.Unicode);
		if (binaryReader.ReadInt32() != 16)
		{
			throw new CryptographicException(System.SR.Cryptography_KeyBlobParsingError);
		}
		if (binaryReader.ReadInt32() != 1380470851)
		{
			throw new CryptographicException(System.SR.Cryptography_KeyBlobParsingError);
		}
		int num = binaryReader.ReadInt32();
		binaryReader.ReadInt32();
		string text = new string(binaryReader.ReadChars(num / 2 - 1));
		if (text != algorithm)
		{
			throw new CryptographicException(System.SR.Format(System.SR.Cryptography_CngKeyWrongAlgorithm, text, algorithm));
		}
		if (binaryReader.ReadChar() != 0)
		{
			throw new CryptographicException(System.SR.Cryptography_KeyBlobParsingError);
		}
		if ((long)binaryReader.ReadInt32() != 1296188491)
		{
			throw new CryptographicException(System.SR.Cryptography_KeyBlobParsingError);
		}
		if ((long)binaryReader.ReadInt32() != 1)
		{
			throw new CryptographicException(System.SR.Cryptography_KeyBlobParsingError);
		}
		int count = binaryReader.ReadInt32();
		return binaryReader.ReadBytes(count);
	}

	internal unsafe static ArraySegment<byte> ToBCryptBlob(this in RSAParameters parameters)
	{
		if (parameters.Exponent == null || parameters.Modulus == null)
		{
			throw new CryptographicException(System.SR.Cryptography_InvalidRsaParameters);
		}
		bool flag;
		if (parameters.D == null)
		{
			flag = false;
			if (parameters.P != null || parameters.DP != null || parameters.Q != null || parameters.DQ != null || parameters.InverseQ != null)
			{
				throw new CryptographicException(System.SR.Cryptography_InvalidRsaParameters);
			}
		}
		else
		{
			flag = true;
			if (parameters.P == null || parameters.DP == null || parameters.Q == null || parameters.DQ == null || parameters.InverseQ == null)
			{
				throw new CryptographicException(System.SR.Cryptography_InvalidRsaParameters);
			}
			int num = (parameters.Modulus.Length + 1) / 2;
			if (parameters.D.Length != parameters.Modulus.Length || parameters.P.Length != num || parameters.Q.Length != num || parameters.DP.Length != num || parameters.DQ.Length != num || parameters.InverseQ.Length != num)
			{
				throw new CryptographicException(System.SR.Cryptography_InvalidRsaParameters);
			}
		}
		int num2 = sizeof(global::Interop.BCrypt.BCRYPT_RSAKEY_BLOB) + parameters.Exponent.Length + parameters.Modulus.Length;
		if (flag)
		{
			num2 += parameters.P.Length + parameters.Q.Length;
		}
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(num2);
		fixed (byte* ptr = &array[0])
		{
			global::Interop.BCrypt.BCRYPT_RSAKEY_BLOB* ptr2 = (global::Interop.BCrypt.BCRYPT_RSAKEY_BLOB*)ptr;
			ptr2->Magic = (flag ? global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAPRIVATE_MAGIC : global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAPUBLIC_MAGIC);
			ptr2->BitLength = parameters.Modulus.Length * 8;
			ptr2->cbPublicExp = parameters.Exponent.Length;
			ptr2->cbModulus = parameters.Modulus.Length;
			if (flag)
			{
				ptr2->cbPrime1 = parameters.P.Length;
				ptr2->cbPrime2 = parameters.Q.Length;
			}
			else
			{
				ptr2->cbPrime1 = (ptr2->cbPrime2 = 0);
			}
			int offset = sizeof(global::Interop.BCrypt.BCRYPT_RSAKEY_BLOB);
			global::Interop.BCrypt.Emit(array, ref offset, parameters.Exponent);
			global::Interop.BCrypt.Emit(array, ref offset, parameters.Modulus);
			if (flag)
			{
				global::Interop.BCrypt.Emit(array, ref offset, parameters.P);
				global::Interop.BCrypt.Emit(array, ref offset, parameters.Q);
			}
		}
		return new ArraySegment<byte>(array, 0, num2);
	}

	internal unsafe static void FromBCryptBlob(this ref RSAParameters rsaParams, ReadOnlySpan<byte> rsaBlob, bool includePrivateParameters)
	{
		if (rsaBlob.Length < sizeof(global::Interop.BCrypt.BCRYPT_RSAKEY_BLOB))
		{
			throw global::Interop.NCrypt.ErrorCode.E_FAIL.ToCryptographicException();
		}
		fixed (byte* ptr = &rsaBlob[0])
		{
			CheckMagicValueOfKey((global::Interop.BCrypt.KeyBlobMagicNumber)Unsafe.ReadUnaligned<int>(ptr), includePrivateParameters);
			global::Interop.BCrypt.BCRYPT_RSAKEY_BLOB* ptr2 = (global::Interop.BCrypt.BCRYPT_RSAKEY_BLOB*)ptr;
			int offset = sizeof(global::Interop.BCrypt.BCRYPT_RSAKEY_BLOB);
			rsaParams.Exponent = global::Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPublicExp);
			rsaParams.Modulus = global::Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbModulus);
			if (includePrivateParameters)
			{
				rsaParams.P = global::Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPrime1);
				rsaParams.Q = global::Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPrime2);
				rsaParams.DP = global::Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPrime1);
				rsaParams.DQ = global::Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPrime2);
				rsaParams.InverseQ = global::Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbPrime1);
				rsaParams.D = global::Interop.BCrypt.Consume(rsaBlob, ref offset, ptr2->cbModulus);
			}
		}
		static void CheckMagicValueOfKey(global::Interop.BCrypt.KeyBlobMagicNumber magic, bool flag)
		{
			if (flag)
			{
				if (magic != global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAPRIVATE_MAGIC && magic != global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAFULLPRIVATE_MAGIC)
				{
					throw new CryptographicException(System.SR.Cryptography_NotValidPrivateKey);
				}
			}
			else if (magic != global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAPUBLIC_MAGIC && magic != global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAPRIVATE_MAGIC && magic != global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_RSAFULLPRIVATE_MAGIC)
			{
				throw new CryptographicException(System.SR.Cryptography_NotValidPublicOrPrivateKey);
			}
		}
	}
}
