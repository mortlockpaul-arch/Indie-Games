using System;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.PC;

public class PlayerSprite
{
	public static PlayerSprite Berserker;

	public static PlayerSprite Gambler;

	public static PlayerSprite Shaman;

	public static PlayerSprite Tinkerer;

	public static PlayerSprite Goblin;

	public static PlayerSprite Peasant;

	public static Sprite Levitate;

	private static AnimatedSprite body;

	private static Texture2D berserkerHead;

	private static Texture2D gamblerHead;

	private static Texture2D shamanHead;

	private static Texture2D tinkererHead;

	private static Texture2D goblinHead;

	private static Texture2D peasantHead;

	private Texture2D head;

	private static Vector2 headOrigin = new Vector2(32f, 32f);

	private Color bodyColor;

	private static readonly Color poisonedBaseColor = new Color(0, 204, 0);

	private static AnimatedSprite protectionAnimation;

	private static StillSprite protectionStill;

	private static StillSprite protectionHit;

	private bool protectIdle;

	private TimeSpan protectTimer;

	private TimeSpan protectIdleDuration = TimeSpan.FromSeconds(1.0);

	private static readonly Color invisColor = new Color(0, 0, 0) * 0.25f;

	public Color BodyColor => bodyColor;

	private PlayerSprite(Color bodyColor, Texture2D head)
	{
		this.bodyColor = bodyColor;
		this.head = head;
	}

	public static void LoadContent(ContentManager content)
	{
		body = new AnimatedSprite(content.Load<Texture2D>("Sprites\\Player\\Body"), new Rectangle(0, 0, 64, 64), null, 8, TimeSpan.FromMilliseconds(100.0));
		berserkerHead = content.Load<Texture2D>("Sprites\\Player\\BerserkerHead");
		gamblerHead = content.Load<Texture2D>("Sprites\\Player\\GamblerHead");
		shamanHead = content.Load<Texture2D>("Sprites\\Player\\ShamanHead");
		tinkererHead = content.Load<Texture2D>("Sprites\\Player\\TinkererHead");
		goblinHead = content.Load<Texture2D>("Sprites\\Player\\GoblinHead");
		peasantHead = content.Load<Texture2D>("Sprites\\Player\\PeasantHead");
		Berserker = new PlayerSprite(new Color(250, 220, 187), berserkerHead);
		Gambler = new PlayerSprite(new Color(236, 195, 152), gamblerHead);
		Shaman = new PlayerSprite(new Color(83, 55, 5), shamanHead);
		Tinkerer = new PlayerSprite(new Color(250, 220, 187), tinkererHead);
		Goblin = new PlayerSprite(new Color(198, 205, 160), goblinHead);
		Peasant = new PlayerSprite(new Color(229, 187, 156), peasantHead);
		Levitate = new StillSprite(content.Load<Texture2D>("Sprites\\Player\\Levitate"), new Rectangle(0, 0, 64, 64), new Vector2(32f, 16f));
		Texture2D texture = content.Load<Texture2D>("Sprites\\Player\\Protection");
		protectionAnimation = new AnimatedSprite(texture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 8, TimeSpan.FromMilliseconds(50.0));
		protectionStill = new StillSprite(texture, new Rectangle(0, 64, 64, 64), new Vector2(32f, 32f));
		protectionHit = new StillSprite(texture, new Rectangle(64, 64, 64, 64), new Vector2(32f, 32f));
	}

	public void Update(GameTime gameTime)
	{
		if (DM.Player.IsMoving)
		{
			body.Update(gameTime);
		}
		if (!protectIdle)
		{
			protectionAnimation.Update(gameTime);
			if (protectionAnimation.HasLooped)
			{
				protectIdle = true;
				protectTimer = TimeSpan.Zero;
			}
		}
		else
		{
			protectTimer += gameTime.ElapsedGameTime;
			if (protectTimer > protectIdleDuration)
			{
				protectIdle = false;
				protectionAnimation.Reset();
			}
		}
	}

	public void Reset()
	{
		body.Reset();
	}

	private void SetFacing()
	{
		switch (DM.Player.Facing)
		{
		case Direction.East:
			body.SubSet = 1;
			body.SpriteEffects = SpriteEffects.None;
			break;
		case Direction.West:
			body.SubSet = 1;
			body.SpriteEffects = SpriteEffects.FlipHorizontally;
			break;
		default:
			body.SubSet = 0;
			body.SpriteEffects = SpriteEffects.None;
			break;
		}
	}

	public void Draw(SpriteBatch spriteBatch, Vector2 pos, float scale)
	{
		SetFacing();
		body.Draw(spriteBatch, pos, bodyColor, scale);
		spriteBatch.Draw(head, pos, headRect(), Color.White, 0f, headOrigin, scale, SpriteEffects.None, 0f);
	}

	public void DrawHead(SpriteBatch spriteBatch, Vector2 pos, Rectangle headRect)
	{
		spriteBatch.Draw(head, pos, headRect, Color.White, 0f, headOrigin, 1f, SpriteEffects.None, 0f);
	}

	public void DrawInvis(SpriteBatch spriteBatch, Vector2 pos, float scale)
	{
		SetFacing();
		body.Draw(spriteBatch, pos, invisColor, scale);
	}

	public void DrawPoisoned(SpriteBatch spriteBatch, Vector2 pos, float scale)
	{
		Color color = Color.Lerp(bodyColor, poisonedBaseColor, (float)((Math.Sin(DM.TwoPiTimer) + 1.0) / 2.0));
		SetFacing();
		body.Draw(spriteBatch, pos, color, scale);
		spriteBatch.Draw(head, pos, headRect(), Color.White, 0f, headOrigin, scale, SpriteEffects.None, 0f);
	}

	public void DrawProtect(SpriteBatch spriteBatch, Vector2 pos, float scale)
	{
		if (protectIdle)
		{
			protectionStill.Draw(spriteBatch, pos, Color.White, scale);
		}
		else
		{
			protectionAnimation.Draw(spriteBatch, pos, Color.White, scale);
		}
	}

	private Rectangle headRect()
	{
		return DM.Player.Facing switch
		{
			Direction.North => new Rectangle(64, 0, 64, 64), 
			Direction.South => new Rectangle(0, 0, 64, 64), 
			Direction.East => new Rectangle(128, 0, 64, 64), 
			Direction.West => new Rectangle(192, 0, 64, 64), 
			_ => throw new Exception("Unknown Direction: " + DM.Player.Facing), 
		};
	}
}
