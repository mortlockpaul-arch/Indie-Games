using System;
using System.IO;
using Loot.Dungeon;
using Loot.Screens;
using Loot.Statuses;

namespace Loot.Items.Potions;

public class PotionLevitate : Potion
{
	protected override int RealValue => 100;

	public PotionLevitate()
		: base("Potion of Levitation")
	{
	}

	public PotionLevitate(BinaryReader reader)
		: base(reader, "Potion of Levitation")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		Message.Display("You float up into the air!");
		DM.Player.Status.Add(new StatusLevitate(TimeSpan.FromSeconds(40.0)));
	}
}
