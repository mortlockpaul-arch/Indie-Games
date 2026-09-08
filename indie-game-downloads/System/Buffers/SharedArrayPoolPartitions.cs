using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Buffers;

internal sealed class SharedArrayPoolPartitions
{
	private sealed class Partition
	{
		private readonly Array[] _arrays = new Array[SharedArrayPoolStatics.s_maxArraysPerPartition];

		private int _count;

		private int _millisecondsTimestamp;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryPush(Array array)
		{
			bool result = false;
			Monitor.Enter(this);
			Array[] arrays = _arrays;
			int count = _count;
			if ((uint)count < (uint)arrays.Length)
			{
				if (count == 0)
				{
					_millisecondsTimestamp = 0;
				}
				Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(arrays), count) = array;
				_count = count + 1;
				result = true;
			}
			Monitor.Exit(this);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Array TryPop()
		{
			Array result = null;
			Monitor.Enter(this);
			Array[] arrays = _arrays;
			int num = _count - 1;
			if ((uint)num < (uint)arrays.Length)
			{
				result = arrays[num];
				arrays[num] = null;
				_count = num;
			}
			Monitor.Exit(this);
			return result;
		}

		public void Trim(int currentMilliseconds, int id, Utilities.MemoryPressure pressure)
		{
			if (_count == 0)
			{
				return;
			}
			int num = ((pressure == Utilities.MemoryPressure.High) ? 10000 : 60000);
			lock (this)
			{
				if (_count == 0)
				{
					return;
				}
				if (_millisecondsTimestamp == 0)
				{
					_millisecondsTimestamp = currentMilliseconds;
				}
				else
				{
					if (currentMilliseconds - _millisecondsTimestamp <= num)
					{
						return;
					}
					int num2 = pressure switch
					{
						Utilities.MemoryPressure.High => SharedArrayPoolStatics.s_maxArraysPerPartition, 
						Utilities.MemoryPressure.Medium => 2, 
						_ => 1, 
					};
					ArrayPoolEventSource log = ArrayPoolEventSource.Log;
					while (_count > 0 && num2-- > 0)
					{
						Array array = _arrays[--_count];
						_arrays[_count] = null;
						if (log.IsEnabled())
						{
							log.BufferTrimmed(array.GetHashCode(), array.Length, id);
						}
					}
					_millisecondsTimestamp = ((_count > 0) ? (_millisecondsTimestamp + num / 4) : 0);
				}
			}
		}
	}

	private readonly Partition[] _partitions;

	public SharedArrayPoolPartitions()
	{
		Partition[] array = new Partition[SharedArrayPoolStatics.s_partitionCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Partition();
		}
		_partitions = array;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryPush(Array array)
	{
		Partition[] partitions = _partitions;
		int num = (int)((uint)Thread.GetCurrentProcessorId() % (uint)SharedArrayPoolStatics.s_partitionCount);
		for (int i = 0; i < partitions.Length; i++)
		{
			if (partitions[num].TryPush(array))
			{
				return true;
			}
			if (++num == partitions.Length)
			{
				num = 0;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Array TryPop()
	{
		Partition[] partitions = _partitions;
		int num = (int)((uint)Thread.GetCurrentProcessorId() % (uint)SharedArrayPoolStatics.s_partitionCount);
		for (int i = 0; i < partitions.Length; i++)
		{
			Array result;
			if ((result = partitions[num].TryPop()) != null)
			{
				return result;
			}
			if (++num == partitions.Length)
			{
				num = 0;
			}
		}
		return null;
	}

	public void Trim(int currentMilliseconds, int id, Utilities.MemoryPressure pressure)
	{
		Partition[] partitions = _partitions;
		for (int i = 0; i < partitions.Length; i++)
		{
			partitions[i].Trim(currentMilliseconds, id, pressure);
		}
	}
}
