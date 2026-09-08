using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Screens;

namespace Loot.Items.Potions;

public class PotionOil : Potion
{
	protected override int RealValue => 25;

	public override Sprite Sprite => PotionSprite.Oil;

	public PotionOil()
		: base("Lantern Oil")
	{
	}

	public PotionOil(BinaryReader reader)
		: base(reader, "Lantern Oil")
	{
	}

	public override void onActivate(int id)
	{
		if ((double)DM.Player.Lantern.Fuel > 0.985)
		{
			Message.Display("Your lantern is already full.");
			return;
		}
		DM.Player.Lantern.Refill();
		DM.Player.RemoveItem(id);
		Message.Display("You have refilled your lantern.");
	}
}
