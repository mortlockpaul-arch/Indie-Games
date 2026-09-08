using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Net;

internal class SafeHandleCache<TKey, THandle> where TKey : IEquatable<TKey> where THandle : SafeHandle, ISafeHandleCachable
{
	private readonly ConcurrentDictionary<TKey, THandle> _cache = new ConcurrentDictionary<TKey, THandle>();

	internal THandle GetOrCreate<TContext>(TKey key, Func<TContext, THandle> factory, TContext factoryContext)
	{
		if (_cache.TryGetValue(key, out var value) && value.TryAddRentCount())
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"Found cached {value}.", "GetOrCreate");
			}
			return value;
		}
		value = factory(factoryContext);
		value.TryAddRentCount();
		THandle orAdd;
		do
		{
			orAdd = _cache.GetOrAdd(key, value);
		}
		while (orAdd != value && !orAdd.TryAddRentCount());
		if (orAdd != value)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"Discarding {value} (preferring cached {orAdd}).", "GetOrCreate");
			}
			value.Dispose();
			value.Dispose();
			return orAdd;
		}
		CheckForCleanup();
		return value;
	}

	private void CheckForCleanup()
	{
		int count = _cache.Count;
		if (count % 32 != 0)
		{
			return;
		}
		lock (_cache)
		{
			if (_cache.Count < count)
			{
				return;
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"Current size: {_cache.Count}.", "CheckForCleanup");
			}
			foreach (KeyValuePair<TKey, THandle> item in _cache)
			{
				item.Deconstruct(out var key, out var value);
				TKey key2 = key;
				THandle val = value;
				if (val.TryMarkForDispose())
				{
					if (System.Net.NetEventSource.Log.IsEnabled())
					{
						System.Net.NetEventSource.Info(this, $"Evicting cached {val}.", "CheckForCleanup");
					}
					_cache.TryRemove(key2, out value);
					val.Dispose();
				}
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"New size: {_cache.Count}.", "CheckForCleanup");
			}
		}
	}
}
