using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates;

internal sealed class CertificatePal : IDisposable, ICertificatePal, ICertificatePalCore
{
	private unsafe delegate T CertContextCallback<T>(global::Interop.Crypt32.CERT_CONTEXT* certContext);

	private SafeCertContextHandle _certContext;

	public nint Handle => _certContext.DangerousGetHandle();

	internal SafeCertContextHandle SafeHandle => _certContext;

	public string Issuer => GetIssuerOrSubject(issuer: true, reverse: true);

	public string Subject => GetIssuerOrSubject(issuer: false, reverse: true);

	public string LegacyIssuer => GetIssuerOrSubject(issuer: true, reverse: false);

	public string LegacySubject => GetIssuerOrSubject(issuer: false, reverse: false);

	public byte[] Thumbprint
	{
		get
		{
			int pcbData = 0;
			if (!global::Interop.Crypt32.CertGetCertificateContextProperty(_certContext, global::Interop.Crypt32.CertContextPropId.CERT_SHA1_HASH_PROP_ID, null, ref pcbData))
			{
				throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
			}
			byte[] array = new byte[pcbData];
			if (!global::Interop.Crypt32.CertGetCertificateContextProperty(_certContext, global::Interop.Crypt32.CertContextPropId.CERT_SHA1_HASH_PROP_ID, array, ref pcbData))
			{
				throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
			}
			return array;
		}
	}

	public unsafe string KeyAlgorithm => InvokeWithCertContext((global::Interop.Crypt32.CERT_CONTEXT* certContext) => Marshal.PtrToStringAnsi(certContext->pCertInfo->SubjectPublicKeyInfo.Algorithm.pszObjId));

	public unsafe byte[] KeyAlgorithmParameters => InvokeWithCertContext(delegate(global::Interop.Crypt32.CERT_CONTEXT* pCertContext)
	{
		string text = Marshal.PtrToStringAnsi(pCertContext->pCertInfo->SubjectPublicKeyInfo.Algorithm.pszObjId);
		int num = ((!(text == "1.2.840.113549.1.1.1")) ? global::Interop.Crypt32.FindOidInfo(global::Interop.Crypt32.CryptOidInfoKeyType.CRYPT_OID_INFO_OID_KEY, text, OidGroup.PublicKeyAlgorithm, fallBackToAllGroups: true).AlgId : 41984);
		byte* ptr = (byte*)5;
		if (num == 8704 && pCertContext->pCertInfo->SubjectPublicKeyInfo.Algorithm.Parameters.cbData == 0 && ((IntPtr*)(&pCertContext->pCertInfo->SubjectPublicKeyInfo.Algorithm.Parameters.pbData))->ToPointer() == ptr)
		{
			return PropagateKeyAlgorithmParametersFromChain();
		}
		global::Interop.Crypt32.DATA_BLOB parameters = pCertContext->pCertInfo->SubjectPublicKeyInfo.Algorithm.Parameters;
		return (parameters.pbData == IntPtr.Zero) ? null : parameters.ToByteArray();
	});

	public unsafe byte[] PublicKeyValue => InvokeWithCertContext((global::Interop.Crypt32.CERT_CONTEXT* pCertContext) => pCertContext->pCertInfo->SubjectPublicKeyInfo.PublicKey.ToByteArray());

	public unsafe byte[] SerialNumber => InvokeWithCertContext(delegate(global::Interop.Crypt32.CERT_CONTEXT* pCertContext)
	{
		byte[] array = pCertContext->pCertInfo->SerialNumber.ToByteArray();
		Array.Reverse(array);
		return array;
	});

	public unsafe string SignatureAlgorithm => InvokeWithCertContext((global::Interop.Crypt32.CERT_CONTEXT* pCertContext) => Marshal.PtrToStringAnsi(pCertContext->pCertInfo->SignatureAlgorithm.pszObjId));

	public unsafe DateTime NotAfter => InvokeWithCertContext((global::Interop.Crypt32.CERT_CONTEXT* pCertContext) => pCertContext->pCertInfo->NotAfter.ToDateTime());

	public unsafe DateTime NotBefore => InvokeWithCertContext((global::Interop.Crypt32.CERT_CONTEXT* pCertContext) => pCertContext->pCertInfo->NotBefore.ToDateTime());

