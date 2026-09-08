using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates;

internal sealed class StorePal : IDisposable, IStorePal, IExportPal, ILoaderPal
{
	internal sealed class CollectionBasedLoader : ILoaderPal, IDisposable
	{
		private X509Certificate2Collection _coll;

		internal CollectionBasedLoader(X509Certificate2Collection coll)
		{
			_coll = coll;
		}

		public void Dispose()
		{
			X509Certificate2Collection coll = _coll;
			_coll = null;
			if (coll == null)
			{
				return;
			}
			foreach (X509Certificate2 item in coll)
			{
				item.Dispose();
			}
		}

		public void MoveTo(X509Certificate2Collection collection)
		{
			collection.AddRange(_coll);
			_coll = null;
		}
	}

	private SafeCertStoreHandle _certStore;

	private static readonly bool s_supportsAes256Sha256 = OperatingSystem.IsWindowsVersionAtLeast(10, 0, 16299);

	internal SafeCertStoreHandle SafeCertStoreHandle => _certStore;

	SafeHandle IStorePal.SafeHandle
	{
		get
		{
			if (_certStore == null || _certStore.IsInvalid || _certStore.IsClosed)
			{
				throw new CryptographicException(System.SR.Cryptography_X509_StoreNotOpen);
			}
			return _certStore;
		}
	}

	internal static IStorePal FromHandle(nint storeHandle)
	{
		if (storeHandle == IntPtr.Zero)
		{
			throw new ArgumentNullException("storeHandle");
		}
		SafeCertStoreHandle safeCertStoreHandle = global::Interop.Crypt32.CertDuplicateStore(storeHandle);
		if (safeCertStoreHandle == null || safeCertStoreHandle.IsInvalid)
		{
			safeCertStoreHandle?.Dispose();
			throw new CryptographicException(System.SR.Cryptography_InvalidStoreHandle, "storeHandle");
		}
		return new StorePal(safeCertStoreHandle);
	}

	internal static ILoaderPal FromBlob(ReadOnlySpan<byte> rawData, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags)
	{
		return FromBlobOrFile(rawData, null, password, keyStorageFlags);
	}

	internal static ILoaderPal FromFile(string fileName, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags)
	{
		return FromBlobOrFile(null, fileName, password, keyStorageFlags);
	}

	internal static IExportPal FromCertificate(ICertificatePalCore cert)
	{
		CertificatePal obj = (CertificatePal)cert;
		SafeCertStoreHandle safeCertStoreHandle = global::Interop.crypt32.CertOpenStore(CertStoreProvider.CERT_STORE_PROV_MEMORY, global::Interop.Crypt32.CertEncodingType.All, IntPtr.Zero, global::Interop.Crypt32.CertStoreFlags.CERT_STORE_DEFER_CLOSE_UNTIL_LAST_FREE_FLAG | global::Interop.Crypt32.CertStoreFlags.CERT_STORE_ENUM_ARCHIVED_FLAG | global::Interop.Crypt32.CertStoreFlags.CERT_STORE_CREATE_NEW_FLAG, null);
		using (SafeCertContextHandle pCertContext = obj.GetCertContext())
		{
			if (safeCertStoreHandle.IsInvalid || !global::Interop.Crypt32.CertAddCertificateLinkToStore(safeCertStoreHandle, pCertContext, global::Interop.Crypt32.CertStoreAddDisposition.CERT_STORE_ADD_ALWAYS, IntPtr.Zero))
			{
				CryptographicException ex = Marshal.GetHRForLastWin32Error().ToCryptographicException();
				safeCertStoreHandle.Dispose();
				throw ex;
			}
		}
		return new StorePal(safeCertStoreHandle);
	}

	internal static IExportPal LinkFromCertificateCollection(X509Certificate2Collection certificates)
	{
		SafeCertStoreHandle safeCertStoreHandle = global::Interop.crypt32.CertOpenStore(CertStoreProvider.CERT_STORE_PROV_MEMORY, global::Interop.Crypt32.CertEncodingType.All, IntPtr.Zero, global::Interop.Crypt32.CertStoreFlags.CERT_STORE_ENUM_ARCHIVED_FLAG | global::Interop.Crypt32.CertStoreFlags.CERT_STORE_CREATE_NEW_FLAG, null);
		try
		{
			if (safeCertStoreHandle.IsInvalid)
			{
				throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
			}
			for (int i = 0; i < certificates.Count; i++)
			{
				using SafeCertContextHandle pCertContext = ((CertificatePal)certificates[i].Pal).GetCertContext();
				if (!global::Interop.Crypt32.CertAddCertificateLinkToStore(safeCertStoreHandle, pCertContext, global::Interop.Crypt32.CertStoreAddDisposition.CERT_STORE_ADD_ALWAYS, IntPtr.Zero))
				{
					throw Marshal.GetLastPInvokeError().ToCryptographicException();
				}
			}
			return new StorePal(safeCertStoreHandle);
		}
		catch
		{
			safeCertStoreHandle.Dispose();
			throw;
		}
	}

