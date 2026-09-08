using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices;

public struct WeakGCHandle<T> : IEquatable<WeakGCHandle<T>>, IDisposable where T : class?
{
	private nint _handle;

	public readonly bool IsAllocated => _handle != IntPtr.Zero;

	public WeakGCHandle(T target, bool trackResurrection = false)
	{
		_handle = GCHandle.InternalAlloc(target, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
	}

	private WeakGCHandle(nint handle)
	{
		_handle = handle;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool TryGetTarget([NotNullWhen(true)] out T? target)
	{
		nint handle = _handle;
		GCHandle.CheckUninitialized(handle);
		return (target = Unsafe.As<T>(GCHandle.InternalGet(handle))) != null;
	}

	public readonly void SetTarget(T target)
	{
		nint handle = _handle;
		GCHandle.CheckUninitialized(handle);
		GCHandle.InternalSet(handle, target);
	}

	public static WeakGCHandle<T> FromIntPtr(nint value)
	{
		return new WeakGCHandle<T>(value);
	}

	public static nint ToIntPtr(WeakGCHandle<T> value)
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
		if (obj is WeakGCHandle<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	public readonly bool Equals(WeakGCHandle<T> other)
	{
		return _handle == other._handle;
	}

	public override readonly int GetHashCode()
	{
		return ((IntPtr)_handle).GetHashCode();
	}
}