	public unsafe byte[] RawData => InvokeWithCertContext((global::Interop.Crypt32.CERT_CONTEXT* pCertContext) => new Span<byte>(pCertContext->pbCertEncoded, pCertContext->cbCertEncoded).ToArray());

	public unsafe int Version => InvokeWithCertContext((global::Interop.Crypt32.CERT_CONTEXT* pCertContext) => pCertContext->pCertInfo->dwVersion + 1);

	public unsafe bool Archived
	{
		get
		{
			int pcbData = 0;
			return global::Interop.Crypt32.CertGetCertificateContextProperty(_certContext, global::Interop.Crypt32.CertContextPropId.CERT_ARCHIVED_PROP_ID, null, ref pcbData);
		}
		set
		{
			global::Interop.Crypt32.DATA_BLOB dATA_BLOB = new global::Interop.Crypt32.DATA_BLOB(IntPtr.Zero, 0u);
			global::Interop.Crypt32.DATA_BLOB* pvData = (value ? (&dATA_BLOB) : null);
			if (!global::Interop.Crypt32.CertSetCertificateContextProperty(_certContext, global::Interop.Crypt32.CertContextPropId.CERT_ARCHIVED_PROP_ID, global::Interop.Crypt32.CertSetPropertyFlags.None, pvData))
			{
				throw Marshal.GetLastPInvokeError().ToCryptographicException();
			}
		}
	}

	public unsafe string FriendlyName
	{
		get
		{
			uint pcbData = 0u;
			if (!global::Interop.Crypt32.CertGetCertificateContextPropertyString(_certContext, global::Interop.Crypt32.CertContextPropId.CERT_FRIENDLY_NAME_PROP_ID, null, ref pcbData))
			{
				return string.Empty;
			}
			uint num = (pcbData + 1) / 2;
			Span<char> span = ((num > 256) ? ((Span<char>)new char[num]) : stackalloc char[(int)num]);
			Span<char> span2 = span;
			fixed (char* reference = &MemoryMarshal.GetReference(span2))
			{
				if (!global::Interop.Crypt32.CertGetCertificateContextPropertyString(_certContext, global::Interop.Crypt32.CertContextPropId.CERT_FRIENDLY_NAME_PROP_ID, (byte*)reference, ref pcbData))
				{
					return string.Empty;
				}
			}
			return new string(span2.Slice(0, (int)pcbData / 2 - 1));
		}
		set
		{
			string text = value ?? string.Empty;
			nint num = Marshal.StringToHGlobalUni(text);
			try
			{
				global::Interop.Crypt32.DATA_BLOB dATA_BLOB = new global::Interop.Crypt32.DATA_BLOB(num, checked(2 * ((uint)text.Length + 1)));
				if (!global::Interop.Crypt32.CertSetCertificateContextProperty(_certContext, global::Interop.Crypt32.CertContextPropId.CERT_FRIENDLY_NAME_PROP_ID, global::Interop.Crypt32.CertSetPropertyFlags.None, &dATA_BLOB))
				{
					throw Marshal.GetLastPInvokeError().ToCryptographicException();
				}
			}
			finally
			{
				Marshal.FreeHGlobal(num);
			}
		}
	}

	public unsafe X500DistinguishedName SubjectName => InvokeWithCertContext((global::Interop.Crypt32.CERT_CONTEXT* certContext) => new X500DistinguishedName(certContext->pCertInfo->Subject.DangerousAsSpan()));

	public unsafe X500DistinguishedName IssuerName => InvokeWithCertContext((global::Interop.Crypt32.CERT_CONTEXT* certContext) => new X500DistinguishedName(certContext->pCertInfo->Issuer.DangerousAsSpan()));

