using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Dungeon;

public class ThoughtBubble
{
	public static ThoughtBubble LTrigger = new ThoughtBubble(0);

	public static ThoughtBubble InventoryFull = new ThoughtBubble(2);

	private static Texture2D Thought;

	private Rectangle srcRect = new Rectangle(0, 0, 72, 80);

	private static readonly Vector2 origin = new Vector2(36f, 40f);

	private static readonly Vector2 bubbleOffset = new Vector2(0f, -72f);

	private ThoughtBubble(int index)
	{
		srcRect = new Rectangle(index * 72, 0, 72, 80);
	}

	public static void LoadContent(ContentManager content)
	{
		Thought = content.Load<Texture2D>("Sprites\\Dungeon\\Thought");
	}

	public void Draw(SpriteBatch spriteBatch, Vector2 offset, float scale)
	{
		spriteBatch.Draw(Thought, offset + bubbleOffset * scale, srcRect, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
	}
}
