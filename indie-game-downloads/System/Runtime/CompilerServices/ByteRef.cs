namespace System.Runtime.CompilerServices;

internal ref struct ByteRef
{
	private ref byte _ref;

	internal ref byte Get()
	{
		return ref _ref;
	}
}
