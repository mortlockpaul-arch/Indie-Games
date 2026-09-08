using System.Diagnostics;

namespace System.Runtime.CompilerServices;

internal ref struct DynamicStaticsInfo
{
	internal nint _pGCStatics;

	internal nint _pNonGCStatics;

	internal unsafe MethodTable* _methodTable;

	[DebuggerHidden]
	[DebuggerStepThrough]
	internal unsafe static ref byte MaskStaticsPointer(ref byte staticsPtr)
	{
		fixed (byte* ptr = &staticsPtr)
		{
			return ref Unsafe.AsRef<byte>((void*)((nuint)ptr & (nuint)(~(nint)1)));
		}
	}
}
