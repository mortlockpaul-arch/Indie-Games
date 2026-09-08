using System;
using System.IO;
using Loot.Dungeon;
using Microsoft.Xna.Framework;

namespace Loot.Statuses;

public abstract class StatusDuration : StatusEffect
{
	protected TimeSpan Duration;

	public StatusDuration(TimeSpan duration)
	{
		Duration = duration;
	}

	public StatusDuration(BinaryReader reader)
		: base(reader)
	{
	}

	public override void Update(GameTime gameTime, Character character)
	{
		Duration -= gameTime.ElapsedGameTime;
		if (Duration <= TimeSpan.Zero)
		{
			character.Status.Remove(this);
		}
	}

	public override void Read(BinaryReader reader)
	{
		Duration = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write((int)Duration.TotalMilliseconds);
	}
}
