using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

internal unsafe readonly struct TypeHandle(void* tAddr)
{
	private unsafe readonly void* m_asTAddr = tAddr;

	public unsafe bool IsNull => m_asTAddr == null;

	public unsafe bool IsTypeDesc
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return ((nuint)m_asTAddr & (nuint)2u) != 0;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe MethodTable* AsMethodTable()
	{
		return (MethodTable*)m_asTAddr;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe TypeDesc* AsTypeDesc()
	{
		return (TypeDesc*)((nuint)m_asTAddr & unchecked((nuint)(-3)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static TypeHandle TypeHandleOf<T>()
	{
		return new TypeHandle((void*)RuntimeTypeHandle.ToIntPtr(typeof(T).TypeHandle));
	}

	public unsafe static bool AreSameType(TypeHandle left, TypeHandle right)
	{
		return left.m_asTAddr == right.m_asTAddr;
	}

	public unsafe int GetCorElementType()
	{
		return GetCorElementType(m_asTAddr);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool CanCastTo(TypeHandle destTH)
	{
		return TryCanCastTo(this, destTH) switch
		{
			CastResult.CanCast => true, 
			CastResult.CannotCast => false, 
			_ => CanCastToWorker(this, destTH, nullableCast: false), 
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CanCastToForReflection(TypeHandle srcTH, TypeHandle destTH)
	{
		return TryCanCastTo(srcTH, destTH) switch
		{
			CastResult.CanCast => true, 
			CastResult.CannotCast => false, 
			_ => CanCastToWorker(srcTH, destTH, nullableCast: true), 
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static CastResult TryCanCastTo(TypeHandle srcTH, TypeHandle destTH)
	{
		if (srcTH.m_asTAddr == destTH.m_asTAddr)
		{
			return CastResult.CanCast;
		}
		if (!srcTH.IsTypeDesc && destTH.IsTypeDesc)
		{
			return CastResult.CannotCast;
		}
		return CastCache.TryGet(CastHelpers.s_table, (nuint)srcTH.m_asTAddr, (nuint)destTH.m_asTAddr);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe static bool CanCastToWorker(TypeHandle srcTH, TypeHandle destTH, bool nullableCast)
	{
		if (!srcTH.IsTypeDesc && !destTH.IsTypeDesc && CastHelpers.IsNullableForType(destTH.AsMethodTable(), srcTH.AsMethodTable()))
		{
			return nullableCast;
		}
		return CanCastTo_NoCacheLookup(srcTH.m_asTAddr, destTH.m_asTAddr) != Interop.BOOL.FALSE;
	}

	[DllImport("QCall", EntryPoint = "TypeHandle_CanCastTo_NoCacheLookup", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "TypeHandle_CanCastTo_NoCacheLookup")]
	private unsafe static extern Interop.BOOL CanCastTo_NoCacheLookup(void* fromTypeHnd, void* toTypeHnd);

	[DllImport("QCall", EntryPoint = "TypeHandle_GetCorElementType", ExactSpelling = true)]
	[SuppressGCTransition]
	[LibraryImport("QCall", EntryPoint = "TypeHandle_GetCorElementType")]
	private unsafe static extern int GetCorElementType(void* typeHnd);
}
