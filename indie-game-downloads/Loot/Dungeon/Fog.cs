using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Dungeon;

public static class Fog
{
	private static Texture2D fogTexture;

	private static readonly Vector2 origin = new Vector2(64f, 64f);

	public static void LoadContent(ContentManager content)
	{
		fogTexture = content.Load<Texture2D>("Sprites\\Dungeon\\Fog");
	}

	public static void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
	{
		spriteBatch.Draw(fogTexture, position, null, color, 0f, origin, DungeonView.Camera.Zoom, SpriteEffects.None, 0f);
	}
}
