using System.Collections.Generic;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Object pool that maintains a list of unused objects
/// which are recycled to avoid allocating new objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PooledObjectFactory<T> where T : new()
{
	/// <summary />
	protected List<T> _UnusedObjectPool = new List<T>();

	/// <summary />
	protected int _LostObjectCount;

	/// <summary>
	/// Returns an existing unused object if one exists,
	/// otherwise a new object is created.
	/// </summary>
	/// <returns></returns>
	public virtual T New()
	{
		_LostObjectCount++;
		if (_UnusedObjectPool.Count < 1)
		{
			return new T();
		}
		int index = _UnusedObjectPool.Count - 1;
		T result = _UnusedObjectPool[index];
		_UnusedObjectPool.RemoveAt(index);
		return result;
	}

	/// <summary>
	/// Places an unused object back in the object pool
	/// for reuse during a later call to the New method.
	/// </summary>
	/// <param name="obj"></param>
	public virtual void Free(T obj)
	{
		_LostObjectCount--;
		_UnusedObjectPool.Add(obj);
	}

	/// <summary>
	/// Removes all objects from the object pool.
	/// </summary>
	public virtual void Clear()
	{
		_UnusedObjectPool.Clear();
	}

	/// <summary>
	/// Returns all unused objects and removes them from the
	/// object pool.  This is useful when pooling disposable
	/// objects, as the method returns all objects for manual
	/// disposal.
	/// </summary>
	public virtual void Clear(List<T> returnedobjects)
	{
		foreach (T item in _UnusedObjectPool)
		{
			returnedobjects.Add(item);
		}
		_UnusedObjectPool.Clear();
	}
}