	internal static IStorePal FromSystemStore(string storeName, StoreLocation storeLocation, OpenFlags openFlags)
	{
		global::Interop.Crypt32.CertStoreFlags dwFlags = MapX509StoreFlags(storeLocation, openFlags);
		SafeCertStoreHandle safeCertStoreHandle = global::Interop.crypt32.CertOpenStore(CertStoreProvider.CERT_STORE_PROV_SYSTEM_W, global::Interop.Crypt32.CertEncodingType.All, IntPtr.Zero, dwFlags, storeName);
		if (safeCertStoreHandle.IsInvalid)
		{
			CryptographicException ex = Marshal.GetLastPInvokeError().ToCryptographicException();
			safeCertStoreHandle.Dispose();
			throw ex;
		}
		global::Interop.Crypt32.CertControlStore(safeCertStoreHandle, global::Interop.Crypt32.CertControlStoreFlags.None, global::Interop.Crypt32.CertControlStoreType.CERT_STORE_CTRL_AUTO_RESYNC, IntPtr.Zero);
		return new StorePal(safeCertStoreHandle);
	}

	public void CloneTo(X509Certificate2Collection collection)
	{
		CopyTo(collection);
	}

	public void CopyTo(X509Certificate2Collection collection)
	{
		SafeCertContextHandle pCertContext = null;
		while (global::Interop.crypt32.CertEnumCertificatesInStore(_certStore, ref pCertContext))
		{
			X509Certificate2 certificate = new X509Certificate2(pCertContext.DangerousGetHandle());
			collection.Add(certificate);
		}
	}

	public void Add(ICertificatePal certificate)
	{
		using SafeCertContextHandle pCertContext = ((CertificatePal)certificate).GetCertContext();
		if (!global::Interop.Crypt32.CertAddCertificateContextToStore(_certStore, pCertContext, global::Interop.Crypt32.CertStoreAddDisposition.CERT_STORE_ADD_REPLACE_EXISTING_INHERIT_PROPERTIES, IntPtr.Zero))
		{
			throw Marshal.GetLastPInvokeError().ToCryptographicException();
		}
	}

	public unsafe void Remove(ICertificatePal certificate)
	{
		using SafeCertContextHandle safeCertContextHandle = ((CertificatePal)certificate).GetCertContext();
		SafeCertContextHandle pCertContext = null;
		global::Interop.Crypt32.CERT_CONTEXT* dangerousCertContext = safeCertContextHandle.DangerousCertContext;
		if (global::Interop.crypt32.CertFindCertificateInStore(_certStore, global::Interop.Crypt32.CertFindType.CERT_FIND_EXISTING, dangerousCertContext, ref pCertContext))
		{
			global::Interop.Crypt32.CERT_CONTEXT* pCertContext2 = pCertContext.Disconnect();
			pCertContext.Dispose();
			if (!global::Interop.Crypt32.CertDeleteCertificateFromStore(pCertContext2))
			{
				throw Marshal.GetLastPInvokeError().ToCryptographicException();
			}
		}
	}

	public void Dispose()
	{
		SafeCertStoreHandle certStore = _certStore;
		if (certStore != null)
		{
			_certStore = null;
			certStore.Dispose();
		}
	}

	internal StorePal(SafeCertStoreHandle certStore)
	{
		_certStore = certStore;
	}

	public void MoveTo(X509Certificate2Collection collection)
	{
		CopyTo(collection);
		Dispose();
	}