	public unsafe IEnumerable<X509Extension> Extensions => InvokeWithCertContext(delegate(global::Interop.Crypt32.CERT_CONTEXT* certContext)
	{
		global::Interop.Crypt32.CERT_INFO* pCertInfo = certContext->pCertInfo;
		int cExtension = pCertInfo->cExtension;
		X509Extension[] array = new X509Extension[cExtension];
		for (int i = 0; i < cExtension; i++)
		{
			byte* num = (byte*)((IntPtr*)(&pCertInfo->rgExtension))->ToPointer() + (nint)i * (nint)sizeof(global::Interop.Crypt32.CERT_EXTENSION);
			Oid oid = new Oid(Marshal.PtrToStringAnsi(((global::Interop.Crypt32.CERT_EXTENSION*)num)->pszObjId), null);
			bool critical = ((global::Interop.Crypt32.CERT_EXTENSION*)num)->fCritical != 0;
			ReadOnlySpan<byte> rawData = ((global::Interop.Crypt32.CERT_EXTENSION*)num)->Value.DangerousAsSpan();
			array[i] = new X509Extension(oid, rawData, critical);
		}
		return array;
	});

	public bool HasPrivateKey => _certContext.ContainsPrivateKey;

	internal static ICertificatePal FromHandle(nint handle)
	{
		if (handle == IntPtr.Zero)
		{
			throw new ArgumentException(System.SR.Arg_InvalidHandle, "handle");
		}
		SafeCertContextHandle safeCertContextHandle = global::Interop.Crypt32.CertDuplicateCertificateContext(handle);
		if (safeCertContextHandle.IsInvalid)
		{
			CryptographicException ex = (-2147024890).ToCryptographicException();
			safeCertContextHandle.Dispose();
			throw ex;
		}
		int pcbData = 0;
		bool deleteKeyContainer = global::Interop.Crypt32.CertGetCertificateContextProperty(safeCertContextHandle, global::Interop.Crypt32.CertContextPropId.CERT_CLR_DELETE_KEY_PROP_ID, out global::Interop.Crypt32.DATA_BLOB _, ref pcbData);
		return new CertificatePal(safeCertContextHandle, deleteKeyContainer);
	}

	internal static ICertificatePal FromOtherCert(X509Certificate copyFrom)
	{
		return new CertificatePal((CertificatePal)copyFrom.Pal);
	}

	internal static ICertificatePal FromBlob(ReadOnlySpan<byte> rawData, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags)
	{
		return FromBlobOrFile(rawData, null, password, keyStorageFlags);
	}

	internal static ICertificatePal FromFile(string fileName, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags)
	{
		return FromBlobOrFile(ReadOnlySpan<byte>.Empty, fileName, password, keyStorageFlags);
	}

	private unsafe byte[] PropagateKeyAlgorithmParametersFromChain()
	{
		SafeX509ChainHandle ppChainContext = null;
		try
		{
			int pcbData = 0;
			if (!global::Interop.Crypt32.CertGetCertificateContextProperty(_certContext, global::Interop.Crypt32.CertContextPropId.CERT_PUBKEY_ALG_PARA_PROP_ID, null, ref pcbData))
			{
				global::Interop.Crypt32.CERT_CHAIN_PARA pChainPara = new global::Interop.Crypt32.CERT_CHAIN_PARA
				{
					cbSize = sizeof(global::Interop.Crypt32.CERT_CHAIN_PARA)
				};
				if (!global::Interop.Crypt32.CertGetCertificateChain(0, _certContext, null, SafeCrypt32Handle<SafeCertStoreHandle>.InvalidHandle, ref pChainPara, global::Interop.Crypt32.CertChainFlags.None, IntPtr.Zero, out ppChainContext))
				{
					throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
				}
				if (!global::Interop.Crypt32.CertGetCertificateContextProperty(_certContext, global::Interop.Crypt32.CertContextPropId.CERT_PUBKEY_ALG_PARA_PROP_ID, null, ref pcbData))
				{
					throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
				}
			}
			byte[] array = new byte[pcbData];
			if (!global::Interop.Crypt32.CertGetCertificateContextProperty(_certContext, global::Interop.Crypt32.CertContextPropId.CERT_PUBKEY_ALG_PARA_PROP_ID, array, ref pcbData))
			{
				throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
			}
			return array;
		}
		finally
		{
			ppChainContext?.Dispose();
		}
	}

	public PolicyData GetPolicyData()
	{
		throw new PlatformNotSupportedException();
	}

	public string GetNameInfo(X509NameType nameType, bool forIssuer)
	{
		return global::Interop.crypt32.CertGetNameString(_certContext, MapNameType(nameType), forIssuer ? global::Interop.Crypt32.CertNameFlags.CERT_NAME_ISSUER_FLAG : global::Interop.Crypt32.CertNameFlags.None, (global::Interop.Crypt32.CertNameStringType)33554435);
	}

