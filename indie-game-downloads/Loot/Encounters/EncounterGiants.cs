using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.NPCs;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterGiants : Encounter
{
	private Item reward;

	public EncounterGiants(Location loc)
		: base(loc)
	{
	}

	public EncounterGiants(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "A pair of giants are dozing peacefully upon the floor.\n\nBetween them, however, you spy a goblin tugging at one of the giant's pockets.", new DialogOption("This will end badly...  Walk away."), new DialogOption("Try to sneak up on the goblin.", sneak), new DialogOption("Attack!", attack));
	}

	private void attack()
	{
		for (int i = 0; i < 2; i++)
		{
			Giant giant = new Giant(base.depth, NearestOpenFloor());
			giant.Elite();
			PoofNPC(giant);
		}
		PoofNPC(new Goblin(base.depth, NearestOpenFloor()));
	}

	private void sneak()
	{
		Dialog.Display(null, "You carefully pick your way toward the goblin, but his unfortunately large goblin ears pick up on your approach.\n\nThe goblin whips around and considers you for a moment.  Then, with a wink he presses his forefinger to his lips.", new DialogOption("Play along and keep quiet.", quiet), new DialogOption("Attack!", attack));
	}

	private void quiet()
	{
		Dialog.Display(null, "The goblin gingerly lifts an amulet and a ring from the giant's pocket.\n\nHolding one in each hand, he signals that he'll share one or the other with you.\n\nWhich will it be?", new DialogOption("Point at the ring.", ring), new DialogOption("Point at the amulet.", amulet));
	}

	private void ring()
	{
		reward = ItemRegistry.Random(base.depth, ItemRegistry.RingFilter);
		catchItem();
	}

	private void amulet()
	{
		reward = ItemRegistry.Random(base.depth, ItemRegistry.AmuletFilter);
		catchItem();
	}

	private void catchItem()
	{
		if (base.goodOutcome)
		{
			goodCatch();
		}
		else
		{
			badCatch();
		}
	}

	private void goodCatch()
	{
		Dialog.Display(null, "The goblin tosses the " + ((reward is Ring) ? "ring" : "amulet") + " toward you, and, before you can catch it, he's gone.\n\nFollowing his lead, you quietly slip away so you can examine your prize more closely.", new DialogOption("Mmm... treasure!"));
		DM.Player.AddItemOrDrop(reward);
	}

	private void badCatch()
	{
		string text = ((reward is Ring) ? "ring" : "amulet");
		Dialog.Display(null, "The goblin tosses the " + text + " toward you, and, before you can catch it, he's gone.\n\nUnfortunately, the " + text + " slips through your fingers and falls to floor.  Ting!\n\nWith a snort, both giants wake, looking rather miffed at having been distubed...", new DialogOption("Fight!", fightGiants));
		DM.Player.AddItemOrDrop(reward);
	}

	private void fightGiants()
	{
		for (int i = 0; i < 2; i++)
		{
			Giant giant = new Giant(base.depth, NearestOpenFloor());
			giant.Elite();
			PoofNPC(giant);
		}
	}
}
