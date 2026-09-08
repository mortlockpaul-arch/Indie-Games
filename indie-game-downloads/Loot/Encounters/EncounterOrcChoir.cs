using System.IO;
using Loot.Dungeon;
using Loot.NPCs;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterOrcChoir : Encounter
{
	public EncounterOrcChoir(Location loc)
		: base(loc)
	{
	}

	public EncounterOrcChoir(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "You seem to have stumbled upon an orcish quartet in the midst of practice:\n\n\"Kill a human!  Kill him dead!\nBash the brains out of his head!\"", new DialogOption("Attack!", attack), new DialogOption("Clear your throat and join in.", join), new DialogOption("I prefer the smooth stylings of elfish music.  Sneak away."));
	}

	private void attack()
	{
		for (int i = 0; i < 4; i++)
		{
			Orc orc = new Orc(base.depth, NearestOpenFloor());
			if (i == 0)
			{
				orc.Elite();
			}
			PoofNPC(orc);
		}
	}

	private void join()
	{
		Dialog.Display(null, "You saunter up to the startled group of orcs and belt out a verse of your own:", new DialogOption("Burn his village to the ground!\nDon't ya love that crackling sound?", burn), new DialogOption("Orcs are ugly in the face,\nbut I'll fix that with my mace!", mace));
	}

	private void burn()
	{
		Dialog.Display(null, "\"Not bad, not bad...\" grunts the largest orc.\n\n\"All right, boys, lets try it again from the top, but with the burning village bit in the second verse...\"", new DialogOption("There's just a small matter of royalties...", royalties), new DialogOption("My work here is done.  Walk away."));
	}

	private void royalties()
	{
		if (base.goodOutcome)
		{
			DM.Player.Gold += 100;
			Dialog.Display(null, "The largest orc grunts and tosses you 100 GP.\n\n\"Now get out of here, " + DM.Player.ClassName + ", before I bash in yer skull.\"", new DialogOption("A pleasure doing business with you!"), new DialogOption("I think I'll take your life as well.  Attack!", attack));
		}
		else
		{
			Dialog.Display(null, "The largest orc lets out an exasperated sigh of frustration, and pulls out a rather large sword.\n\n\"Ain't no need of that, if yer dead " + DM.Player.ClassName + ".\"", new DialogOption("That's the last time I collaborate with orcs!  Attack!", attack));
		}
	}

	private void mace()
	{
		Dialog.Display(null, "The largest orc give you a withering look.\n\n\"If you'll excuse us, " + DM.Player.ClassName + ", we are professionals and have no time for your childish antics.\"", new DialogOption("Okay...  Walk away."), new DialogOption("Hmph.  Then, taste my professional death dealing skill and stuff!", attack));
	}
}