	public void AppendPrivateKeyInfo(StringBuilder sb)
	{
		if (!HasPrivateKey)
		{
			return;
		}
		sb.AppendLine();
		sb.AppendLine();
		sb.AppendLine("[Private Key]");
		CspKeyContainerInfo cspKeyContainerInfo = null;
		try
		{
			CspParameters privateKeyCsp = CertificateHelpers.GetPrivateKeyCsp(_certContext);
			if (privateKeyCsp != null)
			{
				cspKeyContainerInfo = new CspKeyContainerInfo(privateKeyCsp);
			}
		}
		catch (CryptographicException)
		{
		}
		if (cspKeyContainerInfo == null)
		{
			return;
		}
		sb.AppendLine().Append("  Key Store: ").Append(cspKeyContainerInfo.MachineKeyStore ? "Machine" : "User");
		sb.AppendLine().Append("  Provider Name: ").Append(cspKeyContainerInfo.ProviderName);
		sb.AppendLine().Append("  Provider type: ").Append(cspKeyContainerInfo.ProviderType);
		sb.AppendLine().Append("  Key Spec: ").Append(cspKeyContainerInfo.KeyNumber);
		sb.AppendLine().Append("  Key Container Name: ").Append(cspKeyContainerInfo.KeyContainerName);
		try
		{
			string uniqueKeyContainerName = cspKeyContainerInfo.UniqueKeyContainerName;
			sb.AppendLine().Append("  Unique Key Container Name: ").Append(uniqueKeyContainerName);
		}
		catch (CryptographicException)
		{
		}
		catch (NotSupportedException)
		{
		}
		try
		{
			bool hardwareDevice = cspKeyContainerInfo.HardwareDevice;
			sb.AppendLine().Append("  Hardware Device: ").Append(hardwareDevice);
		}
		catch (CryptographicException)
		{
		}
		try
		{
			bool removable = cspKeyContainerInfo.Removable;
			sb.AppendLine().Append("  Removable: ").Append(removable);
		}
		catch (CryptographicException)
		{
		}
		try
		{
			bool value = cspKeyContainerInfo.Protected;
			sb.AppendLine().Append("  Protected: ").Append(value);
		}
		catch (CryptographicException)
		{
		}
		catch (NotSupportedException)
		{
		}
	}

	public void Dispose()
	{
		SafeCertContextHandle certContext = _certContext;
		_certContext = null;
		if (certContext != null && !certContext.IsInvalid)
		{
			certContext.Dispose();
		}
	}

	internal unsafe SafeCertContextHandle GetCertContext()
	{
		return InvokeWithCertContext((global::Interop.Crypt32.CERT_CONTEXT* certContext) => global::Interop.Crypt32.CertDuplicateCertificateContext((nint)certContext));
	}

	private static global::Interop.Crypt32.CertNameType MapNameType(X509NameType nameType)
	{
		switch (nameType)
		{
		case X509NameType.SimpleName:
			return global::Interop.Crypt32.CertNameType.CERT_NAME_SIMPLE_DISPLAY_TYPE;
		case X509NameType.EmailName:
			return global::Interop.Crypt32.CertNameType.CERT_NAME_EMAIL_TYPE;
		case X509NameType.UpnName:
			return global::Interop.Crypt32.CertNameType.CERT_NAME_UPN_TYPE;
		case X509NameType.DnsName:
		case X509NameType.DnsFromAlternativeName:
			return global::Interop.Crypt32.CertNameType.CERT_NAME_DNS_TYPE;
		case X509NameType.UrlName:
			return global::Interop.Crypt32.CertNameType.CERT_NAME_URL_TYPE;
		default:
			throw new ArgumentException(System.SR.Argument_InvalidNameType);
		}
	}

	private string GetIssuerOrSubject(bool issuer, bool reverse)
	{
		return global::Interop.crypt32.CertGetNameString(_certContext, global::Interop.Crypt32.CertNameType.CERT_NAME_RDN_TYPE, issuer ? global::Interop.Crypt32.CertNameFlags.CERT_NAME_ISSUER_FLAG : global::Interop.Crypt32.CertNameFlags.None, (global::Interop.Crypt32.CertNameStringType)(3 | (reverse ? 33554432 : 0)));
	}

