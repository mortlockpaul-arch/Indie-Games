using System.Buffers;

namespace System.Net;

internal struct MultiArrayBuffer : IDisposable
{
	private byte[][] _blocks;

	private uint _allocatedEnd;

	private uint _activeStart;

	private uint _availableStart;

	public bool IsEmpty => _activeStart == _availableStart;

	public MultiMemory ActiveMemory => new MultiMemory(_blocks, _activeStart, _availableStart - _activeStart);

	public MultiMemory AvailableMemory => new MultiMemory(_blocks, _availableStart, _allocatedEnd - _availableStart);

	public void Dispose()
	{
		_activeStart = 0u;
		_availableStart = 0u;
		if (_blocks == null)
		{
			return;
		}
		for (int i = 0; i < _blocks.Length; i++)
		{
			byte[] array = _blocks[i];
			if (array != null)
			{
				_blocks[i] = null;
				ArrayPool<byte>.Shared.Return(array);
			}
		}
		_blocks = null;
		_allocatedEnd = 0u;
	}

	public void Discard(int byteCount)
	{
		if (byteCount == ActiveMemory.Length)
		{
			DiscardAll();
			return;
		}
		uint startBlock = _activeStart / 16384;
		_activeStart += (uint)byteCount;
		uint endBlock = _activeStart / 16384;
		FreeBlocks(startBlock, endBlock);
	}

	public void DiscardAll()
	{
		uint startBlock = _activeStart / 16384;
		uint endBlock = _allocatedEnd / 16384;
		FreeBlocks(startBlock, endBlock);
		_activeStart = (_availableStart = (_allocatedEnd = 0u));
	}

	private void FreeBlocks(uint startBlock, uint endBlock)
	{
		byte[][] blocks = _blocks;
		for (uint num = startBlock; num < endBlock; num++)
		{
			byte[] array = blocks[num];
			blocks[num] = null;
			ArrayPool<byte>.Shared.Return(array);
		}
	}

	public void Commit(int byteCount)
	{
		_availableStart += (uint)byteCount;
	}

	public void EnsureAvailableSpace(int byteCount)
	{
		if (byteCount > AvailableMemory.Length)
		{
			GrowAvailableSpace(byteCount);
		}
	}

	public void GrowAvailableSpace(int byteCount)
	{
		uint num = (uint)(byteCount - AvailableMemory.Length + 16384 - 1) / 16384u;
		if (_blocks == null)
		{
			int num2;
			for (num2 = 4; num2 < num; num2 *= 2)
			{
			}
			_blocks = new byte[num2][];
		}
		else
		{
			uint num3 = _allocatedEnd / 16384;
			uint num4 = (uint)_blocks.Length;
			if (num3 + num > num4)
			{
				uint num5 = _activeStart / 16384;
				uint num6 = num3 - num5;
				uint num7 = num6 + num;
				if (num7 > num4)
				{
					while (num4 < num7)
					{
						num4 *= 2;
					}
					byte[][] array = new byte[num4][];
					_blocks.AsSpan((int)num5, (int)num6).CopyTo(array);
					_blocks = array;
				}
				else
				{
					_blocks.AsSpan((int)num5, (int)num6).CopyTo(_blocks);
					_blocks.AsSpan((int)num6, (int)num5).Clear();
				}
				uint num8 = num5 * 16384;
				_allocatedEnd -= num8;
				_activeStart -= num8;
				_availableStart -= num8;
			}
		}
		uint num9 = _allocatedEnd / 16384;
		for (uint num10 = 0u; num10 < num; num10++)
		{
			_blocks[num9++] = ArrayPool<byte>.Shared.Rent(16384);
		}
		_allocatedEnd = num9 * 16384;
	}
}
