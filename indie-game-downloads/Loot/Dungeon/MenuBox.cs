using Eyehook.Framework;
using Loot.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Dungeon;

public static class MenuBox
{
	private static Texture2D boxTitle;

	private static StretchyTexture box;

	private static Color titleTextColor = new Color(151, 100, 0);

	private static Color titleTextShadowColor = new Color(255, 255, 0);

	private static Vector2 shadowOffset = new Vector2(2f, 2f);

	public static void LoadContent(ContentManager content)
	{
		boxTitle = content.Load<Texture2D>("Sprites\\UI\\MenuBoxTitle");
		box = new StretchyTexture(content.Load<Texture2D>("Sprites\\UI\\MenuBox"), new Rectangle(16, 16, 96, 96));
	}

	public static void Draw(SpriteBatch spriteBatch, Rectangle rect, string title)
	{
		drawBox(spriteBatch, rect, title);
	}

	public static void Draw(SpriteBatch spriteBatch, Rectangle rect, string title, FormattedText text)
	{
		drawBox(spriteBatch, rect, title);
		Text.Draw(spriteBatch, new Vector2(rect.X + 40, rect.Y + 68), text, Color.Black);
	}

	private static void drawBox(SpriteBatch spriteBatch, Rectangle rect, string title)
	{
		box.Draw(spriteBatch, rect);
		if (title != null)
		{
			int num = Text.Width(title) + 40;
			int num2 = 640 - num / 2 - 128;
			int y = rect.Top - 76;
			spriteBatch.Draw(boxTitle, new Rectangle(num2, y, 128, 128), new Rectangle(0, 0, 128, 128), Color.White);
			spriteBatch.Draw(boxTitle, new Rectangle(num2 + 128, y, num, 128), new Rectangle(192, 0, 128, 128), Color.White);
			spriteBatch.Draw(boxTitle, new Rectangle(num2 + 128 + num, y, 128, 128), new Rectangle(384, 0, 128, 128), Color.White);
			Text.DrawCentered(spriteBatch, new Vector2((rect.Left + rect.Right) / 2, rect.Top - 12) + shadowOffset, title, titleTextShadowColor);
			Text.DrawCentered(spriteBatch, new Vector2((rect.Left + rect.Right) / 2, rect.Top - 12), title, titleTextColor);
		}
	}
}
