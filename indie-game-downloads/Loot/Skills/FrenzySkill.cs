using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.NPCs;
using Loot.Statuses;
using Microsoft.Xna.Framework;

namespace Loot.Skills;

public class FrenzySkill : ActiveSkill
{
	private float rotation;

	private float rotNorth;

	private float rotEast = (float)Math.PI / 2f;

	private float rotSouth = (float)Math.PI;

	private float rotWest = 4.712389f;

	private float rad45 = MathHelper.ToRadians(45f);

	private float rad135 = MathHelper.ToRadians(135f);

	private float rad225 = MathHelper.ToRadians(225f);

	private float rad315 = MathHelper.ToRadians(315f);

	public override string Name => "Frenzy";

	public override Sprite Sprite => SkillSprite.Frenzy;

	protected override TimeSpan ActiveTimerDuration => TimeSpan.FromMilliseconds(500.0);

	public float Rotation => rotation;

	public FrenzySkill()
	{
	}

	public FrenzySkill(BinaryReader reader)
		: base(reader)
	{
	}

	protected override void Activate()
	{
		PlaySound.Swing();
		switch (DM.Player.Facing)
		{
		case Direction.North:
			rotation = rotNorth;
			break;
		case Direction.South:
			rotation = rotSouth;
			break;
		case Direction.East:
			rotation = rotEast;
			break;
		case Direction.West:
			rotation = rotWest;
			break;
		}
		hit();
	}

	private void hit()
	{
		Location location = DM.Player.Location;
		for (int i = location.Row - 1; i <= location.Row + 1; i++)
		{
			for (int j = location.Col - 1; j <= location.Col + 1; j++)
			{
				NPC nPC = DM.Map.GetNPC(new Location(i, j));
				if (nPC != null && !nPC.IsAlly)
				{
					bool crit = false;
					int num = (int)((double)DM.Player.Stats.DMG.Damage * ((10.0 + (double)Level) / 10.0));
					if (num == 0)
					{
						num = 1;
					}
					if (nPC.Status.Is<StatusFreeze>())
					{
						crit = true;
						num *= 2;
					}
					int num2 = num - nPC.Stats.DEF;
					if (num2 <= 0)
					{
						num2 = 1;
					}
					nPC.OnHit(num2, crit, DM.Player);
				}
			}
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		if (Active)
		{
			rotation += (float)(Math.PI * 4.0 * gameTime.ElapsedGameTime.TotalSeconds);
			if ((double)rotation > Math.PI * 2.0)
			{
				rotation -= (float)Math.PI * 2f;
			}
			if ((double)rotation > (double)rad315 || (double)rotation < (double)rad45)
			{
				DM.Player.Facing = Direction.North;
			}
			else if ((double)rotation < (double)rad135)
			{
				DM.Player.Facing = Direction.East;
			}
			else if ((double)rotation < (double)rad225)
			{
				DM.Player.Facing = Direction.South;
			}
			else
			{
				DM.Player.Facing = Direction.West;
			}
		}
	}

	protected override void Read(BinaryReader reader)
	{
		base.Read(reader);
		rotation = reader.ReadSingle();
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write(rotation);
	}
}
