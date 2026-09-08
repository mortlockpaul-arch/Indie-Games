namespace System.Runtime.CompilerServices;

internal ref struct QCallTypeHandle
{
	private unsafe void* _ptr;

	private nint _handle;

	internal unsafe QCallTypeHandle(ref RuntimeType type)
	{
		_ptr = Unsafe.AsPointer(in type);
		_handle = type?.GetUnderlyingNativeHandle() ?? IntPtr.Zero;
	}

	internal unsafe QCallTypeHandle(ref RuntimeTypeHandle rth)
	{
		_ptr = Unsafe.AsPointer(in rth);
		_handle = rth.Value;
	}
}
