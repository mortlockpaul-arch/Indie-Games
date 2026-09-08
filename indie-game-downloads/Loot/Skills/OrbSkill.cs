using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Effects;
using Loot.NPCs;

namespace Loot.Skills;

public class OrbSkill : ActiveSkill
{
	public override string Name => "Orb";

	public override Sprite Sprite => SkillSprite.Orb;

	protected override TimeSpan ActiveTimerDuration => TimeSpan.FromMilliseconds(1000.0);

	public OrbSkill()
	{
	}

	public OrbSkill(BinaryReader reader)
		: base(reader)
	{
	}

	protected override void Activate()
	{
		if (DM.Player.Orb != null)
		{
			DM.Player.Orb.CastNova();
			return;
		}
		Orb orb = new Orb(DM.Player.Level, DM.Player.Location);
		DM.Player.Orb = orb;
		DM.AddNPC(orb);
		DM.AddEffect(new FXPoof(orb));
	}
}
