using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[StructLayout(LayoutKind.Explicit)]
internal struct MethodTable
{
	[FieldOffset(0)]
	public ushort ComponentSize;

	[FieldOffset(0)]
	private uint Flags;

	[FieldOffset(4)]
	public uint BaseSize;

	[FieldOffset(14)]
	public ushort InterfaceCount;

	[FieldOffset(16)]
	public unsafe MethodTable* ParentMethodTable;

	[FieldOffset(32)]
	public unsafe MethodTableAuxiliaryData* AuxiliaryData;

	[FieldOffset(48)]
	public unsafe void* ElementType;

	[FieldOffset(48)]
	public unsafe MethodTable*** PerInstInfo;

	[FieldOffset(56)]
	public unsafe MethodTable** InterfaceMap;

	[FieldOffset(56)]
	public uint NullableValueAddrOffset;

	[FieldOffset(60)]
	public uint NullableValueSize;

	public bool HasComponentSize => (Flags & 0x80000000u) != 0;

	public bool ContainsGCPointers => (Flags & 0x1000000) != 0;

	public bool NonTrivialInterfaceCast => (Flags & 0x500C0000) != 0;

	public bool HasTypeEquivalence => (Flags & 0x2000000) != 0;

	public bool HasFinalizer => (Flags & 0x100000) != 0;

	public bool HasDefaultConstructor => (Flags & 0x80000200u) == 512;

	public bool IsSzArray
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return BaseSize == 3 * 8;
		}
	}

	public int MultiDimensionalArrayRank
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (int)((BaseSize - 3 * 8) / 8);
		}
	}

	public bool IsInterface => (Flags & 0xF0000) == 786432;

	public bool IsValueType => (Flags & 0xC0000) == 262144;

	public bool IsNullable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (Flags & 0xF0000) == 327680;
		}
	}

	public bool IsByRefLike => (Flags & 0x80001000u) == 4096;

	public bool IsPrimitive => (Flags & 0xE0000) == 393216;

	public bool IsTruePrimitive => (Flags & 0xF0000) == 458752;

	public bool IsArray => (Flags & 0xC0000) == 524288;

	public bool HasInstantiation
	{
		get
		{
			if ((Flags & 0x80000000u) == 0)
			{
				return (Flags & 0x30) != 0;
			}
			return false;
		}
	}

	public bool IsGenericTypeDefinition => (Flags & 0x80000030u) == 48;

	public bool IsConstructedGenericType
	{
		get
		{
			uint num = Flags & 0x80000030u;
			if (num != 16)
			{
				return num == 32;
			}
			return true;
		}
	}

	public bool IsSharedByGenericInstantiations => (Flags & 0x80000030u) == 32;

	public bool ContainsGenericVariables => (Flags & 0x20000000) != 0;

	internal unsafe static bool AreSameType(MethodTable* mt1, MethodTable* mt2)
	{
		return mt1 == mt2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe TypeHandle GetArrayElementTypeHandle()
	{
		return new TypeHandle(ElementType);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern uint GetNumInstanceFieldBytes();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern CorElementType GetPrimitiveCorElementType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public unsafe extern MethodTable* GetMethodTableMatchingParentClass(MethodTable* parent);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public unsafe extern MethodTable* InstantiationArg0();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint GetNullableNumInstanceFieldBytes()
	{
		return NullableValueAddrOffset + NullableValueSize;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint GetNumInstanceFieldBytesIfContainsGCPointers()
	{
		return BaseSize - 2 * 8;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern nint GetLoaderAllocatorHandle();
}
