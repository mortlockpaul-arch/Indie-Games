using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography;

internal static class Pbkdf2Implementation
{
	private static readonly bool s_useKeyDerivation = OperatingSystem.IsWindowsVersionAtLeast(6, 2);

	private static SafeBCryptAlgorithmHandle s_pbkdf2AlgorithmHandle;

	public static void Fill(ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, int iterations, HashAlgorithmName hashAlgorithmName, Span<byte> destination)
	{
		if (s_useKeyDerivation)
		{
			FillKeyDerivation(password, salt, iterations, hashAlgorithmName.Name, destination);
		}
		else
		{
			FillDeriveKeyPBKDF2(password, salt, iterations, hashAlgorithmName.Name, destination);
		}
	}

	private unsafe static void FillKeyDerivation(ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, int iterations, string hashAlgorithmName, Span<byte> destination)
	{
		int hashBlockSize = GetHashBlockSize(hashAlgorithmName);
		ReadOnlySpan<byte> readOnlySpan;
		int cbSecret;
		Span<byte> span;
		if (password.IsEmpty)
		{
			readOnlySpan = new byte[1] { 0 };
			cbSecret = 0;
			span = default(Span<byte>);
		}
		else if (password.Length <= hashBlockSize)
		{
			readOnlySpan = password;
			cbSecret = password.Length;
			span = default(Span<byte>);
		}
		else
		{
			Span<byte> destination2 = stackalloc byte[64];
			int num;
			switch (hashAlgorithmName)
			{
			case "SHA256":
			case "SHA384":
			case "SHA512":
			case "SHA1":
				num = HashProviderDispenser.OneShotHashProvider.HashData(hashAlgorithmName, password, destination2);
				break;
			case "SHA3-256":
			case "SHA3-384":
			case "SHA3-512":
				if (!HashProviderDispenser.HashSupported(hashAlgorithmName))
				{
					throw new PlatformNotSupportedException();
				}
				num = HashProviderDispenser.OneShotHashProvider.HashData(hashAlgorithmName, password, destination2);
				break;
			default:
				throw new CryptographicException();
			}
			span = destination2.Slice(0, num);
			readOnlySpan = span;
			cbSecret = num;
		}
		global::Interop.BCrypt.NTSTATUS nTSTATUS;
		SafeBCryptKeyHandle phKey;
		if (global::Interop.BCrypt.PseudoHandlesSupported)
		{
			fixed (byte* pbSecret = readOnlySpan)
			{
				nTSTATUS = global::Interop.BCrypt.BCryptGenerateSymmetricKey(817u, out phKey, IntPtr.Zero, 0, pbSecret, cbSecret, 0u);
			}
		}
		else
		{
			if (s_pbkdf2AlgorithmHandle == null)
			{
				global::Interop.BCrypt.NTSTATUS nTSTATUS2 = global::Interop.BCrypt.BCryptOpenAlgorithmProvider(out var phAlgorithm, "PBKDF2", null, global::Interop.BCrypt.BCryptOpenAlgorithmProviderFlags.None);
				if (nTSTATUS2 != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
				{
					phAlgorithm.Dispose();
					CryptographicOperations.ZeroMemory(span);
					throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS2);
				}
				Interlocked.CompareExchange(ref s_pbkdf2AlgorithmHandle, phAlgorithm, null);
			}
			fixed (byte* pbSecret2 = readOnlySpan)
			{
				nTSTATUS = global::Interop.BCrypt.BCryptGenerateSymmetricKey(s_pbkdf2AlgorithmHandle, out phKey, IntPtr.Zero, 0, pbSecret2, cbSecret, 0u);
			}
		}
		CryptographicOperations.ZeroMemory(span);
		if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			phKey.Dispose();
			throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS);
		}
		ulong num2 = (ulong)iterations;
		using (phKey)
		{
			fixed (char* pvBuffer = hashAlgorithmName)
			{
				fixed (byte* pvBuffer2 = salt)
				{
					fixed (byte* pbDerivedKey = destination)
					{
						Span<global::Interop.BCrypt.BCryptBuffer> span2 = stackalloc global::Interop.BCrypt.BCryptBuffer[3];
						span2[0].BufferType = global::Interop.BCrypt.CngBufferDescriptors.KDF_ITERATION_COUNT;
						span2[0].pvBuffer = (nint)(&num2);
						span2[0].cbBuffer = 8;
						span2[1].BufferType = global::Interop.BCrypt.CngBufferDescriptors.KDF_SALT;
						span2[1].pvBuffer = (nint)pvBuffer2;
						span2[1].cbBuffer = salt.Length;
						span2[2].BufferType = global::Interop.BCrypt.CngBufferDescriptors.KDF_HASH_ALGORITHM;
						span2[2].pvBuffer = (nint)pvBuffer;
						span2[2].cbBuffer = checked((hashAlgorithmName.Length + 1) * 2);
						fixed (global::Interop.BCrypt.BCryptBuffer* pBuffers = span2)
						{
							Unsafe.SkipInit(out global::Interop.BCrypt.BCryptBufferDesc bCryptBufferDesc);
							bCryptBufferDesc.ulVersion = 0;
							bCryptBufferDesc.cBuffers = span2.Length;
							bCryptBufferDesc.pBuffers = (nint)pBuffers;
							global::Interop.BCrypt.NTSTATUS nTSTATUS3 = global::Interop.BCrypt.BCryptKeyDerivation(phKey, &bCryptBufferDesc, pbDerivedKey, destination.Length, out var pcbResult, 0);
							if (nTSTATUS3 != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
							{
								throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS3);
							}
							if (destination.Length != pcbResult)
							{
								throw new CryptographicException();
							}
						}
					}
				}
			}
		}
	}

	private unsafe static void FillDeriveKeyPBKDF2(ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, int iterations, string hashAlgorithmName, Span<byte> destination)
	{
		SafeBCryptAlgorithmHandle cachedBCryptAlgorithmHandle = global::Interop.BCrypt.BCryptAlgorithmCache.GetCachedBCryptAlgorithmHandle(hashAlgorithmName, global::Interop.BCrypt.BCryptOpenAlgorithmProviderFlags.BCRYPT_ALG_HANDLE_HMAC_FLAG, out var _);
		fixed (byte* pbPassword = password)
		{
			fixed (byte* pbSalt = salt)
			{
				fixed (byte* pbDerivedKey = destination)
				{
					global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.BCryptDeriveKeyPBKDF2(cachedBCryptAlgorithmHandle, pbPassword, password.Length, pbSalt, salt.Length, (ulong)iterations, pbDerivedKey, destination.Length, 0u);
					if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
					{
						throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS);
					}
				}
			}
		}
	}

	private static int GetHashBlockSize(string hashAlgorithmName)
	{
		switch (hashAlgorithmName)
		{
		case "SHA256":
		case "SHA1":
			return 64;
		case "SHA384":
		case "SHA512":
			return 128;
		case "SHA3-256":
			return 136;
		case "SHA3-384":
			return 104;
		case "SHA3-512":
			return 72;
		default:
			throw new CryptographicException();
		}
	}
}
