using System;
using System.IO;
using Loot.Dungeon;
using Microsoft.Xna.Framework;

namespace Loot.Statuses;

public class StatusPoison : StatusEffect
{
	private TimeSpan timer;

	private int dmg;

	private int tickCount;

	private TimeSpan tickInterval;

	public StatusPoison(int dmg, int tickCount, TimeSpan tickInterval)
	{
		timer = TimeSpan.Zero;
		this.dmg = dmg;
		this.tickCount = tickCount;
		this.tickInterval = tickInterval;
	}

	public StatusPoison(BinaryReader reader)
		: base(reader)
	{
	}

	public override void Update(GameTime gameTime, Character character)
	{
		timer -= gameTime.ElapsedGameTime;
		if (timer <= TimeSpan.Zero)
		{
			character.HP -= dmg;
			timer += tickInterval;
			tickCount--;
			if (tickCount <= 0)
			{
				character.Status.Remove(this);
			}
		}
	}

	public override void Read(BinaryReader reader)
	{
		timer = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
		dmg = reader.ReadInt32();
		tickCount = reader.ReadInt32();
		tickInterval = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write((int)timer.TotalMilliseconds);
		writer.Write(dmg);
		writer.Write(tickCount);
		writer.Write((int)tickInterval.TotalMilliseconds);
	}
}