	public unsafe byte[] Export(X509ContentType contentType, SafePasswordHandle password)
	{
		switch (contentType)
		{
		case X509ContentType.Cert:
		{
			SafeCertContextHandle pCertContext2 = null;
			if (!global::Interop.crypt32.CertEnumCertificatesInStore(_certStore, ref pCertContext2))
			{
				return null;
			}
			try
			{
				byte[] array2 = new byte[pCertContext2.DangerousCertContext->cbCertEncoded];
				Marshal.Copy((nint)pCertContext2.DangerousCertContext->pbCertEncoded, array2, 0, array2.Length);
				GC.KeepAlive(pCertContext2);
				return array2;
			}
			finally
			{
				pCertContext2.Dispose();
			}
		}
		case X509ContentType.SerializedCert:
		{
			SafeCertContextHandle pCertContext = null;
			if (!global::Interop.crypt32.CertEnumCertificatesInStore(_certStore, ref pCertContext))
			{
				return null;
			}
			try
			{
				int pcbElement = 0;
				if (!global::Interop.Crypt32.CertSerializeCertificateStoreElement(pCertContext, 0, null, ref pcbElement))
				{
					throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
				}
				byte[] array = new byte[pcbElement];
				if (!global::Interop.Crypt32.CertSerializeCertificateStoreElement(pCertContext, 0, array, ref pcbElement))
				{
					throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
				}
				return array;
			}
			finally
			{
				pCertContext.Dispose();
			}
		}
		case X509ContentType.Pfx:
			return ExportPkcs12Core(null, password);
		case X509ContentType.SerializedStore:
			return SaveToMemoryStore(global::Interop.Crypt32.CertStoreSaveAs.CERT_STORE_SAVE_AS_STORE);
		case X509ContentType.Pkcs7:
			return SaveToMemoryStore(global::Interop.Crypt32.CertStoreSaveAs.CERT_STORE_SAVE_AS_PKCS7);
		default:
			throw new CryptographicException(System.SR.Cryptography_X509_InvalidContentType);
		}
	}

	public byte[] ExportPkcs12(Pkcs12ExportPbeParameters exportParameters, SafePasswordHandle password)
	{
		return ExportPkcs12Core(exportParameters, password);
	}

	public byte[] ExportPkcs12(PbeParameters exportParameters, SafePasswordHandle password)
	{
		return ReEncryptAndSealPkcs12(ExportPkcs12Core(null, password), password, exportParameters);
	}

	private unsafe byte[] ExportPkcs12Core(Pkcs12ExportPbeParameters? exportParameters, SafePasswordHandle password)
	{
		global::Interop.Crypt32.DATA_BLOB pPFX = new global::Interop.Crypt32.DATA_BLOB(IntPtr.Zero, 0u);
		global::Interop.Crypt32.PFXExportFlags pFXExportFlags = global::Interop.Crypt32.PFXExportFlags.REPORT_NOT_ABLE_TO_EXPORT_PRIVATE_KEY | global::Interop.Crypt32.PFXExportFlags.EXPORT_PRIVATE_KEYS;
		global::Interop.Crypt32.PKCS12_PBES2_EXPORT_PARAMS* pvPara = null;
		PbeParameters pbeParameters = null;
		char* pwszPbes2Alg = stackalloc char[14]
		{
			'A', 'E', 'S', '2', '5', '6', '-', 'S', 'H', 'A',
			'2', '5', '6', '\0'
		};
		global::Interop.Crypt32.PKCS12_PBES2_EXPORT_PARAMS pKCS12_PBES2_EXPORT_PARAMS = new global::Interop.Crypt32.PKCS12_PBES2_EXPORT_PARAMS
		{
			dwSize = (uint)sizeof(global::Interop.Crypt32.PKCS12_PBES2_EXPORT_PARAMS),
			hNcryptDescriptor = 0,
			pwszPbes2Alg = pwszPbes2Alg
		};
		bool flag;
		switch (exportParameters)
		{
		case Pkcs12ExportPbeParameters.Default:
		case Pkcs12ExportPbeParameters.Pbes2Aes256Sha256:
			flag = true;
			break;
		default:
			flag = false;
			break;
		}
		if (flag)
		{
			if (s_supportsAes256Sha256)
			{
				pFXExportFlags |= global::Interop.Crypt32.PFXExportFlags.PKCS12_EXPORT_PBES2_PARAMS;
				pvPara = &pKCS12_PBES2_EXPORT_PARAMS;
			}
			else
			{
				pbeParameters = Helpers.WindowsAesPbe;
			}
		}
		else if (exportParameters == Pkcs12ExportPbeParameters.Pkcs12TripleDesSha1)
		{
			pbeParameters = Helpers.Windows3desPbe;
		}
		if (!global::Interop.Crypt32.PFXExportCertStoreEx(_certStore, ref pPFX, password, pvPara, pFXExportFlags))
		{
			throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
		}
		byte[] array = new byte[pPFX.cbData];
		fixed (byte* value = array)
		{
			pPFX.pbData = new IntPtr(value);
			if (!global::Interop.Crypt32.PFXExportCertStoreEx(_certStore, ref pPFX, password, pvPara, pFXExportFlags))
			{
				throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
			}
		}
		if (pbeParameters == null)
		{
			return array;
		}
		return ReEncryptAndSealPkcs12(array, password, pbeParameters);
	}

