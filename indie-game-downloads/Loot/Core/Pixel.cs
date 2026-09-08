using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Core;

public static class Pixel
{
	private static Texture2D pixel;

	public static void LoadContent(ContentManager content)
	{
		pixel = content.Load<Texture2D>("Sprites\\Pixel");
	}

	public static void Draw(SpriteBatch spriteBatch, Rectangle rect, Color color)
	{
		spriteBatch.Draw(pixel, rect, color);
	}
}
