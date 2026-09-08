using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates;

internal static class CertificateHelpers
{
	private static CryptographicException GetExceptionForLastError()
	{
		return Marshal.GetLastPInvokeError().ToCryptographicException();
	}

	private static SafeNCryptKeyHandle CreateSafeNCryptKeyHandle(nint handle, SafeHandle parentHandle)
	{
		return new SafeNCryptKeyHandle(handle, parentHandle);
	}

	private static CertificatePal CopyFromRawBytes(CertificatePal certificate)
	{
		return (CertificatePal)CertificatePal.FromBlob(certificate.RawData, SafePasswordHandle.InvalidHandle, X509KeyStorageFlags.PersistKeySet);
	}

	private static int GuessKeySpec(CngProvider provider, string keyName, bool machineKey, CngAlgorithmGroup algorithmGroup)
	{
		if (provider == CngProvider.MicrosoftSoftwareKeyStorageProvider || provider == CngProvider.MicrosoftSmartCardKeyStorageProvider)
		{
			return 0;
		}
		try
		{
			CngKeyOpenOptions openOptions = (machineKey ? CngKeyOpenOptions.MachineKey : CngKeyOpenOptions.None);
			using (CngKey.Open(keyName, provider, openOptions))
			{
				return 0;
			}
		}
		catch (CryptographicException)
		{
			CspParameters cspParameters = new CspParameters
			{
				ProviderName = provider.Provider,
				KeyContainerName = keyName,
				Flags = CspProviderFlags.UseExistingKey,
				KeyNumber = 2
			};
			if (machineKey)
			{
				cspParameters.Flags |= CspProviderFlags.UseMachineKeyStore;
			}
			if (TryGuessKeySpec(cspParameters, algorithmGroup, out var keySpec))
			{
				return keySpec;
			}
			throw;
		}
	}

	private static SafeCertContextHandle DuplicateCertificateHandle(CertificatePal certificate)
	{
		SafeCertContextHandle safeHandle = certificate.SafeHandle;
		bool success = false;
		try
		{
			if (safeHandle != null)
			{
				safeHandle.DangerousAddRef(ref success);
				return global::Interop.Crypt32.CertDuplicateCertificateContext(safeHandle.DangerousGetHandle());
			}
		}
		catch (ObjectDisposedException)
		{
		}
		finally
		{
			if (success)
			{
				safeHandle.DangerousRelease();
			}
		}
		throw new CryptographicException(System.SR.Format(System.SR.Cryptography_InvalidHandle, "handle"));
	}

	internal static CertificatePal CopyWithPrivateKey(CertificatePal certificate, MLDsa privateKey)
	{
		if (privateKey is MLDsaCng mLDsaCng)
		{
			CngKey keyNoDuplicate = mLDsaCng.KeyNoDuplicate;
			CertificatePal certificatePal = CopyWithPersistedCngKey(certificate, keyNoDuplicate);
			if (certificatePal != null)
			{
				return certificatePal;
			}
		}
		if (privateKey is MLDsaImplementation mLDsaImplementation)
		{
			using CngKey cngKey = mLDsaImplementation.CreateEphemeralCng();
			return CopyWithEphemeralKey(certificate, cngKey);
		}
		byte[] array = privateKey.ExportPkcs8PrivateKey();
		using (PinAndClear.Track(array))
		{
			int bytesRead;
			using MLDsaCng mLDsaCng2 = MLDsaCng.ImportPkcs8PrivateKey(array, out bytesRead);
			CngKey keyNoDuplicate2 = mLDsaCng2.KeyNoDuplicate;
			if (keyNoDuplicate2.AlgorithmGroup != CngAlgorithmGroup.MLDsa)
			{
				throw new CryptographicException();
			}
			return CopyWithEphemeralKey(certificate, keyNoDuplicate2);
		}
	}

	[SupportedOSPlatform("windows")]
	internal static T GetPrivateKey<T>(CertificatePal certificate, Func<CspParameters, T> createCsp, Func<CngKey, T> createCng) where T : class, IDisposable
	{
		using SafeCertContextHandle safeCertContextHandle = DuplicateCertificateHandle(certificate);
		SafeNCryptKeyHandle safeNCryptKeyHandle = TryAcquireCngPrivateKey(safeCertContextHandle, out var handleOptions);
		if (safeNCryptKeyHandle != null)
		{
			CngKey cngKey = CngKey.OpenNoDuplicate(safeNCryptKeyHandle, handleOptions);
			T val = createCng(cngKey);
			if (val == null)
			{
				cngKey.Dispose();
			}
			return val;
		}
		CspParameters privateKeyCsp = GetPrivateKeyCsp(safeCertContextHandle);
		if (privateKeyCsp == null)
		{
			return null;
		}
		if (privateKeyCsp.ProviderType == 0)
		{
			string providerName = privateKeyCsp.ProviderName;
			CngKey arg = CngKey.Open(privateKeyCsp.KeyContainerName, new CngProvider(providerName));
			return createCng(arg);
		}
		privateKeyCsp.Flags |= CspProviderFlags.UseExistingKey;
		return createCsp(privateKeyCsp);
	}

