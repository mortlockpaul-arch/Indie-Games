using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace System.Threading;

public static class SemaphoreAcl
{
	public unsafe static Semaphore Create(int initialCount, int maximumCount, string? name, out bool createdNew, SemaphoreSecurity? semaphoreSecurity)
	{
		if (semaphoreSecurity == null)
		{
			return new Semaphore(initialCount, maximumCount, name, out createdNew);
		}
		if (initialCount < 0)
		{
			throw new ArgumentOutOfRangeException("initialCount", System.SR.ArgumentOutOfRange_NeedNonNegNum);
		}
		if (maximumCount < 1)
		{
			throw new ArgumentOutOfRangeException("maximumCount", System.SR.ArgumentOutOfRange_NeedPosNum);
		}
		if (initialCount > maximumCount)
		{
			throw new ArgumentException(System.SR.Argument_SemaphoreInitialMaximum);
		}
		fixed (byte* securityDescriptorBinaryForm = semaphoreSecurity.GetSecurityDescriptorBinaryForm())
		{
			global::Interop.Kernel32.SECURITY_ATTRIBUTES sECURITY_ATTRIBUTES = new global::Interop.Kernel32.SECURITY_ATTRIBUTES
			{
				nLength = (uint)sizeof(global::Interop.Kernel32.SECURITY_ATTRIBUTES),
				lpSecurityDescriptor = securityDescriptorBinaryForm
			};
			SafeWaitHandle safeWaitHandle = global::Interop.Kernel32.CreateSemaphoreEx((nint)(&sECURITY_ATTRIBUTES), initialCount, maximumCount, name, 0u, 2031619u);
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			if (safeWaitHandle.IsInvalid)
			{
				safeWaitHandle.Dispose();
				if (!string.IsNullOrEmpty(name) && lastPInvokeError == 6)
				{
					throw new WaitHandleCannotBeOpenedException(System.SR.Format(System.SR.Threading_WaitHandleCannotBeOpenedException_InvalidHandle, name));
				}
				throw System.IO.Win32Marshal.GetExceptionForLastWin32Error();
			}
			createdNew = lastPInvokeError != 183;
			return CreateAndReplaceHandle(safeWaitHandle);
		}
	}

	public static Semaphore OpenExisting(string name, SemaphoreRights rights)
	{
		Semaphore result;
		return OpenExistingWorker(name, rights, out result) switch
		{
			System.Threading.OpenExistingResult.NameNotFound => throw new WaitHandleCannotBeOpenedException(), 
			System.Threading.OpenExistingResult.NameInvalid => throw new WaitHandleCannotBeOpenedException(System.SR.Format(System.SR.Threading_WaitHandleCannotBeOpenedException_InvalidHandle, name)), 
			System.Threading.OpenExistingResult.PathNotFound => throw new IOException(System.SR.Format(System.SR.IO_PathNotFound_Path, name)), 
			_ => result, 
		};
	}

	public static bool TryOpenExisting(string name, SemaphoreRights rights, [NotNullWhen(true)] out Semaphore? result)
	{
		return OpenExistingWorker(name, rights, out result) == System.Threading.OpenExistingResult.Success;
	}

	private static System.Threading.OpenExistingResult OpenExistingWorker(string name, SemaphoreRights rights, out Semaphore result)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		if (name.Length == 0)
		{
			throw new ArgumentException(System.SR.Argument_EmptyName, "name");
		}
		result = null;
		SafeWaitHandle safeWaitHandle = global::Interop.Kernel32.OpenSemaphore((uint)rights, inheritHandle: false, name);
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
				throw System.IO.Win32Marshal.GetExceptionForLastWin32Error();
			}
		}
		result = CreateAndReplaceHandle(safeWaitHandle);
		return System.Threading.OpenExistingResult.Success;
	}

	private static Semaphore CreateAndReplaceHandle(SafeWaitHandle replacementHandle)
	{
		Semaphore semaphore = new Semaphore(1, 2);
		SafeWaitHandle safeWaitHandle = semaphore.SafeWaitHandle;
		semaphore.SafeWaitHandle = replacementHandle;
		safeWaitHandle.Dispose();
		return semaphore;
	}
}
