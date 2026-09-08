namespace System.Runtime.CompilerServices;

internal ref struct ByteRefOnStack
{
	private unsafe readonly void* _pByteRef;

	private unsafe ByteRefOnStack(void* pByteRef)
	{
		_pByteRef = pByteRef;
	}

	internal unsafe static ByteRefOnStack Create(ref ByteRef byteRef)
	{
		return new ByteRefOnStack(Unsafe.AsPointer<ByteRef>(in byteRef));
	}
}
