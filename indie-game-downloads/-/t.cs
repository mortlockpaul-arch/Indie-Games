using System.Collections.Generic;
using SynapseGaming.LightingSystem.Core;

namespace _0003;

internal class t<T> : PooledObjectFactory<T> where T : new()
{
	private List<T> _3A_0018 = new List<T>();

	public override T New()
	{
		T val = base.New();
		_3A_0018.Add(val);
		return val;
	}

	public override void Free(T obj)
	{
		_3A_0018.Remove(obj);
		base.Free(obj);
	}

	public void FreeAllTracked()
	{
		foreach (T item in _3A_0018)
		{
			base.Free(item);
		}
		_3A_0018.Clear();
	}

	public override void Clear()
	{
		FreeAllTracked();
		base.Clear();
	}
}
