using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Effects;
using Loot.NPCs;
using Loot.Statuses;

namespace Loot.Skills;

public class PoisonSkill : ActiveSkill
{
	public override string Name => "Poison";

	public override Sprite Sprite => SkillSprite.Poison;

	protected override TimeSpan ActiveTimerDuration => TimeSpan.FromMilliseconds(1000.0);

	public PoisonSkill()
	{
	}

	public PoisonSkill(BinaryReader reader)
		: base(reader)
	{
	}

	protected override void Activate()
	{
		Location location = DM.Player.Location;
		for (int i = location.Row - 3; i < location.Row; i++)
		{
			for (int j = location.Col - 3; j < location.Col; j++)
			{
				hit(new Location(i, j));
			}
			for (int num = location.Col + 3; num >= location.Col; num--)
			{
				hit(new Location(i, num));
			}
		}
		for (int num2 = location.Row + 3; num2 >= location.Row; num2--)
		{
			for (int k = location.Col - 3; k < location.Col; k++)
			{
				hit(new Location(num2, k));
			}
			for (int num3 = location.Col + 3; num3 >= location.Col; num3--)
			{
				hit(new Location(num2, num3));
			}
		}
		DM.AddEffect(FXPoison.GetFX(DM.Player.Location));
	}

	private void hit(Location l)
	{
		if (DM.Map.IsValid(l) && !((double)DM.Player.Location.Distance(l) > 3.0))
		{
			int level = DM.Player.SkillSet.Poison.Level;
			NPC nPC = DM.Map.GetNPC(l);
			if (nPC != null && !nPC.IsAlly && DM.LineOfSight(DM.Player.Location, l))
			{
				nPC.Status.Add(new StatusPoison(level, 20, TimeSpan.FromMilliseconds(350.0)));
			}
		}
	}
}
