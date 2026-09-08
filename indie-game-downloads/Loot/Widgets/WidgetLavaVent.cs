using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.NPCs;
using Loot.Statuses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Widgets;

public class WidgetLavaVent : Widget
{
	private AnimatedSprite sprite;

	private TimeSpan timer;

	private TimeSpan waitDuration;

	private static Texture2D texture;

	private bool active;

	private bool hasHitPlayer;

	private bool hasHitNPC;

	protected override Sprite Sprite => sprite;

	public WidgetLavaVent(Location loc)
		: base(loc)
	{
		waitDuration = TimeSpan.FromMilliseconds((double)(2000 + DM.Random.Next(2000)));
		init();
	}

	public WidgetLavaVent(BinaryReader reader)
		: base(reader)
	{
		init();
	}

	private void init()
	{
		sprite = new AnimatedSprite(texture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 12, TimeSpan.FromMilliseconds(100.0));
		active = false;
		timer = TimeSpan.FromMilliseconds((double)DM.Random.Next((int)waitDuration.TotalMilliseconds));
	}

	public static void Load(ContentManager content)
	{
		texture = content.Load<Texture2D>("Sprites\\Widgets\\LavaVent");
	}

	public override void Update(GameTime gameTime)
	{
		if (!active)
		{
			timer -= gameTime.ElapsedGameTime;
			if (timer <= TimeSpan.Zero)
			{
				active = true;
				sprite.Reset();
				hasHitPlayer = false;
				hasHitNPC = false;
			}
			return;
		}
		sprite.Update(gameTime);
		if (sprite.HasLooped)
		{
			active = false;
			timer = waitDuration;
		}
		else
		{
			if (sprite.Frame < 2 || sprite.Frame > 9)
			{
				return;
			}
			if (!hasHitPlayer && Location == DM.Player.Location && !DM.Player.Status.Is<StatusLevitate>())
			{
				DM.Player.OnHit(DM.Player.MaxHP / 10, crit: false, this);
				hasHitPlayer = true;
			}
			else if (!hasHitNPC)
			{
				NPC nPC = DM.Map.GetNPC(Location);
				if (nPC != null && !nPC.CanFly)
				{
					int dmg = nPC.MaxHP / 4 + 1;
					nPC.OnHit(dmg, crit: false, this);
					hasHitNPC = true;
				}
			}
		}
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		waitDuration = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write((int)waitDuration.TotalMilliseconds);
	}
}
