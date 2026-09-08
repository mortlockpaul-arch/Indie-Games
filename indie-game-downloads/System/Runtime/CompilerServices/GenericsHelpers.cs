using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[StackTraceHidden]
[DebuggerStepThrough]
internal static class GenericsHelpers
{
	public struct GenericHandleArgs
	{
		public nint signature;

		public nint module;

		public uint dictionaryIndexAndSlot;
	}

	[DllImport("QCall", ExactSpelling = true)]
	[LibraryImport("QCall")]
	private static extern nint GenericHandleWorker(nint pMD, nint pMT, nint signature, uint dictionaryIndexAndSlot, nint pModule);

	[DebuggerHidden]
	public static nint Method(nint methodHnd, nint signature)
	{
		return GenericHandleWorker(methodHnd, IntPtr.Zero, signature, uint.MaxValue, IntPtr.Zero);
	}

	[DebuggerHidden]
	public unsafe static nint MethodWithSlotAndModule(nint methodHnd, GenericHandleArgs* pArgs)
	{
		return GenericHandleWorker(methodHnd, IntPtr.Zero, pArgs->signature, pArgs->dictionaryIndexAndSlot, pArgs->module);
	}

	[DebuggerHidden]
	public static nint Class(nint classHnd, nint signature)
	{
		return GenericHandleWorker(IntPtr.Zero, classHnd, signature, uint.MaxValue, IntPtr.Zero);
	}

	[DebuggerHidden]
	public unsafe static nint ClassWithSlotAndModule(nint classHnd, GenericHandleArgs* pArgs)
	{
		return GenericHandleWorker(IntPtr.Zero, classHnd, pArgs->signature, pArgs->dictionaryIndexAndSlot, pArgs->module);
	}
}
