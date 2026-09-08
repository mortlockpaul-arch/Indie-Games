using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Widgets;

public class WidgetWallSword : Widget
{
	private Sprite activeSprite = wallSwordIdleSprite;

	private float rotation;

	private TimeSpan idleTimer;

	private TimeSpan idleDuration = TimeSpan.FromSeconds(1.0);

	private AnimatedSprite wallSwordAnimation;

	private static Sprite wallSwordIdleSprite;

	private static Texture2D wallSwordTexture;

	private bool hasHitPlayer;

	private bool hasHitNPC;

	protected override Sprite Sprite
	{
		get
		{
			activeSprite.Rotation = rotation;
			return activeSprite;
		}
	}

	public WidgetWallSword(Location loc, float rotation, TimeSpan offset)
		: base(loc)
	{
		this.rotation = rotation;
		idleTimer = offset;
		init();
	}

	public WidgetWallSword(BinaryReader reader)
		: base(reader)
	{
		init();
	}

	public void init()
	{
		wallSwordAnimation = new AnimatedSprite(wallSwordTexture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 16, TimeSpan.FromMilliseconds(40.0));
	}

	public static void Load(ContentManager content)
	{
		wallSwordTexture = content.Load<Texture2D>("Sprites\\Widgets\\WallSword");
		wallSwordIdleSprite = new StillSprite(wallSwordTexture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f));
	}

	public override void Update(GameTime gameTime)
	{
		if (idleTimer > TimeSpan.Zero)
		{
			idleTimer -= gameTime.ElapsedGameTime;
			if (idleTimer <= TimeSpan.Zero)
			{
				idleTimer = TimeSpan.Zero;
				wallSwordAnimation.Reset();
				activeSprite = wallSwordAnimation;
				hasHitPlayer = false;
				hasHitNPC = false;
			}
		}
		else
		{
			wallSwordAnimation.Update(gameTime);
			if (wallSwordAnimation.HasLooped)
			{
				activeSprite = wallSwordIdleSprite;
				idleTimer = idleDuration;
			}
		}
		if (activeSprite != wallSwordAnimation || wallSwordAnimation.Frame <= 4 || wallSwordAnimation.Frame >= 11)
		{
			return;
		}
		if (!hasHitPlayer && DM.Player.Location == Location)
		{
			DM.Player.OnHit(DM.Player.MaxHP / 10 + 1, crit: false, this);
			hasHitPlayer = true;
		}
		if (!hasHitNPC)
		{
			NPC nPC = DM.Map.GetNPC(Location);
			if (nPC != null)
			{
				nPC.OnHit(nPC.MaxHP / 4 + 1, crit: false, this);
				hasHitNPC = true;
			}
		}
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		rotation = reader.ReadSingle();
		idleTimer = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write(rotation);
		writer.Write((int)idleTimer.TotalMilliseconds);
	}
}