	private CertificatePal(CertificatePal copyFrom)
	{
		_certContext = new SafeCertContextHandle(copyFrom._certContext);
	}

	internal CertificatePal(SafeCertContextHandle certContext, bool deleteKeyContainer)
	{
		if (deleteKeyContainer)
		{
			using SafeCertContextHandle safeCertContextHandle = certContext;
			certContext = global::Interop.Crypt32.CertDuplicateCertificateContextWithKeyContainerDeletion(safeCertContextHandle.DangerousGetHandle());
		}
		_certContext = certContext;
	}

	public byte[] Export(X509ContentType contentType, SafePasswordHandle password)
	{
		using IExportPal exportPal = StorePal.FromCertificate(this);
		return exportPal.Export(contentType, password);
	}

	public byte[] ExportPkcs12(Pkcs12ExportPbeParameters exportParameters, SafePasswordHandle password)
	{
		using IExportPal exportPal = StorePal.FromCertificate(this);
		return exportPal.ExportPkcs12(exportParameters, password);
	}

	public byte[] ExportPkcs12(PbeParameters exportParameters, SafePasswordHandle password)
	{
		using IExportPal exportPal = StorePal.FromCertificate(this);
		return exportPal.ExportPkcs12(exportParameters, password);
	}

	private unsafe T InvokeWithCertContext<T>(CertContextCallback<T> callback)
	{
		bool success = false;
		_certContext.DangerousAddRef(ref success);
		try
		{
			return callback(_certContext.DangerousCertContext);
		}
		finally
		{
			if (success)
			{
				_certContext.DangerousRelease();
			}
		}
	}

	private unsafe static CertificatePal FromBlobOrFile(ReadOnlySpan<byte> rawData, string fileName, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags)
	{
		bool flag = fileName != null;
		bool deleteKeyContainer = false;
		SafeCertStoreHandle phCertStore = null;
		SafeCryptMsgHandle phMsg = null;
		SafeCertContextHandle ppvContext = null;
		try
		{
			global::Interop.Crypt32.ContentType pdwContentType;
			fixed (byte* value = rawData)
			{
				fixed (char* ptr = fileName)
				{
					global::Interop.Crypt32.DATA_BLOB dATA_BLOB = new global::Interop.Crypt32.DATA_BLOB(new IntPtr(value), (!flag) ? ((uint)rawData.Length) : 0u);
					int dwObjectType = (flag ? 1 : 2);
					void* pvObject = (flag ? ((void*)ptr) : ((void*)(&dATA_BLOB)));
					if (!global::Interop.Crypt32.CryptQueryObject((global::Interop.Crypt32.CertQueryObjectType)dwObjectType, pvObject, global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_CERT | global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_SERIALIZED_CERT | global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED | global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED_EMBED | global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_PFX, global::Interop.Crypt32.ExpectedFormatTypeFlags.CERT_QUERY_FORMAT_FLAG_ALL, 0, out var _, out pdwContentType, out var _, out phCertStore, out phMsg, out ppvContext))
					{
						throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
					}
				}
			}
			switch (pdwContentType)
			{
			case global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_PKCS7_SIGNED:
			case global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_PKCS7_SIGNED_EMBED:
				ppvContext?.Dispose();
				ppvContext = GetSignerInPKCS7Store(phCertStore, phMsg);
				break;
			case global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_PFX:
				try
				{
					Pkcs12LoaderLimits pkcs12Limits = X509Certificate.GetPkcs12Limits(flag, password);
					if (flag)
					{
						return (CertificatePal)X509CertificateLoader.LoadPkcs12PalFromFile(fileName, password.DangerousGetSpan(), keyStorageFlags, pkcs12Limits);
					}
					return (CertificatePal)X509CertificateLoader.LoadPkcs12Pal(rawData, password.DangerousGetSpan(), keyStorageFlags, pkcs12Limits);
				}
				catch (Pkcs12LoadLimitExceededException inner)
				{
					throw new CryptographicException(System.SR.Cryptography_X509_PfxWithoutPassword_MaxAllowedIterationsExceeded, inner);
				}
			}
			CertificatePal result = new CertificatePal(ppvContext, deleteKeyContainer);
			ppvContext = null;
			return result;
		}
		finally
		{
			phCertStore?.Dispose();
			phMsg?.Dispose();
			ppvContext?.Dispose();
		}
	}