	private static byte[] ReEncryptAndSealPkcs12(byte[] pkcs12, SafePasswordHandle password, PbeParameters newPbeParameters)
	{
		bool success = false;
		try
		{
			password.DangerousAddRef(ref success);
			ReadOnlySpan<char> password2 = password.DangerousGetSpan();
			Pkcs12Info pkcs12Info = Pkcs12Info.Decode(pkcs12, out var bytesConsumed, skipCopy: true);
			if (!pkcs12Info.VerifyMac(password2))
			{
				throw new CryptographicException();
			}
			Pkcs12Builder pkcs12Builder = new Pkcs12Builder();
			foreach (Pkcs12SafeContents item in pkcs12Info.AuthenticatedSafe)
			{
				bool flag;
				switch (item.ConfidentialityMode)
				{
				case Pkcs12ConfidentialityMode.Password:
					item.Decrypt(password2);
					flag = true;
					break;
				case Pkcs12ConfidentialityMode.None:
					flag = false;
					break;
				default:
					throw new CryptographicException();
				}
				Pkcs12SafeContents pkcs12SafeContents = new Pkcs12SafeContents();
				foreach (Pkcs12SafeBag bag in item.GetBags())
				{
					if (bag is Pkcs12ShroudedKeyBag pkcs12ShroudedKeyBag)
					{
						Pkcs12ShroudedKeyBag pkcs12ShroudedKeyBag2 = new Pkcs12ShroudedKeyBag(Pkcs8PrivateKeyInfo.DecryptAndDecode(password2, pkcs12ShroudedKeyBag.EncryptedPkcs8PrivateKey, out bytesConsumed).Encrypt(password2, newPbeParameters), skipCopy: true);
						pkcs12ShroudedKeyBag2.Attributes = pkcs12ShroudedKeyBag.Attributes;
						pkcs12SafeContents.AddSafeBag(pkcs12ShroudedKeyBag2);
					}
					else
					{
						pkcs12SafeContents.AddSafeBag(bag);
					}
				}
				if (flag)
				{
					pkcs12Builder.AddSafeContentsEncrypted(pkcs12SafeContents, password2, newPbeParameters);
				}
				else
				{
					pkcs12Builder.AddSafeContentsUnencrypted(pkcs12SafeContents);
				}
			}
			pkcs12Builder.SealWithMac(password2, newPbeParameters.HashAlgorithm, newPbeParameters.IterationCount);
			return pkcs12Builder.Encode();
		}
		finally
		{
			if (success)
			{
				password.DangerousRelease();
			}
		}
	}

	private unsafe byte[] SaveToMemoryStore(global::Interop.Crypt32.CertStoreSaveAs dwSaveAs)
	{
		global::Interop.Crypt32.DATA_BLOB pvSaveToPara = new global::Interop.Crypt32.DATA_BLOB(IntPtr.Zero, 0u);
		if (!global::Interop.Crypt32.CertSaveStore(_certStore, global::Interop.Crypt32.CertEncodingType.All, dwSaveAs, global::Interop.Crypt32.CertStoreSaveTo.CERT_STORE_SAVE_TO_MEMORY, ref pvSaveToPara, 0))
		{
			throw Marshal.GetLastPInvokeError().ToCryptographicException();
		}
		byte[] array = new byte[pvSaveToPara.cbData];
		fixed (byte* value = array)
		{
			pvSaveToPara.pbData = new IntPtr(value);
			if (!global::Interop.Crypt32.CertSaveStore(_certStore, global::Interop.Crypt32.CertEncodingType.All, dwSaveAs, global::Interop.Crypt32.CertStoreSaveTo.CERT_STORE_SAVE_TO_MEMORY, ref pvSaveToPara, 0))
			{
				throw Marshal.GetLastPInvokeError().ToCryptographicException();
			}
		}
		if (array.Length != pvSaveToPara.cbData)
		{
			return array[0..(int)pvSaveToPara.cbData];
		}
		return array;
	}

