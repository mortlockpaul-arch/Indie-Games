using System.Buffers;
using System.Runtime.CompilerServices;

namespace System.IO.Pipelines;

internal sealed class BufferSegment : ReadOnlySequenceSegment<byte>
{
	private IMemoryOwner<byte> _memoryOwner;

	private byte[] _array;

	private BufferSegment _next;

	private int _end;

	public int End
	{
		get
		{
			return _end;
		}
		set
		{
			_end = value;
			base.Memory = AvailableMemory.Slice(0, value);
		}
	}

	public BufferSegment? NextSegment
	{
		get
		{
			return _next;
		}
		set
		{
			base.Next = value;
			_next = value;
		}
	}

	internal object? MemoryOwner => ((object)_memoryOwner) ?? ((object)_array);

	public Memory<byte> AvailableMemory { get; private set; }

	public int Length => End;

	public int WritableBytes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return AvailableMemory.Length - End;
		}
	}

	public void SetOwnedMemory(IMemoryOwner<byte> memoryOwner)
	{
		_memoryOwner = memoryOwner;
		AvailableMemory = memoryOwner.Memory;
	}

	public void SetOwnedMemory(byte[] arrayPoolBuffer)
	{
		_array = arrayPoolBuffer;
		AvailableMemory = arrayPoolBuffer;
	}

	public void Reset()
	{
		ResetMemory();
		base.Next = null;
		base.RunningIndex = 0L;
		_next = null;
	}

	public void ResetMemory()
	{
		IMemoryOwner<byte> memoryOwner = _memoryOwner;
		if (memoryOwner != null)
		{
			_memoryOwner = null;
			memoryOwner.Dispose();
		}
		else
		{
			ArrayPool<byte>.Shared.Return(_array);
			_array = null;
		}
		base.Memory = default(ReadOnlyMemory<byte>);
		_end = 0;
		AvailableMemory = default(Memory<byte>);
	}

	public void SetNext(BufferSegment segment)
	{
		NextSegment = segment;
		segment = this;
		while (segment.Next != null)
		{
			segment.NextSegment.RunningIndex = segment.RunningIndex + segment.Length;
			segment = segment.NextSegment;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static long GetLength(BufferSegment startSegment, int startIndex, BufferSegment endSegment, int endIndex)
	{
		return endSegment.RunningIndex + (uint)endIndex - (startSegment.RunningIndex + (uint)startIndex);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static long GetLength(long startPosition, BufferSegment endSegment, int endIndex)
	{
		return endSegment.RunningIndex + (uint)endIndex - startPosition;
	}
}
