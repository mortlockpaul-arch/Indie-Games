using System.Collections.Generic;

namespace FarseerGames.FarseerPhysics;

public class Pool<T> : Stack<T> where T : new()
{
	public Pool()
	{
	}

	public Pool(int size)
	{
		for (int i = 0; i < size; i++)
		{
			Push(new T());
		}
	}

	public T Fetch()
	{
		if (base.Count > 0)
		{
			return Pop();
		}
		return new T();
	}

	public void Insert(T item)
	{
		Push(item);
	}
}
