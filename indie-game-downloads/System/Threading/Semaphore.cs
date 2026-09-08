using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;

namespace System.Threading;

public sealed class Semaphore : WaitHandle
{
	public Semaphore(int initialCount, int maximumCount)
	{
		CreateSemaphoreCore(initialCount, maximumCount);
	}

	public Semaphore(int initialCount, int maximumCount, string? name, NamedWaitHandleOptions options)
	{
		CreateSemaphoreCore(initialCount, maximumCount, name, new NamedWaitHandleOptionsInternal(options), out var _);
	}

	public Semaphore(int initialCount, int maximumCount, string? name)
	{
		CreateSemaphoreCore(initialCount, maximumCount, name, default(NamedWaitHandleOptionsInternal), out var _);
	}

	public Semaphore(int initialCount, int maximumCount, string? name, NamedWaitHandleOptions options, out bool createdNew)
	{
		CreateSemaphoreCore(initialCount, maximumCount, name, new NamedWaitHandleOptionsInternal(options), out createdNew);
	}

	public Semaphore(int initialCount, int maximumCount, string? name, out bool createdNew)
	{
		CreateSemaphoreCore(initialCount, maximumCount, name, default(NamedWaitHandleOptionsInternal), out createdNew);
	}

	private static void ValidateArguments(int initialCount, int maximumCount)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(initialCount, "initialCount");
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumCount, "maximumCount");
		if (initialCount > maximumCount)
		{
			throw new ArgumentException(SR.Argument_SemaphoreInitialMaximum);
		}
	}

	[SupportedOSPlatform("windows")]
	public static Semaphore OpenExisting(string name, NamedWaitHandleOptions options)
	{
		OpenExistingResult openExistingResult = OpenExistingWorker(name, new NamedWaitHandleOptionsInternal(options), out var result);
		if (openExistingResult != OpenExistingResult.Success)
		{
			ThrowForOpenExistingFailure(openExistingResult, name);
		}
		return result;
	}

	[SupportedOSPlatform("windows")]
	public static Semaphore OpenExisting(string name)
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
			throw new IOException(SR.Format(SR.IO_PathNotFound_Path, name));
		default:
			throw new WaitHandleCannotBeOpenedException(SR.Format(SR.NamedWaitHandles_ExistingObjectIncompatibleWithCurrentUserOnly, name));
		}
	}

	[SupportedOSPlatform("windows")]
	public static bool TryOpenExisting(string name, NamedWaitHandleOptions options, [NotNullWhen(true)] out Semaphore? result)
	{
		return OpenExistingWorker(name, new NamedWaitHandleOptionsInternal(options), out result) == OpenExistingResult.Success;
	}

	[SupportedOSPlatform("windows")]
	public static bool TryOpenExisting(string name, [NotNullWhen(true)] out Semaphore? result)
	{
		return OpenExistingWorker(name, default(NamedWaitHandleOptionsInternal), out result) == OpenExistingResult.Success;
	}

	public int Release()
	{
		return ReleaseCore(1);
	}

	public int Release(int releaseCount)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(releaseCount, "releaseCount");
		return ReleaseCore(releaseCount);
	}

	private Semaphore(SafeWaitHandle handle)
	{
		base.SafeWaitHandle = handle;
	}

	private void CreateSemaphoreCore(int initialCount, int maximumCount)
	{
		ValidateArguments(initialCount, maximumCount);
		SafeWaitHandle safeWaitHandle = Interop.Kernel32.CreateSemaphoreEx(0, initialCount, maximumCount, null, 0u, 34603010u);
		if (safeWaitHandle.IsInvalid)
		{
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			safeWaitHandle.SetHandleAsInvalid();
			throw Win32Marshal.GetExceptionForWin32Error(lastPInvokeError);
		}
		base.SafeWaitHandle = safeWaitHandle;
	}

	private unsafe void CreateSemaphoreCore(int initialCount, int maximumCount, string name, NamedWaitHandleOptionsInternal options, out bool createdNew)
	{
		ValidateArguments(initialCount, maximumCount);
		void* ptr = null;
		Thread.CurrentUserSecurityDescriptorInfo currentUserSecurityDescriptorInfo = default(Thread.CurrentUserSecurityDescriptorInfo);
		Interop.Kernel32.SECURITY_ATTRIBUTES sECURITY_ATTRIBUTES = default(Interop.Kernel32.SECURITY_ATTRIBUTES);
		if (!string.IsNullOrEmpty(name) && options.WasSpecified)
		{
			name = options.GetNameWithSessionPrefix(name);
			if (options.CurrentUserOnly)
			{
				currentUserSecurityDescriptorInfo = new Thread.CurrentUserSecurityDescriptorInfo(2031618);
				sECURITY_ATTRIBUTES.nLength = (uint)sizeof(Interop.Kernel32.SECURITY_ATTRIBUTES);
				sECURITY_ATTRIBUTES.lpSecurityDescriptor = (void*)currentUserSecurityDescriptorInfo.SecurityDescriptor;
				ptr = &sECURITY_ATTRIBUTES;
			}
		}
		SafeWaitHandle safeWaitHandle;
		int lastPInvokeError;
		using (currentUserSecurityDescriptorInfo)
		{
			safeWaitHandle = Interop.Kernel32.CreateSemaphoreEx((nint)ptr, initialCount, maximumCount, name, 0u, 34603010u);
			lastPInvokeError = Marshal.GetLastPInvokeError();
			if (safeWaitHandle.IsInvalid)
			{
				safeWaitHandle.SetHandleAsInvalid();
				if (!string.IsNullOrEmpty(name) && lastPInvokeError == 6)
				{
					throw new WaitHandleCannotBeOpenedException(SR.Format(SR.Threading_WaitHandleCannotBeOpenedException_InvalidHandle, name));
				}
				throw Win32Marshal.GetExceptionForWin32Error(lastPInvokeError, name);
			}
			if (lastPInvokeError == 183 && ptr != null)
			{
				try
				{
					if (!Thread.CurrentUserSecurityDescriptorInfo.IsSecurityDescriptorCompatible(currentUserSecurityDescriptorInfo.TokenUser, safeWaitHandle, 2))
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

	private static OpenExistingResult OpenExistingWorker(string name, NamedWaitHandleOptionsInternal options, out Semaphore result)
	{
		ArgumentException.ThrowIfNullOrEmpty(name, "name");
		if (options.WasSpecified)
		{
			name = options.GetNameWithSessionPrefix(name);
		}
		SafeWaitHandle safeWaitHandle = Interop.Kernel32.OpenSemaphore(34603010u, inheritHandle: false, name);
		if (safeWaitHandle.IsInvalid)
		{
			result = null;
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			safeWaitHandle.Dispose();
			switch (lastPInvokeError)
			{
			case 2:
			case 123:
				return OpenExistingResult.NameNotFound;
			case 3:
				return OpenExistingResult.PathNotFound;
			case 6:
				return OpenExistingResult.NameInvalid;
			default:
				throw Win32Marshal.GetExceptionForLastWin32Error();
			}
		}
		if (options.WasSpecified && options.CurrentUserOnly)
		{
			try
			{
				if (!Thread.CurrentUserSecurityDescriptorInfo.IsValidSecurityDescriptor(safeWaitHandle, 2))
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
		result = new Semaphore(safeWaitHandle);
		return OpenExistingResult.Success;
	}

	private int ReleaseCore(int releaseCount)
	{
		if (!Interop.Kernel32.ReleaseSemaphore(base.SafeWaitHandle, releaseCount, out var previousCount))
		{
			throw new SemaphoreFullException();
		}
		return previousCount;
	}
}
