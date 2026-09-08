using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[StackTraceHidden]
[DebuggerStepThrough]
internal static class CastHelpers
{
	internal static int[] s_table;

	[DllImport("QCall", EntryPoint = "ThrowInvalidCastException", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThrowInvalidCastException")]
	private unsafe static extern void ThrowInvalidCastExceptionInternal(void* fromTypeHnd, void* toTypeHnd);

	[DoesNotReturn]
	internal unsafe static void ThrowInvalidCastException(void* fromTypeHnd, void* toTypeHnd)
	{
		ThrowInvalidCastExceptionInternal(fromTypeHnd, toTypeHnd);
		throw null;
	}

	[DoesNotReturn]
	internal unsafe static void ThrowInvalidCastException(object fromType, void* toTypeHnd)
	{
		ThrowInvalidCastExceptionInternal(RuntimeHelpers.GetMethodTable(fromType), toTypeHnd);
		GC.KeepAlive(fromType);
		throw null;
	}

	[LibraryImport("QCall")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private unsafe static bool IsInstanceOf_NoCacheLookup(void* toTypeHnd, [MarshalAs(UnmanagedType.Bool)] bool throwCastException, ObjectHandleOnStack obj)
	{
		int _throwCastException_native = (throwCastException ? 1 : 0);
		return __PInvoke(toTypeHnd, _throwCastException_native, obj) != 0;
		[DllImport("QCall", EntryPoint = "IsInstanceOf_NoCacheLookup", ExactSpelling = true)]
		unsafe static extern int __PInvoke(void* __toTypeHnd_native, int __throwCastException_native, ObjectHandleOnStack __obj_native);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe static object IsInstanceOfAny_NoCacheLookup(void* toTypeHnd, object obj)
	{
		if (IsInstanceOf_NoCacheLookup(toTypeHnd, throwCastException: false, ObjectHandleOnStack.Create(ref obj)))
		{
			return obj;
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe static object ChkCastAny_NoCacheLookup(void* toTypeHnd, object obj)
	{
		IsInstanceOf_NoCacheLookup(toTypeHnd, throwCastException: true, ObjectHandleOnStack.Create(ref obj));
		return obj;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WriteBarrier(ref object dst, object obj);

	[DebuggerHidden]
	internal unsafe static object IsInstanceOfAny(void* toTypeHnd, object obj)
	{
		if (obj != null)
		{
			void* methodTable = RuntimeHelpers.GetMethodTable(obj);
			if (methodTable != toTypeHnd)
			{
				CastResult castResult = CastCache.TryGet(s_table, (nuint)methodTable, (nuint)toTypeHnd);
				if (castResult != CastResult.CanCast)
				{
					if (castResult != CastResult.CannotCast)
					{
						return IsInstanceOfAny_NoCacheLookup(toTypeHnd, obj);
					}
					obj = null;
				}
			}
		}
		return obj;
	}

	[DebuggerHidden]
	private unsafe static object IsInstanceOfInterface(void* toTypeHnd, object obj)
	{
		MethodTable* methodTable;
		nint num;
		MethodTable** ptr;
		if (obj != null)
		{
			methodTable = RuntimeHelpers.GetMethodTable(obj);
			num = methodTable->InterfaceCount;
			if (num == 0)
			{
				goto IL_0083;
			}
			ptr = methodTable->InterfaceMap;
			if (num < 4)
			{
				goto IL_006c;
			}
			while (*ptr != toTypeHnd && ptr[1] != toTypeHnd && ptr[2] != toTypeHnd && ptr[3] != toTypeHnd)
			{
				ptr += 4;
				num -= 4;
				if (num >= 4)
				{
					continue;
				}
				goto IL_0069;
			}
		}
		goto IL_008e;
		IL_006c:
		while (*ptr != toTypeHnd)
		{
			ptr++;
			num--;
			if (num > 0)
			{
				continue;
			}
			goto IL_0083;
		}
		goto IL_008e;
		IL_0083:
		if (!methodTable->NonTrivialInterfaceCast)
		{
			obj = null;
			goto IL_008e;
		}
		return IsInstance_Helper(toTypeHnd, obj);
		IL_008e:
		return obj;
		IL_0069:
		if (num != 0)
		{
			goto IL_006c;
		}
		goto IL_0083;
	}

	[DebuggerHidden]
	private unsafe static object IsInstanceOfClass(void* toTypeHnd, object obj)
	{
		if (obj == null || RuntimeHelpers.GetMethodTable(obj) == toTypeHnd)
		{
			return obj;
		}
		MethodTable* parentMethodTable = RuntimeHelpers.GetMethodTable(obj)->ParentMethodTable;
		while (parentMethodTable != toTypeHnd)
		{
			if (parentMethodTable != null)
			{
				parentMethodTable = parentMethodTable->ParentMethodTable;
				if (parentMethodTable == toTypeHnd)
				{
					break;
				}
				if (parentMethodTable != null)
				{
					parentMethodTable = parentMethodTable->ParentMethodTable;
					if (parentMethodTable == toTypeHnd)
					{
						break;
					}
					if (parentMethodTable != null)
					{
						parentMethodTable = parentMethodTable->ParentMethodTable;
						if (parentMethodTable == toTypeHnd)
						{
							break;
						}
						if (parentMethodTable != null)
						{
							parentMethodTable = parentMethodTable->ParentMethodTable;
							continue;
						}
					}
				}
			}
			obj = null;
			break;
		}
		return obj;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static object IsInstance_Helper(void* toTypeHnd, object obj)
	{
		return CastCache.TryGet(s_table, (nuint)RuntimeHelpers.GetMethodTable(obj), (nuint)toTypeHnd) switch
		{
			CastResult.CanCast => obj, 
			CastResult.CannotCast => null, 
			_ => IsInstanceOfAny_NoCacheLookup(toTypeHnd, obj), 
		};
	}

	[DebuggerHidden]
	internal unsafe static object ChkCastAny(void* toTypeHnd, object obj)
	{
		if (obj != null)
		{
			void* methodTable = RuntimeHelpers.GetMethodTable(obj);
			if (methodTable != toTypeHnd && CastCache.TryGet(s_table, (nuint)methodTable, (nuint)toTypeHnd) != CastResult.CanCast)
			{
				return ChkCastAny_NoCacheLookup(toTypeHnd, obj);
			}
		}
		return obj;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static object ChkCast_Helper(void* toTypeHnd, object obj)
	{
		if (CastCache.TryGet(s_table, (nuint)RuntimeHelpers.GetMethodTable(obj), (nuint)toTypeHnd) == CastResult.CanCast)
		{
			return obj;
		}
		return ChkCastAny_NoCacheLookup(toTypeHnd, obj);
	}

	[DebuggerHidden]
	private unsafe static object ChkCastInterface(void* toTypeHnd, object obj)
	{
		nint num;
		MethodTable** ptr;
		if (obj != null)
		{
			MethodTable* methodTable = RuntimeHelpers.GetMethodTable(obj);
			num = methodTable->InterfaceCount;
			if (num == 0)
			{
				goto IL_0084;
			}
			ptr = methodTable->InterfaceMap;
			if (num < 4)
			{
				goto IL_0069;
			}
			while (*ptr != toTypeHnd && ptr[1] != toTypeHnd && ptr[2] != toTypeHnd && ptr[3] != toTypeHnd)
			{
				ptr += 4;
				num -= 4;
				if (num >= 4)
				{
					continue;
				}
				goto IL_0066;
			}
		}
		goto IL_0082;
		IL_0082:
		return obj;
		IL_0069:
		while (*ptr != toTypeHnd)
		{
			ptr++;
			num--;
			if (num > 0)
			{
				continue;
			}
			goto IL_0084;
		}
		goto IL_0082;
		IL_0066:
		if (num != 0)
		{
			goto IL_0069;
		}
		goto IL_0084;
		IL_0084:
		return ChkCast_Helper(toTypeHnd, obj);
	}

	[DebuggerHidden]
	private unsafe static object ChkCastClass(void* toTypeHnd, object obj)
	{
		if (obj == null || RuntimeHelpers.GetMethodTable(obj) == toTypeHnd)
		{
			return obj;
		}
		return ChkCastClassSpecial(toTypeHnd, obj);
	}

	[DebuggerHidden]
	private unsafe static object ChkCastClassSpecial(void* toTypeHnd, object obj)
	{
		MethodTable* ptr = RuntimeHelpers.GetMethodTable(obj);
		while (true)
		{
			ptr = ptr->ParentMethodTable;
			if (ptr != toTypeHnd)
			{
				if (ptr == null)
				{
					break;
				}
				ptr = ptr->ParentMethodTable;
				if (ptr != toTypeHnd)
				{
					if (ptr == null)
					{
						break;
					}
					ptr = ptr->ParentMethodTable;
					if (ptr != toTypeHnd)
					{
						if (ptr == null)
						{
							break;
						}
						ptr = ptr->ParentMethodTable;
						if (ptr != toTypeHnd)
						{
							if (ptr == null)
							{
								break;
							}
							continue;
						}
					}
				}
			}
			return obj;
		}
		return ChkCast_Helper(toTypeHnd, obj);
	}

	[DebuggerHidden]
	private unsafe static ref byte Unbox(MethodTable* toTypeHnd, object obj)
	{
		if (RuntimeHelpers.GetMethodTable(obj) == toTypeHnd)
		{
			return ref obj.GetRawData();
		}
		return ref Unbox_Helper(toTypeHnd, obj);
	}

	[DebuggerHidden]
	private static void ThrowIndexOutOfRangeException()
	{
		throw new IndexOutOfRangeException();
	}

	[DebuggerHidden]
	private static void ThrowArrayMismatchException()
	{
		throw new ArrayTypeMismatchException();
	}

	[DebuggerHidden]
	private unsafe static ref object LdelemaRef(object[] array, nint index, void* type)
	{
		if ((nuint)index >= (nuint)(uint)array.Length)
		{
			ThrowIndexOutOfRangeException();
		}
		ref object result = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), index);
		if (RuntimeHelpers.GetMethodTable(array)->ElementType != type)
		{
			ThrowArrayMismatchException();
		}
		return ref result;
	}

	[DebuggerHidden]
	private unsafe static void StelemRef(object[] array, nint index, object obj)
	{
		if ((nuint)index >= (nuint)(uint)array.Length)
		{
			ThrowIndexOutOfRangeException();
		}
		ref object reference = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), index);
		void* elementType = RuntimeHelpers.GetMethodTable(array)->ElementType;
		if (obj != null)
		{
			if (elementType == RuntimeHelpers.GetMethodTable(obj) || array.GetType() == typeof(object[]))
			{
				WriteBarrier(ref reference, obj);
			}
			else
			{
				StelemRef_Helper(ref reference, elementType, obj);
			}
		}
		else
		{
			reference = null;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static void StelemRef_Helper(ref object element, void* elementType, object obj)
	{
		if (CastCache.TryGet(s_table, (nuint)RuntimeHelpers.GetMethodTable(obj), (nuint)elementType) == CastResult.CanCast)
		{
			WriteBarrier(ref element, obj);
		}
		else
		{
			StelemRef_Helper_NoCacheLookup(ref element, elementType, obj);
		}
	}

	[DebuggerHidden]
	private unsafe static void StelemRef_Helper_NoCacheLookup(ref object element, void* elementType, object obj)
	{
		object obj2 = IsInstanceOfAny_NoCacheLookup(elementType, obj);
		if (obj2 == null)
		{
			ThrowArrayMismatchException();
		}
		WriteBarrier(ref element, obj2);
	}

	[DebuggerHidden]
	private unsafe static void ArrayTypeCheck(object obj, Array array)
	{
		void* elementType = RuntimeHelpers.GetMethodTable(array)->ElementType;
		if (CastCache.TryGet(s_table, (nuint)RuntimeHelpers.GetMethodTable(obj), (nuint)elementType) != CastResult.CanCast)
		{
			ArrayTypeCheck_Helper(obj, elementType);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static void ArrayTypeCheck_Helper(object obj, void* elementType)
	{
		if (IsInstanceOfAny_NoCacheLookup(elementType, obj) == null)
		{
			ThrowArrayMismatchException();
		}
	}

	[DebuggerHidden]
	internal unsafe static object Box_Nullable(MethodTable* srcMT, ref byte nullableData)
	{
		if (nullableData == 0)
		{
			return null;
		}
		return Box(srcMT->InstantiationArg0(), ref Unsafe.Add(ref nullableData, srcMT->NullableValueAddrOffset));
	}

	[DebuggerHidden]
	internal unsafe static object Box(MethodTable* typeMT, ref byte unboxedData)
	{
		Unsafe.ReadUnaligned<byte>(in unboxedData);
		object obj = RuntimeTypeHandle.InternalAllocNoChecks(typeMT);
		if (typeMT->ContainsGCPointers)
		{
			Buffer.BulkMoveWithWriteBarrier(ref obj.GetRawData(), ref unboxedData, typeMT->GetNumInstanceFieldBytesIfContainsGCPointers());
		}
		else
		{
			SpanHelpers.Memmove(ref obj.GetRawData(), ref unboxedData, typeMT->GetNumInstanceFieldBytes());
		}
		return obj;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static bool AreTypesEquivalent(MethodTable* pMTa, MethodTable* pMTb)
	{
		if (pMTa == pMTb)
		{
			return true;
		}
		if (!pMTa->HasTypeEquivalence || !pMTb->HasTypeEquivalence)
		{
			return false;
		}
		return RuntimeHelpers.AreTypesEquivalent(pMTa, pMTb);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerHidden]
	internal unsafe static bool IsNullableForType(MethodTable* typeMT, MethodTable* boxedMT)
	{
		if (!typeMT->IsNullable)
		{
			return false;
		}
		MethodTable* perInstInfo = *(*typeMT->PerInstInfo);
		if (perInstInfo == boxedMT)
		{
			return true;
		}
		return AreTypesEquivalent(perInstInfo, boxedMT);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static void Unbox_Nullable_NotIsNullableForType(ref byte destPtr, MethodTable* typeMT, object obj)
	{
		if (typeMT != RuntimeHelpers.GetMethodTable(obj))
		{
			ThrowInvalidCastException(obj, typeMT);
		}
		Buffer.BulkMoveWithWriteBarrier(ref destPtr, ref obj.GetRawData(), typeMT->GetNullableNumInstanceFieldBytes());
	}

	[DebuggerHidden]
	internal unsafe static void Unbox_Nullable(ref byte destPtr, MethodTable* typeMT, object obj)
	{
		if (obj == null)
		{
			if (!typeMT->ContainsGCPointers)
			{
				SpanHelpers.ClearWithoutReferences(ref destPtr, typeMT->GetNullableNumInstanceFieldBytes());
			}
			else
			{
				SpanHelpers.ClearWithReferences(ref Unsafe.As<byte, nint>(ref destPtr), (nuint)typeMT->GetNumInstanceFieldBytesIfContainsGCPointers() / (nuint)8u);
			}
			return;
		}
		if (!IsNullableForType(typeMT, RuntimeHelpers.GetMethodTable(obj)))
		{
			Unbox_Nullable_NotIsNullableForType(ref destPtr, typeMT, obj);
			return;
		}
		Unsafe.As<byte, bool>(ref destPtr) = true;
		ref byte reference = ref Unsafe.Add(ref destPtr, typeMT->NullableValueAddrOffset);
		uint nullableValueSize = typeMT->NullableValueSize;
		ref byte rawData = ref obj.GetRawData();
		if (typeMT->ContainsGCPointers)
		{
			Buffer.BulkMoveWithWriteBarrier(ref reference, ref rawData, nullableValueSize);
		}
		else
		{
			SpanHelpers.Memmove(ref reference, ref rawData, nullableValueSize);
		}
	}

	[DebuggerHidden]
	internal unsafe static object ReboxFromNullable(MethodTable* srcMT, object src)
	{
		return Box_Nullable(srcMT, ref src.GetRawData());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static ref byte Unbox_Helper(MethodTable* pMT1, object obj)
	{
		MethodTable* methodTable = RuntimeHelpers.GetMethodTable(obj);
		if ((!pMT1->IsPrimitive || !methodTable->IsPrimitive || pMT1->GetPrimitiveCorElementType() != methodTable->GetPrimitiveCorElementType()) && !AreTypesEquivalent(pMT1, methodTable))
		{
			ThrowInvalidCastException(obj, pMT1);
		}
		return ref obj.GetRawData();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static void Unbox_TypeTest_Helper(MethodTable* pMT1, MethodTable* pMT2)
	{
		if ((!pMT1->IsPrimitive || !pMT2->IsPrimitive || pMT1->GetPrimitiveCorElementType() != pMT2->GetPrimitiveCorElementType()) && !AreTypesEquivalent(pMT1, pMT2))
		{
			ThrowInvalidCastException(pMT1, pMT2);
		}
	}

	[DebuggerHidden]
	private unsafe static void Unbox_TypeTest(MethodTable* pMT1, MethodTable* pMT2)
	{
		if (pMT1 != pMT2)
		{
			Unbox_TypeTest_Helper(pMT1, pMT2);
		}
	}
}
