using System;
using System.Collections.Generic;
using System.Threading;

namespace l;

internal class a<TKey, TValue>
{
	internal Dictionary<TKey, TValue> a5h = new Dictionary<TKey, TValue>();

	private Action a5b;

	public Dictionary<TKey, TValue> WrappedDictionary
	{
		get
		{
			return a5h;
		}
		set
		{
			a5h = value;
			OnChanged();
		}
	}

	public event Action Changed
	{
		add
		{
			Action action = a5b;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange(ref a5b, value2, action2);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = a5b;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange(ref a5b, value2, action2);
			}
			while ((object)action != action2);
		}
	}

	public void Add(TKey key, TValue value)
	{
		a5h.Add(key, value);
		OnChanged();
	}

	public bool Remove(TKey key)
	{
		if (a5h.Remove(key))
		{
			OnChanged();
			return true;
		}
		return false;
	}

	public void Clear()
	{
		a5h.Clear();
		OnChanged();
	}

	protected void OnChanged()
	{
		if (a5b != null)
		{
			a5b();
		}
	}
}
