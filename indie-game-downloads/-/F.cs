using System;

namespace _0003;

internal class F<T> : global::_0003.t<T> where T : IDisposable, new()
{
	public override void Clear()
	{
		FreeAllTracked();
		foreach (T item in _UnusedObjectPool)
		{
			item.Dispose();
		}
		_UnusedObjectPool.Clear();
		if (_LostObjectCount > 0)
		{
			throw new Exception("Some tracked pool objects were not disposed.");
		}
	}

	public void Unload()
	{
		Clear();
	}
}
