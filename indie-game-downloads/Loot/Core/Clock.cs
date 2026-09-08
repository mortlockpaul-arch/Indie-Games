using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Core;

public static class Clock
{
	private static Vector2 centerOffset = new Vector2(-72f, 0f);

	public static void DrawCentered(SpriteBatch spriteBatch, Vector2 v, TimeSpan t, Color color)
	{
		Draw(spriteBatch, v + centerOffset, t, color);
	}

	public static void Draw(SpriteBatch spriteBatch, Vector2 v, TimeSpan t, Color color)
	{
		if (t.Hours < 10)
		{
			Text.Draw(spriteBatch, ref v, '0', color);
		}
		Text.Draw(spriteBatch, ref v, t.Hours, color);
		v.X -= 6f;
		Text.Draw(spriteBatch, ref v, ':', color);
		v.X -= 6f;
		if (t.Minutes < 10)
		{
			Text.Draw(spriteBatch, ref v, '0', color);
		}
		Text.Draw(spriteBatch, ref v, t.Minutes, color);
		v.X -= 6f;
		Text.Draw(spriteBatch, ref v, ':', color);
		v.X -= 6f;
		if (t.Seconds < 10)
		{
			Text.Draw(spriteBatch, ref v, '0', color);
		}
		Text.Draw(spriteBatch, ref v, t.Seconds, color);
	}
}
