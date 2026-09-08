using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates;

internal sealed class X509Pal : IX509Pal
{
	internal static IX509Pal Instance { get; } = BuildSingleton();

	private static IX509Pal BuildSingleton()
	{
		return new X509Pal();
	}

	public unsafe X509ContentType GetCertContentType(ReadOnlySpan<byte> rawData)
	{
		global::Interop.Crypt32.ContentType pdwContentType;
		fixed (byte* value = rawData)
		{
			global::Interop.Crypt32.DATA_BLOB dATA_BLOB = new global::Interop.Crypt32.DATA_BLOB(new IntPtr(value), (uint)rawData.Length);
			if (!global::Interop.Crypt32.CryptQueryObject(global::Interop.Crypt32.CertQueryObjectType.CERT_QUERY_OBJECT_BLOB, &dATA_BLOB, global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_ALL, global::Interop.Crypt32.ExpectedFormatTypeFlags.CERT_QUERY_FORMAT_FLAG_ALL, 0, IntPtr.Zero, out pdwContentType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
			{
				throw Marshal.GetLastPInvokeError().ToCryptographicException();
			}
		}
		return MapContentType(pdwContentType);
	}

	public unsafe X509ContentType GetCertContentType(string fileName)
	{
		global::Interop.Crypt32.ContentType pdwContentType;
		fixed (char* pvObject = fileName)
		{
			if (!global::Interop.Crypt32.CryptQueryObject(global::Interop.Crypt32.CertQueryObjectType.CERT_QUERY_OBJECT_FILE, pvObject, global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_ALL, global::Interop.Crypt32.ExpectedFormatTypeFlags.CERT_QUERY_FORMAT_FLAG_ALL, 0, IntPtr.Zero, out pdwContentType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
			{
				throw Marshal.GetLastPInvokeError().ToCryptographicException();
			}
		}
		return MapContentType(pdwContentType);
	}

	private static X509ContentType MapContentType(global::Interop.Crypt32.ContentType contentType)
	{
		switch (contentType)
		{
		case global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_CERT:
			return X509ContentType.Cert;
		case global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_SERIALIZED_STORE:
			return X509ContentType.SerializedStore;
		case global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_SERIALIZED_CERT:
			return X509ContentType.SerializedCert;
		case global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_PKCS7_SIGNED:
		case global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_PKCS7_UNSIGNED:
			return X509ContentType.Pkcs7;
		case global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_PKCS7_SIGNED_EMBED:
			return X509ContentType.Authenticode;
		case global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_PFX:
			return X509ContentType.Pfx;
		default:
			return X509ContentType.Unknown;
		}
	}

	public ECDsa DecodeECDsaPublicKey(ICertificatePal certificatePal)
	{
		if (certificatePal is CertificatePal certificatePal2)
		{
			return DecodeECPublicKey(certificatePal2, (CngKey cngKey) => new ECDsaCng(cngKey, transferOwnership: true));
		}
		throw new NotSupportedException(System.SR.NotSupported_KeyAlgorithm);
	}

	public ECDiffieHellman DecodeECDiffieHellmanPublicKey(ICertificatePal certificatePal)
	{
		if (certificatePal is CertificatePal certificatePal2)
		{
			return DecodeECPublicKey(certificatePal2, (CngKey cngKey) => new ECDiffieHellmanCng(cngKey, transferOwnership: true), global::Interop.Crypt32.CryptImportPublicKeyInfoFlags.CRYPT_OID_INFO_PUBKEY_ENCRYPT_KEY_FLAG);
		}
		throw new NotSupportedException(System.SR.NotSupported_KeyAlgorithm);
	}

	public AsymmetricAlgorithm DecodePublicKey(Oid oid, byte[] encodedKeyValue, byte[] encodedParameters, ICertificatePal certificatePal)
	{
		switch (global::Interop.Crypt32.FindOidInfo(global::Interop.Crypt32.CryptOidInfoKeyType.CRYPT_OID_INFO_OID_KEY, oid.Value, OidGroup.PublicKeyAlgorithm, fallBackToAllGroups: true).AlgId)
		{
		case 9216:
		case 41984:
		{
			RSABCrypt rSABCrypt = new RSABCrypt();
			rSABCrypt.ImportRSAPublicKey(encodedKeyValue, out var _);
			return rSABCrypt;
		}
		case 8704:
		{
			byte[] keyBlob = ConstructDSSPublicKeyCspBlob(encodedKeyValue, encodedParameters);
			DSACryptoServiceProvider dSACryptoServiceProvider = new DSACryptoServiceProvider();
			dSACryptoServiceProvider.ImportCspBlob(keyBlob);
			return dSACryptoServiceProvider;
		}
		default:
			throw new NotSupportedException(System.SR.NotSupported_KeyAlgorithm);
		}
	}

	private static TAlgorithm DecodeECPublicKey<TAlgorithm>(CertificatePal certificatePal, Func<CngKey, TAlgorithm> factory, global::Interop.Crypt32.CryptImportPublicKeyInfoFlags importFlags = global::Interop.Crypt32.CryptImportPublicKeyInfoFlags.NONE) where TAlgorithm : ECAlgorithm, new()
	{
		TAlgorithm val;
		using (SafeCertContextHandle certContext = certificatePal.GetCertContext())
		{
			using SafeBCryptKeyHandle safeBCryptKeyHandle = ImportPublicKeyInfo(certContext, importFlags);
			string curveName = GetCurveName(safeBCryptKeyHandle);
			if (curveName == null)
			{
				CngKeyBlobFormat cngKeyBlobFormat = ((!HasExplicitParameters(safeBCryptKeyHandle)) ? CngKeyBlobFormat.EccPublicBlob : CngKeyBlobFormat.EccFullPublicBlob);
				ArraySegment<byte> arraySegment = ExportKeyBlob(safeBCryptKeyHandle, cngKeyBlobFormat);
				try
				{
					val = factory(CngKey.Import(arraySegment, cngKeyBlobFormat));
				}
				finally
				{
					System.Security.Cryptography.CryptoPool.Return(arraySegment);
				}
			}
			else
			{
				CngKeyBlobFormat cngKeyBlobFormat = CngKeyBlobFormat.EccPublicBlob;
				ArraySegment<byte> arraySegment2 = ExportKeyBlob(safeBCryptKeyHandle, cngKeyBlobFormat);
				ECParameters ecParams = default(ECParameters);
				ExportNamedCurveParameters(ref ecParams, arraySegment2, includePrivateParameters: false);
				System.Security.Cryptography.CryptoPool.Return(arraySegment2);
				ecParams.Curve = ECCurve.CreateFromFriendlyName(curveName);
				val = new TAlgorithm();
				val.ImportParameters(ecParams);
			}
		}
		return val;
	}

	private unsafe static SafeBCryptKeyHandle ImportPublicKeyInfo(SafeCertContextHandle certContext, global::Interop.Crypt32.CryptImportPublicKeyInfoFlags importFlags)
	{
		bool success = false;
		certContext.DangerousAddRef(ref success);
		try
		{
			if (!global::Interop.Crypt32.CryptImportPublicKeyInfoEx2(global::Interop.Crypt32.CertEncodingType.X509_ASN_ENCODING, &certContext.DangerousCertContext->pCertInfo->SubjectPublicKeyInfo, importFlags, null, out var phKey))
			{
				CryptographicException ex = Marshal.GetHRForLastWin32Error().ToCryptographicException();
				phKey.Dispose();
				throw ex;
			}
			return phKey;
		}
		finally
		{
			if (success)
			{
				certContext.DangerousRelease();
			}
		}
	}

	private static ArraySegment<byte> ExportKeyBlob(SafeBCryptKeyHandle bCryptKeyHandle, CngKeyBlobFormat blobFormat)
	{
		string format = blobFormat.Format;
		return global::Interop.BCrypt.BCryptExportKey(bCryptKeyHandle, format);
	}

	private unsafe static void ExportNamedCurveParameters(ref ECParameters ecParams, ReadOnlySpan<byte> ecBlob, bool includePrivateParameters)
	{
		fixed (byte* ptr = &ecBlob[0])
		{
			global::Interop.BCrypt.BCRYPT_ECCKEY_BLOB* ptr2 = (global::Interop.BCrypt.BCRYPT_ECCKEY_BLOB*)ptr;
			int offset = sizeof(global::Interop.BCrypt.BCRYPT_ECCKEY_BLOB);
			ecParams.Q = new ECPoint
			{
				X = global::Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbKey),
				Y = global::Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbKey)
			};
			if (includePrivateParameters)
			{
				ecParams.D = global::Interop.BCrypt.Consume(ecBlob, ref offset, ptr2->cbKey);
			}
		}
	}

	private static byte[] ConstructDSSPublicKeyCspBlob(byte[] encodedKeyValue, byte[] encodedParameters)
	{
		if (encodedParameters == null)
		{
			throw (-2146893803).ToCryptographicException();
		}
		byte[] array = DecodeDssKeyValue(encodedKeyValue);
		DecodeDssParameters(encodedParameters, out var p, out var q, out var g);
		int num = p.Length;
		if (num == 0)
		{
			throw (-2146893803).ToCryptographicException();
		}
		MemoryStream memoryStream = new MemoryStream(16 + num + 20 + num + num + 24);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write((byte)6);
		binaryWriter.Write((byte)2);
		binaryWriter.Write((short)0);
		binaryWriter.Write(8704u);
		binaryWriter.Write(827544388);
		binaryWriter.Write(num * 8);
		binaryWriter.Write(p);
		int num2 = q.Length;
		if (num2 == 0 || num2 > 20)
		{
			throw (-2146893803).ToCryptographicException();
		}
		binaryWriter.Write(q);
		if (20 > num2)
		{
			binaryWriter.Write(new byte[20 - num2]);
		}
		num2 = g.Length;
		if (num2 == 0 || num2 > num)
		{
			throw (-2146893803).ToCryptographicException();
		}
		binaryWriter.Write(g);
		if (num > num2)
		{
			binaryWriter.Write(new byte[num - num2]);
		}
		num2 = array.Length;
		if (num2 == 0 || num2 > num)
		{
			throw (-2146893803).ToCryptographicException();
		}
		binaryWriter.Write(array);
		if (num > num2)
		{
			binaryWriter.Write(new byte[num - num2]);
		}
		binaryWriter.Write(uint.MaxValue);
		binaryWriter.Write(new byte[20]);
		return memoryStream.ToArray();
	}

	private unsafe static byte[] DecodeDssKeyValue(byte[] encodedKeyValue)
	{
		return encodedKeyValue.DecodeObject(CryptDecodeObjectStructType.X509_DSS_PUBLICKEY, (void* pvDecoded, int cbDecoded) => ((global::Interop.Crypt32.DATA_BLOB*)pvDecoded)->ToByteArray());
	}

	private unsafe static void DecodeDssParameters(byte[] encodedParameters, out byte[] p, out byte[] q, out byte[] g)
	{
		(p, q, g) = encodedParameters.DecodeObject(CryptDecodeObjectStructType.X509_DSS_PARAMETERS, (void* pvDecoded, int cbDecoded) => (((CERT_DSS_PARAMETERS*)pvDecoded)->p.ToByteArray(), ((CERT_DSS_PARAMETERS*)pvDecoded)->q.ToByteArray(), ((CERT_DSS_PARAMETERS*)pvDecoded)->g.ToByteArray()));
	}

	private static bool HasExplicitParameters(SafeBCryptKeyHandle bcryptHandle)
	{
		byte[] property = GetProperty(bcryptHandle, "ECCParameters");
		if (property != null)
		{
			return property.Length != 0;
		}
		return false;
	}

	private static string GetCurveName(SafeBCryptKeyHandle bcryptHandle)
	{
		return GetPropertyAsString(bcryptHandle, "ECCCurveName");
	}

	private unsafe static string GetPropertyAsString(SafeBCryptKeyHandle cryptHandle, string propertyName)
	{
		byte[] property = GetProperty(cryptHandle, propertyName);
		if (property == null || property.Length == 0)
		{
			return null;
		}
		fixed (byte* ptr = &property[0])
		{
			return Marshal.PtrToStringUni((nint)ptr);
		}
	}

	private unsafe static byte[] GetProperty(SafeBCryptKeyHandle cryptHandle, string propertyName)
	{
		if (global::Interop.BCrypt.BCryptGetProperty(cryptHandle, propertyName, null, 0, out var pcbResult, 0) != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			return null;
		}
		byte[] array = new byte[pcbResult];
		global::Interop.BCrypt.NTSTATUS num;
		fixed (byte* pbOutput = array)
		{
			num = global::Interop.BCrypt.BCryptGetProperty(cryptHandle, propertyName, pbOutput, array.Length, out pcbResult, 0);
		}
		if (num != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			return null;
		}
		Array.Resize(ref array, pcbResult);
		return array;
	}

	public unsafe string X500DistinguishedNameDecode(byte[] encodedDistinguishedName, X500DistinguishedNameFlags flag)
	{
		int dwStrType = (int)(global::Interop.Crypt32.CertNameStrTypeAndFlags.CERT_X500_NAME_STR | MapNameToStrFlag(flag));
		fixed (byte* value = encodedDistinguishedName)
		{
			Unsafe.SkipInit(out global::Interop.Crypt32.DATA_BLOB dATA_BLOB);
			dATA_BLOB.cbData = (uint)encodedDistinguishedName.Length;
			dATA_BLOB.pbData = new IntPtr(value);
			int num = global::Interop.Crypt32.CertNameToStr(65537, &dATA_BLOB, dwStrType, null, 0);
			if (num == 0)
			{
				throw (-2146762476).ToCryptographicException();
			}
			Span<char> span = ((num > 256) ? ((Span<char>)new char[num]) : stackalloc char[num]);
			Span<char> span2 = span;
			fixed (char* psz = span2)
			{
				if (global::Interop.Crypt32.CertNameToStr(65537, &dATA_BLOB, dwStrType, psz, num) == 0)
				{
					throw (-2146762476).ToCryptographicException();
				}
			}
			return new string(span2.Slice(0, num - 1));
		}
	}

	public byte[] X500DistinguishedNameEncode(string distinguishedName, X500DistinguishedNameFlags flag)
	{
		global::Interop.Crypt32.CertNameStrTypeAndFlags dwStrType = global::Interop.Crypt32.CertNameStrTypeAndFlags.CERT_X500_NAME_STR | MapNameToStrFlag(flag);
		int pcbEncoded = 0;
		if (!global::Interop.Crypt32.CertStrToName(global::Interop.Crypt32.CertEncodingType.All, distinguishedName, dwStrType, IntPtr.Zero, null, ref pcbEncoded, IntPtr.Zero))
		{
			throw Marshal.GetLastPInvokeError().ToCryptographicException();
		}
		byte[] array = new byte[pcbEncoded];
		if (!global::Interop.Crypt32.CertStrToName(global::Interop.Crypt32.CertEncodingType.All, distinguishedName, dwStrType, IntPtr.Zero, array, ref pcbEncoded, IntPtr.Zero))
		{
			throw Marshal.GetLastPInvokeError().ToCryptographicException();
		}
		return array;
	}

	public unsafe string X500DistinguishedNameFormat(byte[] encodedDistinguishedName, bool multiLine)
	{
		if (encodedDistinguishedName == null || encodedDistinguishedName.Length == 0)
		{
			return string.Empty;
		}
		int dwFormatStrType = (multiLine ? 1 : 0);
		int pcbFormat = 0;
		if (!global::Interop.Crypt32.CryptFormatObject(1, 0, dwFormatStrType, IntPtr.Zero, (byte*)7, encodedDistinguishedName, encodedDistinguishedName.Length, null, ref pcbFormat))
		{
			return encodedDistinguishedName.ToHexStringUpper();
		}
		int num = (pcbFormat + 1) / 2;
		Span<char> span = ((num > 256) ? ((Span<char>)new char[num]) : stackalloc char[num]);
		Span<char> span2 = span;
		fixed (char* pbFormat = span2)
		{
			if (!global::Interop.Crypt32.CryptFormatObject(1, 0, dwFormatStrType, IntPtr.Zero, (byte*)7, encodedDistinguishedName, encodedDistinguishedName.Length, pbFormat, ref pcbFormat))
			{
				return encodedDistinguishedName.ToHexStringUpper();
			}
		}
		return new string(span2.Slice(0, pcbFormat / 2 - 1));
	}

	private static global::Interop.Crypt32.CertNameStrTypeAndFlags MapNameToStrFlag(X500DistinguishedNameFlags flag)
	{
		global::Interop.Crypt32.CertNameStrTypeAndFlags certNameStrTypeAndFlags = (global::Interop.Crypt32.CertNameStrTypeAndFlags)0;
		if (flag != X500DistinguishedNameFlags.None)
		{
			if ((flag & X500DistinguishedNameFlags.Reversed) == X500DistinguishedNameFlags.Reversed)
			{
				certNameStrTypeAndFlags |= global::Interop.Crypt32.CertNameStrTypeAndFlags.CERT_NAME_STR_REVERSE_FLAG;
			}
			if ((flag & X500DistinguishedNameFlags.UseSemicolons) == X500DistinguishedNameFlags.UseSemicolons)
			{
				certNameStrTypeAndFlags |= global::Interop.Crypt32.CertNameStrTypeAndFlags.CERT_NAME_STR_SEMICOLON_FLAG;
			}
			else if ((flag & X500DistinguishedNameFlags.UseCommas) == X500DistinguishedNameFlags.UseCommas)
			{
				certNameStrTypeAndFlags |= global::Interop.Crypt32.CertNameStrTypeAndFlags.CERT_NAME_STR_COMMA_FLAG;
			}
			else if ((flag & X500DistinguishedNameFlags.UseNewLines) == X500DistinguishedNameFlags.UseNewLines)
			{
				certNameStrTypeAndFlags |= global::Interop.Crypt32.CertNameStrTypeAndFlags.CERT_NAME_STR_CRLF_FLAG;
			}
			if ((flag & X500DistinguishedNameFlags.DoNotUsePlusSign) == X500DistinguishedNameFlags.DoNotUsePlusSign)
			{
				certNameStrTypeAndFlags |= global::Interop.Crypt32.CertNameStrTypeAndFlags.CERT_NAME_STR_NO_PLUS_FLAG;
			}
			if ((flag & X500DistinguishedNameFlags.DoNotUseQuotes) == X500DistinguishedNameFlags.DoNotUseQuotes)
			{
				certNameStrTypeAndFlags |= global::Interop.Crypt32.CertNameStrTypeAndFlags.CERT_NAME_STR_NO_QUOTING_FLAG;
			}
			if ((flag & X500DistinguishedNameFlags.ForceUTF8Encoding) == X500DistinguishedNameFlags.ForceUTF8Encoding)
			{
				certNameStrTypeAndFlags |= global::Interop.Crypt32.CertNameStrTypeAndFlags.CERT_NAME_STR_FORCE_UTF8_DIR_STR_FLAG;
			}
			if ((flag & X500DistinguishedNameFlags.UseUTF8Encoding) == X500DistinguishedNameFlags.UseUTF8Encoding)
			{
				certNameStrTypeAndFlags |= global::Interop.Crypt32.CertNameStrTypeAndFlags.CERT_NAME_STR_ENABLE_UTF8_UNICODE_FLAG;
			}
			else if ((flag & X500DistinguishedNameFlags.UseT61Encoding) == X500DistinguishedNameFlags.UseT61Encoding)
			{
				certNameStrTypeAndFlags |= global::Interop.Crypt32.CertNameStrTypeAndFlags.CERT_NAME_STR_ENABLE_T61_UNICODE_FLAG;
			}
		}
		return certNameStrTypeAndFlags;
	}
}
