using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Manic_Miner_360;

public static class SpriteBatchExtensions
{
	public static void DrawCenteredString(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 pos, float length)
	{
		spriteBatch.DrawCenteredString(spriteFont, text, pos, length, Color.White);
	}

	public static void DrawCenteredString(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 pos, float length, Color color)
	{
		Vector2 vector = spriteFont.MeasureString(text);
		Vector2 vector2 = new Vector2((pos.X + length) / 2f, pos.Y);
		spriteBatch.DrawString(spriteFont, text, vector2 - vector / 2f, color);
	}
}
