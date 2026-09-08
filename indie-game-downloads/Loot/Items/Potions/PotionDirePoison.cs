using System;
using System.IO;
using Loot.Dungeon;
using Loot.Screens;
using Loot.Statuses;

namespace Loot.Items.Potions;

public class PotionDirePoison : Potion
{
	protected override int RealValue => 10;

	public PotionDirePoison()
		: base("Dire Poison")
	{
	}

	public PotionDirePoison(BinaryReader reader)
		: base(reader, "Dire Poison")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		Message.Display("You feel horribly sick!");
		DM.Player.Status.Add(new StatusPoison(DM.Player.MaxHP / 80, 40, TimeSpan.FromMilliseconds(500.0)));
	}
}
