using System;
using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Widgets;

public static class WidgetSprite
{
	public static Sprite ArrowDown;

	public static Sprite ArrowUp;

	public static Sprite AwardmentChest;

	public static Sprite CaveExit;

	public static Sprite LadderUp;

	public static Sprite LadderDown;

	public static Sprite GlowingHole;

	public static Sprite Hole;

	public static Sprite Shop;

	public static Sprite Slots;

	public static Sprite Encounter;

	public static Sprite HPFountainFull;

	public static Sprite HPFountainEmpty;

	public static Sprite PoisonVent;

	public static Sprite LavaVent;

	public static Sprite WallGear;

	public static Sprite Tombstone;

	public static Texture2D PoisonVentAnimation;

	public static Sprite ChestClosed;

	public static Sprite ChestOpened;

	public static Texture2D FireAnimation;

	public static void UpdateAnimatedSprites(GameTime gameTime)
	{
		Encounter.Update(gameTime);
		HPFountainFull.Update(gameTime);
	}

	public static void LoadContent(ContentManager content)
	{
		WidgetLavaVent.Load(content);
		WidgetTrapSpike.Load(content);
		WidgetWallSword.Load(content);
		WidgetFountainXP.Load(content);
		ArrowDown = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\ArrowDown"));
		ArrowUp = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\ArrowUp"));
		AwardmentChest = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\AwardmentChest"));
		CaveExit = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\CaveExit"));
		LadderUp = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\LadderUp"));
		LadderDown = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\LadderDown"));
		GlowingHole = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\GlowingHole"));
		Hole = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\Hole"));
		Shop = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\Shop"));
		HPFountainEmpty = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\HPFountainEmpty"));
		PoisonVent = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\PoisonVent"));
		PoisonVentAnimation = content.Load<Texture2D>("Sprites\\Widgets\\PoisonVentAnimation");
		Tombstone = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\Tombstone"));
		WallGear = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\WallGear"));
		Encounter = new AnimatedSprite(content.Load<Texture2D>("Sprites\\Widgets\\Encounter"), new Rectangle(0, 0, 64, 64), null, 8, TimeSpan.FromMilliseconds(100.0));
		HPFountainFull = new AnimatedSprite(content.Load<Texture2D>("Sprites\\Widgets\\HPFountainFull"), new Rectangle(0, 0, 64, 64), null, 4, TimeSpan.FromMilliseconds(150.0));
		Texture2D texture = content.Load<Texture2D>("Sprites\\Widgets\\Chest");
		ChestClosed = new StillSprite(texture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f));
		ChestOpened = new StillSprite(texture, new Rectangle(64, 0, 64, 64), new Vector2(32f, 32f));
		FireAnimation = content.Load<Texture2D>("Sprites\\Widgets\\Fire");
		Slots = new StillSprite(content.Load<Texture2D>("Sprites\\Widgets\\Slots"));
	}
}
