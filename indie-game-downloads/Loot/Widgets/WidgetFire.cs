using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.NPCs;
using Microsoft.Xna.Framework;

namespace Loot.Widgets;

public class WidgetFire : Widget
{
	private AnimatedSprite sprite;

	private TimeSpan hitTimer = TimeSpan.Zero;

	private static TimeSpan hitFreq = TimeSpan.FromSeconds(0.5);

	protected override Sprite Sprite => sprite;

	public WidgetFire(Location loc)
		: base(loc)
	{
		init();
	}

	public WidgetFire(BinaryReader reader)
		: base(reader)
	{
		init();
	}

	private void init()
	{
		sprite = new AnimatedSprite(WidgetSprite.FireAnimation, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 8, TimeSpan.FromMilliseconds(100.0));
		sprite.Frame = DM.Random.Next(8);
	}

	public override void Update(GameTime gameTime)
	{
		sprite.Update(gameTime);
		hitTimer += gameTime.ElapsedGameTime;
		if (!(hitTimer >= hitFreq))
		{
			return;
		}
		hitTimer -= hitFreq;
		if (Location == DM.Player.Location)
		{
			DM.Player.OnHit((int)Math.Ceiling((double)DM.Player.MaxHP / 20.0), crit: false, this);
			return;
		}
		NPC nPC = DM.Map.GetNPC(Location);
		if (nPC != null)
		{
			int dmg = (int)Math.Ceiling((double)nPC.MaxHP / 8.0);
			nPC.OnHit(dmg, crit: false, this);
		}
	}
}
