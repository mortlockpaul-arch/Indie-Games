using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace System.Threading;

public static class EventWaitHandleAcl
{
	public unsafe static EventWaitHandle Create(bool initialState, EventResetMode mode, string? name, out bool createdNew, EventWaitHandleSecurity? eventSecurity)
	{
		if (eventSecurity == null)
		{
			return new EventWaitHandle(initialState, mode, name, out createdNew);
		}
		if (mode != EventResetMode.AutoReset && mode != EventResetMode.ManualReset)
		{
			throw new ArgumentOutOfRangeException("mode");
		}
		uint num = (initialState ? 2u : 0u);
		if (mode == EventResetMode.ManualReset)
		{
			num |= 1;
		}
		fixed (byte* securityDescriptorBinaryForm = eventSecurity.GetSecurityDescriptorBinaryForm())
		{
			global::Interop.Kernel32.SECURITY_ATTRIBUTES sECURITY_ATTRIBUTES = new global::Interop.Kernel32.SECURITY_ATTRIBUTES
			{
				nLength = (uint)sizeof(global::Interop.Kernel32.SECURITY_ATTRIBUTES),
				lpSecurityDescriptor = securityDescriptorBinaryForm
			};
			SafeWaitHandle safeWaitHandle = global::Interop.Kernel32.CreateEventEx((nint)(&sECURITY_ATTRIBUTES), name, num, 2031619u);
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			if (safeWaitHandle.IsInvalid)
			{
				safeWaitHandle.SetHandleAsInvalid();
				if (!string.IsNullOrEmpty(name) && lastPInvokeError == 6)
				{
					throw new WaitHandleCannotBeOpenedException(System.SR.Format(System.SR.WaitHandleCannotBeOpenedException_InvalidHandle, name));
				}
				throw System.IO.Win32Marshal.GetExceptionForWin32Error(lastPInvokeError, name);
			}
			createdNew = lastPInvokeError != 183;
			return CreateAndReplaceHandle(safeWaitHandle);
		}
	}

	public static EventWaitHandle OpenExisting(string name, EventWaitHandleRights rights)
	{
		EventWaitHandle result;
		return OpenExistingWorker(name, rights, out result) switch
		{
			System.Threading.OpenExistingResult.NameNotFound => throw new WaitHandleCannotBeOpenedException(), 
			System.Threading.OpenExistingResult.NameInvalid => throw new WaitHandleCannotBeOpenedException(System.SR.Format(System.SR.Threading_WaitHandleCannotBeOpenedException_InvalidHandle, name)), 
			System.Threading.OpenExistingResult.PathNotFound => throw new DirectoryNotFoundException(System.SR.Format(System.SR.IO_PathNotFound_Path, name)), 
			_ => result, 
		};
	}

	public static bool TryOpenExisting(string name, EventWaitHandleRights rights, [NotNullWhen(true)] out EventWaitHandle? result)
	{
		return OpenExistingWorker(name, rights, out result) == System.Threading.OpenExistingResult.Success;
	}

	private static System.Threading.OpenExistingResult OpenExistingWorker(string name, EventWaitHandleRights rights, out EventWaitHandle result)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		if (name.Length == 0)
		{
			throw new ArgumentException(System.SR.Argument_EmptyName, "name");
		}
		result = null;
		SafeWaitHandle safeWaitHandle = global::Interop.Kernel32.OpenEvent((uint)rights, inheritHandle: false, name);
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

	private static EventWaitHandle CreateAndReplaceHandle(SafeWaitHandle replacementHandle)
	{
		EventWaitHandle eventWaitHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);
		SafeWaitHandle safeWaitHandle = eventWaitHandle.SafeWaitHandle;
		eventWaitHandle.SafeWaitHandle = replacementHandle;
		safeWaitHandle.Dispose();
		return eventWaitHandle;
	}
}
