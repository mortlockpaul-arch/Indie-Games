using System.IO;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Items;
using Loot.Items.Armors;
using Loot.NPCs;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterSarcophagus : Encounter
{
	public EncounterSarcophagus(Location loc)
		: base(loc)
	{
	}

	public EncounterSarcophagus(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "You discover a large stone sarcophagus etched with elaborate markings.  You wonder what riches might be hidden within.", new DialogOption("A grave robbery a day keeps the poorhouse away.  Open it!", open), new DialogOption("No way!  That's creepy.", leave));
	}

	private void open()
	{
		if (DM.Player.IsLucky())
		{
			item();
		}
		else
		{
			vampire();
		}
	}

	private void item()
	{
		int num = DM.Player.Depth;
		Armor armor = ((num < 5) ? new ArmorClothes(EquipmentTier.Dwarven) : ((num < 10) ? new ArmorLeather(EquipmentTier.Dwarven) : ((num < 15) ? new ArmorStudded(EquipmentTier.Dwarven) : ((num < 20) ? new ArmorChainMail(EquipmentTier.Dwarven) : ((num < 30) ? new ArmorSplintMail(EquipmentTier.Dwarven) : ((num >= 40) ? ((Armor)new ArmorFullPlate(EquipmentTier.Dwarven)) : ((Armor)new ArmorPlateMail(EquipmentTier.Dwarven))))))));
		armor.Identify();
		Dialog.Display(null, "You push with all your might, and the stone lid slowly moves aside.\n\nIt appears to be a dwarven warrior--or what's left of one.  His sword is broken, but his '" + armor.Name + "' appears to be in perfect condition!\n\nFinders keepers!");
		DM.Player.AddItemOrDrop(armor);
	}

	private void vampire()
	{
		Dialog.Display(null, "The lid crashes to ground, shattering into a thousand stone fragments.\n\nAs you peer inside, a cloud of bats burst from the coffin, revealing a Vampire Lord with blood-red eyes...\n\nHe is not amused.", new DialogOption("\"Die foul creature!\"", fight));
	}

	private void fight()
	{
		Vampire vampire = new Vampire(DM.Player.Depth + 3, DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor));
		vampire.Elite();
		DM.AddNPC(vampire);
		DM.AddEffect(new FXPoof(vampire));
		for (int i = 0; i < 3; i++)
		{
			Bat bat = new Bat(DM.Player.Depth, DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor));
			DM.AddNPC(bat);
			DM.AddEffect(new FXPoof(bat));
		}
	}

	private void leave()
	{
	}
}
