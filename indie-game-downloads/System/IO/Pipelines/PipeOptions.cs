using System.Buffers;

namespace System.IO.Pipelines;

public class PipeOptions
{
	public static PipeOptions Default { get; } = new PipeOptions(null, null, null, -1L, -1L);

	public bool UseSynchronizationContext { get; }

	public long PauseWriterThreshold { get; }

	public long ResumeWriterThreshold { get; }

	public int MinimumSegmentSize { get; }

	public PipeScheduler WriterScheduler { get; }

	public PipeScheduler ReaderScheduler { get; }

	public MemoryPool<byte> Pool { get; }

	internal bool IsDefaultSharedMemoryPool { get; }

	internal int InitialSegmentPoolSize { get; }

	internal int MaxSegmentPoolSize { get; }

	public PipeOptions(MemoryPool<byte>? pool = null, PipeScheduler? readerScheduler = null, PipeScheduler? writerScheduler = null, long pauseWriterThreshold = -1L, long resumeWriterThreshold = -1L, int minimumSegmentSize = -1, bool useSynchronizationContext = true)
	{
		MinimumSegmentSize = ((minimumSegmentSize == -1) ? 4096 : minimumSegmentSize);
		InitialSegmentPoolSize = 4;
		MaxSegmentPoolSize = 256;
		if (pauseWriterThreshold == -1)
		{
			pauseWriterThreshold = 65536L;
		}
		else if (pauseWriterThreshold < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.pauseWriterThreshold);
		}
		switch (resumeWriterThreshold)
		{
		case -1L:
			resumeWriterThreshold = 32768L;
			break;
		case 0L:
			resumeWriterThreshold = 1L;
			break;
		}
		if (resumeWriterThreshold < 0 || (pauseWriterThreshold > 0 && resumeWriterThreshold > pauseWriterThreshold))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.resumeWriterThreshold);
		}
		Pool = pool ?? MemoryPool<byte>.Shared;
		IsDefaultSharedMemoryPool = Pool == MemoryPool<byte>.Shared;
		ReaderScheduler = readerScheduler ?? PipeScheduler.ThreadPool;
		WriterScheduler = writerScheduler ?? PipeScheduler.ThreadPool;
		PauseWriterThreshold = pauseWriterThreshold;
		ResumeWriterThreshold = resumeWriterThreshold;
		UseSynchronizationContext = useSynchronizationContext;
	}
}
