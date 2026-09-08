using System;
using System.Collections.Generic;

namespace Quasar.Global;

public abstract class Manager<T> : IDisposable where T : class, IDisposable
{
	protected Dictionary<string, T> items = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);

	protected T defaultItem = null;

	private object lockObject = new object();

	public T DefaultItem => defaultItem;

	public T this[string name]
	{
		get
		{
			T value = defaultItem;
			if (items.TryGetValue(name, out value))
			{
				return value;
			}
			try
			{
				value = LoadItem(name);
			}
			catch
			{
				value = DefaultItem;
			}
			lock (lockObject)
			{
				if (!items.ContainsKey(name))
				{
					items.Add(name, value);
				}
			}
			return value;
		}
	}

	protected Manager()
	{
		defaultItem = createDefaultItem();
		Engine.RegisterDisposeHandler(Dispose);
	}

	protected abstract T createDefaultItem();

	protected abstract T LoadItem(string name);

	public void Add(string name, T item)
	{
		if (!TryGetValue(name, out var _))
		{
			items.Add(name, item);
		}
	}

	public bool TryGetValue(string name, out T item)
	{
		return items.TryGetValue(name, out item);
	}

	public virtual void Dispose()
	{
		if (items != null)
		{
			foreach (T value in items.Values)
			{
				T current = value;
				if (current != defaultItem)
				{
					current.Dispose();
				}
			}
			defaultItem = null;
			items.Clear();
		}
		GC.SuppressFinalize(this);
	}

	~Manager()
	{
		Dispose();
	}
}