	private static SafeNCryptKeyHandle TryAcquireCngPrivateKey(SafeCertContextHandle certificateContext, out CngKeyHandleOpenOptions handleOptions)
	{
		if (!certificateContext.HasPersistedPrivateKey)
		{
			int pcbData = IntPtr.Size;
			if (global::Interop.Crypt32.CertGetCertificateContextProperty(certificateContext, global::Interop.Crypt32.CertContextPropId.CERT_NCRYPT_KEY_HANDLE_PROP_ID, out nint pvData, ref pcbData))
			{
				handleOptions = CngKeyHandleOpenOptions.EphemeralKey;
				return CreateSafeNCryptKeyHandle(pvData, certificateContext);
			}
		}
		bool pfCallerFreeProvOrNCryptKey = true;
		SafeNCryptKeyHandle phCryptProvOrNCryptKey = null;
		handleOptions = CngKeyHandleOpenOptions.None;
		try
		{
			if (!global::Interop.Crypt32.CryptAcquireCertificatePrivateKey(certificateContext, global::Interop.Crypt32.CryptAcquireCertificatePrivateKeyFlags.CRYPT_ACQUIRE_ONLY_NCRYPT_KEY_FLAG, IntPtr.Zero, out phCryptProvOrNCryptKey, out var _, out pfCallerFreeProvOrNCryptKey))
			{
				pfCallerFreeProvOrNCryptKey = false;
				phCryptProvOrNCryptKey?.SetHandleAsInvalid();
				return null;
			}
			if (!pfCallerFreeProvOrNCryptKey && phCryptProvOrNCryptKey != null && !phCryptProvOrNCryptKey.IsInvalid)
			{
				SafeNCryptKeyHandle safeNCryptKeyHandle = CreateSafeNCryptKeyHandle(phCryptProvOrNCryptKey.DangerousGetHandle(), certificateContext);
				phCryptProvOrNCryptKey.SetHandleAsInvalid();
				phCryptProvOrNCryptKey = safeNCryptKeyHandle;
				pfCallerFreeProvOrNCryptKey = true;
			}
			return phCryptProvOrNCryptKey;
		}
		catch
		{
			if (phCryptProvOrNCryptKey != null && !pfCallerFreeProvOrNCryptKey)
			{
				phCryptProvOrNCryptKey.SetHandleAsInvalid();
			}
			throw;
		}
	}

	internal unsafe static CspParameters GetPrivateKeyCsp(SafeCertContextHandle hCertContext)
	{
		int pcbData = 0;
		if (!global::Interop.Crypt32.CertGetCertificateContextProperty(hCertContext, global::Interop.Crypt32.CertContextPropId.CERT_KEY_PROV_INFO_PROP_ID, null, ref pcbData))
		{
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			if (lastPInvokeError == -2146885628)
			{
				return null;
			}
			throw lastPInvokeError.ToCryptographicException();
		}
		byte[] array = new byte[pcbData];
		fixed (byte* ptr = array)
		{
			if (!global::Interop.Crypt32.CertGetCertificateContextProperty(hCertContext, global::Interop.Crypt32.CertContextPropId.CERT_KEY_PROV_INFO_PROP_ID, array, ref pcbData))
			{
				throw GetExceptionForLastError();
			}
			global::Interop.Crypt32.CRYPT_KEY_PROV_INFO* ptr2 = (global::Interop.Crypt32.CRYPT_KEY_PROV_INFO*)ptr;
			return new CspParameters
			{
				ProviderName = Marshal.PtrToStringUni((nint)ptr2->pwszProvName),
				KeyContainerName = Marshal.PtrToStringUni((nint)ptr2->pwszContainerName),
				ProviderType = ptr2->dwProvType,
				KeyNumber = ptr2->dwKeySpec,
				Flags = (((ptr2->dwFlags & global::Interop.Crypt32.CryptAcquireContextFlags.CRYPT_MACHINE_KEYSET) == global::Interop.Crypt32.CryptAcquireContextFlags.CRYPT_MACHINE_KEYSET) ? CspProviderFlags.UseMachineKeyStore : CspProviderFlags.NoFlags)
			};
		}
	}

