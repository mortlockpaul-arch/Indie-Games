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

public class WidgetTrapSpike : Widget
{
	private static Texture2D spikeTexture;

	private Sprite activeSprite;

	private static StillSprite spikeDown;

	private AnimatedSprite spikeGoingUp;

	private static StillSprite spikeUp;

	private AnimatedSprite spikeGoingDown;

	private bool isUp;

	private TimeSpan timer;

	private TimeSpan downTime;

	private TimeSpan upTime;

	private bool hasSpikedPlayer;

	private bool hasSpikedNPC;

	protected override Sprite Sprite => activeSprite;

	public WidgetTrapSpike(Location loc)
		: base(loc)
	{
		downTime = TimeSpan.FromMilliseconds((double)(500 + DM.Random.Next(500)));
		upTime = TimeSpan.FromMilliseconds((double)(500 + DM.Random.Next(250)));
		init();
	}

	public WidgetTrapSpike(BinaryReader reader)
		: base(reader)
	{
		init();
	}

	private void init()
	{
		spikeGoingUp = new AnimatedSprite(spikeTexture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 4, TimeSpan.FromMilliseconds(40.0));
		spikeGoingDown = new AnimatedSprite(spikeTexture, new Rectangle(0, 64, 64, 64), new Vector2(32f, 32f), 4, TimeSpan.FromMilliseconds(40.0));
		isUp = false;
		activeSprite = spikeDown;
		timer = TimeSpan.Zero;
	}

	public static void Load(ContentManager content)
	{
		spikeTexture = content.Load<Texture2D>("Sprites\\Widgets\\TrapSpike");
		spikeUp = new StillSprite(spikeTexture, new Rectangle(0, 64, 64, 64), new Vector2(32f, 32f));
		spikeDown = new StillSprite(spikeTexture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f));
	}

	public override void Update(GameTime gameTime)
	{
		activeSprite.Update(gameTime);
		if (activeSprite == spikeGoingUp && spikeGoingUp.HasLooped)
		{
			isUp = true;
			hasSpikedNPC = false;
			hasSpikedPlayer = false;
			activeSprite = spikeUp;
		}
		else if (activeSprite == spikeGoingDown && spikeGoingDown.HasLooped)
		{
			activeSprite = spikeDown;
		}
		timer += gameTime.ElapsedGameTime;
		if (isUp)
		{
			if (timer > upTime)
			{
				timer = TimeSpan.Zero;
				isUp = false;
				spikeGoingDown.Reset();
				activeSprite = spikeGoingDown;
			}
		}
		else if (timer > downTime)
		{
			timer = TimeSpan.Zero;
			spikeGoingUp.Reset();
			activeSprite = spikeGoingUp;
		}
		if (!isUp)
		{
			return;
		}
		if (!hasSpikedPlayer && DM.Player.Location == Location && !DM.Player.Status.Is<StatusLevitate>())
		{
			DM.Player.OnHit(DM.Player.MaxHP / 10 + 1, crit: false, this);
			hasSpikedPlayer = true;
		}
		if (!hasSpikedNPC)
		{
			NPC nPC = DM.Map.GetNPC(Location);
			if (nPC != null && !nPC.CanFly)
			{
				nPC.OnHit(nPC.MaxHP / 4 + 1, crit: false, this);
				hasSpikedNPC = true;
			}
		}
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		downTime = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
		upTime = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write((int)downTime.TotalMilliseconds);
		writer.Write((int)upTime.TotalMilliseconds);
	}
}
