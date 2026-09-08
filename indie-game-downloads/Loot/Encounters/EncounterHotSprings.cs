using System.IO;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.NPCs;
using Loot.Screens;
using Loot.Statuses;

namespace Loot.Encounters;

public class EncounterHotSprings : Encounter
{
	public EncounterHotSprings(Location loc)
		: base(loc)
	{
	}

	public EncounterHotSprings(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "An inviting hot spring lays before you.\n\nA thin haze of steam wafts across its surface, and, here and there, bubbles rise from some underground source.", new DialogOption("At last, some decent skinny dipping!", dip), new DialogOption("Bathe?  I think not.  It's unhealthy!"));
	}

	private void dip()
	{
		if (DM.Player.Armor == null)
		{
			nude();
		}
		else if (DM.Player.Armor.Cursed)
		{
			cursed();
		}
		else
		{
			dip2();
		}
	}

	private void nude()
	{
		Dialog.Display(null, "You hop into the hot spring and splash about.", new DialogOption("That was fun."));
	}

	private void cursed()
	{
		Dialog.Display(null, "You tug at your armor, but it doesn't come off.\n\nThe last thing you want to do is trudge around a dungeon in soaking wet armor.", new DialogOption("Stupid cursed armor.  Grr..."));
	}

	private void dip2()
	{
		Dialog.Display(null, "You slip out of your dusty armor and stick your big toe into the water.  Ah... It's perfect...\n\nWith no further ado, you splash into the hot spring and stretch out.", new DialogOption("This is the life.", dip3));
	}

	private void dip3()
	{
		if (base.goodOutcome)
		{
			clean();
		}
		else
		{
			thief();
		}
	}

	private void clean()
	{
		Dialog.Display(null, "Old aches and pains evaporate, and you are filled with a sense of peaceful well-being.  (+1 DEX, +1 LCK)\n\nYou could stay here forever, but, alas, you have monsters to slay and treasure to loot.", new DialogOption("I'm a whole new person."));
		ref Stats reference = ref DM.Player.Base;
		int dEX = reference.DEX + 1;
		reference.DEX = dEX;
		ref Stats reference2 = ref DM.Player.Base;
		dEX = reference2.LCK + 1;
		reference2.LCK = dEX;
		DM.Player.CalcStats();
		DM.Player.HP = DM.Player.MaxHP;
		DM.Player.Status.RemoveAll<StatusPoison>();
	}

	private void thief()
	{
		Dialog.Display(null, "Your peaceful repose is disturbed by a rustling at the bank.\n\nYou turn around to see a goblin snatch up your armor and run off into the darkness!", new DialogOption("Catch the goblin!", catchHim));
	}

	private void catchHim()
	{
		PoofNPC(new GoblinThief(base.depth, DM.Map.FindFarthest(DM.Player.Location, DM.Map.IsOpenFloor), DM.Player.Armor));
		DM.Player.ClearArmor();
		Profile.Awardments.Unlock(Awardment.Streaker);
	}
}
