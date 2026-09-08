using System.IO;
using Loot.Dungeon;
using Loot.Screens;
using Loot.Statuses;

namespace Loot.Items.Potions;

public class PotionAntidote : Potion
{
	protected override int RealValue => 50;

	public PotionAntidote()
		: base("Antidote")
	{
	}

	public PotionAntidote(BinaryReader reader)
		: base(reader, "Antidote")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		if (DM.Player.Status.Is<StatusPoison>())
		{
			Message.Display("You feel healthy.");
		}
		else
		{
			Message.Display("No effect...");
		}
		DM.Player.Status.RemoveAll<StatusPoison>();
	}
}
