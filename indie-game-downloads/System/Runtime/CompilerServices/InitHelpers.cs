using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[StackTraceHidden]
[DebuggerStepThrough]
internal static class InitHelpers
{
	[DllImport("QCall", ExactSpelling = true)]
	[LibraryImport("QCall")]
	private unsafe static extern void InitClassHelper(MethodTable* mt);

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	internal unsafe static void InitClassSlow(MethodTable* mt)
	{
		InitClassHelper(mt);
	}

	[DebuggerHidden]
	private unsafe static void InitClass(MethodTable* mt)
	{
		if (!mt->AuxiliaryData->IsClassInited)
		{
			InitClassSlow(mt);
		}
	}

	[DebuggerHidden]
	private unsafe static void InitInstantiatedClass(MethodTable* mt, MethodDesc* methodDesc)
	{
		MethodTable* methodTable = methodDesc->MethodTable;
		MethodTable* ptr = ((!methodTable->IsSharedByGenericInstantiations) ? methodTable : mt->GetMethodTableMatchingParentClass(methodTable));
		if (!ptr->AuxiliaryData->IsClassInitedAndActive)
		{
			InitClassSlow(ptr);
		}
	}
}