	private unsafe static ILoaderPal FromBlobOrFile(ReadOnlySpan<byte> rawData, string fileName, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags)
	{
		bool flag = fileName != null;
		fixed (byte* value = rawData)
		{
			fixed (char* ptr = fileName)
			{
				global::Interop.Crypt32.DATA_BLOB dATA_BLOB = new global::Interop.Crypt32.DATA_BLOB(new IntPtr(value), (!flag) ? ((uint)rawData.Length) : 0u);
				void* pvObject = (flag ? ((void*)ptr) : ((void*)(&dATA_BLOB)));
				if (!global::Interop.Crypt32.CryptQueryObject(flag ? global::Interop.Crypt32.CertQueryObjectType.CERT_QUERY_OBJECT_FILE : global::Interop.Crypt32.CertQueryObjectType.CERT_QUERY_OBJECT_BLOB, pvObject, global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_CERT | global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_SERIALIZED_STORE | global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_SERIALIZED_CERT | global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED | global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_PKCS7_UNSIGNED | global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED_EMBED | global::Interop.Crypt32.ExpectedContentTypeFlags.CERT_QUERY_CONTENT_FLAG_PFX, global::Interop.Crypt32.ExpectedFormatTypeFlags.CERT_QUERY_FORMAT_FLAG_ALL, 0, IntPtr.Zero, out var pdwContentType, IntPtr.Zero, out var phCertStore, IntPtr.Zero, IntPtr.Zero))
				{
					CryptographicException ex = Marshal.GetLastPInvokeError().ToCryptographicException();
					phCertStore.Dispose();
					throw ex;
				}
				if (pdwContentType == global::Interop.Crypt32.ContentType.CERT_QUERY_CONTENT_PFX)
				{
					phCertStore.Dispose();
					X509Certificate2Collection coll;
					try
					{
						Pkcs12LoaderLimits pkcs12Limits = X509Certificate.GetPkcs12Limits(flag, password);
						coll = ((!flag) ? X509CertificateLoader.LoadPkcs12Collection(rawData, password.DangerousGetSpan(), keyStorageFlags, pkcs12Limits) : X509CertificateLoader.LoadPkcs12CollectionFromFile(fileName, password.DangerousGetSpan(), keyStorageFlags, pkcs12Limits));
					}
					catch (Pkcs12LoadLimitExceededException inner)
					{
						throw new CryptographicException(System.SR.Cryptography_X509_PfxWithoutPassword_MaxAllowedIterationsExceeded, inner);
					}
					return new CollectionBasedLoader(coll);
				}
				return new StorePal(phCertStore);
			}
		}
	}

	private static global::Interop.Crypt32.CertStoreFlags MapX509StoreFlags(StoreLocation storeLocation, OpenFlags flags)
	{
		global::Interop.Crypt32.CertStoreFlags certStoreFlags = global::Interop.Crypt32.CertStoreFlags.None;
		switch ((uint)(flags & (OpenFlags.ReadWrite | OpenFlags.MaxAllowed)))
		{
		case 0u:
			certStoreFlags |= global::Interop.Crypt32.CertStoreFlags.CERT_STORE_READONLY_FLAG;
			break;
		case 2u:
			certStoreFlags |= global::Interop.Crypt32.CertStoreFlags.CERT_STORE_MAXIMUM_ALLOWED_FLAG;
			break;
		}
		if ((flags & OpenFlags.OpenExistingOnly) == OpenFlags.OpenExistingOnly)
		{
			certStoreFlags |= global::Interop.Crypt32.CertStoreFlags.CERT_STORE_OPEN_EXISTING_FLAG;
		}
		if ((flags & OpenFlags.IncludeArchived) == OpenFlags.IncludeArchived)
		{
			certStoreFlags |= global::Interop.Crypt32.CertStoreFlags.CERT_STORE_ENUM_ARCHIVED_FLAG;
		}
		switch (storeLocation)
		{
		case StoreLocation.LocalMachine:
			certStoreFlags |= global::Interop.Crypt32.CertStoreFlags.CERT_SYSTEM_STORE_LOCAL_MACHINE;
			break;
		case StoreLocation.CurrentUser:
			certStoreFlags |= global::Interop.Crypt32.CertStoreFlags.CERT_SYSTEM_STORE_CURRENT_USER;
			break;
		}
		return certStoreFlags;
	}
}
