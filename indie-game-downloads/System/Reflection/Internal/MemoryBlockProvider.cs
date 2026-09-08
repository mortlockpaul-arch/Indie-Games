using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace System.Reflection.Internal;

internal abstract class MemoryBlockProvider : IDisposable
{
	public abstract int Size { get; }

	public AbstractMemoryBlock GetMemoryBlock()
	{
		return GetMemoryBlockImpl(0, Size);
	}

	public AbstractMemoryBlock GetMemoryBlock(int start, int size)
	{
		if ((ulong)((long)(uint)start + (long)(uint)size) > (ulong)Size)
		{
			Throw.ImageTooSmallOrContainsInvalidOffsetOrCount();
		}
		return GetMemoryBlockImpl(start, size);
	}

	protected abstract AbstractMemoryBlock GetMemoryBlockImpl(int start, int size);

	public virtual bool TryGetUnderlyingStream([NotNullWhen(true)] out Stream? stream, out long imageStart, out int imageSize, [NotNullWhen(true)] out object? streamGuard)
	{
		stream = null;
		imageStart = 0L;
		imageSize = 0;
		streamGuard = null;
		return false;
	}

	protected abstract void Dispose(bool disposing);

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
