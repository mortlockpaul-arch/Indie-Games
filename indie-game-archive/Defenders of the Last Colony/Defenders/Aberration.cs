using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

public class Aberration
{
	public Vector2 position;

	public float w;

	public float size;

	public Texture2D tx;

	public Color color;

	private float transp = 0.5f;

	public Aberration(Color color, int randomSeed)
	{
		Random random = new Random(randomSeed);
		if (random.Next(100) < 50)
		{
			w = (float)random.Next(-10, 100) / 100f;
		}
		else
		{
			w = (float)random.Next(1, 20) / -100f;
		}
		size = (float)random.Next(20, 175) / 100f;
		this.color = Color.LightCyan;
		transp = (float)random.Next(5, 50) / 100f;
	}

	public void Update(Vector2 screen, Vector2 flare, Color color)
	{
		Vector2 vector = new Vector2(screen.X - flare.X, screen.Y - flare.Y);
		position = new Vector2((1f - w) * vector.X + w * flare.X, (1f - w) * vector.Y + w * flare.Y);
		this.color = color;
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(tx, position, null, color * transp, 0f, new Vector2((float)tx.Width / 2f, (float)tx.Height / 2f), size, SpriteEffects.None, 0f);
	}
}
