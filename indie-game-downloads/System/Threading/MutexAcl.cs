using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace System.Threading;

public static class MutexAcl
{
	public unsafe static Mutex Create(bool initiallyOwned, string? name, out bool createdNew, MutexSecurity? mutexSecurity)
	{
		if (mutexSecurity == null)
		{
			return new Mutex(initiallyOwned, name, out createdNew);
		}
		uint flags = (initiallyOwned ? 1u : 0u);
		fixed (byte* securityDescriptorBinaryForm = mutexSecurity.GetSecurityDescriptorBinaryForm())
		{
			global::Interop.Kernel32.SECURITY_ATTRIBUTES sECURITY_ATTRIBUTES = new global::Interop.Kernel32.SECURITY_ATTRIBUTES
			{
				nLength = (uint)sizeof(global::Interop.Kernel32.SECURITY_ATTRIBUTES),
				lpSecurityDescriptor = securityDescriptorBinaryForm
			};
			SafeWaitHandle safeWaitHandle = global::Interop.Kernel32.CreateMutexEx((nint)(&sECURITY_ATTRIBUTES), name, flags, 2031617u);
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			if (safeWaitHandle.IsInvalid)
			{
				safeWaitHandle.SetHandleAsInvalid();
				switch (lastPInvokeError)
				{
				case 206:
					throw new ArgumentException(System.SR.Argument_WaitHandleNameTooLong, "name");
				case 6:
					throw new WaitHandleCannotBeOpenedException(System.SR.Format(System.SR.Threading_WaitHandleCannotBeOpenedException_InvalidHandle, name));
				default:
					throw System.IO.Win32Marshal.GetExceptionForWin32Error(lastPInvokeError, name);
				}
			}
			createdNew = lastPInvokeError != 183;
			return CreateAndReplaceHandle(safeWaitHandle);
		}
	}

	public static Mutex OpenExisting(string name, MutexRights rights)
	{
		Mutex result;
		return OpenExistingWorker(name, rights, out result) switch
		{
			System.Threading.OpenExistingResult.NameNotFound => throw new WaitHandleCannotBeOpenedException(), 
			System.Threading.OpenExistingResult.NameInvalid => throw new WaitHandleCannotBeOpenedException(System.SR.Format(System.SR.Threading_WaitHandleCannotBeOpenedException_InvalidHandle, name)), 
			System.Threading.OpenExistingResult.PathNotFound => throw new DirectoryNotFoundException(System.SR.Format(System.SR.IO_PathNotFound_Path, name)), 
			_ => result, 
		};
	}

	public static bool TryOpenExisting(string name, MutexRights rights, [NotNullWhen(true)] out Mutex? result)
	{
		return OpenExistingWorker(name, rights, out result) == System.Threading.OpenExistingResult.Success;
	}

	private static System.Threading.OpenExistingResult OpenExistingWorker(string name, MutexRights rights, out Mutex result)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		if (name.Length == 0)
		{
			throw new ArgumentException(System.SR.Argument_EmptyName, "name");
		}
		result = null;
		SafeWaitHandle safeWaitHandle = global::Interop.Kernel32.OpenMutex((uint)rights, inheritHandle: false, name);
		int lastPInvokeError = Marshal.GetLastPInvokeError();
		if (safeWaitHandle.IsInvalid)
		{
			safeWaitHandle.Dispose();
			switch (lastPInvokeError)
			{
			case 2:
			case 123:
				return System.Threading.OpenExistingResult.NameNotFound;
			case 3:
				return System.Threading.OpenExistingResult.PathNotFound;
			case 6:
				return System.Threading.OpenExistingResult.NameInvalid;
			default:
				throw System.IO.Win32Marshal.GetExceptionForWin32Error(lastPInvokeError, name);
			}
		}
		result = CreateAndReplaceHandle(safeWaitHandle);
		return System.Threading.OpenExistingResult.Success;
	}

	private static Mutex CreateAndReplaceHandle(SafeWaitHandle replacementHandle)
	{
		Mutex mutex = new Mutex(initiallyOwned: false);
		SafeWaitHandle safeWaitHandle = mutex.SafeWaitHandle;
		mutex.SafeWaitHandle = replacementHandle;
		safeWaitHandle.Dispose();
		return mutex;
	}
}
