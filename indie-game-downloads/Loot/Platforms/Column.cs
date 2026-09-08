using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Platforms;

public struct Column
{
	public int height;

	public int gold;

	public bool collected;

	private int style;

	private static int styleCounter = 0;

	private float goldTextAlpha;

	private float goldTextOffset;

	private static Sprite[] platform;

	private static Sprite platformLeft;

	private static Sprite platformRight;

	private static Sprite platformBg;

	private static Sprite platformLeftBg;

	private static Sprite platformRightBg;

	private static Color platformBgColor = new Color(0.63f, 0.63f, 0.63f);

	private static Color goldTextColor = new Color(255, 255, 51);

	private static Color goldTextShadowColor = new Color(204, 153, 0);

	public Column(int columnHeight)
	{
		height = columnHeight;
		gold = 0;
		collected = false;
		goldTextAlpha = 1f;
		goldTextOffset = 0f;
		style = styleCounter;
		styleCounter += DM.Random.Next(5) + 1;
		if (styleCounter > 5)
		{
			styleCounter -= 6;
		}
	}

	public static void LoadContent(ContentManager content)
	{
		Texture2D texture = content.Load<Texture2D>("Sprites\\Platformer\\Tiles");
		platformLeft = new StillSprite(texture, new Rectangle(0, 0, 64, 512), new Vector2(32f, 32f));
		platform = new Sprite[6];
		for (int i = 0; i < platform.Length; i++)
		{
			platform[i] = new StillSprite(texture, new Rectangle((i + 1) * 64, 0, 64, 512), new Vector2(32f, 32f));
		}
		platformRight = new StillSprite(texture, new Rectangle(448, 0, 64, 512), new Vector2(32f, 32f));
		Texture2D texture2 = content.Load<Texture2D>("Sprites\\Platformer\\TilesBg");
		platformLeftBg = new StillSprite(texture2, new Rectangle(0, 0, 64, 512), new Vector2(32f, 32f));
		platformBg = new StillSprite(texture2, new Rectangle(64, 0, 64, 512), new Vector2(32f, 32f));
		platformRightBg = new StillSprite(texture2, new Rectangle(128, 0, 64, 512), new Vector2(32f, 32f));
	}

	public void Update(GameTime gameTime)
	{
		if (collected && gold > 0)
		{
			float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
			goldTextOffset -= 200f * num;
			goldTextAlpha -= num;
			if (!((double)goldTextAlpha >= 0.0))
			{
				goldTextAlpha = 0f;
			}
		}
	}

	public void Draw(SpriteBatch spriteBatch, float offset, Column prev, Column next)
	{
		Vector2 vector = new Vector2(offset, 720 - height * 64);
		if (prev.height == 0)
		{
			platformLeftBg.Draw(spriteBatch, vector, platformBgColor, 1f);
			platformLeft.Draw(spriteBatch, vector, Color.White, 1f);
		}
		else if (next.height == 0)
		{
			platformRightBg.Draw(spriteBatch, vector, platformBgColor, 1f);
			platformRight.Draw(spriteBatch, vector, Color.White, 1f);
		}
		else
		{
			platformBg.Draw(spriteBatch, vector, platformBgColor, 1f);
			platform[style].Draw(spriteBatch, vector, Color.White, 1f);
		}
		if (gold > 0)
		{
			if (!collected)
			{
				GPAnim.Draw(spriteBatch, vector + new Vector2(0f, -16f));
				return;
			}
			float num = (Text.Width(gold) + 72) / 2;
			Vector2 v = vector + new Vector2(0f - num, goldTextOffset);
			Text.DrawShadowString(spriteBatch, ref v, "+", goldTextColor * goldTextAlpha, goldTextShadowColor * goldTextAlpha, 1f);
			Text.DrawShadowString(spriteBatch, ref v, gold, goldTextColor * goldTextAlpha, goldTextShadowColor * goldTextAlpha, 1f);
			Text.DrawShadowString(spriteBatch, ref v, " GP", goldTextColor * goldTextAlpha, goldTextShadowColor * goldTextAlpha, 1f);
		}
	}
}
