using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hammer;

public class Primitive2D
{
	private Texture2D pixel;

	public Primitive2D(GraphicsDevice GraphicsDevice)
	{
		fillPixel(GraphicsDevice);
	}

	public void fillPixel(GraphicsDevice GraphicsDevice)
	{
		pixel = new Texture2D(GraphicsDevice, 1, 1);
		Color[] data = new Color[1] { Color.White };
		pixel.SetData(data);
	}

	public void DrawPixel(SpriteBatch sb, Rectangle rec, Color col, float layer)
	{
		sb.Draw(pixel, rec, null, col, 0f, Vector2.Zero, SpriteEffects.None, layer);
	}

	public void DrawPixel(SpriteBatch sb, Rectangle rec, Color col)
	{
		sb.Draw(pixel, rec, col);
	}

	public void drawLine(SpriteBatch sb, Vector2 ini, Vector2 fin, float size, Color col)
	{
		float x = Vector2.Distance(ini, fin);
		Vector2 vector = fin - ini;
		float rotation = (float)Math.Atan2(vector.Y, vector.X);
		sb.Draw(pixel, ini, null, col, rotation, new Vector2(0f, size / 2f), new Vector2(x, size), SpriteEffects.None, 0.1f);
	}

	public void DrawRectangle(SpriteBatch sb, Rectangle rec, Color col, int width)
	{
		sb.Draw(pixel, new Rectangle(rec.Left + width / 2, rec.Top, rec.Width, width), col);
		sb.Draw(pixel, new Rectangle(rec.Left + width / 2, rec.Top, width, rec.Height), col);
		sb.Draw(pixel, new Rectangle(rec.Right - width / 2, rec.Top, width, rec.Height), col);
		sb.Draw(pixel, new Rectangle(rec.Left + width / 2, rec.Bottom, rec.Width, width), col);
	}
}
