using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Statuses;
using Microsoft.Xna.Framework;

namespace Loot.Skills;

public class RegenSkill : Skill
{
	private TimeSpan hitWait = TimeSpan.Zero;

	private TimeSpan hitWaitDuration = TimeSpan.FromSeconds(1.0);

	private TimeSpan healWait;

	private TimeSpan healWaitDuration = TimeSpan.FromMilliseconds(500.0);

	public override string Name => "Regenerate";

	public override Sprite Sprite => SkillSprite.Regen;

	public bool Active => Level > 0 && DM.Player.HP < DM.Player.MaxHP && hitWait == TimeSpan.Zero;

	public RegenSkill(BinaryReader reader)
		: base(reader)
	{
	}

	public RegenSkill()
	{
		hitWait = TimeSpan.Zero;
		healWait = TimeSpan.Zero;
	}

	public void OnHit()
	{
		if (Level != 0)
		{
			hitWait = hitWaitDuration;
		}
	}

	public override void Update(GameTime gameTime)
	{
		if (Level == 0)
		{
			return;
		}
		if (DM.Player.HP <= 0)
		{
			hitWait = hitWaitDuration;
		}
		else if (DM.Player.Status.Is<StatusPoison>())
		{
			hitWait = hitWaitDuration;
		}
		else if (hitWait > TimeSpan.Zero)
		{
			hitWait -= gameTime.ElapsedGameTime;
			if (hitWait < TimeSpan.Zero)
			{
				hitWait = TimeSpan.Zero;
				healWait = TimeSpan.Zero;
			}
		}
		else if (healWait > TimeSpan.Zero)
		{
			healWait -= gameTime.ElapsedGameTime;
			if (healWait < TimeSpan.Zero)
			{
				healWait = TimeSpan.Zero;
			}
		}
		else if (DM.Player.HP < DM.Player.MaxHP)
		{
			DM.Player.HP += Level * 2;
			healWait = healWaitDuration;
		}
	}

	protected override void Read(BinaryReader reader)
	{
		base.Read(reader);
		hitWait = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
		healWait = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write((int)hitWait.TotalMilliseconds);
		writer.Write((int)healWait.TotalMilliseconds);
	}
}
