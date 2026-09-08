using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Runtime.InteropServices;

public struct GCHandle : IEquatable<GCHandle>
{
	private nint _handle;

	public object? Target
	{
		readonly get
		{
			nint handle = _handle;
			ThrowIfInvalid(handle);
			return InternalGet(GetHandleValue(handle));
		}
		set
		{
			nint handle = _handle;
			ThrowIfInvalid(handle);
			if (IsPinned(handle) && !Marshal.IsPinnable(value))
			{
				throw new ArgumentException(SR.ArgumentException_NotIsomorphic, "value");
			}
			InternalSet(GetHandleValue(handle), value);
		}
	}

	public readonly bool IsAllocated => _handle != 0;

	internal static nint InternalAlloc(object value, GCHandleType type)
	{
		nint num = _InternalAlloc(value, type);
		if (num == 0)
		{
			num = InternalAllocWithGCTransition(value, type);
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern nint _InternalAlloc(object value, GCHandleType type);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static nint InternalAllocWithGCTransition(object value, GCHandleType type)
	{
		return _InternalAllocWithGCTransition(ObjectHandleOnStack.Create(ref value), type);
	}

	[DllImport("QCall", EntryPoint = "GCHandle_InternalAllocWithGCTransition", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "GCHandle_InternalAllocWithGCTransition")]
	private static extern nint _InternalAllocWithGCTransition(ObjectHandleOnStack value, GCHandleType type);

	internal static void InternalFree(nint handle)
	{
		if (!_InternalFree(handle))
		{
			InternalFreeWithGCTransition(handle);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool _InternalFree(nint handle);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void InternalFreeWithGCTransition(nint dependentHandle)
	{
		_InternalFreeWithGCTransition(dependentHandle);
	}

	[DllImport("QCall", EntryPoint = "GCHandle_InternalFreeWithGCTransition", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "GCHandle_InternalFreeWithGCTransition")]
	private static extern void _InternalFreeWithGCTransition(nint dependentHandle);

	internal unsafe static object InternalGet(nint handle)
	{
		return Unsafe.Read<object>((void*)handle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void InternalSet(nint handle, object value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern object InternalCompareExchange(nint handle, object value, object oldValue);

	private GCHandle(object value, GCHandleType type)
	{
		switch (type)
		{
		default:
			throw new ArgumentOutOfRangeException("type", SR.ArgumentOutOfRange_Enum);
		case GCHandleType.Pinned:
			if (!Marshal.IsPinnable(value))
			{
				throw new ArgumentException(SR.ArgumentException_NotIsomorphic, "value");
			}
			break;
		case GCHandleType.Weak:
		case GCHandleType.WeakTrackResurrection:
		case GCHandleType.Normal:
			break;
		}
		nint num = InternalAlloc(value, type);
		if (type == GCHandleType.Pinned)
		{
			num |= 1;
		}
		_handle = num;
	}

	private GCHandle(nint handle)
	{
		_handle = handle;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GCHandle Alloc(object? value)
	{
		return new GCHandle(value, GCHandleType.Normal);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GCHandle Alloc(object? value, GCHandleType type)
	{
		return new GCHandle(value, type);
	}

	public void Free()
	{
		nint handle = Interlocked.Exchange(ref _handle, IntPtr.Zero);
		ThrowIfInvalid(handle);
		InternalFree(GetHandleValue(handle));
	}

	public unsafe readonly nint AddrOfPinnedObject()
	{
		nint handle = _handle;
		ThrowIfInvalid(handle);
		if (!IsPinned(handle))
		{
			ThrowHelper.ThrowInvalidOperationException_HandleIsNotPinned();
		}
		object obj = InternalGet(GetHandleValue(handle));
		if (obj == null)
		{
			return 0;
		}
		if (RuntimeHelpers.ObjectHasComponentSize(obj))
		{
			if (obj.GetType() == typeof(string))
			{
				return (nint)Unsafe.AsPointer(in Unsafe.As<string>(obj).GetRawStringData());
			}
			return (nint)Unsafe.AsPointer(in MemoryMarshal.GetArrayDataReference(Unsafe.As<Array>(obj)));
		}
		return (nint)Unsafe.AsPointer(in obj.GetRawData());
	}

	public static explicit operator GCHandle(nint value)
	{
		return FromIntPtr(value);
	}

	public static GCHandle FromIntPtr(nint value)
	{
		ThrowIfInvalid(value);
		return new GCHandle(value);
	}

	public static explicit operator nint(GCHandle value)
	{
		return ToIntPtr(value);
	}

	public static nint ToIntPtr(GCHandle value)
	{
		return value._handle;
	}

	public override readonly int GetHashCode()
	{
		return ((IntPtr)_handle).GetHashCode();
	}

	public override readonly bool Equals([NotNullWhen(true)] object? o)
	{
		if (o is GCHandle other)
		{
			return Equals(other);
		}
		return false;
	}

	public readonly bool Equals(GCHandle other)
	{
		return _handle == other._handle;
	}

	public static bool operator ==(GCHandle a, GCHandle b)
	{
		return a._handle == b._handle;
	}

	public static bool operator !=(GCHandle a, GCHandle b)
	{
		return a._handle != b._handle;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static nint GetHandleValue(nint handle)
	{
		return new IntPtr(handle & -2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsPinned(nint handle)
	{
		return (handle & 1) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ThrowIfInvalid(nint handle)
	{
		if (handle == 0)
		{
			ThrowHelper.ThrowInvalidOperationException_HandleIsNotInitialized();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static void CheckUninitialized(nint handle)
	{
		Unsafe.Read<object>((void*)handle);
	}
}
public struct GCHandle<T> : IEquatable<GCHandle<T>>, IDisposable where T : class?
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

	public GCHandle(T target)
	{
		_handle = GCHandle.InternalAlloc(target, GCHandleType.Normal);
	}

	private GCHandle(nint handle)
	{
		_handle = handle;
	}

	public static GCHandle<T> FromIntPtr(nint value)
	{
		return new GCHandle<T>(value);
	}

	public static nint ToIntPtr(GCHandle<T> value)
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
		if (obj is GCHandle<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	public readonly bool Equals(GCHandle<T> other)
	{
		return _handle == other._handle;
	}

	public override readonly int GetHashCode()
	{
		return ((IntPtr)_handle).GetHashCode();
	}
}
