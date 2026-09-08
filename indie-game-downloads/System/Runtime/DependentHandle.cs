using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Runtime;

public struct DependentHandle : IDisposable
{
	private nint _handle;

	public readonly bool IsAllocated => _handle != 0;

	public object? Target
	{
		readonly get
		{
			nint handle = _handle;
			if (handle == 0)
			{
				ThrowHelper.ThrowInvalidOperationException();
			}
			return InternalGetTarget(handle);
		}
		set
		{
			nint handle = _handle;
			if (handle == 0 || value != null)
			{
				ThrowHelper.ThrowInvalidOperationException();
			}
			InternalSetTargetToNull(handle);
		}
	}

	public object? Dependent
	{
		readonly get
		{
			nint handle = _handle;
			if (handle == 0)
			{
				ThrowHelper.ThrowInvalidOperationException();
			}
			return InternalGetDependent(handle);
		}
		set
		{
			nint handle = _handle;
			if (handle == 0)
			{
				ThrowHelper.ThrowInvalidOperationException();
			}
			InternalSetDependent(handle, value);
		}
	}

	public readonly (object? Target, object? Dependent) TargetAndDependent
	{
		get
		{
			nint handle = _handle;
			if (handle == 0)
			{
				ThrowHelper.ThrowInvalidOperationException();
			}
			object dependent;
			return (Target: InternalGetTargetAndDependent(handle, out dependent), Dependent: dependent);
		}
	}

	public DependentHandle(object? target, object? dependent)
	{
		nint num = InternalAlloc(target, dependent);
		if (num == 0)
		{
			num = InternalAllocWithGCTransition(target, dependent);
		}
		_handle = num;
	}

	internal readonly object UnsafeGetTarget()
	{
		return InternalGetTarget(_handle);
	}

	internal readonly object UnsafeGetTargetAndDependent(out object dependent)
	{
		return InternalGetTargetAndDependent(_handle, out dependent);
	}

	internal readonly void UnsafeSetTargetToNull()
	{
		InternalSetTargetToNull(_handle);
	}

	internal readonly void UnsafeSetDependent(object dependent)
	{
		InternalSetDependent(_handle, dependent);
	}

	public void Dispose()
	{
		nint handle = _handle;
		if (handle != 0)
		{
			_handle = 0;
			if (!InternalFree(handle))
			{
				InternalFreeWithGCTransition(handle);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern nint InternalAlloc(object target, object dependent);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static nint InternalAllocWithGCTransition(object target, object dependent)
	{
		return _InternalAllocWithGCTransition(ObjectHandleOnStack.Create(ref target), ObjectHandleOnStack.Create(ref dependent));
	}

	[DllImport("QCall", EntryPoint = "DependentHandle_InternalAllocWithGCTransition", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "DependentHandle_InternalAllocWithGCTransition")]
	private static extern nint _InternalAllocWithGCTransition(ObjectHandleOnStack target, ObjectHandleOnStack dependent);

	private unsafe static object InternalGetTarget(nint dependentHandle)
	{
		return Unsafe.Read<object>((void*)dependentHandle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object InternalGetDependent(nint dependentHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object InternalGetTargetAndDependent(nint dependentHandle, out object dependent);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetDependent(nint dependentHandle, object dependent);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetTargetToNull(nint dependentHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool InternalFree(nint dependentHandle);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void InternalFreeWithGCTransition(nint dependentHandle)
	{
		_InternalFreeWithGCTransition(dependentHandle);
	}

	[DllImport("QCall", EntryPoint = "DependentHandle_InternalFreeWithGCTransition", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "DependentHandle_InternalFreeWithGCTransition")]
	private static extern void _InternalFreeWithGCTransition(nint dependentHandle);
}
