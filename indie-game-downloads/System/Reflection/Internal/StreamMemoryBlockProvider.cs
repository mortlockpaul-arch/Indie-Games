using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace System.Reflection.Internal;

internal sealed class StreamMemoryBlockProvider : MemoryBlockProvider
{
	internal const int MemoryMapThreshold = 16384;

	private Stream _stream;

	private readonly object _streamGuard;

	private readonly bool _leaveOpen;

	private readonly bool _useMemoryMap;

	private readonly long _imageStart;

	private readonly int _imageSize;

	private MemoryMappedFile _lazyMemoryMap;

	public override int Size => _imageSize;

	public StreamMemoryBlockProvider(Stream stream, long imageStart, int imageSize, bool leaveOpen)
	{
		_stream = stream;
		_streamGuard = new object();
		_imageStart = imageStart;
		_imageSize = imageSize;
		_leaveOpen = leaveOpen;
		_useMemoryMap = stream is FileStream;
	}

	protected override void Dispose(bool disposing)
	{
		if (!_leaveOpen)
		{
			Interlocked.Exchange(ref _stream, null)?.Dispose();
		}
		Interlocked.Exchange(ref _lazyMemoryMap, null)?.Dispose();
	}

	internal unsafe static NativeHeapMemoryBlock ReadMemoryBlockNoLock(Stream stream, long start, int size)
	{
		NativeHeapMemoryBlock nativeHeapMemoryBlock = new NativeHeapMemoryBlock(size);
		bool flag = true;
		try
		{
			stream.Seek(start, SeekOrigin.Begin);
			stream.ReadExactly(nativeHeapMemoryBlock.Pointer, size);
			flag = false;
		}
		finally
		{
			if (flag)
			{
				nativeHeapMemoryBlock.Dispose();
			}
		}
		return nativeHeapMemoryBlock;
	}

	public override bool TryGetUnderlyingStream([NotNullWhen(true)] out Stream? stream, out long imageStart, out int imageSize, [NotNullWhen(true)] out object? streamGuard)
	{
		stream = _stream;
		imageStart = _imageStart;
		imageSize = _imageSize;
		streamGuard = _streamGuard;
		return true;
	}

	protected override AbstractMemoryBlock GetMemoryBlockImpl(int start, int size)
	{
		long start2 = _imageStart + start;
		if (_useMemoryMap && size > 16384)
		{
			return CreateMemoryMappedFileBlock(start2, size);
		}
		lock (_streamGuard)
		{
			return ReadMemoryBlockNoLock(_stream, start2, size);
		}
	}

	private MemoryMappedFileBlock CreateMemoryMappedFileBlock(long start, int size)
	{
		if (_lazyMemoryMap == null)
		{
			lock (_streamGuard)
			{
				try
				{
					if (_lazyMemoryMap == null)
					{
						_lazyMemoryMap = MemoryMappedFile.CreateFromFile((FileStream)_stream, null, 0L, MemoryMappedFileAccess.Read, HandleInheritability.None, leaveOpen: true);
					}
				}
				catch (UnauthorizedAccessException ex)
				{
					throw new IOException(ex.Message, ex);
				}
			}
		}
		MemoryMappedViewAccessor memoryMappedViewAccessor;
		lock (_streamGuard)
		{
			memoryMappedViewAccessor = _lazyMemoryMap.CreateViewAccessor(start, size, MemoryMappedFileAccess.Read);
		}
		return new MemoryMappedFileBlock(memoryMappedViewAccessor, memoryMappedViewAccessor.SafeMemoryMappedViewHandle, memoryMappedViewAccessor.PointerOffset, size);
	}
}
