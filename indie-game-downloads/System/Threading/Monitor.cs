using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.Threading;

public static class Monitor
{
	private enum EnterHelperResult
	{
		Contention,
		Entered,
		UseSlowPath
	}

	private enum LeaveHelperAction
	{
		None,
		Signal,
		Yield,
		Contention,
		Error
	}

	public static long LockContentionCount => GetLockContentionCount() + Lock.ContentionCount;

	public static void Enter(object obj)
	{
		ArgumentNullException.ThrowIfNull(obj);
		if (!TryEnter_FastPath(obj))
		{
			Enter_Slowpath(obj);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool TryEnter_FastPath(object obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern EnterHelperResult TryEnter_FastPath_WithTimeout(object obj, int timeout);

	[DllImport("QCall", EntryPoint = "Monitor_Enter_Slowpath", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Monitor_Enter_Slowpath")]
	private static extern void Enter_Slowpath(ObjectHandleOnStack obj);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void Enter_Slowpath(object obj)
	{
		Enter_Slowpath(ObjectHandleOnStack.Create(ref obj));
	}

	[DllImport("QCall", EntryPoint = "Monitor_TryEnter_Slowpath", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Monitor_TryEnter_Slowpath")]
	private static extern int TryEnter_Slowpath(ObjectHandleOnStack obj, int timeout);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static bool TryEnter_Slowpath(object obj)
	{
		if (TryEnter_Slowpath(ObjectHandleOnStack.Create(ref obj), 0) != 0)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static bool TryEnter_Slowpath(object obj, int timeout)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(timeout, -1);
		if (TryEnter_Slowpath(ObjectHandleOnStack.Create(ref obj), timeout) != 0)
		{
			return true;
		}
		return false;
	}

	public static void Enter(object obj, ref bool lockTaken)
	{
		if (lockTaken)
		{
			ThrowLockTakenException();
		}
		ArgumentNullException.ThrowIfNull(obj);
		if (!TryEnter_FastPath(obj))
		{
			Enter_Slowpath(obj);
		}
		lockTaken = true;
	}

	[DoesNotReturn]
	private static void ThrowLockTakenException()
	{
		throw new ArgumentException(SR.Argument_MustBeFalse, "lockTaken");
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LeaveHelperAction Exit_FastPath(object obj);

	[DllImport("QCall", EntryPoint = "Monitor_Exit_Slowpath", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Monitor_Exit_Slowpath")]
	private static extern void Exit_Slowpath(ObjectHandleOnStack obj, LeaveHelperAction exitBehavior);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void Exit_Slowpath(LeaveHelperAction exitBehavior, object obj)
	{
		Exit_Slowpath(ObjectHandleOnStack.Create(ref obj), exitBehavior);
	}

	public static void Exit(object obj)
	{
		ArgumentNullException.ThrowIfNull(obj);
		LeaveHelperAction leaveHelperAction = Exit_FastPath(obj);
		if (leaveHelperAction != LeaveHelperAction.None)
		{
			Exit_Slowpath(leaveHelperAction, obj);
		}
	}

	internal static void ExitIfLockTaken(object obj, ref bool lockTaken)
	{
		ArgumentNullException.ThrowIfNull(obj);
		if (lockTaken)
		{
			LeaveHelperAction leaveHelperAction = Exit_FastPath(obj);
			if (leaveHelperAction != LeaveHelperAction.None)
			{
				Exit_Slowpath(leaveHelperAction, obj);
			}
			else
			{
				lockTaken = false;
			}
		}
	}

	public static bool TryEnter(object obj)
	{
		ArgumentNullException.ThrowIfNull(obj);
		return TryEnter_FastPath_WithTimeout(obj, 0) switch
		{
			EnterHelperResult.Entered => true, 
			EnterHelperResult.Contention => false, 
			_ => TryEnter_Slowpath(obj), 
		};
	}

	private static void TryEnter_Timeout_WithLockTaken(object obj, int millisecondsTimeout, ref bool lockTaken)
	{
		if (millisecondsTimeout >= -1)
		{
			EnterHelperResult enterHelperResult = TryEnter_FastPath_WithTimeout(obj, millisecondsTimeout);
			if (enterHelperResult == EnterHelperResult.Entered)
			{
				lockTaken = true;
				return;
			}
			if (millisecondsTimeout == 0 && enterHelperResult == EnterHelperResult.Contention)
			{
				return;
			}
		}
		if (TryEnter_Slowpath(obj, millisecondsTimeout))
		{
			lockTaken = true;
		}
	}

	public static void TryEnter(object obj, ref bool lockTaken)
	{
		if (lockTaken)
		{
			ThrowLockTakenException();
		}
		ArgumentNullException.ThrowIfNull(obj);
		TryEnter_Timeout_WithLockTaken(obj, 0, ref lockTaken);
	}

	public static bool TryEnter(object obj, int millisecondsTimeout)
	{
		ArgumentNullException.ThrowIfNull(obj);
		if (millisecondsTimeout >= -1)
		{
			EnterHelperResult enterHelperResult = TryEnter_FastPath_WithTimeout(obj, millisecondsTimeout);
			if (enterHelperResult == EnterHelperResult.Entered)
			{
				return true;
			}
			if (millisecondsTimeout == 0 && enterHelperResult == EnterHelperResult.Contention)
			{
				return false;
			}
		}
		return TryEnter_Slowpath(obj, millisecondsTimeout);
	}

	public static void TryEnter(object obj, int millisecondsTimeout, ref bool lockTaken)
	{
		if (lockTaken)
		{
			ThrowLockTakenException();
		}
		ArgumentNullException.ThrowIfNull(obj);
		TryEnter_Timeout_WithLockTaken(obj, millisecondsTimeout, ref lockTaken);
	}

	public static bool IsEntered(object obj)
	{
		ArgumentNullException.ThrowIfNull(obj, "obj");
		return IsEnteredNative(obj);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsEnteredNative(object obj);

	[LibraryImport("QCall", EntryPoint = "Monitor_Wait")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static bool Wait(ObjectHandleOnStack obj, int millisecondsTimeout)
	{
		return __PInvoke(obj, millisecondsTimeout) != 0;
		[DllImport("QCall", EntryPoint = "Monitor_Wait", ExactSpelling = true)]
		static extern int __PInvoke(ObjectHandleOnStack __obj_native, int __millisecondsTimeout_native);
	}

	[UnsupportedOSPlatform("browser")]
	public static bool Wait(object obj, int millisecondsTimeout)
	{
		ArgumentNullException.ThrowIfNull(obj, "obj");
		ArgumentOutOfRangeException.ThrowIfLessThan(millisecondsTimeout, -1, "millisecondsTimeout");
		return Wait(ObjectHandleOnStack.Create(ref obj), millisecondsTimeout);
	}

	[DllImport("QCall", EntryPoint = "Monitor_Pulse", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Monitor_Pulse")]
	private static extern void Pulse(ObjectHandleOnStack obj);

	public static void Pulse(object obj)
	{
		ArgumentNullException.ThrowIfNull(obj, "obj");
		Pulse(ObjectHandleOnStack.Create(ref obj));
	}

	[DllImport("QCall", EntryPoint = "Monitor_PulseAll", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Monitor_PulseAll")]
	private static extern void PulseAll(ObjectHandleOnStack obj);

	public static void PulseAll(object obj)
	{
		ArgumentNullException.ThrowIfNull(obj, "obj");
		PulseAll(ObjectHandleOnStack.Create(ref obj));
	}

	[DllImport("QCall", EntryPoint = "Monitor_GetLockContentionCount", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Monitor_GetLockContentionCount")]
	private static extern long GetLockContentionCount();

	public static bool TryEnter(object obj, TimeSpan timeout)
	{
		return TryEnter(obj, WaitHandle.ToTimeoutMilliseconds(timeout));
	}

	public static void TryEnter(object obj, TimeSpan timeout, ref bool lockTaken)
	{
		TryEnter(obj, WaitHandle.ToTimeoutMilliseconds(timeout), ref lockTaken);
	}

	[UnsupportedOSPlatform("browser")]
	public static bool Wait(object obj, TimeSpan timeout)
	{
		return Wait(obj, WaitHandle.ToTimeoutMilliseconds(timeout));
	}

	[UnsupportedOSPlatform("browser")]
	public static bool Wait(object obj)
	{
		return Wait(obj, -1);
	}

	[UnsupportedOSPlatform("browser")]
	public static bool Wait(object obj, int millisecondsTimeout, bool exitContext)
	{
		return Wait(obj, millisecondsTimeout);
	}

	[UnsupportedOSPlatform("browser")]
	public static bool Wait(object obj, TimeSpan timeout, bool exitContext)
	{
		return Wait(obj, WaitHandle.ToTimeoutMilliseconds(timeout));
	}
}
