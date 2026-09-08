using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace Internal.NativeCrypto;

internal static class Cng
{
	[Flags]
	public enum OpenAlgorithmProviderFlags
	{
		NONE = 0,
		BCRYPT_ALG_HANDLE_HMAC_FLAG = 8
	}

	internal static class Interop
	{
		[LibraryImport("BCrypt.dll", StringMarshalling = StringMarshalling.Utf16)]
		[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
		public unsafe static global::Interop.BCrypt.NTSTATUS BCryptOpenAlgorithmProvider(out SafeAlgorithmHandle phAlgorithm, string pszAlgId, string pszImplementation, int dwFlags)
		{
			bool flag = false;
			phAlgorithm = null;
			nint value = 0;
			global::Interop.BCrypt.NTSTATUS result = global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS;
			SafeHandleMarshaller<SafeAlgorithmHandle>.ManagedToUnmanagedOut managedToUnmanagedOut = new SafeHandleMarshaller<SafeAlgorithmHandle>.ManagedToUnmanagedOut();
			try
			{
				fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(pszImplementation))
				{
					void* _pszImplementation_native = ptr;
					fixed (char* ptr2 = &Utf16StringMarshaller.GetPinnableReference(pszAlgId))
					{
						void* _pszAlgId_native = ptr2;
						result = __PInvoke(&value, (ushort*)_pszAlgId_native, (ushort*)_pszImplementation_native, dwFlags);
					}
				}
				flag = true;
				managedToUnmanagedOut.FromUnmanaged(value);
				phAlgorithm = managedToUnmanagedOut.ToManaged();
				return result;
			}
			finally
			{
				if (flag)
				{
					managedToUnmanagedOut.Free();
				}
			}
			[DllImport("BCrypt.dll", EntryPoint = "BCryptOpenAlgorithmProvider", ExactSpelling = true)]
			unsafe static extern global::Interop.BCrypt.NTSTATUS __PInvoke(nint* __phAlgorithm_native, ushort* __pszAlgId_native, ushort* __pszImplementation_native, int __dwFlags_native);
		}

		[LibraryImport("BCrypt.dll", StringMarshalling = StringMarshalling.Utf16)]
		[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
		public unsafe static global::Interop.BCrypt.NTSTATUS BCryptSetProperty(SafeAlgorithmHandle hObject, string pszProperty, string pbInput, int cbInput, int dwFlags)
		{
			global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS;
			SafeHandleMarshaller<SafeAlgorithmHandle>.ManagedToUnmanagedIn managedToUnmanagedIn = default(SafeHandleMarshaller<SafeAlgorithmHandle>.ManagedToUnmanagedIn);
			try
			{
				managedToUnmanagedIn.FromManaged(hObject);
				fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(pbInput))
				{
					void* _pbInput_native = ptr;
					fixed (char* ptr2 = &Utf16StringMarshaller.GetPinnableReference(pszProperty))
					{
						void* _pszProperty_native = ptr2;
						return __PInvoke(managedToUnmanagedIn.ToUnmanaged(), (ushort*)_pszProperty_native, (ushort*)_pbInput_native, cbInput, dwFlags);
					}
				}
			}
			finally
			{
				managedToUnmanagedIn.Free();
			}
			[DllImport("BCrypt.dll", EntryPoint = "BCryptSetProperty", ExactSpelling = true)]
			unsafe static extern global::Interop.BCrypt.NTSTATUS __PInvoke(nint __hObject_native, ushort* __pszProperty_native, ushort* __pbInput_native, int __cbInput_native, int __dwFlags_native);
		}

		[LibraryImport("BCrypt.dll", EntryPoint = "BCryptSetProperty", StringMarshalling = StringMarshalling.Utf16)]
		[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
		private unsafe static global::Interop.BCrypt.NTSTATUS BCryptSetIntPropertyPrivate(SafeBCryptHandle hObject, string pszProperty, ref int pdwInput, int cbInput, int dwFlags)
		{
			global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS;
			SafeHandleMarshaller<SafeBCryptHandle>.ManagedToUnmanagedIn managedToUnmanagedIn = default(SafeHandleMarshaller<SafeBCryptHandle>.ManagedToUnmanagedIn);
			try
			{
				managedToUnmanagedIn.FromManaged(hObject);
				fixed (int* _pdwInput_native = &pdwInput)
				{
					fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(pszProperty))
					{
						void* _pszProperty_native = ptr;
						return __PInvoke(managedToUnmanagedIn.ToUnmanaged(), (ushort*)_pszProperty_native, _pdwInput_native, cbInput, dwFlags);
					}
				}
			}
			finally
			{
				managedToUnmanagedIn.Free();
			}
			[DllImport("BCrypt.dll", EntryPoint = "BCryptSetProperty", ExactSpelling = true)]
			unsafe static extern global::Interop.BCrypt.NTSTATUS __PInvoke(nint __hObject_native, ushort* __pszProperty_native, int* __pdwInput_native, int __cbInput_native, int __dwFlags_native);
		}

