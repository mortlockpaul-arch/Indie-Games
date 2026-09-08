using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Scrolls;
using Loot.PC;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterPuzzleChest : Encounter
{
	public EncounterPuzzleChest(Location loc)
		: base(loc)
	{
	}

	public EncounterPuzzleChest(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "Lying amidst a pile of rubble, you discover a strange copper bound chest.  On its lid are four dials labeled with strange markings.", new DialogOption("Force it open.", force), new DialogOption("Try to unlock it.", unlock), new DialogOption("Leave it."));
	}

	private void force()
	{
		Dialog.Display(null, "You smash a rock upon the chest, splitting it open.\n\nUnfortunately, a vial of acid was protecting its contents.\n\nThe hissing fluid quickly destroys the box's contents and splashes across your armor! (-1 Armor DEF)", new DialogOption("Curses!"));
		Armor armor = DM.Player.Armor;
		if (armor != null && armor.Stats.DEF > 0)
		{
			ref Stats stats = ref armor.Stats;
			int dEF = stats.DEF - 1;
			stats.DEF = dEF;
			DM.Player.CalcStats();
		}
	}

	private void unlock()
	{
		if (DM.Player is Tinkerer)
		{
			tinkerer();
		}
		else if (DM.Player.IsLucky() || DM.Random.Next(10) < 2)
		{
			success();
		}
		else
		{
			fail();
		}
	}

	private void tinkerer()
	{
		Dialog.Display(null, "You examing the locking mechanism carefully.\n\nWith a smirk you realize it is nothing more than a Moribund Matching Lock.  A baby could open it.\n\nYou whip our your screw driver and quickly disable the lock.", new DialogOption("Too easy!", reward));
	}

	private void success()
	{
		Dialog.Display(null, "You twiddle around with the dials, and, to your surprise, you hear a soft click...", new DialogOption("I'm a genius!", reward));
	}

	private void fail()
	{
		Dialog.Display(null, "You twist and turn the dials this way and that way to no avail.  You slump to the floor give the chest a baleful glare.", new DialogOption("Leave it."), new DialogOption("Smash it open.", force));
	}

	private void reward()
	{
		Dialog.Display(null, "The lid falls back with a creak, and you peer inside.\n\nA trove of ancient scrolls lie neatly stacked upon the bottom.", new DialogOption("I'll be taking those."));
		DM.Player.AddItemOrDrop(new ScrollRemoveCurse());
		DM.Player.AddItemOrDrop(new ScrollEnchantArmor());
		DM.Player.AddItemOrDrop(new ScrollEnchantWeapon());
	}
}