	internal unsafe static CertificatePal CopyWithPersistedCngKey(CertificatePal certificate, CngKey cngKey)
	{
		//The blocks IL_0077, IL_0090, IL_0093, IL_0095, IL_00b4 are reachable both inside and outside the pinned region starting at IL_0072. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if (string.IsNullOrEmpty(cngKey.KeyName))
		{
			return null;
		}
		CertificatePal certificatePal = CopyFromRawBytes(certificate);
		CngProvider? provider = cngKey.Provider;
		string keyName = cngKey.KeyName;
		bool isMachineKey = cngKey.IsMachineKey;
		int dwKeySpec = GuessKeySpec(provider, keyName, isMachineKey, cngKey.AlgorithmGroup);
		global::Interop.Crypt32.CRYPT_KEY_PROV_INFO cRYPT_KEY_PROV_INFO = default(global::Interop.Crypt32.CRYPT_KEY_PROV_INFO);
		fixed (char* keyName2 = cngKey.KeyName)
		{
			string provider2 = cngKey.Provider.Provider;
			char* intPtr;
			ref global::Interop.Crypt32.CRYPT_KEY_PROV_INFO reference;
			int dwFlags;
			CryptographicException exceptionForLastError;
			if (provider2 == null)
			{
				char* pwszProvName;
				intPtr = (pwszProvName = null);
				cRYPT_KEY_PROV_INFO.pwszContainerName = keyName2;
				cRYPT_KEY_PROV_INFO.pwszProvName = pwszProvName;
				reference = ref cRYPT_KEY_PROV_INFO;
				dwFlags = (isMachineKey ? 32 : 0);
				reference.dwFlags = (global::Interop.Crypt32.CryptAcquireContextFlags)dwFlags;
				cRYPT_KEY_PROV_INFO.dwKeySpec = dwKeySpec;
				if (!global::Interop.Crypt32.CertSetCertificateContextProperty(certificatePal.Handle, global::Interop.Crypt32.CertContextPropId.CERT_KEY_PROV_INFO_PROP_ID, global::Interop.Crypt32.CertSetPropertyFlags.None, &cRYPT_KEY_PROV_INFO))
				{
					exceptionForLastError = GetExceptionForLastError();
					certificatePal.Dispose();
					throw exceptionForLastError;
				}
			}
			else
			{
				fixed (char* ptr = &provider2.GetPinnableReference())
				{
					char* pwszProvName;
					intPtr = (pwszProvName = ptr);
					cRYPT_KEY_PROV_INFO.pwszContainerName = keyName2;
					cRYPT_KEY_PROV_INFO.pwszProvName = pwszProvName;
					reference = ref cRYPT_KEY_PROV_INFO;
					dwFlags = (int)(reference.dwFlags = (isMachineKey ? global::Interop.Crypt32.CryptAcquireContextFlags.CRYPT_MACHINE_KEYSET : global::Interop.Crypt32.CryptAcquireContextFlags.None));
					cRYPT_KEY_PROV_INFO.dwKeySpec = dwKeySpec;
					if (!global::Interop.Crypt32.CertSetCertificateContextProperty(certificatePal.Handle, global::Interop.Crypt32.CertContextPropId.CERT_KEY_PROV_INFO_PROP_ID, global::Interop.Crypt32.CertSetPropertyFlags.None, &cRYPT_KEY_PROV_INFO))
					{
						exceptionForLastError = GetExceptionForLastError();
						certificatePal.Dispose();
						throw exceptionForLastError;
					}
				}
			}
		}
		return certificatePal;
	}

	internal static CertificatePal CopyWithEphemeralKey(CertificatePal certificate, CngKey cngKey)
	{
		using SafeNCryptKeyHandle safeNCryptKeyHandle = cngKey.Handle;
		CertificatePal certificatePal = CopyFromRawBytes(certificate);
		try
		{
			if (!global::Interop.Crypt32.CertSetCertificateContextProperty(certificatePal.Handle, global::Interop.Crypt32.CertContextPropId.CERT_NCRYPT_KEY_HANDLE_PROP_ID, global::Interop.Crypt32.CertSetPropertyFlags.CERT_SET_PROPERTY_INHIBIT_PERSIST_FLAG, safeNCryptKeyHandle))
			{
				throw GetExceptionForLastError();
			}
			safeNCryptKeyHandle.SetHandleAsInvalid();
			return certificatePal;
		}
		catch
		{
			certificatePal.Dispose();
			throw;
		}
	}

	private static bool TryGuessKeySpec(CspParameters cspParameters, CngAlgorithmGroup algorithmGroup, out int keySpec)
	{
		if (algorithmGroup == CngAlgorithmGroup.Rsa)
		{
			return TryGuessRsaKeySpec(cspParameters, out keySpec);
		}
		if (algorithmGroup == CngAlgorithmGroup.Dsa)
		{
			return TryGuessDsaKeySpec(cspParameters, out keySpec);
		}
		keySpec = 0;
		return false;
	}

	private static bool TryGuessRsaKeySpec(CspParameters cspParameters, out int keySpec)
	{
		ReadOnlySpan<int> readOnlySpan = new int[4] { 1, 24, 12, 2 };
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			int providerType = readOnlySpan[i];
			cspParameters.ProviderType = providerType;
			try
			{
				using (new RSACryptoServiceProvider(cspParameters))
				{
					keySpec = cspParameters.KeyNumber;
					return true;
				}
			}
			catch (CryptographicException)
			{
			}
		}
		keySpec = 0;
		return false;
	}

	private static bool TryGuessDsaKeySpec(CspParameters cspParameters, out int keySpec)
	{
		ReadOnlySpan<int> readOnlySpan = new int[2] { 13, 3 };
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			int providerType = readOnlySpan[i];
			cspParameters.ProviderType = providerType;
			try
			{
				using (new DSACryptoServiceProvider(cspParameters))
				{
					keySpec = cspParameters.KeyNumber;
					return true;
				}
			}
			catch (CryptographicException)
			{
			}
		}
		keySpec = 0;
		return false;
	}
}
