using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Kobingo.Xna.Library;

public static class Manager
{
	public static Manager<T> Create<T>(int count) where T : Actor, new()
	{
		T[] array = new T[count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new T();
		}
		return new Manager<T>(array);
	}
}
public class Manager<T> : Actor, IEnumerable<T>, IEnumerable where T : Actor
{
	protected List<T> Actors { get; private set; }

	public IEnumerable<T> Active
	{
		get
		{
			for (int i = 0; i < Actors.Count; i++)
			{
				T val = Actors[i];
				if (val.IsActive)
				{
					yield return Actors[i];
				}
			}
		}
	}

	public Manager(params T[] gameObjects)
	{
		Actors = new List<T>();
		Initialize(gameObjects);
	}

	public void Initialize(params T[] gameObjects)
	{
		for (int i = 0; i < gameObjects.Length; i++)
		{
			Actors.Add(gameObjects[i]);
		}
	}

	protected override void DoUpdate(GameTime gameTime)
	{
		foreach (T item in Active)
		{
			T current = item;
			current.Update(gameTime);
		}
	}

	protected override void DoDraw(GameTime gameTime)
	{
		foreach (T item in Active)
		{
			T current = item;
			current.Draw(gameTime);
		}
	}

	public virtual TGameObject GetObject<TGameObject>() where TGameObject : T
	{
		foreach (T actor in Actors)
		{
			T current = actor;
			if (!current.IsActive && current is TGameObject)
			{
				return (TGameObject)current;
			}
		}
		return null;
	}

	public virtual T GetObject()
	{
		foreach (T actor in Actors)
		{
			T current = actor;
			if (!current.IsActive)
			{
				return current;
			}
		}
		return null;
	}

	public void Clear()
	{
		foreach (T item in Active)
		{
			T current = item;
			current.Deactivate();
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		for (int i = 0; i < Actors.Count; i++)
		{
			T val = Actors[i];
			if (val.IsActive)
			{
				yield return Actors[i];
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		for (int i = 0; i < Actors.Count; i++)
		{
			T val = Actors[i];
			if (val.IsActive)
			{
				yield return Actors[i];
			}
		}
	}
}
