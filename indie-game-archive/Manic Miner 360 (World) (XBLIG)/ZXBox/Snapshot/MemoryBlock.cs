using System.Collections.Generic;

namespace ZXBox.Snapshot;

public class MemoryBlock
{
	private int _MemoryBlockNumber;

	private List<byte> _MemoryData;

	public int MemoryBlockNumber
	{
		get
		{
			return _MemoryBlockNumber;
		}
		set
		{
			_MemoryBlockNumber = value;
		}
	}

	public List<byte> MemoryData
	{
		get
		{
			return _MemoryData;
		}
		set
		{
			_MemoryData = value;
		}
	}
}
