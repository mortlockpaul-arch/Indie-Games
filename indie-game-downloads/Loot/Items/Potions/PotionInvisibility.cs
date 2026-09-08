using System;
using System.IO;
using Loot.Dungeon;
using Loot.Screens;
using Loot.Statuses;

namespace Loot.Items.Potions;

public class PotionInvisibility : Potion
{
	protected override int RealValue => 200;

	public PotionInvisibility()
		: base("Invisibility Potion")
	{
	}

	public PotionInvisibility(BinaryReader reader)
		: base(reader, "Invisibility Potion")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		Message.Display("You fade out of existence!");
		DM.Player.Status.Add(new StatusInvisible(TimeSpan.FromSeconds(10.0)));
	}
}
