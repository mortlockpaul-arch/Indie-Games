using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Viking_x86.loader;

public class Loader
{
	private float frame;

	public void Update()
	{
		frame += Game1.frameTime;
	}

	public void Draw()
	{
		Draw(0f);
	}

	public void Draw(float y)
	{
		SpriteTools.BeginAdditive();
		for (int i = 0; i < 8; i++)
		{
			float num = (float)i * 0.785f + frame * 2f;
			SpriteTools.sprite.Draw(Game1.nullTex, VScroll.screenSize / 2f + new Vector2((float)Math.Cos(num), (float)Math.Sin(num)) * 16f + new Vector2(0f, y), new Rectangle(0, 0, 1, 1), Color.White, 0f, new Vector2(0.5f, 0.5f), 4f, SpriteEffects.None, 1f);
		}
		SpriteTools.End();
	}
}
