using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEdit.glows;

public class GlowMgr
{
	public const int GLOW_FLOOR = 0;

	public const int GLOW_NORMAL = 1;

	private GlowList[] list = new GlowList[2]
	{
		new GlowList(0),
		new GlowList(1)
	};

	public void Draw(Texture2D xt)
	{
		for (int i = 0; i < list.Length; i++)
		{
			list[i].Draw(xt);
		}
	}

	internal void Add(int type, Vector2 loc, float r, float g, float b, float size)
	{
		list[type].AddGlow(loc, r, g, b, size);
	}

	internal void Draw(Texture2D xt, int idx)
	{
		list[idx].Draw(xt);
	}
}
