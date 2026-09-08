using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.Threading;

public sealed class Mutex : WaitHandle
{
	public Mutex(bool initiallyOwned, string? name, NamedWaitHandleOptions options, out bool createdNew)
	{
		CreateMutexCore(initiallyOwned, name, new NamedWaitHandleOptionsInternal(options), out createdNew);
	}

	public Mutex(bool initiallyOwned, string? name, out bool createdNew)
	{
		CreateMutexCore(initiallyOwned, name, default(NamedWaitHandleOptionsInternal), out createdNew);
	}

	public Mutex(bool initiallyOwned, string? name, NamedWaitHandleOptions options)
	{
		CreateMutexCore(initiallyOwned, name, new NamedWaitHandleOptionsInternal(options), out var _);
	}

	public Mutex(bool initiallyOwned, string? name)
	{
		CreateMutexCore(initiallyOwned, name, default(NamedWaitHandleOptionsInternal), out var _);
	}

	public Mutex(string? name, NamedWaitHandleOptions options)
	{
		CreateMutexCore(initiallyOwned: false, name, new NamedWaitHandleOptionsInternal(options), out var _);
	}

	public Mutex(bool initiallyOwned)
	{
		CreateMutexCore(initiallyOwned);
	}

	public Mutex()
	{
		CreateMutexCore(initiallyOwned: false);
	}

	private Mutex(SafeWaitHandle handle)
	{
		base.SafeWaitHandle = handle;
	}

	public static Mutex OpenExisting(string name, NamedWaitHandleOptions options)
	{
		OpenExistingResult openExistingResult = OpenExistingWorker(name, new NamedWaitHandleOptionsInternal(options), out var result);
		if (openExistingResult != OpenExistingResult.Success)
		{
			ThrowForOpenExistingFailure(openExistingResult, name);
		}
		return result;
	}

	public static Mutex OpenExisting(string name)
	{
		OpenExistingResult openExistingResult = OpenExistingWorker(name, default(NamedWaitHandleOptionsInternal), out var result);
		if (openExistingResult != OpenExistingResult.Success)
		{
			ThrowForOpenExistingFailure(openExistingResult, name);
		}
		return result;
	}

	[DoesNotReturn]
	private static void ThrowForOpenExistingFailure(OpenExistingResult openExistingResult, string name)
	{
		switch (openExistingResult)
		{
		case OpenExistingResult.NameNotFound:
			throw new WaitHandleCannotBeOpenedException();
		case OpenExistingResult.NameInvalid:
			throw new WaitHandleCannotBeOpenedException(SR.Format(SR.Threading_WaitHandleCannotBeOpenedException_InvalidHandle, name));
		case OpenExistingResult.PathNotFound:
			throw new DirectoryNotFoundException(SR.Format(SR.IO_PathNotFound_Path, name));
		default:
			throw new WaitHandleCannotBeOpenedException(SR.Format(SR.NamedWaitHandles_ExistingObjectIncompatibleWithCurrentUserOnly, name));
		}
	}

	public static bool TryOpenExisting(string name, NamedWaitHandleOptions options, [NotNullWhen(true)] out Mutex? result)
	{
		return OpenExistingWorker(name, new NamedWaitHandleOptionsInternal(options), out result) == OpenExistingResult.Success;
	}

	public static bool TryOpenExisting(string name, [NotNullWhen(true)] out Mutex? result)
	{
		return OpenExistingWorker(name, default(NamedWaitHandleOptionsInternal), out result) == OpenExistingResult.Success;
	}

	private void CreateMutexCore(bool initiallyOwned)
	{
		uint flags = (initiallyOwned ? 1u : 0u);
		SafeWaitHandle safeWaitHandle = Interop.Kernel32.CreateMutexEx(0, null, flags, 34603009u);
		if (safeWaitHandle.IsInvalid)
		{
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			safeWaitHandle.SetHandleAsInvalid();
			throw Win32Marshal.GetExceptionForWin32Error(lastPInvokeError);
		}
		base.SafeWaitHandle = safeWaitHandle;
	}

