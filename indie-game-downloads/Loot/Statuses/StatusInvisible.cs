using System;
using System.IO;
using Loot.Dungeon;
using Loot.Effects;
using Microsoft.Xna.Framework;

namespace Loot.Statuses;

public class StatusInvisible : StatusDuration
{
	public StatusInvisible(TimeSpan duration)
		: base(duration)
	{
	}

	public StatusInvisible(BinaryReader reader)
		: base(reader)
	{
	}

	public override void Update(GameTime gameTime, Character character)
	{
		base.Update(gameTime, character);
		if (Duration <= TimeSpan.Zero && !character.Status.Is<StatusInvisible>())
		{
			DM.AddEffect(new FXPoof(character));
		}
	}
}
