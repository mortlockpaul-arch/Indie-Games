using System.IO;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Items;
using Loot.Items.Scrolls;
using Loot.NPCs;
using Loot.PC;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterNecro : Encounter
{
	public EncounterNecro(Location loc)
		: base(loc)
	{
	}

	public EncounterNecro(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "You discover an ancient tome bound in human flesh.  It looks harmless enough...", new DialogOption("Nothing ventured, nothing gained.  Open the book!", open), new DialogOption("Curiosity killed the cat.  Leave it alone.", leave));
	}

	private void open()
	{
		switch (DM.Random.Next(2))
		{
		case 0:
			undeadRising();
			break;
		case 1:
			scroll();
			break;
		}
	}

	private void undeadRising()
	{
		Player player = DM.Player;
		int hP = player.HP - 1;
		player.HP = hP;
		Dialog.Display(null, "Uh oh.  This probably wasn't such a good idea.\n\nWhile flipping through the pages, you get a nasty paper cut!  Ouch! (-1 HP)\n\nOh, but it gets worse!  Your blood is drawn into the page and the earth around you trembles...  The undead are rising!", new DialogOption("Attack!", attack));
	}

	private void scroll()
	{
		Scroll scroll = new ScrollIdentify();
		Dialog.Display(null, "As you flip through the musty pages, you discover an '" + scroll.Name + "'.\n\nOtherwise, the book seems useless, and you toss it aside.");
		DM.Player.AddItemOrDrop(scroll);
	}

	private void attack()
	{
		for (int i = 0; i < 4; i++)
		{
			Location location = DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor);
			if (!(location == Location.Zero))
			{
				addUndead(location);
				continue;
			}
			break;
		}
		for (int j = 0; j < DM.Map.Rows; j++)
		{
			for (int k = 0; k < DM.Map.Cols; k++)
			{
				Location loc = new Location(j, k);
				if (DM.Map.IsOpenFloor(loc) && DM.Random.Next(4) == 0)
				{
					addUndead(loc);
				}
			}
		}
		Profile.Awardments.Unlock(Awardment.UndeadRising);
	}

	private void addUndead(Location loc)
	{
		NPC nPC = ((DM.Random.Next(2) != 0) ? ((NPC)new Zombie(DM.Player.Depth + 1, loc)) : ((NPC)new Skeleton(DM.Player.Depth + 1, loc)));
		if (DM.Random.Next(10) == 0)
		{
			nPC.Elite();
		}
		DM.AddNPC(nPC);
		DM.AddEffect(new FXPoof(nPC));
	}

	private void leave()
	{
		Dialog.Display(null, "You cast the foul book aside and proceed on your way.");
	}
}