	private unsafe void CreateMutexCore(bool initiallyOwned, string name, NamedWaitHandleOptionsInternal options, out bool createdNew)
	{
		Thread.CurrentUserSecurityDescriptorInfo currentUserSecurityDescriptorInfo = default(Thread.CurrentUserSecurityDescriptorInfo);
		Interop.Kernel32.SECURITY_ATTRIBUTES sECURITY_ATTRIBUTES = default(Interop.Kernel32.SECURITY_ATTRIBUTES);
		Interop.Kernel32.SECURITY_ATTRIBUTES* ptr = null;
		if (!string.IsNullOrEmpty(name) && options.WasSpecified)
		{
			name = options.GetNameWithSessionPrefix(name);
			if (options.CurrentUserOnly)
			{
				currentUserSecurityDescriptorInfo = new Thread.CurrentUserSecurityDescriptorInfo(2031617);
				sECURITY_ATTRIBUTES.nLength = (uint)sizeof(Interop.Kernel32.SECURITY_ATTRIBUTES);
				sECURITY_ATTRIBUTES.lpSecurityDescriptor = (void*)currentUserSecurityDescriptorInfo.SecurityDescriptor;
				ptr = &sECURITY_ATTRIBUTES;
			}
		}
		SafeWaitHandle safeWaitHandle;
		int lastPInvokeError;
		using (currentUserSecurityDescriptorInfo)
		{
			uint flags = (initiallyOwned ? 1u : 0u);
			safeWaitHandle = Interop.Kernel32.CreateMutexEx((nint)ptr, name, flags, 34603009u);
			lastPInvokeError = Marshal.GetLastPInvokeError();
			if (safeWaitHandle.IsInvalid)
			{
				safeWaitHandle.SetHandleAsInvalid();
				if (lastPInvokeError == 6)
				{
					throw new WaitHandleCannotBeOpenedException(SR.Format(SR.Threading_WaitHandleCannotBeOpenedException_InvalidHandle, name));
				}
				throw Win32Marshal.GetExceptionForWin32Error(lastPInvokeError, name);
			}
			if (lastPInvokeError == 183 && ptr != null)
			{
				try
				{
					if (!Thread.CurrentUserSecurityDescriptorInfo.IsSecurityDescriptorCompatible(currentUserSecurityDescriptorInfo.TokenUser, safeWaitHandle, 1))
					{
						throw new WaitHandleCannotBeOpenedException(SR.Format(SR.NamedWaitHandles_ExistingObjectIncompatibleWithCurrentUserOnly, name));
					}
				}
				catch
				{
					safeWaitHandle.Dispose();
					throw;
				}
			}
		}
		createdNew = lastPInvokeError != 183;
		base.SafeWaitHandle = safeWaitHandle;
	}

	private static OpenExistingResult OpenExistingWorker(string name, NamedWaitHandleOptionsInternal options, out Mutex result)
	{
		ArgumentException.ThrowIfNullOrEmpty(name, "name");
		if (options.WasSpecified)
		{
			name = options.GetNameWithSessionPrefix(name);
		}
		SafeWaitHandle safeWaitHandle = Interop.Kernel32.OpenMutex(34603009u, inheritHandle: false, name);
		if (safeWaitHandle.IsInvalid)
		{
			result = null;
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			safeWaitHandle.Dispose();
			if (2 == lastPInvokeError || 123 == lastPInvokeError)
			{
				return OpenExistingResult.NameNotFound;
			}
			if (3 == lastPInvokeError)
			{
				return OpenExistingResult.PathNotFound;
			}
			if (6 == lastPInvokeError)
			{
				return OpenExistingResult.NameInvalid;
			}
			throw Win32Marshal.GetExceptionForWin32Error(lastPInvokeError, name);
		}
		if (options.WasSpecified && options.CurrentUserOnly)
		{
			try
			{
				if (!Thread.CurrentUserSecurityDescriptorInfo.IsValidSecurityDescriptor(safeWaitHandle, 1))
				{
					safeWaitHandle.Dispose();
					result = null;
					return OpenExistingResult.ObjectIncompatibleWithCurrentUserOnly;
				}
			}
			catch
			{
				safeWaitHandle.Dispose();
				throw;
			}
		}
		result = new Mutex(safeWaitHandle);
		return OpenExistingResult.Success;
	}

	public void ReleaseMutex()
	{
		if (!Interop.Kernel32.ReleaseMutex(base.SafeWaitHandle))
		{
			throw new ApplicationException(SR.Arg_SynchronizationLockException);
		}
	}
}