		[LibraryImport("BCrypt.dll", StringMarshalling = StringMarshalling.Utf16)]
		[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
		public unsafe static global::Interop.BCrypt.NTSTATUS BCryptSetProperty(SafeBCryptHandle hObject, string pszProperty, ReadOnlySpan<byte> pbInput, int cbInput, int dwFlags)
		{
			global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS;
			SafeHandleMarshaller<SafeBCryptHandle>.ManagedToUnmanagedIn managedToUnmanagedIn = default(SafeHandleMarshaller<SafeBCryptHandle>.ManagedToUnmanagedIn);
			try
			{
				managedToUnmanagedIn.FromManaged(hObject);
				fixed (byte* ptr = &ReadOnlySpanMarshaller<byte, byte>.ManagedToUnmanagedIn.GetPinnableReference(pbInput))
				{
					void* _pbInput_native = ptr;
					fixed (char* ptr2 = &Utf16StringMarshaller.GetPinnableReference(pszProperty))
					{
						void* _pszProperty_native = ptr2;
						return __PInvoke(managedToUnmanagedIn.ToUnmanaged(), (ushort*)_pszProperty_native, (byte*)_pbInput_native, cbInput, dwFlags);
					}
				}
			}
			finally
			{
				managedToUnmanagedIn.Free();
			}
			[DllImport("BCrypt.dll", EntryPoint = "BCryptSetProperty", ExactSpelling = true)]
			unsafe static extern global::Interop.BCrypt.NTSTATUS __PInvoke(nint __hObject_native, ushort* __pszProperty_native, byte* __pbInput_native, int __cbInput_native, int __dwFlags_native);
		}

		public static global::Interop.BCrypt.NTSTATUS BCryptSetIntProperty(SafeBCryptHandle hObject, string pszProperty, ref int pdwInput, int dwFlags)
		{
			return BCryptSetIntPropertyPrivate(hObject, pszProperty, ref pdwInput, 4, dwFlags);
		}
	}

	public static SafeAlgorithmHandle BCryptOpenAlgorithmProvider(string pszAlgId, string pszImplementation = null, OpenAlgorithmProviderFlags dwFlags = OpenAlgorithmProviderFlags.NONE)
	{
		global::Interop.BCrypt.NTSTATUS nTSTATUS = Interop.BCryptOpenAlgorithmProvider(out var phAlgorithm, pszAlgId, pszImplementation, (int)dwFlags);
		if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			throw CreateCryptographicException(nTSTATUS);
		}
		return phAlgorithm;
	}

	public static void SetFeedbackSize(this SafeAlgorithmHandle hAlg, int dwFeedbackSize)
	{
		global::Interop.BCrypt.NTSTATUS nTSTATUS = Interop.BCryptSetIntProperty(hAlg, "MessageBlockLength", ref dwFeedbackSize, 0);
		if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			throw CreateCryptographicException(nTSTATUS);
		}
	}

	public static void SetCipherMode(this SafeAlgorithmHandle hAlg, string cipherMode)
	{
		global::Interop.BCrypt.NTSTATUS nTSTATUS = Interop.BCryptSetProperty(hAlg, "ChainingMode", cipherMode, (cipherMode.Length + 1) * 2, 0);
		if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			throw CreateCryptographicException(nTSTATUS);
		}
	}

	public static void SetEffectiveKeyLength(this SafeAlgorithmHandle hAlg, int effectiveKeyLength)
	{
		global::Interop.BCrypt.NTSTATUS nTSTATUS = Interop.BCryptSetIntProperty(hAlg, "EffectiveKeyLength", ref effectiveKeyLength, 0);
		if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			throw CreateCryptographicException(nTSTATUS);
		}
	}

	private static CryptographicException CreateCryptographicException(global::Interop.BCrypt.NTSTATUS ntStatus)
	{
		return ((int)(ntStatus | (global::Interop.BCrypt.NTSTATUS)0x1000000u)).ToCryptographicException();
	}
}
