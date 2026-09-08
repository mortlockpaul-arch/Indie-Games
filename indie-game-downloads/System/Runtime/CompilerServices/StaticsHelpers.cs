using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Runtime.CompilerServices;

[StackTraceHidden]
[DebuggerStepThrough]
internal static class StaticsHelpers
{
	internal struct ThreadLocalData
	{
		internal int _cNonCollectibleTlsData;

		internal int _cCollectibleTlsData;

		private nint _nonCollectibleTlsArrayData_private;

		internal unsafe nint* _collectibleTlsArrayData;

		internal object[] NonCollectibleTlsArrayData
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Unsafe.As<nint, object[]>(ref _nonCollectibleTlsArrayData_private);
			}
		}
	}

	private struct StaticFieldAddressArgs
	{
		public unsafe delegate*<nint, ref byte> staticBaseHelper;

		public nint arg0;

		public nint offset;
	}

	[LibraryImport("QCall")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private static void GetThreadStaticsByIndex(ByteRefOnStack result, int index, [MarshalAs(UnmanagedType.Bool)] bool gcStatics)
	{
		int _gcStatics_native = (gcStatics ? 1 : 0);
		__PInvoke(result, index, _gcStatics_native);
		[DllImport("QCall", EntryPoint = "GetThreadStaticsByIndex", ExactSpelling = true)]
		static extern void __PInvoke(ByteRefOnStack __result_native, int __index_native, int __gcStatics_native);
	}

	[LibraryImport("QCall")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void GetThreadStaticsByMethodTable(ByteRefOnStack result, MethodTable* pMT, [MarshalAs(UnmanagedType.Bool)] bool gcStatics)
	{
		int _gcStatics_native = (gcStatics ? 1 : 0);
		__PInvoke(result, pMT, _gcStatics_native);
		[DllImport("QCall", EntryPoint = "GetThreadStaticsByMethodTable", ExactSpelling = true)]
		unsafe static extern void __PInvoke(ByteRefOnStack __result_native, MethodTable* __pMT_native, int __gcStatics_native);
	}

	[Intrinsic]
	private static ref byte VolatileReadAsByref(ref nint address)
	{
		return ref VolatileReadAsByref(ref address);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static ref byte GetNonGCStaticBaseSlow(MethodTable* mt)
	{
		InitHelpers.InitClassSlow(mt);
		return ref DynamicStaticsInfo.MaskStaticsPointer(ref VolatileReadAsByref(ref mt->AuxiliaryData->GetDynamicStaticsInfo()._pNonGCStatics));
	}

	[DebuggerHidden]
	private unsafe static ref byte GetNonGCStaticBase(MethodTable* mt)
	{
		ref byte reference = ref VolatileReadAsByref(ref mt->AuxiliaryData->GetDynamicStaticsInfo()._pNonGCStatics);
		if (((nuint)Unsafe.AsPointer(in reference) & (nuint)1u) != 0)
		{
			return ref GetNonGCStaticBaseSlow(mt);
		}
		return ref reference;
	}

	[DebuggerHidden]
	private unsafe static ref byte GetDynamicNonGCStaticBase(DynamicStaticsInfo* dynamicStaticsInfo)
	{
		ref byte reference = ref VolatileReadAsByref(ref dynamicStaticsInfo->_pNonGCStatics);
		if (((nuint)Unsafe.AsPointer(in reference) & (nuint)1u) != 0)
		{
			return ref GetNonGCStaticBaseSlow(dynamicStaticsInfo->_methodTable);
		}
		return ref reference;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static ref byte GetGCStaticBaseSlow(MethodTable* mt)
	{
		InitHelpers.InitClassSlow(mt);
		return ref DynamicStaticsInfo.MaskStaticsPointer(ref VolatileReadAsByref(ref mt->AuxiliaryData->GetDynamicStaticsInfo()._pGCStatics));
	}

	[DebuggerHidden]
	private unsafe static ref byte GetGCStaticBase(MethodTable* mt)
	{
		ref byte reference = ref VolatileReadAsByref(ref mt->AuxiliaryData->GetDynamicStaticsInfo()._pGCStatics);
		if (((nuint)Unsafe.AsPointer(in reference) & (nuint)1u) != 0)
		{
			return ref GetGCStaticBaseSlow(mt);
		}
		return ref reference;
	}

	[DebuggerHidden]
	private unsafe static ref byte GetDynamicGCStaticBase(DynamicStaticsInfo* dynamicStaticsInfo)
	{
		ref byte reference = ref VolatileReadAsByref(ref dynamicStaticsInfo->_pGCStatics);
		if (((nuint)Unsafe.AsPointer(in reference) & (nuint)1u) != 0)
		{
			return ref GetGCStaticBaseSlow(dynamicStaticsInfo->_methodTable);
		}
		return ref reference;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerHidden]
	private unsafe static ref byte GetObjectAsRefByte(object obj)
	{
		return ref Unsafe.Subtract(ref obj.GetRawData(), sizeof(MethodTable*));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerHidden]
	private static int GetIndexOffset(int index)
	{
		return index & 0xFFFFFF;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerHidden]
	private static int GetIndexType(int index)
	{
		return index >> 24;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerHidden]
	private static bool IsIndexAllocated(int index)
	{
		return index != -1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private static ref byte GetNonGCThreadStaticsByIndexSlow(int index)
	{
		ByteRef byteRef = default(ByteRef);
		GetThreadStaticsByIndex(ByteRefOnStack.Create(ref byteRef), index, gcStatics: false);
		return ref byteRef.Get();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private static ref byte GetGCThreadStaticsByIndexSlow(int index)
	{
		ByteRef byteRef = default(ByteRef);
		GetThreadStaticsByIndex(ByteRefOnStack.Create(ref byteRef), index, gcStatics: true);
		return ref byteRef.Get();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static ref byte GetNonGCThreadStaticBaseSlow(MethodTable* mt)
	{
		ByteRef byteRef = default(ByteRef);
		GetThreadStaticsByMethodTable(ByteRefOnStack.Create(ref byteRef), mt, gcStatics: false);
		return ref byteRef.Get();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static ref byte GetGCThreadStaticBaseSlow(MethodTable* mt)
	{
		ByteRef byteRef = default(ByteRef);
		GetThreadStaticsByMethodTable(ByteRefOnStack.Create(ref byteRef), mt, gcStatics: true);
		return ref byteRef.Get();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerHidden]
	private unsafe static ref byte GetThreadLocalStaticBaseByIndex(int index, bool gcStatics)
	{
		ThreadLocalData* threadStaticsBase = Thread.GetThreadStaticsBase();
		int indexOffset = GetIndexOffset(index);
		if (GetIndexType(index) == 0)
		{
			if (threadStaticsBase->_cNonCollectibleTlsData > GetIndexOffset(index))
			{
				object obj = Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(threadStaticsBase->NonCollectibleTlsArrayData), indexOffset - 2);
				if (obj != null)
				{
					return ref GetObjectAsRefByte(obj);
				}
			}
		}
		else
		{
			if (GetIndexType(index) == 2)
			{
				return ref Unsafe.Add(ref Unsafe.AsRef<byte>(threadStaticsBase), indexOffset);
			}
			if (threadStaticsBase->_cCollectibleTlsData > indexOffset)
			{
				nint num = threadStaticsBase->_collectibleTlsArrayData[indexOffset];
				if (num != IntPtr.Zero)
				{
					object obj2 = GCHandle.InternalGet(num);
					if (obj2 != null)
					{
						return ref GetObjectAsRefByte(obj2);
					}
				}
			}
		}
		if (gcStatics)
		{
			return ref GetGCThreadStaticsByIndexSlow(index);
		}
		return ref GetNonGCThreadStaticsByIndexSlow(index);
	}

	[DebuggerHidden]
	private unsafe static ref byte GetNonGCThreadStaticBase(MethodTable* mt)
	{
		int nonGCTlsIndex = mt->AuxiliaryData->GetThreadStaticsInfo()._nonGCTlsIndex;
		if (IsIndexAllocated(nonGCTlsIndex))
		{
			return ref GetThreadLocalStaticBaseByIndex(nonGCTlsIndex, gcStatics: false);
		}
		return ref GetNonGCThreadStaticBaseSlow(mt);
	}

	[DebuggerHidden]
	private unsafe static ref byte GetGCThreadStaticBase(MethodTable* mt)
	{
		int gcTlsIndex = mt->AuxiliaryData->GetThreadStaticsInfo()._gcTlsIndex;
		if (IsIndexAllocated(gcTlsIndex))
		{
			return ref GetThreadLocalStaticBaseByIndex(gcTlsIndex, gcStatics: true);
		}
		return ref GetGCThreadStaticBaseSlow(mt);
	}

	[DebuggerHidden]
	private unsafe static ref byte GetDynamicNonGCThreadStaticBase(ThreadStaticsInfo* threadStaticsInfo)
	{
		int nonGCTlsIndex = threadStaticsInfo->_nonGCTlsIndex;
		if (IsIndexAllocated(nonGCTlsIndex))
		{
			return ref GetThreadLocalStaticBaseByIndex(nonGCTlsIndex, gcStatics: false);
		}
		return ref GetNonGCThreadStaticBaseSlow(threadStaticsInfo->_genericStatics._dynamicStatics._methodTable);
	}

	[DebuggerHidden]
	private unsafe static ref byte GetDynamicGCThreadStaticBase(ThreadStaticsInfo* threadStaticsInfo)
	{
		int gcTlsIndex = threadStaticsInfo->_gcTlsIndex;
		if (IsIndexAllocated(gcTlsIndex))
		{
			return ref GetThreadLocalStaticBaseByIndex(gcTlsIndex, gcStatics: true);
		}
		return ref GetGCThreadStaticBaseSlow(threadStaticsInfo->_genericStatics._dynamicStatics._methodTable);
	}

	[DebuggerHidden]
	private static ref byte GetOptimizedNonGCThreadStaticBase(int index)
	{
		return ref GetThreadLocalStaticBaseByIndex(index, gcStatics: false);
	}

	[DebuggerHidden]
	private static ref byte GetOptimizedGCThreadStaticBase(int index)
	{
		return ref GetThreadLocalStaticBaseByIndex(index, gcStatics: true);
	}

	[DebuggerHidden]
	private unsafe static ref byte StaticFieldAddress_Dynamic(StaticFieldAddressArgs* pArgs)
	{
		return ref Unsafe.Add(ref pArgs->staticBaseHelper(pArgs->arg0), pArgs->offset);
	}

	[DebuggerHidden]
	private unsafe static ref byte StaticFieldAddressUnbox_Dynamic(StaticFieldAddressArgs* pArgs)
	{
		return ref Unsafe.As<byte, object>(ref Unsafe.Add(ref pArgs->staticBaseHelper(pArgs->arg0), pArgs->offset)).GetRawData();
	}
}
