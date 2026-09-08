namespace System.Runtime.InteropServices;

public readonly struct HandleRef(object? wrapper, nint handle)
{
	private readonly object _wrapper = wrapper;

	private readonly nint _handle = handle;

	public object? Wrapper => _wrapper;

	public nint Handle => _handle;

	public static explicit operator nint(HandleRef value)
	{
		return value._handle;
	}

	public static nint ToIntPtr(HandleRef value)
	{
		return value._handle;
	}
}