	private unsafe static SafeCertContextHandle GetSignerInPKCS7Store(SafeCertStoreHandle hCertStore, SafeCryptMsgHandle hCryptMsg)
	{
		int pcbData = 4;
		if (!global::Interop.Crypt32.CryptMsgGetParam(hCryptMsg, global::Interop.Crypt32.CryptMsgParamType.CMSG_SIGNER_COUNT_PARAM, 0, out var pvData, ref pcbData))
		{
			throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
		}
		if (pvData == 0)
		{
			throw (-2146889714).ToCryptographicException();
		}
		int pcbData2 = 0;
		if (!global::Interop.Crypt32.CryptMsgGetParam(hCryptMsg, global::Interop.Crypt32.CryptMsgParamType.CMSG_SIGNER_INFO_PARAM, 0, null, ref pcbData2))
		{
			throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
		}
		fixed (byte* ptr = new byte[pcbData2])
		{
			if (!global::Interop.Crypt32.CryptMsgGetParam(hCryptMsg, global::Interop.Crypt32.CryptMsgParamType.CMSG_SIGNER_INFO_PARAM, 0, ptr, ref pcbData2))
			{
				throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
			}
			CMSG_SIGNER_INFO_Partial* ptr2 = (CMSG_SIGNER_INFO_Partial*)ptr;
			global::Interop.Crypt32.CERT_INFO cERT_INFO = new global::Interop.Crypt32.CERT_INFO
			{
				Issuer = 
				{
					cbData = ptr2->Issuer.cbData,
					pbData = ptr2->Issuer.pbData
				},
				SerialNumber = 
				{
					cbData = ptr2->SerialNumber.cbData,
					pbData = ptr2->SerialNumber.pbData
				}
			};
			SafeCertContextHandle pCertContext = null;
			if (!global::Interop.crypt32.CertFindCertificateInStore(hCertStore, global::Interop.Crypt32.CertFindType.CERT_FIND_SUBJECT_CERT, &cERT_INFO, ref pCertContext))
			{
				CryptographicException ex = Marshal.GetHRForLastWin32Error().ToCryptographicException();
				pCertContext.Dispose();
				throw ex;
			}
			return pCertContext;
		}
	}

	public RSA GetRSAPrivateKey()
	{
		return GetPrivateKey<RSA>((CspParameters csp) => new RSACryptoServiceProvider(csp), (CngKey cngKey) => new RSACng(cngKey, transferOwnership: true));
	}

	public DSA GetDSAPrivateKey()
	{
		return GetPrivateKey((Func<CspParameters, DSA>)((CspParameters csp) => new DSACryptoServiceProvider(csp)), (Func<CngKey, DSA>)((CngKey cngKey) => new DSACng(cngKey, transferOwnership: true)));
	}

	public ECDsa GetECDsaPrivateKey()
	{
		return GetPrivateKey<ECDsa>(delegate
		{
			throw new NotSupportedException(System.SR.NotSupported_ECDsa_Csp);
		}, (CngKey cngKey) => new ECDsaCng(cngKey, transferOwnership: true));
	}

	public ECDiffieHellman GetECDiffieHellmanPrivateKey()
	{
		return GetPrivateKey<ECDiffieHellman>(delegate
		{
			throw new NotSupportedException(System.SR.NotSupported_ECDiffieHellman_Csp);
		}, FromCngKey);
		static ECDiffieHellmanCng FromCngKey(CngKey cngKey)
		{
			if (cngKey.AlgorithmGroup == CngAlgorithmGroup.ECDiffieHellman)
			{
				return new ECDiffieHellmanCng(cngKey, transferOwnership: true);
			}
			return null;
		}
	}

	public MLDsa GetMLDsaPrivateKey()
	{
		return GetPrivateKey<MLDsa>(delegate
		{
			throw new PlatformNotSupportedException();
		}, (CngKey cngKey) => new MLDsaCng(cngKey, transferOwnership: true));
	}

	public MLKem GetMLKemPrivateKey()
	{
		return null;
	}

