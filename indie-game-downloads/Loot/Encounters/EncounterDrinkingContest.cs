using System.IO;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Items;
using Loot.Items.Weapons;
using Loot.NPCs;
using Loot.PC;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterDrinkingContest : Encounter
{
	private Weapon weapon;

	public EncounterDrinkingContest(Location loc)
		: base(loc)
	{
	}

	public EncounterDrinkingContest(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "You hear mugs banging on tables, followed by gruff howls and cheers.\n\nInching closer you discover a group of orcs engaged in some serious drinking.", new DialogOption("I'm a bit parched, myself.  I should join them.", drink), new DialogOption("Drink with orcs?  Never!"));
	}

	private void drink()
	{
		Dialog.Display(null, "As you walk into the group of orcs, silence descends upon them.\n\n\"Look here, mate,\" the largest orc growls.  \"We don't want yer kind around here.\"", new DialogOption("Aw, shut up and toss me a brew.", brew), new DialogOption("Then die, orcs.  Die!", kill), new DialogOption("My mistake!", mistake));
	}

	private void mistake()
	{
		Dialog.Display(null, "You slink away from the laughing orcs, hoping that they don't toss an axe into your back.", new DialogOption("Stupid orcs.  Harrumph."));
	}

	private void kill()
	{
		Orc orc = new Orc(DM.Player.Depth, DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor));
		orc.Elite();
		DM.AddNPC(orc);
		DM.AddEffect(new FXPoof(orc));
		for (int i = 0; i < 5; i++)
		{
			Orc orc2 = new Orc(DM.Player.Depth, DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor));
			DM.AddNPC(orc2);
			DM.AddEffect(new FXPoof(orc2));
		}
	}

	private void brew()
	{
		int num = DM.Player.Depth;
		EquipmentTier tier = EquipmentTier.Orcish;
		weapon = ((num < 5) ? new WeaponClub(tier) : ((num < 15) ? new WeaponQuarterstaff(tier) : ((num < 25) ? ((Weapon)new WeaponMace(tier)) : ((Weapon)((num >= 40) ? new WeaponWarHammer(EquipmentTier.Epic) : new WeaponWarHammer(tier))))));
		weapon.Identify();
		Dialog.Display(null, "The orc gives you a second look.\n\n\"All right then, " + DM.Player.ClassName + ".  You think you can drink with us, do ya?\"\n\n\"You can have me " + weapon.Name + " if you can drink me under the table.  If not, I'll take what I please from ya.  Fair enough?\"", new DialogOption("Oh, it's on!", brew2), new DialogOption("Yeah, I think I'll pass.", mistake));
	}

	private void brew2()
	{
		Player player = DM.Player;
		Player player2 = player;
		if (player2 is Berserker || player2 is Gambler || player2 is Peasant)
		{
			win();
		}
		else
		{
			lose();
		}
	}

	private void win()
	{
		DM.Player.AddItemOrDrop(weapon);
		Profile.Awardments.Unlock(Awardment.Drinkaholic);
		Dialog.Display(null, "Contrary to what mother always said, drinking at the pub was time well spent!\n\nAfter a few dozen rounds the poor orc collapses under the table, leaving his " + weapon.Name + " for the taking.", new DialogOption("Ha ha!"));
	}

	private void lose()
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < DM.Player.Inventory.Length; i++)
		{
			Item item = DM.Player.Inventory[i];
			if (item != null && item.Value > num)
			{
				num2 = i;
				num = item.Value;
			}
		}
		if (num2 == -1)
		{
			int num3 = DM.Player.Depth * 10;
			if (num3 > DM.Player.Gold)
			{
				num3 = DM.Player.Gold;
			}
			DM.Player.Gold -= num3;
			Dialog.Display(null, "If only you had spent more time drinking than studying!\n\nAlas, you quickly collapse under the table, losing both " + num3 + " gold pieces and your pride.", new DialogOption("Ugh.  I feel woozy!"));
		}
		else
		{
			Item item2 = DM.Player.Inventory[num2];
			DM.Player.RemoveItem(num2);
			Dialog.Display(null, "If only you had spent more time drinking than studying!\n\nAlas, you quickly collapse under the table, losing both your " + item2.Name + " and your pride.", new DialogOption("Ugh.  I feel woozy!"));
		}
	}
}
