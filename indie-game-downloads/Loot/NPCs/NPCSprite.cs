using System;
using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.NPCs;

public class NPCSprite
{
	public class Info
	{
		public readonly Texture2D Texture;

		public readonly Rectangle Rectangle;

		public readonly Vector2 Origin;

		public Info(Texture2D texture)
		{
			Texture = texture;
			Rectangle = new Rectangle(0, 0, 64, 64);
			Origin = new Vector2(32f, 32f);
		}

		public Info(Texture2D texture, Rectangle rect, Vector2 origin)
		{
			Texture = texture;
			Rectangle = rect;
			Origin = origin;
		}
	}

	public static Sprite Epic;

	public static Info Rat;

	public static Info Bat;

	public static Info Spider;

	public static Info Wolf;

	public static Info Jelly;

	public static Info Goblin;

	public static Info Orc;

	public static Info Witch;

	public static Info Skeleton;

	public static Info Zombie;

	public static Info Vampire;

	public static Info Giant;

	public static Info Dragon;

	public static Info Reaper;

	public static Info Beholder;

	public Sprite Move;

	public Sprite Attack;

	public Sprite Frozen;

	public NPCSprite(Info info)
	{
		generate(info.Texture, info.Rectangle, info.Origin);
	}

	private void generate(Texture2D texture, Rectangle rect, Vector2 origin)
	{
		Move = new AnimatedSprite(texture, rect, origin, 2, TimeSpan.FromMilliseconds(250.0));
		rect.Y += rect.Height;
		Attack = new AnimatedSprite(texture, rect, origin, 2, TimeSpan.FromMilliseconds(250.0));
		rect.Y += rect.Height;
		Frozen = new AnimatedSprite(texture, rect, origin, 2, TimeSpan.FromMilliseconds(250.0));
	}

	public static void LoadContent(ContentManager content)
	{
		Epic = new StillSprite(content.Load<Texture2D>("Sprites\\Mobs\\Epic"));
		Rat = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Rat"));
		Bat = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Bat"));
		Spider = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Spider"));
		Wolf = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Wolf"));
		Jelly = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Jelly"));
		Goblin = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Goblin"));
		Orc = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Orc"));
		Witch = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Witch"));
		Skeleton = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Skeleton"));
		Zombie = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Zombie"));
		Vampire = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Vampire"));
		Giant = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Giant"), new Rectangle(0, 0, 128, 128), new Vector2(64f, 96f));
		Dragon = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Dragon"));
		Reaper = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Reaper"));
		Beholder = new Info(content.Load<Texture2D>("Sprites\\Mobs\\Beholder"));
		SandWorm.Load(content);
	}
}