	public SlhDsa GetSlhDsaPrivateKey()
	{
		return null;
	}

	public ICertificatePal CopyWithPrivateKey(DSA dsa)
	{
		if (dsa is DSACng dSACng)
		{
			ICertificatePal certificatePal = CopyWithPersistedCngKey(dSACng.Key);
			if (certificatePal != null)
			{
				return certificatePal;
			}
		}
		if (dsa is DSACryptoServiceProvider dSACryptoServiceProvider)
		{
			ICertificatePal certificatePal = CopyWithPersistedCapiKey(dSACryptoServiceProvider.CspKeyContainerInfo);
			if (certificatePal != null)
			{
				return certificatePal;
			}
		}
		DSAParameters parameters = dsa.ExportParameters(includePrivateParameters: true);
		using (PinAndClear.Track(parameters.X))
		{
			using DSACng dSACng2 = new DSACng();
			dSACng2.ImportParameters(parameters);
			return CopyWithEphemeralKey(dSACng2.Key);
		}
	}

	public ICertificatePal CopyWithPrivateKey(ECDsa ecdsa)
	{
		if (ecdsa is ECDsaCng eCDsaCng)
		{
			ICertificatePal certificatePal = CopyWithPersistedCngKey(eCDsaCng.Key);
			if (certificatePal != null)
			{
				return certificatePal;
			}
		}
		ECParameters parameters = ecdsa.ExportParameters(includePrivateParameters: true);
		using (PinAndClear.Track(parameters.D))
		{
			using ECDsaCng eCDsaCng2 = new ECDsaCng();
			eCDsaCng2.ImportParameters(parameters);
			return CopyWithEphemeralKey(eCDsaCng2.Key);
		}
	}

	public ICertificatePal CopyWithPrivateKey(ECDiffieHellman ecdh)
	{
		if (ecdh is ECDiffieHellmanCng eCDiffieHellmanCng)
		{
			ICertificatePal certificatePal = CopyWithPersistedCngKey(eCDiffieHellmanCng.Key);
			if (certificatePal != null)
			{
				return certificatePal;
			}
		}
		ECParameters parameters = ecdh.ExportParameters(includePrivateParameters: true);
		using (PinAndClear.Track(parameters.D))
		{
			using ECDiffieHellmanCng eCDiffieHellmanCng2 = new ECDiffieHellmanCng();
			eCDiffieHellmanCng2.ImportParameters(parameters);
			return CopyWithEphemeralKey(eCDiffieHellmanCng2.Key);
		}
	}

	public ICertificatePal CopyWithPrivateKey(MLDsa privateKey)
	{
		return CertificateHelpers.CopyWithPrivateKey(this, privateKey);
	}

