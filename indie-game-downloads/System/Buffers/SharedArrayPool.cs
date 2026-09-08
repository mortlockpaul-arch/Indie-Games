using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Buffers;

internal sealed class SharedArrayPool<T> : ArrayPool<T>
{
	[ThreadStatic]
	private static SharedArrayPoolThreadLocalArray[] t_tlsBuckets;

	private readonly ConditionalWeakTable<SharedArrayPoolThreadLocalArray[], object> _allTlsBuckets = new ConditionalWeakTable<SharedArrayPoolThreadLocalArray[], object>();

	private readonly SharedArrayPoolPartitions[] _buckets = new SharedArrayPoolPartitions[27];

	private bool _trimCallbackCreated;

	private int Id => GetHashCode();

	private SharedArrayPoolPartitions CreatePerCorePartitions(int bucketIndex)
	{
		SharedArrayPoolPartitions sharedArrayPoolPartitions = new SharedArrayPoolPartitions();
		return Interlocked.CompareExchange(ref _buckets[bucketIndex], sharedArrayPoolPartitions, null) ?? sharedArrayPoolPartitions;
	}

	public override T[] Rent(int minimumLength)
	{
		ArrayPoolEventSource log = ArrayPoolEventSource.Log;
		int num = Utilities.SelectBucketIndex(minimumLength);
		SharedArrayPoolThreadLocalArray[] array = t_tlsBuckets;
		T[] array2;
		if (array != null && (uint)num < (uint)array.Length)
		{
			array2 = Unsafe.As<T[]>(array[num].Array);
			if (array2 != null)
			{
				array[num].Array = null;
				if (log.IsEnabled())
				{
					log.BufferRented(array2.GetHashCode(), array2.Length, Id, num);
				}
				return array2;
			}
		}
		SharedArrayPoolPartitions[] buckets = _buckets;
		if ((uint)num < (uint)buckets.Length)
		{
			SharedArrayPoolPartitions sharedArrayPoolPartitions = buckets[num];
			if (sharedArrayPoolPartitions != null)
			{
				array2 = Unsafe.As<T[]>(sharedArrayPoolPartitions.TryPop());
				if (array2 != null)
				{
					if (log.IsEnabled())
					{
						log.BufferRented(array2.GetHashCode(), array2.Length, Id, num);
					}
					return array2;
				}
			}
			minimumLength = Utilities.GetMaxSizeForBucket(num);
		}
		else
		{
			if (minimumLength == 0)
			{
				return Array.Empty<T>();
			}
			ArgumentOutOfRangeException.ThrowIfNegative(minimumLength, "minimumLength");
		}
		array2 = GC.AllocateUninitializedArray<T>(minimumLength);
		if (log.IsEnabled())
		{
			int hashCode = array2.GetHashCode();
			log.BufferRented(hashCode, array2.Length, Id, -1);
			log.BufferAllocated(hashCode, array2.Length, Id, -1, (num >= _buckets.Length) ? ArrayPoolEventSource.BufferAllocatedReason.OverMaximumSize : ArrayPoolEventSource.BufferAllocatedReason.PoolExhausted);
		}
		return array2;
	}

	public override void Return(T[] array, bool clearArray = false)
	{
		if (array == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
		}
		int num = Utilities.SelectBucketIndex(array.Length);
		SharedArrayPoolThreadLocalArray[] array2 = t_tlsBuckets ?? InitializeTlsBucketsAndTrimming();
		bool flag = false;
		bool flag2 = true;
		if ((uint)num < (uint)array2.Length)
		{
			flag = true;
			if (clearArray)
			{
				Array.Clear(array);
			}
			if (array.Length != Utilities.GetMaxSizeForBucket(num))
			{
				throw new ArgumentException(SR.ArgumentException_BufferNotFromPool, "array");
			}
			ref SharedArrayPoolThreadLocalArray reference = ref array2[num];
			Array array3 = reference.Array;
			reference = new SharedArrayPoolThreadLocalArray(array);
			if (array3 != null)
			{
				flag2 = (_buckets[num] ?? CreatePerCorePartitions(num)).TryPush(array3);
			}
		}
		ArrayPoolEventSource log = ArrayPoolEventSource.Log;
		if (log.IsEnabled() && array.Length != 0)
		{
			log.BufferReturned(array.GetHashCode(), array.Length, Id);
			if (!(flag & flag2))
			{
				log.BufferDropped(array.GetHashCode(), array.Length, Id, flag ? num : (-1), (!flag) ? ArrayPoolEventSource.BufferDroppedReason.OverMaximumSize : ArrayPoolEventSource.BufferDroppedReason.Full);
			}
		}
	}

