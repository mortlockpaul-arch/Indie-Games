using System.IO;
using Loot.Dungeon;
using Loot.Effects;
using Loot.NPCs;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterCards : Encounter
{
	public EncounterCards(Location loc)
		: base(loc)
	{
	}

	public EncounterCards(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "You encounter a group of off-duty goblins playing cards.", new DialogOption("Attack!", attack), new DialogOption("Ask if you can join.", join), new DialogOption("Try to sneak by.", sneak));
	}

	public void attack()
	{
		for (int i = 0; i < 4; i++)
		{
			Location location = DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor);
			if (location == Location.Zero)
			{
				break;
			}
			NPC nPC = new Goblin(DM.Player.Depth, location);
			if (i == 0)
			{
				nPC.Elite();
			}
			DM.AddNPC(nPC);
			DM.AddEffect(new FXPoof(nPC));
		}
	}

	public void join()
	{
		if (DM.Player.Gold < 100)
		{
			noGold();
			return;
		}
		Dialog.Display(null, "Since they are off-duty, they're happy to have you join their game.", new DialogOption("Play for 100 GP.", playFair), new DialogOption("Play for 100 GP and CHEAT!", cheat));
	}

	public void noGold()
	{
		Dialog.Display(null, "The goblins look at your slack coinpurse and laugh.", new DialogOption("Attack!", attack), new DialogOption("Walk away in shame.", shame));
	}

	public void shame()
	{
		Dialog.Display(null, "Head hung low, you slink away from the goblin's cruel jeers.");
	}

	public void playFair()
	{
		bool flag;
		if (DM.Player.IsLucky() || DM.Random.NextDouble() <= 0.5)
		{
			flag = true;
			DM.Player.Gold += 100;
		}
		else
		{
			flag = false;
			DM.Player.Gold -= 100;
		}
		Dialog.Display(null, (!flag) ? "You lost 100 GP.  The goblins greedily gather up your coins." : "You won 100 GP!  And, it doesn't look like the goblins are too happy about it, either.", new DialogOption("Attack!", attack), new DialogOption("Say farewell.", end));
	}

	public void cheat()
	{
		if (DM.Random.NextDouble() <= 0.5)
		{
			if (DM.Player.Base.LCK > 0)
			{
				ref Stats reference = ref DM.Player.Base;
				int lCK = reference.LCK - 1;
				reference.LCK = lCK;
				DM.Player.CalcStats();
			}
			Dialog.Display(null, "You craftily pull an Ace of Axes from up your sleeve.  Unfortunately, the goblins catch you! (-1 LCK)\n\nYou have no choice, now.", new DialogOption("Attack!", attack));
		}
		else
		{
			DM.Player.Gold += 100;
			Dialog.Display(null, "You craftily pull an Ace of Axes from up your sleeve... and win 100 GP!  The goblins aren't looking too happy about it, though.", new DialogOption("Attack!", attack), new DialogOption("Say farewell.", end));
		}
	}

	public void end()
	{
		Dialog.Display(null, "The goblins grunt and send you on your way.");
	}

	public void sneak()
	{
		if (DM.Player.IsLucky() || DM.Random.Next(4 * DM.Player.Depth) < DM.Player.Stats.DEX)
		{
			Dialog.Display(null, "You successfully snuck by.");
			return;
		}
		Dialog.Display(null, "Uh oh!  They spotted you!");
		attack();
	}
}
