using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Effects;
using Loot.NPCs;
using Loot.Statuses;

namespace Loot.Skills;

public class FreezeSkill : ActiveSkill
{
	public override string Name => "Freeze";

	public override Sprite Sprite => SkillSprite.Freeze;

	protected override TimeSpan ActiveTimerDuration => TimeSpan.FromMilliseconds(1000.0);

	public FreezeSkill()
	{
	}

	public FreezeSkill(BinaryReader reader)
		: base(reader)
	{
	}

	protected override void Activate()
	{
		TimeSpan duration = TimeSpan.FromSeconds(1.0 + 0.2 * (double)DM.Player.SkillSet.Freeze.Level);
		for (int i = DM.Player.Location.Row - 3; i <= DM.Player.Location.Row + 3; i++)
		{
			for (int j = DM.Player.Location.Col - 3; j <= DM.Player.Location.Col + 3; j++)
			{
				Location location = new Location(i, j);
				if (DM.Map.IsValid(location) && (double)DM.Player.Location.Distance(location) <= 3.0)
				{
					NPC nPC = DM.Map.GetNPC(location);
					if (nPC != null && !nPC.IsAlly && DM.LineOfSight(DM.Player.Location, location))
					{
						nPC.Status.Add(new StatusFreeze(duration));
					}
				}
			}
		}
		DM.AddEffect(FXFreeze.GetFX(DM.Player.Location));
	}
}
