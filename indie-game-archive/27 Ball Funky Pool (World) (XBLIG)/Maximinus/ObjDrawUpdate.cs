using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Maximinus;

public abstract class ObjDrawUpdate : ObjUpdate
{
	private static List<ObjDrawUpdate> objToDraw = new List<ObjDrawUpdate>();

	public ObjDrawUpdate()
		: this(useAutoUpdate: true, useAutoDraw: true)
	{
	}

	public ObjDrawUpdate(bool useAutoUpdate, bool useAutoDraw)
		: base(useAutoUpdate)
	{
		if (useAutoDraw)
		{
			objToDraw.Add(this);
		}
	}

	public virtual void Draw(GameTime gameTime)
	{
	}

	public static void DrawAll(GameTime gameTime)
	{
		foreach (ObjDrawUpdate item in objToDraw)
		{
			item.Draw(gameTime);
		}
	}
}
