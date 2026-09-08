using System;

namespace Quasar.GameUtils.Logic;

public class LazyPooledReusableEntityList<T> : LazyPooledList<T> where T : class, IReusableEntity
{
	public new event Action<T> OnItemRemoved;

	public LazyPooledReusableEntityList(ICreator<T> creator, int initialPoolCapacity)
		: base(creator, initialPoolCapacity)
	{
		base.OnItemRemoved += LazyPooledReusableEntityList_OnItemRemoved;
	}

	public LazyPooledReusableEntityList(int capacity, ICreator<T> creator, int initialPoolCapacity)
		: base(capacity, creator, initialPoolCapacity)
	{
		base.OnItemRemoved += LazyPooledReusableEntityList_OnItemRemoved;
	}

	private void LazyPooledReusableEntityList_OnItemRemoved(T obj)
	{
		if (!obj.Released)
		{
			if (OnItemRemoved != null)
			{
				OnItemRemoved(obj);
			}
			obj.Release();
		}
	}
}
