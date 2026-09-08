using System.IO;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Items;
using Loot.NPCs;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterCrazyGoblin : Encounter
{
	public EncounterCrazyGoblin(Location loc)
		: base(loc)
	{
	}

	public EncounterCrazyGoblin(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "In the distance, you spy a ragged goblin rummaging through a pile of garbage.", new DialogOption("\"Looking for something, little friend?\"", looking), new DialogOption("\"It's time to take out the trash!\"  Attack!", attack), new DialogOption("Walk away.", end));
	}

	private void looking()
	{
		Dialog.Display(null, "The goblin whips around and glares at you with a huge crazy eye.\n\n\"It's mine!  Mine, I says!\"", new DialogOption("Um.  What is?", whatis), new DialogOption("\"Riiiight.  I'll just be moving on, then.\"", movingon));
	}

	private void whatis()
	{
		Dialog.Display(null, "The goblin eyes you suspiciously as shifts farther back into the pile of garbage.\n\n\"No.  You try to tricks me!\"", new DialogOption("I totally know.  I was just wondering if you did.", iknow), new DialogOption("\"Oh well.  Time to die, then.\"  Attack!", attack));
	}

	private void iknow()
	{
		Dialog.Display(null, "\"I know!  Yessss.  I know!  Ha!  Stupid trick!\n\nGo away now, or feel my sharp bite upon your tasties...\"", new DialogOption("Okaaay...  Time to go.", noidea), new DialogOption("Ha!  You'll not taste my tastiness!  Attack!", attack));
	}

	private void movingon()
	{
		Dialog.Display(null, "\"Wait!\"  The goblin screams.  \"You!  You have it!  You must!  You do!\"", new DialogOption("\"No, but I know where it is...\"", whereitis), new DialogOption("\"Relax.  I have no idea what you are talking about.\"", noidea));
	}

	private void noidea()
	{
		Dialog.Display(null, "The goblin sighs.\n\n\"Yes.  Yesss...  You are just a stupid " + DM.Player.ClassName + ".  Good-bye stupid " + DM.Player.ClassName + ".\"", new DialogOption("Hrmph.  Good-bye.", end));
	}

	private void whereitis()
	{
		Dialog.Display(null, "The goblin's huge eye bulges desperately.\n\n\"You do?!  You do!  Ah ha ha ha!  Tell me!  Tell me quick!\"", new DialogOption("\"Just kidding!  Heh heh!\"", killyou), new DialogOption("Pull something out of the pile of garbage.", pullItem));
	}

	private void pullItem()
	{
		Dialog.Display(null, "You reach into the pile of stinking garbage, hoping for the best.\n\nAfter a few nauseating moments, you grab a frightfully soiled teddy bear and hand it to the goblin.", new DialogOption("\"Here you go!\"", hereyougo));
	}

	private void hereyougo()
	{
		if (base.goodOutcome)
		{
			Dialog.Display(null, "A huge tear wells up in the goblin's huge eye.\n\n\"Pookey...\"  He whispers shakily, clutching it tight.\n\nThen, with a sniff, he presses a glowing ring into your hand and darts off into the darkness.", new DialogOption("Ok, that was weird.", reward));
		}
		else
		{
			killyou();
		}
	}

	private void reward()
	{
		DM.Player.AddItemOrDrop(ItemRegistry.Random(base.depth, ItemRegistry.RingFilter));
	}

	private void killyou()
	{
		Dialog.Display(null, "\"no no No No NO!\"  The goblin cries.  \"I kill you!  I kill you now!\"", new DialogOption("Fight for your life!", attack));
	}

	private void attack()
	{
		Location location = DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor);
		if (!(location == Location.Zero))
		{
			NPC nPC = new Goblin(DM.Player.Depth, location);
			nPC.Elite();
			DM.AddNPC(nPC);
			DM.AddEffect(new FXPoof(nPC));
		}
	}

	private void end()
	{
	}
}
