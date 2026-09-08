using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.PC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Widgets;

public class WidgetFountainXP : Widget
{
	private bool full;

	private Sprite activeSprite;

	private AnimatedSprite fullSprite;

	private static Texture2D texture;

	private static StillSprite emptySprite;

	protected override Sprite Sprite => activeSprite;

	public WidgetFountainXP(Location loc)
		: base(loc)
	{
		full = true;
		init();
	}

	public WidgetFountainXP(BinaryReader reader)
		: base(reader)
	{
		init();
	}

	private void init()
	{
		fullSprite = new AnimatedSprite(texture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 7, TimeSpan.FromMilliseconds(100.0));
		if (full)
		{
			activeSprite = fullSprite;
		}
		else
		{
			activeSprite = emptySprite;
		}
	}

	public static void Load(ContentManager content)
	{
		texture = content.Load<Texture2D>("Sprites\\Widgets\\XPFountain");
		emptySprite = new StillSprite(texture, new Rectangle(448, 0, 64, 64), new Vector2(32f, 32f));
	}

	public override void Update(GameTime gameTime)
	{
		activeSprite.Update(gameTime);
	}

	public override void OnMove()
	{
		if (full)
		{
			DM.Player.Thought = ThoughtBubble.LTrigger;
		}
	}

	public override void OnClick()
	{
		if (full)
		{
			PlaySound.Potion();
			Player player = DM.Player;
			int xp = (player.NextXP - player.PrevXP) / 2;
			player.AddXP(xp);
			player.Thought = null;
			full = false;
			activeSprite = emptySprite;
		}
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		full = reader.ReadBoolean();
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write(full);
	}
}
