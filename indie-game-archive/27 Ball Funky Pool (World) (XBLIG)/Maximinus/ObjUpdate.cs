using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Maximinus;

public abstract class ObjUpdate
{
	private static List<ObjUpdate> objToUpdate = new List<ObjUpdate>();

	public ObjUpdate()
		: this(useAutoUpdate: true)
	{
	}

	public ObjUpdate(bool useAutoUpdate)
	{
		if (useAutoUpdate)
		{
			objToUpdate.Add(this);
		}
	}

	public abstract void Update(GameTime gameTime);

	public virtual void StartOfFrame(GameTime gameTime)
	{
	}

	public static void StartOfFrameAll(GameTime gameTime)
	{
		foreach (ObjUpdate item in objToUpdate)
		{
			item.StartOfFrame(gameTime);
		}
	}

	public static void UpdateAll(GameTime gameTime)
	{
		foreach (ObjUpdate item in objToUpdate)
		{
			item.Update(gameTime);
		}
	}
}
