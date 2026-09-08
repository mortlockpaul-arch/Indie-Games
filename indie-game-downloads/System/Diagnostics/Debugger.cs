using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Diagnostics;

public static class Debugger
{
	private sealed class CrossThreadDependencyNotification : ICustomDebuggerNotification
	{
	}

	public static readonly string? DefaultCategory;

	public static bool IsAttached => IsManagedDebuggerAttached() != 0;

	[FeatureSwitchDefinition("System.Diagnostics.Debugger.IsSupported")]
	internal static bool IsSupported { get; } = InitializeIsSupported();

	[DllImport("QCall", EntryPoint = "DebugDebugger_Break", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "DebugDebugger_Break")]
	private static extern void BreakInternal();

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void Break()
	{
		BreakInternal();
	}

	public static bool Launch()
	{
		if (!IsAttached)
		{
			return LaunchInternal();
		}
		return true;
	}

	public static void NotifyOfCrossThreadDependency()
	{
		if (IsAttached)
		{
			NotifyOfCrossThreadDependencySlow();
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void NotifyOfCrossThreadDependencySlow()
		{
			CrossThreadDependencyNotification o = new CrossThreadDependencyNotification();
			CustomNotification(ObjectHandleOnStack.Create(ref o));
		}
	}

	[LibraryImport("QCall", EntryPoint = "DebugDebugger_Launch")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static bool LaunchInternal()
	{
		return __PInvoke() != 0;
		[DllImport("QCall", EntryPoint = "DebugDebugger_Launch", ExactSpelling = true)]
		static extern int __PInvoke();
	}

	[DllImport("QCall", EntryPoint = "DebugDebugger_IsManagedDebuggerAttached", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "DebugDebugger_IsManagedDebuggerAttached")]
	[SuppressGCTransition]
	private static extern int IsManagedDebuggerAttached();

	public static void Log(int level, string? category, string? message)
	{
		LogInternal(level, category, message);
	}

	[LibraryImport("QCall", EntryPoint = "DebugDebugger_Log", StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void LogInternal(int level, string category, string message)
	{
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(message))
		{
			void* _message_native = ptr;
			fixed (char* ptr2 = &Utf16StringMarshaller.GetPinnableReference(category))
			{
				void* _category_native = ptr2;
				__PInvoke(level, (ushort*)_category_native, (ushort*)_message_native);
			}
		}
		[DllImport("QCall", EntryPoint = "DebugDebugger_Log", ExactSpelling = true)]
		unsafe static extern void __PInvoke(int __level_native, ushort* __category_native, ushort* __message_native);
	}

	public static bool IsLogging()
	{
		return IsLoggingInternal() != 0;
	}

	[DllImport("QCall", EntryPoint = "DebugDebugger_IsLoggingHelper", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "DebugDebugger_IsLoggingHelper")]
	[SuppressGCTransition]
	private static extern int IsLoggingInternal();

	[DllImport("QCall", EntryPoint = "DebugDebugger_CustomNotification", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "DebugDebugger_CustomNotification")]
	private static extern void CustomNotification(ObjectHandleOnStack data);

	[StackTraceHidden]
	[DebuggerStepThrough]
	[DebuggerHidden]
	internal static void UserBreakpoint()
	{
		Break();
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	public static void BreakForUserUnhandledException(Exception exception)
	{
	}

	private static bool InitializeIsSupported()
	{
		if (!AppContext.TryGetSwitch("System.Diagnostics.Debugger.IsSupported", out var isEnabled))
		{
			return true;
		}
		return isEnabled;
	}
}
