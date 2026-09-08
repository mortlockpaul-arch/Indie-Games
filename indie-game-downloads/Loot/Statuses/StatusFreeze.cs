using System;
using System.IO;
using Loot.Dungeon;
using Loot.Effects;
using Microsoft.Xna.Framework;

namespace Loot.Statuses;

public class StatusFreeze : StatusDuration
{
	public StatusFreeze(TimeSpan duration)
		: base(duration)
	{
	}

	public StatusFreeze(BinaryReader reader)
		: base(reader)
	{
	}

	public override void Update(GameTime gameTime, Character character)
	{
		base.Update(gameTime, character);
		if (Duration <= TimeSpan.Zero && !character.Status.Is<StatusFreeze>())
		{
			DM.AddEffect(FXIce.GetFX(character.Location));
		}
	}
}
