using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices;

public struct PinnedGCHandle<T> : IEquatable<PinnedGCHandle<T>>, IDisposable where T : class?
{
	private nint _handle;

	public readonly bool IsAllocated => _handle != IntPtr.Zero;

	public readonly T Target
	{
		get
		{
			nint handle = _handle;
			GCHandle.CheckUninitialized(handle);
			return Unsafe.As<T>(GCHandle.InternalGet(handle));
		}
		set
		{
			nint handle = _handle;
			GCHandle.CheckUninitialized(handle);
			GCHandle.InternalSet(handle, value);
		}
	}

	public PinnedGCHandle(T target)
	{
		_handle = GCHandle.InternalAlloc(target, GCHandleType.Pinned);
	}

	private PinnedGCHandle(nint handle)
	{
		_handle = handle;
	}

	[CLSCompliant(false)]
	public unsafe readonly void* GetAddressOfObjectData()
	{
		object target = Target;
		if (target == null)
		{
			return null;
		}
		return Unsafe.AsPointer(in target.GetRawData());
	}

	public static PinnedGCHandle<T> FromIntPtr(nint value)
	{
		return new PinnedGCHandle<T>(value);
	}

	public static nint ToIntPtr(PinnedGCHandle<T> value)
	{
		return value._handle;
	}

	public void Dispose()
	{
		nint handle = _handle;
		if (handle != IntPtr.Zero)
		{
			_handle = IntPtr.Zero;
			GCHandle.InternalFree(handle);
		}
	}

	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is PinnedGCHandle<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	public readonly bool Equals(PinnedGCHandle<T> other)
	{
		return _handle == other._handle;
	}

	public override readonly int GetHashCode()
	{
		return ((IntPtr)_handle).GetHashCode();
	}
}
