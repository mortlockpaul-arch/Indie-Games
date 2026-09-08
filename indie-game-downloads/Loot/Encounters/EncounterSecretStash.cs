using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.NPCs;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterSecretStash : Encounter
{
	public EncounterSecretStash(Location loc)
		: base(loc)
	{
	}

	public EncounterSecretStash(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "You can't quite put your finger on it, but something seems strange here.\n\nYou look around and don't see anything out of the ordinary, but your gut disagrees.", new DialogOption("Search the area...", search), new DialogOption("Nothing to see here.  Move along."));
	}

	private void search()
	{
		if (base.goodOutcome)
		{
			secretStash();
		}
		else if (DM.Random.Next(100) < 50)
		{
			nothing();
		}
		else
		{
			ratsNest();
		}
	}

	private void secretStash()
	{
		Dialog.Display(null, "Aha!  While examining the floor, you discover a loose stone.\n\nYou pry it up and discover a secret stash filled with a few items.", new DialogOption("Finders keepers!"));
		for (int i = 0; i < 3; i++)
		{
			DM.Player.AddItemOrDrop(ItemRegistry.ShopItem(base.depth));
		}
	}

	private void nothing()
	{
		Dialog.Display(null, "You search the area, but don't discover anything.", new DialogOption("I guess it was just my imagination..."));
	}

	private void ratsNest()
	{
		Dialog.Display(null, "Aha!  While examining the floor, you discover a loose stone.\n\nYou pry it up and discover a nest of beady-eyed rats!\n\nYou leap back as they pour out onto the floor around you.", new DialogOption("Yech.  Filthy creatures.", fight));
	}

	private void fight()
	{
		for (int i = 0; i < 6; i++)
		{
			PoofNPC(new Rat(base.depth, NearestOpenFloor()));
		}
	}
}