	public bool Trim()
	{
		int tickCount = Environment.TickCount;
		Utilities.MemoryPressure memoryPressure = Utilities.GetMemoryPressure();
		ArrayPoolEventSource log = ArrayPoolEventSource.Log;
		if (log.IsEnabled())
		{
			log.BufferTrimPoll(tickCount, (int)memoryPressure);
		}
		SharedArrayPoolPartitions[] buckets = _buckets;
		for (int i = 0; i < buckets.Length; i++)
		{
			buckets[i]?.Trim(tickCount, Id, memoryPressure);
		}
		if (memoryPressure == Utilities.MemoryPressure.High)
		{
			if (!log.IsEnabled())
			{
				foreach (KeyValuePair<SharedArrayPoolThreadLocalArray[], object> item in (IEnumerable<KeyValuePair<SharedArrayPoolThreadLocalArray[], object>>)_allTlsBuckets)
				{
					Array.Clear(item.Key);
				}
			}
			else
			{
				foreach (KeyValuePair<SharedArrayPoolThreadLocalArray[], object> item2 in (IEnumerable<KeyValuePair<SharedArrayPoolThreadLocalArray[], object>>)_allTlsBuckets)
				{
					SharedArrayPoolThreadLocalArray[] key = item2.Key;
					for (int j = 0; j < key.Length; j++)
					{
						if (Interlocked.Exchange(ref key[j].Array, null) is T[] array)
						{
							log.BufferTrimmed(array.GetHashCode(), array.Length, Id);
						}
					}
				}
			}
		}
		else
		{
			uint num = ((memoryPressure != Utilities.MemoryPressure.Medium) ? 30000u : 15000u);
			uint num2 = num;
			foreach (KeyValuePair<SharedArrayPoolThreadLocalArray[], object> item3 in (IEnumerable<KeyValuePair<SharedArrayPoolThreadLocalArray[], object>>)_allTlsBuckets)
			{
				SharedArrayPoolThreadLocalArray[] key2 = item3.Key;
				for (int k = 0; k < key2.Length; k++)
				{
					if (key2[k].Array != null)
					{
						int millisecondsTimeStamp = key2[k].MillisecondsTimeStamp;
						if (millisecondsTimeStamp == 0)
						{
							key2[k].MillisecondsTimeStamp = tickCount;
						}
						else if (tickCount - millisecondsTimeStamp >= num2 && Interlocked.Exchange(ref key2[k].Array, null) is T[] array2 && log.IsEnabled())
						{
							log.BufferTrimmed(array2.GetHashCode(), array2.Length, Id);
						}
					}
				}
			}
		}
		return true;
	}

	private SharedArrayPoolThreadLocalArray[] InitializeTlsBucketsAndTrimming()
	{
		SharedArrayPoolThreadLocalArray[] array = (t_tlsBuckets = new SharedArrayPoolThreadLocalArray[27]);
		_allTlsBuckets.Add(array, null);
		if (!Interlocked.Exchange(ref _trimCallbackCreated, value: true))
		{
			Gen2GcCallback.Register((object s) => ((SharedArrayPool<T>)s).Trim(), this);
		}
		return array;
	}
}
