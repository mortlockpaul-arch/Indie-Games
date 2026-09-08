using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[StackTraceHidden]
[DebuggerStepThrough]
internal static class VirtualDispatchHelpers
{
	private unsafe struct VirtualResolutionData(MethodTable* objectMethodTable, nint classHandle, nint methodHandle) : IEquatable<VirtualResolutionData>
	{
		public unsafe int _hashCode = (int)objectMethodTable + (int)BitOperations.RotateLeft((uint)classHandle, 5) + (int)BitOperations.RotateRight((uint)methodHandle, 5);

		public unsafe MethodTable* _objectMethodTable = objectMethodTable;

		public nint _classHandle = classHandle;

		public nint _methodHandle = methodHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool Equals(VirtualResolutionData other)
		{
			if (_hashCode == other._hashCode)
			{
				return ((nuint)((byte*)_objectMethodTable - (nuint)other._objectMethodTable) | (nuint)(_classHandle - other._classHandle) | (nuint)(_methodHandle - other._methodHandle)) == 0;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is VirtualResolutionData other)
			{
				return Equals(other);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return _hashCode;
		}
	}

	private struct VirtualFunctionPointerArgs
	{
		public nint classHnd;

		public nint methodHnd;
	}

	private static GenericCache<VirtualResolutionData, nint> s_virtualFunctionPointerCache = new GenericCache<VirtualResolutionData, nint>(128, 8192);

	[DllImport("QCall", ExactSpelling = true)]
	[LibraryImport("QCall")]
	private static extern nint ResolveVirtualFunctionPointer(ObjectHandleOnStack obj, nint classHandle, nint methodHandle);

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerHidden]
	private unsafe static nint VirtualFunctionPointerSlow(object obj, nint classHandle, nint methodHandle)
	{
		nint num = ResolveVirtualFunctionPointer(ObjectHandleOnStack.Create(ref obj), classHandle, methodHandle);
		s_virtualFunctionPointerCache.TrySet(new VirtualResolutionData(RuntimeHelpers.GetMethodTable(obj), classHandle, methodHandle), num);
		GC.KeepAlive(obj);
		return num;
	}

	[DebuggerHidden]
	private unsafe static nint VirtualFunctionPointer(object obj, nint classHandle, nint methodHandle)
	{
		if (s_virtualFunctionPointerCache.TryGet(new VirtualResolutionData(RuntimeHelpers.GetMethodTable(obj), classHandle, methodHandle), out var value))
		{
			return value;
		}
		return VirtualFunctionPointerSlow(obj, classHandle, methodHandle);
	}

	[DebuggerHidden]
	private unsafe static nint VirtualFunctionPointer_Dynamic(object obj, ref VirtualFunctionPointerArgs virtualFunctionPointerArgs)
	{
		nint classHnd = virtualFunctionPointerArgs.classHnd;
		nint methodHnd = virtualFunctionPointerArgs.methodHnd;
		if (s_virtualFunctionPointerCache.TryGet(new VirtualResolutionData(RuntimeHelpers.GetMethodTable(obj), classHnd, methodHnd), out var value))
		{
			return value;
		}
		return VirtualFunctionPointerSlow(obj, classHnd, methodHnd);
	}
}