	public ICertificatePal CopyWithPrivateKey(MLKem privateKey)
	{
		throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "MLKem"));
	}

	public ICertificatePal CopyWithPrivateKey(SlhDsa privateKey)
	{
		throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "SlhDsa"));
	}

	public ICertificatePal CopyWithPrivateKey(RSA rsa)
	{
		if (rsa is RSACng rSACng)
		{
			ICertificatePal certificatePal = CopyWithPersistedCngKey(rSACng.Key);
			if (certificatePal != null)
			{
				return certificatePal;
			}
		}
		if (rsa is RSACryptoServiceProvider rSACryptoServiceProvider)
		{
			ICertificatePal certificatePal = CopyWithPersistedCapiKey(rSACryptoServiceProvider.CspKeyContainerInfo);
			if (certificatePal != null)
			{
				return certificatePal;
			}
		}
		RSAParameters parameters = rsa.ExportParameters(includePrivateParameters: true);
		using (PinAndClear.Track(parameters.D))
		{
			using (PinAndClear.Track(parameters.P))
			{
				using (PinAndClear.Track(parameters.Q))
				{
					using (PinAndClear.Track(parameters.DP))
					{
						using (PinAndClear.Track(parameters.DQ))
						{
							using (PinAndClear.Track(parameters.InverseQ))
							{
								using RSACng rSACng2 = new RSACng();
								rSACng2.ImportParameters(parameters);
								return CopyWithEphemeralKey(rSACng2.Key);
							}
						}
					}
				}
			}
		}
	}

	private unsafe CertificatePal CopyWithPersistedCapiKey(CspKeyContainerInfo keyContainerInfo)
	{
		//The blocks IL_0063, IL_0080, IL_0083, IL_0085, IL_00b6 are reachable both inside and outside the pinned region starting at IL_005e. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if (string.IsNullOrEmpty(keyContainerInfo.KeyContainerName))
		{
			return null;
		}
		CertificatePal certificatePal = (CertificatePal)FromBlob(RawData, SafePasswordHandle.InvalidHandle, X509KeyStorageFlags.PersistKeySet);
		global::Interop.Crypt32.CRYPT_KEY_PROV_INFO cRYPT_KEY_PROV_INFO = default(global::Interop.Crypt32.CRYPT_KEY_PROV_INFO);
		fixed (char* keyContainerName = keyContainerInfo.KeyContainerName)
		{
			string? providerName = keyContainerInfo.ProviderName;
			char* intPtr;
			ref global::Interop.Crypt32.CRYPT_KEY_PROV_INFO reference;
			int dwFlags;
			CryptographicException ex;
			if (providerName == null)
			{
				char* pwszProvName;
				intPtr = (pwszProvName = null);
				cRYPT_KEY_PROV_INFO.pwszContainerName = keyContainerName;
				cRYPT_KEY_PROV_INFO.pwszProvName = pwszProvName;
				reference = ref cRYPT_KEY_PROV_INFO;
				dwFlags = (keyContainerInfo.MachineKeyStore ? 32 : 0);
				reference.dwFlags = (global::Interop.Crypt32.CryptAcquireContextFlags)dwFlags;
				cRYPT_KEY_PROV_INFO.dwProvType = keyContainerInfo.ProviderType;
				cRYPT_KEY_PROV_INFO.dwKeySpec = (int)keyContainerInfo.KeyNumber;
				if (!global::Interop.Crypt32.CertSetCertificateContextProperty(certificatePal._certContext, global::Interop.Crypt32.CertContextPropId.CERT_KEY_PROV_INFO_PROP_ID, global::Interop.Crypt32.CertSetPropertyFlags.None, &cRYPT_KEY_PROV_INFO))
				{
					ex = Marshal.GetLastPInvokeError().ToCryptographicException();
					certificatePal.Dispose();
					throw ex;
				}
			}
			else
			{
				fixed (char* ptr = &providerName.GetPinnableReference())
				{
					char* pwszProvName;
					intPtr = (pwszProvName = ptr);
					cRYPT_KEY_PROV_INFO.pwszContainerName = keyContainerName;
					cRYPT_KEY_PROV_INFO.pwszProvName = pwszProvName;
					reference = ref cRYPT_KEY_PROV_INFO;
					dwFlags = (int)(reference.dwFlags = (keyContainerInfo.MachineKeyStore ? global::Interop.Crypt32.CryptAcquireContextFlags.CRYPT_MACHINE_KEYSET : global::Interop.Crypt32.CryptAcquireContextFlags.None));
					cRYPT_KEY_PROV_INFO.dwProvType = keyContainerInfo.ProviderType;
					cRYPT_KEY_PROV_INFO.dwKeySpec = (int)keyContainerInfo.KeyNumber;
					if (!global::Interop.Crypt32.CertSetCertificateContextProperty(certificatePal._certContext, global::Interop.Crypt32.CertContextPropId.CERT_KEY_PROV_INFO_PROP_ID, global::Interop.Crypt32.CertSetPropertyFlags.None, &cRYPT_KEY_PROV_INFO))
					{
						ex = Marshal.GetLastPInvokeError().ToCryptographicException();
						certificatePal.Dispose();
						throw ex;
					}
				}
			}
		}
		return certificatePal;
	}

	private T GetPrivateKey<T>(Func<CspParameters, T> createCsp, Func<CngKey, T> createCng) where T : class, IDisposable
	{
		return CertificateHelpers.GetPrivateKey(this, createCsp, createCng);
	}

	private CertificatePal CopyWithPersistedCngKey(CngKey cngKey)
	{
		return CertificateHelpers.CopyWithPersistedCngKey(this, cngKey);
	}

	private CertificatePal CopyWithEphemeralKey(CngKey cngKey)
	{
		return CertificateHelpers.CopyWithEphemeralKey(this, cngKey);
	}
}
