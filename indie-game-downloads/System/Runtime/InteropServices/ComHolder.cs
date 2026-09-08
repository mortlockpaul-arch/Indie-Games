namespace System.Runtime.InteropServices;

internal readonly struct ComHolder : IDisposable
{
	private readonly nint _ptr;

	internal nint Ptr => _ptr;

	public ComHolder(nint ptr)
	{
		_ptr = ptr;
	}

	public void Dispose()
	{
		Marshal.Release(_ptr);
	}
}
