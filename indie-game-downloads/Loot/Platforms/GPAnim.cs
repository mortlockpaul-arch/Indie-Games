using System;
using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Platforms;

public class GPAnim
{
	private static Sprite gpAnim;

	public static void LoadContent(ContentManager content)
	{
		gpAnim = new AnimatedSprite(content.Load<Texture2D>("Sprites\\Platformer\\GP"), new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 8, TimeSpan.FromMilliseconds(50.0));
	}

	public static void Update(GameTime gameTime)
	{
		gpAnim.Update(gameTime);
	}

	public static void Draw(SpriteBatch spriteBatch, Vector2 v)
	{
		gpAnim.Draw(spriteBatch, v);
	}
}
