using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Effects;
using Loot.NPCs;
using Loot.Statuses;
using Microsoft.Xna.Framework;

namespace Loot.Widgets;

public class WidgetPoisonVent : Widget
{
	private const string poisonText = "POISON!";

	private bool isActive;

	private TimeSpan timer;

	private TimeSpan waitDuration;

	private Sprite sprite;

	private AnimatedSprite eruptAnimation;

	private static Color poisonTextColor = new Color(0, 255, 0);

	private static Color poisonTextShadowColor = new Color(0, 153, 0);

	private bool hasPoisonedPlayer;

	private bool hasPoisonedNPC;

	protected override Sprite Sprite => sprite;

	public WidgetPoisonVent(Location loc)
		: base(loc)
	{
		waitDuration = TimeSpan.FromMilliseconds((double)(2000 + DM.Random.Next(2000)));
		init();
	}

	public WidgetPoisonVent(BinaryReader reader)
		: base(reader)
	{
		init();
	}

	private void init()
	{
		isActive = false;
		timer = TimeSpan.FromMilliseconds((double)DM.Random.Next((int)waitDuration.TotalMilliseconds));
		sprite = WidgetSprite.PoisonVent;
		eruptAnimation = new AnimatedSprite(WidgetSprite.PoisonVentAnimation, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 10, TimeSpan.FromMilliseconds(100.0));
	}

	public override void Update(GameTime gameTime)
	{
		sprite.Update(gameTime);
		if (sprite == eruptAnimation && eruptAnimation.HasLooped)
		{
			sprite = WidgetSprite.PoisonVent;
			timer = TimeSpan.Zero;
			isActive = false;
		}
		if (!isActive)
		{
			timer += gameTime.ElapsedGameTime;
			if (timer >= waitDuration)
			{
				eruptAnimation.Reset();
				sprite = eruptAnimation;
				timer = TimeSpan.Zero;
				isActive = true;
				hasPoisonedPlayer = false;
				hasPoisonedNPC = false;
			}
		}
		if (!isActive)
		{
			return;
		}
		if (!hasPoisonedPlayer && DM.Player.Location == Location && !DM.Player.Status.Is<StatusLevitate>())
		{
			PlaySound.Squish();
			DM.AddEffect(FXText.GetFX(DM.Player.Location, "POISON!", poisonTextColor, poisonTextShadowColor));
			DM.Player.Status.Add(new StatusPoison(DM.Player.MaxHP / 50, 10, TimeSpan.FromSeconds(1.0)));
			hasPoisonedPlayer = true;
		}
		if (!hasPoisonedNPC)
		{
			NPC nPC = DM.Map.GetNPC(Location);
			if (nPC != null && !nPC.CanFly)
			{
				int num = nPC.MaxHP / 10;
				nPC.Status.Add(new StatusPoison(1, 5, TimeSpan.FromSeconds(1.0)));
				hasPoisonedNPC = true;
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
