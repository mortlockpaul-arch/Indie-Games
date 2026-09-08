using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;

namespace System.Threading;

public class EventWaitHandle : WaitHandle
{
	public EventWaitHandle(bool initialState, EventResetMode mode)
	{
		CreateEventCore(initialState, mode);
	}

	public EventWaitHandle(bool initialState, EventResetMode mode, string? name, NamedWaitHandleOptions options)
	{
		CreateEventCore(initialState, mode, name, new NamedWaitHandleOptionsInternal(options), out var _);
	}

	public EventWaitHandle(bool initialState, EventResetMode mode, string? name)
	{
		CreateEventCore(initialState, mode, name, default(NamedWaitHandleOptionsInternal), out var _);
	}

	public EventWaitHandle(bool initialState, EventResetMode mode, string? name, NamedWaitHandleOptions options, out bool createdNew)
	{
		CreateEventCore(initialState, mode, name, new NamedWaitHandleOptionsInternal(options), out createdNew);
	}

	public EventWaitHandle(bool initialState, EventResetMode mode, string? name, out bool createdNew)
	{
		CreateEventCore(initialState, mode, name, default(NamedWaitHandleOptionsInternal), out createdNew);
	}

	private static void ValidateMode(EventResetMode mode)
	{
		if (mode != EventResetMode.AutoReset && mode != EventResetMode.ManualReset)
		{
			throw new ArgumentException(SR.Argument_InvalidFlag, "mode");
		}
	}

	[SupportedOSPlatform("windows")]
	public static EventWaitHandle OpenExisting(string name, NamedWaitHandleOptions options)
	{
		OpenExistingResult openExistingResult = OpenExistingWorker(name, new NamedWaitHandleOptionsInternal(options), out var result);
		if (openExistingResult != OpenExistingResult.Success)
		{
			ThrowForOpenExistingFailure(openExistingResult, name);
		}
		return result;
	}

	[SupportedOSPlatform("windows")]
	public static EventWaitHandle OpenExisting(string name)
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

	[SupportedOSPlatform("windows")]
	public static bool TryOpenExisting(string name, NamedWaitHandleOptions options, [NotNullWhen(true)] out EventWaitHandle? result)
	{
		return OpenExistingWorker(name, new NamedWaitHandleOptionsInternal(options), out result) == OpenExistingResult.Success;
	}

	[SupportedOSPlatform("windows")]
	public static bool TryOpenExisting(string name, [NotNullWhen(true)] out EventWaitHandle? result)
	{
		return OpenExistingWorker(name, default(NamedWaitHandleOptionsInternal), out result) == OpenExistingResult.Success;
	}

	private EventWaitHandle(SafeWaitHandle handle)
	{
		base.SafeWaitHandle = handle;
	}

	private void CreateEventCore(bool initialState, EventResetMode mode)
	{
		ValidateMode(mode);
		uint num = (initialState ? 2u : 0u);
		if (mode == EventResetMode.ManualReset)
		{
			num |= 1;
		}
		SafeWaitHandle safeWaitHandle = Interop.Kernel32.CreateEventEx(0, null, num, 34603010u);
		if (safeWaitHandle.IsInvalid)
		{
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			safeWaitHandle.SetHandleAsInvalid();
			throw Win32Marshal.GetExceptionForWin32Error(lastPInvokeError);
		}
		base.SafeWaitHandle = safeWaitHandle;
	}

	private unsafe void CreateEventCore(bool initialState, EventResetMode mode, string name, NamedWaitHandleOptionsInternal options, out bool createdNew)
	{
		ValidateMode(mode);
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
			uint num = (initialState ? 2u : 0u);
			if (mode == EventResetMode.ManualReset)
			{
				num |= 1;
			}
			safeWaitHandle = Interop.Kernel32.CreateEventEx((nint)ptr, name, num, 34603010u);
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

	private static OpenExistingResult OpenExistingWorker(string name, NamedWaitHandleOptionsInternal options, out EventWaitHandle result)
	{
		ArgumentException.ThrowIfNullOrEmpty(name, "name");
		if (options.WasSpecified)
		{
			name = options.GetNameWithSessionPrefix(name);
		}
		SafeWaitHandle safeWaitHandle = Interop.Kernel32.OpenEvent(34603010u, inheritHandle: false, name);
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
				throw Win32Marshal.GetExceptionForWin32Error(lastPInvokeError, name);
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
		result = new EventWaitHandle(safeWaitHandle);
		return OpenExistingResult.Success;
	}

	public bool Reset()
	{
		bool num = Interop.Kernel32.ResetEvent(base.SafeWaitHandle);
		if (!num)
		{
			throw Win32Marshal.GetExceptionForLastWin32Error();
		}
		return num;
	}

	public bool Set()
	{
		bool num = Interop.Kernel32.SetEvent(base.SafeWaitHandle);
		if (!num)
		{
			throw Win32Marshal.GetExceptionForLastWin32Error();
		}
		return num;
	}

	internal static bool Set(SafeWaitHandle waitHandle)
	{
		return Interop.Kernel32.SetEvent(waitHandle);
	}
}
