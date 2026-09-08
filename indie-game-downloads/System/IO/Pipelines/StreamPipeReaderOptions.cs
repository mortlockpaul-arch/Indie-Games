using System.Buffers;

namespace System.IO.Pipelines;

public class StreamPipeReaderOptions
{
	internal const int DefaultMaxBufferSize = 2097152;

	internal static readonly StreamPipeReaderOptions s_default = new StreamPipeReaderOptions();

	public int BufferSize { get; }

	internal int MaxBufferSize { get; } = 2097152;

	public int MinimumReadSize { get; }

	public MemoryPool<byte> Pool { get; }

	public bool LeaveOpen { get; }

	public bool UseZeroByteReads { get; }

	internal bool IsDefaultSharedMemoryPool { get; }

	public StreamPipeReaderOptions(MemoryPool<byte>? pool, int bufferSize, int minimumReadSize, bool leaveOpen)
		: this(pool, bufferSize, minimumReadSize, leaveOpen, false)
	{
	}

	public StreamPipeReaderOptions(MemoryPool<byte>? pool = null, int bufferSize = -1, int minimumReadSize = -1, bool leaveOpen = false, bool useZeroByteReads = false)
	{
		Pool = pool ?? MemoryPool<byte>.Shared;
		IsDefaultSharedMemoryPool = Pool == MemoryPool<byte>.Shared;
		int num;
		if (bufferSize != -1)
		{
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			num = bufferSize;
		}
		else
		{
			num = 4096;
		}
		BufferSize = num;
		int num2;
		if (minimumReadSize != -1)
		{
			if (minimumReadSize <= 0)
			{
				throw new ArgumentOutOfRangeException("minimumReadSize");
			}
			num2 = minimumReadSize;
		}
		else
		{
			num2 = 1024;
		}
		MinimumReadSize = num2;
		LeaveOpen = leaveOpen;
		UseZeroByteReads = useZeroByteReads;
	}
}
