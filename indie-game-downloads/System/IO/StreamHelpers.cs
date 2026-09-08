namespace System.IO;

internal static class StreamHelpers
{
	public static void ValidateCopyToArgs(Stream source, Stream destination, int bufferSize)
	{
		ArgumentNullException.ThrowIfNull(destination, "destination");
		if (bufferSize <= 0)
		{
			throw new ArgumentOutOfRangeException("bufferSize", bufferSize, System.SR.ArgumentOutOfRange_NeedPosNum);
		}
		bool canRead = source.CanRead;
		if (!canRead && !source.CanWrite)
		{
			throw new ObjectDisposedException(null, System.SR.ObjectDisposed_StreamClosed);
		}
		bool canWrite = destination.CanWrite;
		if (!canWrite && !destination.CanRead)
		{
			throw new ObjectDisposedException("destination", System.SR.ObjectDisposed_StreamClosed);
		}
		if (!canRead)
		{
			throw new NotSupportedException(System.SR.NotSupported_UnreadableStream);
		}
		if (!canWrite)
		{
			throw new NotSupportedException(System.SR.NotSupported_UnwritableStream);
		}
	}
}
