using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEdit.glows;

public class GlowList
{
	private Glow[] glow;

	public int totalGlows;

	public int idx;

	public GlowList(int idx)
	{
		this.idx = idx;
		glow = new Glow[128];
		for (int i = 0; i < glow.Length; i++)
		{
			glow[i] = new Glow();
		}
	}

	public void AddGlow(Vector2 loc, float r, float g, float b, float size)
	{
		if (totalGlows < glow.Length)
		{
			glow[totalGlows].Init(loc, r, g, b, size);
			totalGlows++;
		}
	}

	public void Draw(Texture2D xt)
	{
		for (int i = 0; i < totalGlows; i++)
		{
			glow[i].Draw(xt, idx);
		}
		totalGlows = 0;
	}
}
