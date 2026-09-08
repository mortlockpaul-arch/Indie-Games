namespace System.Buffers;

internal struct SharedArrayPoolThreadLocalArray(Array array)
{
	public Array Array = array;

	public int MillisecondsTimeStamp = 0;
}
